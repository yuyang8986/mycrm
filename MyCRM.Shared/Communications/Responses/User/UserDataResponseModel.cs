using MyCRM.Shared.Communications.Responses.Dashboard;
using MyCRM.Shared.Constants;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyCRM.Shared.Communications.Responses.User
{
    public class UserDataResponseModel
    {
        public ApplicationUser ApplicationUser { get; set; }

        public bool IsEmployeeAccount => ApplicationUser != null;

        public bool IsSuperAdminAccount { get; set; } = false;

        [JsonIgnore]
        public Organization Organization { get; set; }

        public int OrganizationId { get; set; }

        public bool IsSubscribed => SubscriptionPlan != SubscriptionPlan.None;

        public SubscriptionPlan SubscriptionPlan { get; set; } = SubscriptionPlan.None;

        public bool IsTrialUser { get; set; }

        public DateTime? SubscriptionStartDate { get; set; }

        public DateTime? SubscriptionExpirationDate { get; set; }

        public List<Models.Pipelines.Pipeline> PipeLineFlows { get; set; }

        public MainSections MainSections =>
            new MainSections
            {
                BottomSections = DashboardSectionsSettings.GeneralPipelineMainSections,
                MenuSections = DashboardSectionsSettings.GeneralPipelineMenuSections
            };

        public DashboardResponseModel DashboardResponseModel { get; set; }
    }
}