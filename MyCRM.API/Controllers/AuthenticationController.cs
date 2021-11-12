using ETLib.Models.QueryResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence;
using MyCRM.Persistence.Data;
using MyCRM.Services.Repository.OrganizationRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Services.Services.EmailSenderService;
using MyCRM.Shared.Communications.Requests.Authentication;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.User;
using MyCRM.Shared.ViewModels.AuthenticationViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETLib.Extensions;
using MyCRM.Shared.Constants;
using Stripe;

namespace MyCRM.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IAccountUserService _accountUserService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public AuthenticationController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IOrganizationRepository organizationRepository, IEmailSender emailSender,
            ILogger<AuthenticationController> logger, IAccountUserService accountUserService)
        {
            _userManager = userManager;
            _context = context;
            _organizationRepository = organizationRepository;
            _emailSender = emailSender;
            _logger = logger;
            _accountUserService = accountUserService;
            StripeConfiguration.ApiKey = "sk_live_qrJZNaDlZ4AdlQVDB8yarMNB003JmZgyBu";
        }

        private char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        //[HttpGet]
        //[Route("Register")]
        //public IActionResult Register(string returnUrl = null)
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View();
        //}

        /// <summary>
        /// register a new user with a new company, this is for new business owner to register, for employee register will be internal process
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (_context.ApplicationUsers.Any(x => x.Email == model.Email)) return BadRequest("User is already exist");
            var nowTime = DateTime.Now;
            int organizationId;
            Organization newOrganization;
            var transction = _context.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(model.OrganizationName))
                {
                    newOrganization = new Organization(model.Name);
                    var organizationAdded = await _organizationRepository.Add(newOrganization);
                    if (!organizationAdded.Success) return BadRequest("Organization Failed to Add");
                    organizationId = newOrganization.Id;
                }
                else
                {
                    newOrganization = new Organization(model.OrganizationName);
                    var organizationAdded = await _organizationRepository.Add(newOrganization);
                    if (!organizationAdded.Success) return BadRequest("Organization Failed to Add");
                    organizationId = newOrganization.Id;
                }

                var newUser = new ApplicationUser
                {
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Phone,
                    Email = model.Email,
                    OrganizationId = organizationId,
                    CreatedDate = nowTime,
                };
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (!result.Succeeded)
                {
                    _context.Organizations.Remove(newOrganization);
                    _context.SaveChanges();
                    return BadRequest(result.Errors);
                }
                var roleResult = await _userManager.AddToRolesAsync(newUser, new List<string> { "admin", "manager", "employee" });
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(newUser);
                    _context.Organizations.Remove(newOrganization);
                    _context.SaveChanges();
                    return BadRequest(roleResult.Errors);
                }
                _logger.LogInformation("User created a new account with password.");

                CustomerService customerService = new CustomerService();
                CustomerCreateOptions customerCreateOptions = new CustomerCreateOptions
                {
                    Name = newOrganization.Name,
                    Email = model.Email,
                    Phone = model.Phone ?? ""
                };
                var customerCreated = await customerService.CreateAsync(customerCreateOptions);
                if (customerCreated != null)
                {
                    newOrganization.StripeCustomerId = customerCreated.Id;
                }
                PlanService planService = new PlanService();
                var optionsPlan = new PlanListOptions { Limit = 3 };
                StripeList<Plan> plans = planService.List(optionsPlan);

                if (!plans.Any())
                {
                    CreatePlan(SubscriptionPlan.Essential.ToDescriptionString(), 499, "month");
                    CreatePlan(SubscriptionPlan.Advanced.ToDescriptionString(), 699, "month");
                    CreatePlan(SubscriptionPlan.Premium.ToDescriptionString(), 899, "month");
                }

                //var updatedCustomer = AttachPaymentMethodToCustomer(user.Organization.StripeCustomerId, stripeSubscriptionRequest.PaymentMethodId);
                plans = planService.List(optionsPlan);
                var premium = plans.FirstOrDefault(s => String.Equals(s.Nickname, "Premium", StringComparison.CurrentCultureIgnoreCase));
                if (premium == null) return BadRequest("plan not exist");

                TaxRateService taxRateService = new TaxRateService();
                var taxRates = taxRateService.List().Select(s => s.Id).ToList();
                if (!taxRates.Any()) return BadRequest("tax rate not found");

                var items = new List<SubscriptionItemOption> {
                    new SubscriptionItemOption {Plan = premium.Id, Quantity = 3}
                };
                if (customerCreated == null) return BadRequest("Stripe Customer create failed");
                var options = new SubscriptionCreateOptions
                {
                    Customer = customerCreated.Id,
                    Items = items,
                    TrialPeriodDays = 14,
                    //CancelAtPeriodEnd = true
                    //TrialEnd = DateTime.Now.AddMinutes(5),
                    DefaultTaxRates = taxRates
                };
                var service = new SubscriptionService();

                Subscription subscription = service.Create(options);

                if (subscription != null)
                {
                    newOrganization.StripeSubscriptionId = subscription.Id;
                    newOrganization.SubscriptionQuantity = 3;
                    //newOrganization.IsFreeTrail = true;
                    newOrganization.SubscriptionPlan = SubscriptionPlan.Premium;
                    newOrganization.SubscriptionStartDate = nowTime;
                    newOrganization.SubscriptionExpirationDate = nowTime.AddDays(14);
                    _context.Organizations.Update(newOrganization);
                    _context.SaveChanges();

                    await _emailSender.SendEmailAsync("yuyang8986@gmail.com", "New User Registered!",
                        $"New User: {model.Phone}, {model.Email}");

                    await _emailSender.SendEmailAsync("info@gis-global.co", "New User Registered!",
                        $"New User: {model.Phone}, {model.Email}");

                    await _emailSender.SendEmailAsync("weicherng.thong@gmail.com", "New User Registered!",
                        $"New User: {model.Phone}, {model.Email}");
                }

                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                //var callbackUrl = Url.EmailConfirmationLink(newUser.Id, code, Request.Scheme);
                transction.Commit();
                return Ok();
            }
            catch (Exception e)
            {
                transction.Rollback();
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> GetConfirmEmailCode()
        {
            var user = await _accountUserService.GetCurrentApplicationUserWithOutData();
            var random = new Random();
            string verifyCode = "";
            for (int i = 0; i < 6; i++)
            {
                int rnd = random.Next(0, Pattern.Length);
                verifyCode += Pattern[rnd];
            }
            user.VerifyCode = verifyCode;
            user.VerifyExpiredDateTime = DateTime.Now.AddMinutes(59);
            _context.Update(user);
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $" <img border=\"0\" style=\"max-width:15%!important; width:15%; height:auto \" src=\"http://cdn.mcauto-images-production.sendgrid.net/d0589116d89f7f13/4442293c-f53c-41d9-8554-9a764803b29b/3000x782.png \">" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">Hi {user.Name},</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">Please confirm your email address with code '{verifyCode}'</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">Thank you</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">Dealo Team</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>");

            if (await _accountUserService.Save()) return Ok();
            return BadRequest();
        }

        [HttpPost]
        [Route("ConfirmEmail/{code}")]
        public async Task<IActionResult> ConfirmEmail(string code)
        {
            var user = await _accountUserService.GetCurrentApplicationUserWithOutData();
            if (user.VerifyExpiredDateTime < DateTime.Now) return BadRequest("Code Expired, Please try again.");
            if (user.VerifyCode == code)
            {
                user.EmailConfirmed = true;
                _context.Update(user);
                if (await _accountUserService.Save()) return Ok();
            }
            return BadRequest("Wrong Code, Please try again.");
        }

        //[HttpGet]
        //[Route("ForgotPassword")]
        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}
        [ApiExplorerSettings(IgnoreApi = true)]
        public Plan CreatePlan(string planName, double amount, string interval)
        {
            var options = new PlanCreateOptions
            {
                Product = new PlanProductCreateOptions
                {
                    Name = planName
                },
                Amount = (long)amount,
                Currency = "aud",
                Interval = interval,
                Nickname = planName
            };

            var service = new PlanService();
            Plan plan = service.Create(options);

            return plan;
        }

        [HttpPost]
        [Route("ForgotPassword")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return BadRequest("Account is not exist!");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                var random = new Random();
                string verifyCode = "";
                for (int i = 0; i < 6; i++)
                {
                    int rnd = random.Next(0, Pattern.Length);
                    verifyCode += Pattern[rnd];
                }
                user.VerifyCode = verifyCode;
                user.VerifyExpiredDateTime = DateTime.Now.AddMinutes(59);
                _context.Update(user);
                if (await _accountUserService.Save())
                {
                    await _emailSender.SendEmailAsync(model.Email, "Reset Password Code",
                      $" <img border=\"0\" style=\"max-width:15%!important; width:15%; height:auto \" src=\"http://cdn.mcauto-images-production.sendgrid.net/d0589116d89f7f13/4442293c-f53c-41d9-8554-9a764803b29b/3000x782.png \">" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">Hi {user.Name},</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">The code to reset your Dealo password is '{verifyCode}'</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">Thank you</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">Dealo Team</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>");
                    return Ok();
                }
            }

            // If execution got this far, something failed, redisplay the form.
            return BadRequest();
        }

        //[HttpGet]
        //[Route("ForgotPasswordConfirmation")]
        //public IActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        //[HttpGet]
        //[Route("ResetPassword")]
        //public IActionResult ResetPassword(string code = null)
        //{
        //    if (code == null)
        //    {
        //        throw new ApplicationException("A code must be supplied for password reset.");
        //    }
        //    var model = new ResetPasswordViewModel { Code = code };
        //    return View(model);
        //}
        [HttpPost]
        [Route("ResetPassword")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest("User is not exist.");
            }
            if (user.VerifyExpiredDateTime < DateTime.Now) return BadRequest("Code Expired, Please try again.");
            if (user.VerifyCode == model.VerifyCode)
            {
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, model.Password);
                return Ok();
            }

            return BadRequest("Wrong Code, Please try again.");
        }

        //[HttpGet]
        //[Route("ResetPasswordConfirmation")]
        //public IActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _accountUserService.GetCurrentApplicationUserWithOutData();
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
            if (result.Succeeded) return Ok();
            if (result.ToString().Equals("Failed : PasswordMismatch"))
            {
                return BadRequest("Wrong Old Password, Please try again.");
            }
            return BadRequest(result);
        }
    }
}