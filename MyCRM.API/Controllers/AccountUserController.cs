using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.AccountUser;
using MyCRM.Shared.Communications.Responses.User;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.User;
using System;
using System.Threading.Tasks;
using System.Linq;
using Stripe;

namespace MyCRM.API.Controllers
{
    [Route("api/user")]
    //[Authorize(Policy = "Manager")]
    [ApiController]
    public class AccountUserController : ControllerBase
    {
        private readonly IAccountUserService _accountUserService;
        private readonly IAccountManager _accountManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public AccountUserController(IAccountUserService accountUserService, UserManager<ApplicationUser> userManager, IAccountManager accountManager, ILogger<AccountUserController> logger, ApplicationDbContext applicationDbContext)
        {
            _accountUserService = accountUserService;
            _accountManager = accountManager;
            _logger = logger;
            _context = applicationDbContext;
        }

        /// <summary>
        /// fetch data for user dashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInformation(LoggingEvents.GetItem, "Getting Current User");
                var currentUserData = await _accountUserService.GetCurrentUserWithEmployeAllEvents();
                var roles = await _accountManager.GetUserRolesAsync(currentUserData);
                var subService = new SubscriptionService();
                var sub = await subService.GetAsync(currentUserData.Organization.StripeSubscriptionId);
                bool isFreeTrial = false;
                if (sub.TrialEnd != null)
                {
                    if (sub.TrialEnd > DateTime.Now)
                    {
                        isFreeTrial = true;
                    }
                }
                var basicUser = new BasicUserDataModel
                {
                    Sub = currentUserData.Id,
                    EmailConfirmed = currentUserData.EmailConfirmed,
                    CompanyName = currentUserData.Organization.Name,
                    FirstName = currentUserData.FirstName,
                    LastName = currentUserData.LastName,
                    Phone = currentUserData.PhoneNumber,
                    Name = currentUserData.Name,
                    Email = currentUserData.Email,
                    IsAdmin = roles.Contains("admin"),
                    IsManager = roles.Contains("manager"),
                    SubscriptionPlan = currentUserData.Organization.SubscriptionPlan,
                    //IsFreeTrail = currentUserData.Organization.IsFreeTrail,
                    IsSubAboutToExpire = currentUserData.Organization.IsSubAboutToExpire,
                    IsFreeTrail = isFreeTrial,
                    IsSubExpired = currentUserData.Organization.IsSubExpired,
                    SubId = currentUserData.Organization.StripeSubscriptionId,
                    EventNumbers = currentUserData.Appointments.Count(x => x.EventStartDateTime.Day == DateTime.UtcNow.Day && x.EventStartDateTime.Month == DateTime.UtcNow.Month && x.EventStartDateTime.Year == DateTime.UtcNow.Year)
                    + currentUserData.Organization.Events.Count(x => x.EventStartDateTime.Day == DateTime.UtcNow.Day && x.EventStartDateTime.Month == DateTime.UtcNow.Month && x.EventStartDateTime.Year == DateTime.UtcNow.Year)
                    + currentUserData.Tasks.Count(x => x.EventStartDateTime.Day == DateTime.UtcNow.Day && x.EventStartDateTime.Month == DateTime.UtcNow.Month && x.EventStartDateTime.Year == DateTime.UtcNow.Year),
                };
                return Ok(basicUser);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(BasicUserDataModel request)
        {
            var currentUser = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
            currentUser.FirstName = request.FirstName;
            currentUser.LastName = request.LastName;
            currentUser.PhoneNumber = request.Phone;
            currentUser.UpdatedBy = currentUser.Name;
            currentUser.UpdatedDate = DateTime.Now;

            _context.Update(currentUser);
            if (await Save()) return Ok();
            return BadRequest();
        }

        /// <summary>
        /// add a new company to the user account
        /// </summary>
        /// <param name="addCompanyToAccountUserRequest"></param>
        /// <returns></returns>
        [HttpPost("company")]
        public async Task<IActionResult> AddNewOrganizationToAccount(AddOrganizationToAccountUserRequest addOrganizationToAccountUserRequest)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
            var organization = new Organization(addOrganizationToAccountUserRequest.OrganizationName);
            var result = await _accountUserService.AddNewOrganizationToUserAccount(organization, user);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Added new organization to user account");
            if (result.Success) return Ok(result.Model);
            return BadRequest(result.Message);
        }

        /// <summary>
        /// update current user subscription plan
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[HttpPut("subscription")]
        //public async Task<IActionResult> UpdateSubscription(UpdateSubscriptionPlanRequest request)
        //{
        //    //var data = await _accountUserService.GetCurrentUserBasicInfo();
        //    //var result = await _accountUserService.SetSubscriptionPlan(data.Model, request.SubscriptionPlan, request.SubscriptionExpirationDate);

        //    //if (result.Success) return Ok(result.Model);
        //    //return BadRequest(result.Message);
        //}
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> Save()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning(LoggingEvents.SaveToDatabaseFailed, "Save to database failed.");
                return false;
            }
        }
    }
}