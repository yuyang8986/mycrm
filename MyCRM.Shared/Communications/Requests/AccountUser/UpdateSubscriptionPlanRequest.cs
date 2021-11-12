using MyCRM.Shared.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyCRM.Shared.Communications.Requests.AccountUser
{
    public class UpdateSubscriptionPlanRequest
    {
        [Required]
        public string AccountEmail { get; set; }

        [Required]
        public SubscriptionPlan SubscriptionPlan { get; set; }

        [Required]
        public DateTime SubscriptionExpirationDate { get; set; }
    }
}