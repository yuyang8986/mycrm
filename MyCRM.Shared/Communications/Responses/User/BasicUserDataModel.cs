using System;
using System.Collections.Generic;
using System.Text;
using MyCRM.Shared.Constants;

namespace MyCRM.Shared.Communications.Responses.User
{
    public class BasicUserDataModel
    {
        public string Sub { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }

        public SubscriptionPlan SubscriptionPlan { get; set; }

        public bool IsSubExpired { get; set; }

        public bool IsSubAboutToExpire { get; set; }

        public bool IsFreeTrail { get; set; }

        public int EventNumbers { get; set; }

        public bool IsManager { get; set; }

        public bool IsAdmin { get; set; }

        public string SubId { get; set; }
    }
}