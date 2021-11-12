using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Persistence.Data;
using Stripe;

namespace MyCRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeWebHookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StripeWebHookController(ApplicationDbContext context)
        {
            _context = context;
            //StripeConfiguration.ApiKey = "sk_test_3SCBM5Mi7KdaT76zZvitqxc200wUoj1PHy";
            StripeConfiguration.ApiKey = "sk_live_qrJZNaDlZ4AdlQVDB8yarMNB003JmZgyBu";
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            try
            {
                var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();
                var stripeEvent = EventUtility.ParseEvent(json);

                //// Handle the event
                //if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                //{
                //    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                //    handlePaymentIntentSucceeded(paymentIntent);
                //}
                //else if (stripeEvent.Type == Events.PaymentMethodAttached)
                //{
                //    var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                //    handlePaymentMethodAttached(paymentMethod);
                //}
                //invoice.payment_succeeded
                //else
                if (stripeEvent.Type == Events.InvoicePaymentSucceeded)
                {
                    var invoice = stripeEvent.Data.Object as Invoice;
                    if (invoice == null)
                    {
                        return BadRequest("invoice not exist");
                    }

                    if (invoice.AmountPaid == 0)
                    {
                        return Ok();
                    }

                    CustomerService customerService = new CustomerService();

                    var customer = await customerService.GetAsync(invoice.CustomerId);
                    var organization = _context.Organizations
                        .FirstOrDefault(s => s.StripeCustomerId == customer.Id);

                    if (organization == null) return BadRequest("organization not exist");

                    var subService = new SubscriptionService();
                    var subscription = subService.Get(invoice.SubscriptionId);
                    if (subscription == null) return BadRequest("Subscription not found");
                    if (subscription.Id == organization.StripeSubscriptionId)
                    {
                        organization.SubscriptionExpirationDate =
                            subscription.CurrentPeriodEnd.GetValueOrDefault().AddDays(7);
                        //if (subscription.TrialEnd != null) organization.IsFreeTrail = false;
                        _context.Organizations.Update(organization);
                        _context.SaveChanges();
                        return Ok();
                    }

                    return BadRequest("Subscription id not match");
                }
                // ... handle other event types
                else
                {
                    // Unexpected event type
                    return Ok("No Matching Events Handled");
                }
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private void handlePaymentMethodAttached(PaymentMethod paymentMethod)
        {
            throw new NotImplementedException();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private void handlePaymentIntentSucceeded(PaymentIntent paymentIntent)
        {
            throw new NotImplementedException();
        }
    }
}