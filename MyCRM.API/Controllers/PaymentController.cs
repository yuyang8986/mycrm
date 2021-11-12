using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETLib.Extensions;
using ETLib.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Constants;
using MyCRM.Shared.Models.Managements;
using Stripe;

namespace MyCRM.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "admin")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IAccountUserService _accountUserService;
        private readonly ApplicationDbContext _context;

        public PaymentController(IAccountUserService accountUserService, ApplicationDbContext context)
        {
            _accountUserService = accountUserService;
            _context = context;
            //StripeConfiguration.ApiKey = "sk_test_3SCBM5Mi7KdaT76zZvitqxc200wUoj1PHy";
            StripeConfiguration.ApiKey = "sk_live_qrJZNaDlZ4AdlQVDB8yarMNB003JmZgyBu";
        }

        [HttpGet]
        public async Task<IActionResult> GetBillingInfo()
        {
            try
            {
                var user = await _accountUserService.GetUserWithEmployeeOrganizationData(true);

                var invoiceService = new InvoiceService();
                var subService = new SubscriptionService();
                var customerService = new CustomerService();
                var paymentMethodService = new PaymentMethodService();
                var customer = await customerService.GetAsync(user.Organization.StripeCustomerId);
                var sub = await subService.GetAsync(user.Organization.StripeSubscriptionId);
                var invoices = await invoiceService.ListAsync();
                PaymentMethod paymentMethod;
                if (customer.InvoiceSettings.DefaultPaymentMethodId != null)
                {
                    paymentMethod = await paymentMethodService.GetAsync(customer.InvoiceSettings.DefaultPaymentMethodId);
                }
                else
                {
                    paymentMethod = null;
                }
                var options = new UpcomingInvoiceOptions
                {
                    Customer = customer.Id,
                };
                var totalAmount = 0.00;
                //if (sub.TrialEnd != null)
                //{
                //    if (sub.TrialEnd > DateTime.Now)
                //    {
                //        totalAmount = 0.00;
                //    }
                //}
                //else
                if (sub.CanceledAt != null)
                {
                    totalAmount = 0.00;
                }
                else
                {
                    Invoice upcoming = invoiceService.Upcoming(options);
                    totalAmount = upcoming.AmountDue;
                }

                var nextBillingDate = sub.CurrentPeriodEnd;
                var totalActiveAccounts = user.Organization.ApplicationUsers.Count(s => s.IsActive);

                var response = new GetAccountInfoResponse
                {
                    DueAmount = (double)totalAmount / 100,
                    CurrentPlan = sub?.Plan.Nickname,
                    Last4Digits = paymentMethod?.Card.Last4,
                    NextBillingDate = nextBillingDate.GetValueOrDefault(),
                    // ReSharper disable once PossibleInvalidOperationException
                    TotalSubQuantity = (int)sub?.Quantity.GetValueOrDefault(0),
                    TotalActiveAccounts = totalActiveAccounts,
                    CancelAt = sub.CanceledAt,
                    TrialEnd = sub.TrialEnd,
                    CancelAtPeriodEnd = sub.CancelAtPeriodEnd
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// 1. ensure has plans, if not create plans, 2.
        /// </summary>
        /// <param name="stripeSubscriptionRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("subscribe")]
        public async Task<IActionResult> Subscribe(StripeSubscriptionRequest stripeSubscriptionRequest)
        {
            var organization = _context.Organizations.FirstOrDefault(s =>
                s.StripeSubscriptionId == stripeSubscriptionRequest.SubscriptionId);
            if (organization == null) return BadRequest("Organization not found");

            var optionsPlan = new PlanListOptions { Limit = 3 };
            var servicePlan = new PlanService();
            StripeList<Plan> plans = servicePlan.List(optionsPlan);

            if (!plans.Any())
            {
                CreatePlan(SubscriptionPlan.Essential.ToDescriptionString(), 499, "month");
                CreatePlan(SubscriptionPlan.Advanced.ToDescriptionString(), 699, "month");
                CreatePlan(SubscriptionPlan.Premium.ToDescriptionString(), 899, "month");
            }
            //var updatedCustomer = AttachPaymentMethodToCustomer(user.Organization.StripeCustomerId, stripeSubscriptionRequest.PaymentMethodId);
            plans = servicePlan.List(optionsPlan);

            var planAdding = plans.FirstOrDefault(s => String.Equals(s.Nickname, stripeSubscriptionRequest.Plan.ToString(), StringComparison.CurrentCultureIgnoreCase));
            if (planAdding == null) return BadRequest("plan not exist");
            List<SubscriptionItemOption> items = new List<SubscriptionItemOption>
            {
                new SubscriptionItemOption
                {
                    Plan = planAdding.Id, Quantity = stripeSubscriptionRequest.SubscriptionQuantity
                }
            };

            if (string.IsNullOrWhiteSpace(organization.StripeCustomerId))
            {
                return BadRequest("Stripe Id not Exist");
            }
            var customerService = new CustomerService();

            var currentCustomer = await customerService.GetAsync(organization.StripeCustomerId);

            if (currentCustomer == null)
            {
                return BadRequest("Stripe Id not Exist");
            }

            //CustomerUpdateOptions customerUpdateOptions = new CustomerUpdateOptions
            //{
            //    DefaultSource = stripeSubscriptionRequest.PaymentMethodId,
            //    InvoiceSettings = new CustomerInvoiceSettingsOptions
            //    {
            //        DefaultPaymentMethod = stripeSubscriptionRequest.PaymentMethodId
            //    }
            //};
            TaxRateService taxRateService = new TaxRateService();
            var taxRates = taxRateService.List().Select(s => s.Id).ToList();
            if (!taxRates.Any()) return BadRequest("tax rate not found");
            //customerService.Update(currentCustomer.Id, customerUpdateOptions);
            var options = new SubscriptionCreateOptions
            {
                Customer = currentCustomer.Id,
                Items = items,
                DefaultPaymentMethod = currentCustomer.InvoiceSettings.DefaultPaymentMethodId,
                DefaultTaxRates = taxRates
            };

            //options.AddExpand("latest_invoice.payment_intent");

            var service = new SubscriptionService();
            Subscription subscription = service.Create(options);

            if (subscription != null)
            {
                //var refundService = new RefundService();
                //RefundCreateOptions refundOptions = new RefundCreateOptions
                //{
                //};
                //refundService.Create(refundOptions);
                organization.SubscriptionPlan = stripeSubscriptionRequest.Plan;
                organization.StripeSubscriptionId = subscription.Id;
                organization.SubscriptionQuantity = stripeSubscriptionRequest.SubscriptionQuantity;
                await _accountUserService.Save();
            }

            return Ok(subscription);
        }

        //[HttpGet]
        //[Route("unsubscribe")]
        //public async Task<IActionResult> UnSubscribe()
        //{
        //}

        /// <summary>
        /// 1. create stripe customer and attached the payment method to user
        /// 2. save customer id to organization
        /// 3. attempt charge 0.01 and generate clientsecret and return, wait for frondend to confirm
        /// </summary>
        /// <param name="stripePaymentRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("card")]
        public async Task<IActionResult> AttachReplaceCard(StripePaymentRequest stripePaymentRequest)
        {
            Organization organization = new Organization("");
            Customer customer = null;
            String originalCardId = "";
            try
            {
                if (stripePaymentRequest.SubscriptionId == null)
                {
                    var user = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
                    organization = user.Organization;
                }
                else
                {
                    organization = _context.Organizations.FirstOrDefault(s =>
                        s.StripeSubscriptionId == stripePaymentRequest.SubscriptionId);
                }
            }
            catch (Exception e)
            {
                return BadRequest();
            }

            try
            {
                if (organization == null) return BadRequest("Organization not found");

                var service = new CustomerService();
                if (organization.StripeCustomerId == null)
                {
                    return BadRequest("customer not found");
                }
                else
                {
                    customer = service.Get(organization.StripeCustomerId);
                    if (customer == null) return BadRequest("stripe customer not exist");

                    if (customer.InvoiceSettings.DefaultPaymentMethodId == null)
                    {
                        AttachPaymentMethodToCustomer(customer.Id, stripePaymentRequest.SourceId);
                    }
                    else
                    {
                        originalCardId = customer.InvoiceSettings.DefaultPaymentMethodId;

                        AttachPaymentMethodToCustomer(customer.Id, stripePaymentRequest.SourceId);

                        if (stripePaymentRequest.SubscriptionId == null)
                        {
                            var result = await RemovePaymentMethodFromCustomerAdmin(originalCardId);
                            if (!result) return BadRequest("Remove card failed");
                        }
                        else
                        {
                            var result = await RemovePaymentMethodFromCustomer(stripePaymentRequest.SubscriptionId, originalCardId);
                            if (!result) return BadRequest("Remove card failed");
                        }
                    }
                }

                if (customer != null)
                {
                    customer = await service.GetAsync(customer.Id);
                    PaymentMethodService paymentMethodService = new PaymentMethodService();
                    var paymentMethod = await paymentMethodService.GetAsync(customer.InvoiceSettings.DefaultPaymentMethodId);
                    var cardLast4 = paymentMethod.Card.Last4;
                    GetIntentResultResponse intentResultResponse = new GetIntentResultResponse
                    {
                        //ClientSecret = intent.ClientSecret,
                        Last4Digits = cardLast4
                    };
                    return Ok(intentResultResponse);
                    // }
                    //}
                }

                return BadRequest("customer create failed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //if (!string.IsNullOrWhiteSpace(originalCardId))
                //{
                //    AttachPaymentMethodToCustomer(customer.Id, originalCardId);
                //}
                return BadRequest(e.Message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> RemovePaymentMethodFromCustomer(string subId, string originalCardId)
        {
            try
            {
                //Subscription subscription = new Subscription();
                //SubscriptionService subscriptionService = new SubscriptionService();
                //if (subId == null)
                //{
                //    try
                //    {
                //        var user = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
                //        subscription = await subscriptionService.GetAsync(user.Organization.StripeSubscriptionId);
                //    }
                //    catch (Exception e)
                //    {
                //        subscription = await subscriptionService.GetAsync(subId);
                //    }
                //}

                //var organization = _context.Organizations
                //    .FirstOrDefault(s => s.StripeSubscriptionId == subId);
                //if (organization == null) throw new Exception("Organization not found");
                //CustomerService customerService = new CustomerService();
                //var customer = await customerService.GetAsync(organization.StripeCustomerId);
                var service = new PaymentMethodService();
                var paymentMethod = service.Detach(originalCardId, null);
                if (paymentMethod.CustomerId == null) return true;
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> RemovePaymentMethodFromCustomerAdmin(string originalCardId)
        {
            try
            {
                //Subscription subscription = new Subscription();
                //SubscriptionService subscriptionService = new SubscriptionService();

                //var user = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
                //subscription = await subscriptionService.GetAsync(user.Organization.StripeSubscriptionId);

                //CustomerService customerService = new CustomerService();
                //var customer = await customerService.GetAsync(subscription.CustomerId);
                var service = new PaymentMethodService();
                var paymentMethod = service.Detach(originalCardId, null);
                if (paymentMethod.CustomerId == null) return true;
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// interval => "month" or "year"
        /// </summary>
        /// <param name="planName"></param>
        /// <param name="amount"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
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

        [ApiExplorerSettings(IgnoreApi = true)]
        public PaymentMethod AttachPaymentMethodToCustomer(string customerId, string paymentMethodId)
        {
            var service = new PaymentMethodService();
            var options = new PaymentMethodAttachOptions
            {
                Customer = customerId,
            };
            var paymentMethod = service.Attach(paymentMethodId, options);

            if (paymentMethod != null)
            {
                UpdateInvoiceSetting(paymentMethodId, customerId);
            }

            return paymentMethod;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public Customer UpdateInvoiceSetting(string paymentMethodId, string customerId)
        {
            var invoiceUpdateOption = new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = paymentMethodId,
                },
            };

            var updateCustomerService = new CustomerService();
            var customer = updateCustomerService.Update(customerId, invoiceUpdateOption);

            return customer;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                    new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
    }

    public class StripePaymentRequest
    {
        public string SourceId { get; set; }
        public string ProductName { get; set; }
        public double Amount { get; set; }

        public string SubscriptionId { get; set; }

        public string ContactEmail { get; set; }

        public SubscriptionPlan SubscriptionPlan { get; set; }
    }

    public class StripeSubscriptionRequest
    {
        public string PaymentMethodId { get; set; }
        public SubscriptionPlan Plan { get; set; }
        public string SubscriptionId { get; set; }
        public int SubscriptionQuantity { get; set; }
    }

    public class GetAccountInfoResponse
    {
        public DateTime NextBillingDate { get; set; }
        public double DueAmount { get; set; }
        public int TotalSubQuantity { get; set; }

        public string CompanyName { get; set; }

        public string CurrentPlan { get; set; }

        public int TotalActiveAccounts { get; set; }
        public string Last4Digits { get; set; }

        public string Email { get; set; }

        public DateTime? CancelAt { get; set; }

        public DateTime? TrialEnd { get; set; }

        public bool CancelAtPeriodEnd { get; set; }
    }

    public class GetIntentResultResponse
    {
        public string Last4Digits { get; set; }
        public string ClientSecret { get; set; }
    }
}