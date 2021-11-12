using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Constants;
using Stripe;

namespace MyCRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountUserService _accountUserService;

        public SubscriptionController(ApplicationDbContext context, IAccountUserService accountUserService)
        {
            _context = context;
            _accountUserService = accountUserService;
            StripeConfiguration.ApiKey = "sk_live_qrJZNaDlZ4AdlQVDB8yarMNB003JmZgyBu";
        }

        [HttpGet("{quantity}")]
        public async Task<IActionResult> UpdateQuantity(int quantity)
        {
            try
            {
                var subItemService = new SubscriptionItemService();
                SubscriptionService subscriptionService = new SubscriptionService();
                var user = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
                Subscription subscription = await subscriptionService.GetAsync(user.Organization.StripeSubscriptionId);

                if (subscription == null) return BadRequest("Subscription not found");
                //subscription.Quantity = quantity;
                var subItem = subscription.Items.FirstOrDefault();
                if (subItem == null) return BadRequest("Sub Item Not Found");
                var options = new SubscriptionItemUpdateOptions
                {
                    Plan = subItem.Plan.Id,
                    Quantity = quantity
                };

                var subUpdated = subItemService.Update(subItem.Id, options);
                //var subUpdated = await subscriptionService.UpdateAsync(subscription.Id, serviceUpdateOptions);
                user.Organization.SubscriptionQuantity = (int)subUpdated.Quantity;
                _context.Organizations.Update(user.Organization);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{plan}")]
        public async Task<IActionResult> UpdatePlan(string plan)
        {
            try
            {
                var planService = new PlanService();
                var planSelected = planService.List().FirstOrDefault(s => String.Equals(s.Nickname, plan, StringComparison.CurrentCultureIgnoreCase));
                if (planSelected == null) return BadRequest("plan not found");

                var subItemService = new SubscriptionItemService();
                SubscriptionService subscriptionService = new SubscriptionService();
                var user = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
                Subscription subscription = await subscriptionService.GetAsync(user.Organization.StripeSubscriptionId);

                if (subscription == null) return BadRequest("Subscription not found");
                //subscription.Quantity = quantity;
                var subItem = subscription.Items.FirstOrDefault();
                if (subItem == null) return BadRequest("Sub Item Not Found");
                var options = new SubscriptionItemUpdateOptions
                {
                    Plan = planSelected.Id,
                    Quantity = subItem.Quantity
                };

                var subUpdated = subItemService.Update(subItem.Id, options);
                //var subUpdated = await subscriptionService.UpdateAsync(subscription.Id, serviceUpdateOptions);
                //SubscriptionPlan subscriptionPlan = SubscriptionPlan.None;
                Enum.TryParse(subUpdated.Plan.Nickname, out SubscriptionPlan subscriptionPlan);
                user.Organization.SubscriptionPlan = subscriptionPlan;
                _context.Organizations.Update(user.Organization);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("cancel")]
        public async Task<IActionResult> Cancel()
        {
            try
            {
                var user = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
                var subService = new SubscriptionService();
                var sub = await subService.GetAsync(user.Organization.StripeSubscriptionId);
                if (sub == null) return BadRequest("sub not found");

                //SubscriptionCancelOptions subscriptionCancelOptions = new SubscriptionCancelOptions
                //{
                //    Prorate = false
                //};
                var subscriptionCancelOptions = new SubscriptionUpdateOptions
                {
                    CancelAtPeriodEnd = true,
                };
                var subCanceled = await subService.UpdateAsync(sub.Id, subscriptionCancelOptions);

                user.Organization.SubscriptionExpirationDate = subCanceled.CurrentPeriodEnd;
                _context.Organizations.Update(user.Organization);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("cancel failed");
            }
        }

        [HttpPut]
        [Route("resubscribe")]
        public async Task<IActionResult> Resubscribe()
        {
            try
            {
                var user = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
                var subService = new SubscriptionService();

                var subCancelled = await subService.GetAsync(user.Organization.StripeSubscriptionId);
                if (subCancelled == null) return BadRequest("sub cancelled not found");

                var subItem = subCancelled.Items.FirstOrDefault();
                if (subItem == null) return BadRequest("Sub Item not found");
                var items = new List<SubscriptionItemOption> {
                    new SubscriptionItemOption {Plan = subItem.Plan.Id, Quantity = subItem.Quantity}
                };

                if (subCancelled.CanceledAt != null)
                {
                    var options = new SubscriptionCreateOptions
                    {
                        Customer = user.Organization.StripeCustomerId,
                        Items = items
                    };
                    var subscriptionReactivated = await subService.CreateAsync(options);
                    //var items = new List<SubscriptionItemUpdateOption> {
                    //    new SubscriptionItemUpdateOption {
                    //        Id = subscription.Items.Data[0].Id,
                    //        Plan = "plan_CBb6IXqvTLXp3f",
                    //    },
                    //};

                    user.Organization.StripeSubscriptionId = subscriptionReactivated.Id;
                    //user.Organization.IsFreeTrail = false;
                    _context.Organizations.Update(user.Organization);
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    var subUpdateOptions = new SubscriptionUpdateOptions
                    {
                        CancelAtPeriodEnd = false
                    };

                    var subscriptionReactivated = await subService.UpdateAsync(subCancelled.Id, subUpdateOptions);
                    if (!subscriptionReactivated.CancelAtPeriodEnd)
                    {
                        user.Organization.StripeSubscriptionId = subscriptionReactivated.Id;
                        //user.Organization.IsFreeTrail = false;
                        _context.Organizations.Update(user.Organization);
                        _context.SaveChanges();
                        return Ok();
                    }
                    return BadRequest("reactive failed");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("cancel failed");
            }
        }
    }
}