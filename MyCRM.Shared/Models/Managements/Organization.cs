using ETLib.Extensions;
using ETLib.Models;
using MyCRM.Shared.Constants;
using MyCRM.Shared.Models.Activities;
using MyCRM.Shared.Models.Events;
using MyCRM.Shared.Models.Stages;
using MyCRM.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCRM.Shared.Models.Managements
{
    /// <summary>
    /// internal
    /// </summary>
    public class Organization : BaseEntity<Organization>
    {
        public Organization(string name) : base(name)
        {
        }

        //public IEnumerable<CompanyOwnedTemplate> CompanyOwnedTemplate {
        //    get { return CompanyCompanyTemplates.Select(s => s.CompanyOwnedTemplate).AsEnumerable(); }
        //}

        //public ICollection<CompanyCompanyOwnedTemplates> CompanyCompanyTemplates { get; set; } //Company Owned PipelineType Templates

        public ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public ICollection<Event> Events { get; set; }

        public ICollection<Stage> Stages { get; set; }

        public ICollection<Activity> Activities { get; set; }
        public ICollection<TargetTemplate.TargetTemplate> TargetTemplates { get; set; }

        public bool IsDeleted { get; set; } = false;

        //[NotMapped]
        //public bool IsSubscribed => SubscriptionPlan != SubscriptionPlan.None;

        public SubscriptionPlan SubscriptionPlan { get; set; }

        public Guid? ReferralCode { get; set; }

        //public bool IsFreeTrail { get; set; }

        public DateTime? SubscriptionStartDate { get; set; }

        /// <summary>
        /// used for when user cancelled subscription and set this next billing date
        /// </summary>
        public DateTime? SubscriptionExpirationDate { get; set; }

        public string StripeCustomerId { get; set; }

        public string StripeSubscriptionId { get; set; }

        public int SubscriptionQuantity { get; set; }

        [NotMapped]
        public bool IsSubExpired => SubscriptionExpirationDate < DateTime.Now;

        [NotMapped]
        public bool IsLoginDisabled => SubscriptionExpirationDate < DateTime.Now;

        [NotMapped]
        public bool IsSubAboutToExpire => SubscriptionExpirationDate < DateTime.Now.AddDays(+7);

        [NotMapped]
        public static List<Stage> DefaultStages
        {
            get
            {
                var stages = new List<Stage>
                {
                    new Stage
                    {
                        Name = DefaultPipeLineFlowStages.LeadIn.ToDescriptionString(),
                        //ThisMonth = DefaultStageSummaryNames.ThisMonth,
                        //ThisQuater = DefaultStageSummaryNames.ThisQuarter,
                        DisplayIndex = 1
                    },
                    //new Stage
                    //{
                    //    Name = DefaultPipeLineFlowStages.Contact.ToDescriptionString(),
                    //    PrimarySummaryName = DefaultStageSummaryNames.New,
                    //    SecondarySummaryName = DefaultStageSummaryNames.ThisWeek,
                    //    DisplayIndex = 2
                    //},
                    new Stage
                    {
                        Name = DefaultPipeLineFlowStages.Appointment.ToDescriptionString(),
                        //ThisMonth = DefaultStageSummaryNames.Upcoming,
                        //ThisQuater = DefaultStageSummaryNames.Completed,
                        DisplayIndex = 2
                    },
                    //new Stage
                    //{
                    //    Name = DefaultPipeLineFlowStages.Evaluation.ToDescriptionString(),
                    //    PrimarySummaryName = DefaultStageSummaryNames.ThisMonth,
                    //    SecondarySummaryName = DefaultStageSummaryNames.ThisQuarter,
                    //    DisplayIndex = 4
                    //},
                    //new Stage
                    //{
                    //    Name = DefaultPipeLineFlowStages.Prospect.ToDescriptionString(),
                    //    PrimarySummaryName = DefaultStageSummaryNames.ThisMonth,
                    //    SecondarySummaryName = DefaultStageSummaryNames.ThisQuarter,
                    //    DisplayIndex = 5
                    //},
                    //new Stage
                    //{
                    //    Name = DefaultPipeLineFlowStages.NeedsDefined.ToDescriptionString(),
                    //    PrimarySummaryName = DefaultStageSummaryNames.ThisMonth,
                    //    SecondarySummaryName = DefaultStageSummaryNames.ThisQuarter,
                    //    DisplayIndex = 6
                    //},
                    new Stage
                    {
                        Name = DefaultPipeLineFlowStages.Proposal.ToDescriptionString(),
                        //ThisMonth = DefaultStageSummaryNames.New,
                        //ThisQuater = DefaultStageSummaryNames.Sent,

                        DisplayIndex = 3
                    },
                    //new Stage
                    //{
                    //    Name = DefaultPipeLineFlowStages.NegotiationsStarted.ToDescriptionString(),
                    //    PrimarySummaryName = DefaultStageSummaryNames.Started,
                    //    SecondarySummaryName = DefaultStageSummaryNames.Success,
                    //    ThirdSummaryName = DefaultStageSummaryNames.Unsuccess,
                    //    DisplayIndex = 8
                    //},
                    //new Stage
                    //{
                    //    Name = DefaultPipeLineFlowStages.NeedsFulfilled.ToDescriptionString(),
                    //    PrimarySummaryName = DefaultStageSummaryNames.Success,
                    //    SecondarySummaryName = DefaultStageSummaryNames.Unsuccess,
                    //    DisplayIndex = 9
                    //},
                    //new Stage
                    //{
                    //    Name = DefaultPipeLineFlowStages.VerbalAgreement.ToDescriptionString(),
                    //    PrimarySummaryName = DefaultStageSummaryNames.ThisMonth,
                    //    SecondarySummaryName = DefaultStageSummaryNames.ThisQuarter,
                    //    DisplayIndex = 10
                    //},
                    new Stage
                    {
                        Name = DefaultPipeLineFlowStages.Quotation.ToDescriptionString(),
                        //ThisMonth = DefaultStageSummaryNames.New,
                        //ThisQuater = DefaultStageSummaryNames.InProgess,

                        DisplayIndex = 4
                    },
                    new Stage
                    {
                        Name = DefaultPipeLineFlowStages.Won.ToDescriptionString(),
                        //ThisMonth = DefaultStageSummaryNames.ThisMonth,
                        //ThisQuater = DefaultStageSummaryNames.ThisQuarter,
                        DisplayIndex = 5
                    },
                    new Stage
                    {
                        Name = DefaultPipeLineFlowStages.Lost.ToDescriptionString(),
                        //ThisMonth = DefaultStageSummaryNames.ThisMonth,
                        //ThisQuater = DefaultStageSummaryNames.ThisQuarter,
                        DisplayIndex = 6
                    },
                    new Stage
                    {
                        Name = DefaultPipeLineFlowStages.Closed.ToDescriptionString(),
                        //ThisMonth = DefaultStageSummaryNames.ThisMonth,
                        //ThisQuater = DefaultStageSummaryNames.ThisQuarter,
                        DisplayIndex = 7
                    },
                    //new Stage
                    //{
                    //    Name = DefaultPipeLineFlowStages.Invoiced.ToDescriptionString(),
                    //    PrimarySummaryName = DefaultStageSummaryNames.ThisMonth,
                    //    SecondarySummaryName = DefaultStageSummaryNames.ThisQuarter,
                    //    DisplayIndex = 15
                    //},
                    //new Stage
                    //{
                    //    Name = DefaultPipeLineFlowStages.Payment.ToDescriptionString(),
                    //    PrimarySummaryName = DefaultStageSummaryNames.ThisMonth,
                    //    SecondarySummaryName = DefaultStageSummaryNames.ThisQuarter,
                    //    ThirdSummaryName = DefaultStageSummaryNames.FollowUp,
                    //    DisplayIndex = 16
                    //}
                };
                return stages;
            }
        }
    }
}