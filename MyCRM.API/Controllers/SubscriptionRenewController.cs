using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace MyCRM.API.Controllers
{
    [Route("api/renew")]
    [AllowAnonymous]
    [ApiController]
    public class SubscriptionRenewController : ControllerBase
    {
        public SubscriptionRenewController()
        {
            //StripeConfiguration.ApiKey = "sk_test_3SCBM5Mi7KdaT76zZvitqxc200wUoj1PHy";
            StripeConfiguration.ApiKey = "sk_live_qrJZNaDlZ4AdlQVDB8yarMNB003JmZgyBu";
        }

        [HttpGet("{subId}")]
        public async Task<IActionResult> GetSubInfoWithoutLogin(string subId)
        {
            SubscriptionService subscriptionService = new SubscriptionService();
            var sub = await subscriptionService.GetAsync(subId);
            CustomerService customerService = new CustomerService();
            Customer customer = await customerService.GetAsync(sub.CustomerId);
            PaymentMethodService paymentMethodService = new PaymentMethodService();
            PaymentMethod paymentMethod;
            if (customer.InvoiceSettings.DefaultPaymentMethodId != null)
            {
                paymentMethod = await paymentMethodService.GetAsync(customer.InvoiceSettings.DefaultPaymentMethodId);
            }
            else
            {
                paymentMethod = null;
            }

            GetAccountInfoResponse getAccountInfoResponse = new GetAccountInfoResponse();
            long? sum = 0;
            foreach (var s in sub.Items)
            {
                var l = s.Plan.Amount * s.Quantity;
                if (l.HasValue) sum += l.Value;
            }

            getAccountInfoResponse.DueAmount = (long)sum;
            getAccountInfoResponse.CompanyName = customer.Name ?? customer.Email;
            getAccountInfoResponse.Last4Digits = paymentMethod?.Card?.Last4;
            getAccountInfoResponse.CurrentPlan = sub.Plan.Nickname;
            getAccountInfoResponse.NextBillingDate = sub.CanceledAt ?? sub.TrialEnd.GetValueOrDefault();
            getAccountInfoResponse.Email = customer.Email;
            return Ok(getAccountInfoResponse);
        }
    }
}