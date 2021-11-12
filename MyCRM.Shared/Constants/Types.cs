using System.Collections.Generic;
using System.ComponentModel;

namespace MyCRM.Shared.Constants
{
    //TODO DO not delete, may use for default
    //public enum PipeLineFlowType
    //{
    //    [Description("General Pipeline")]
    //    GeneralPipeLine = 1,

    //    [Description("Account Manager")]
    //    AccountManager = 2
    //}

    public enum DefaultPipeLineFlowStages
    {
        [Description("Lead In")]
        LeadIn = 1,

        [Description("Contact")]
        Contact,

        [Description("Appointment")]
        Appointment,

        [Description("Evaluation")]
        Evaluation,

        [Description("Propect")]
        Prospect,

        [Description("Needs Defined")]
        NeedsDefined,

        [Description("Proposal")]
        Proposal,

        [Description("Negotiation")]
        NegotiationsStarted,

        [Description("Needs Fulfilled")]
        NeedsFulfilled,

        [Description("Verbal Agreement")]
        VerbalAgreement,

        [Description("Quotation")]
        Quotation,

        [Description("Won")]
        Won,

        [Description("Lost")]
        Lost,

        [Description("Closed")]
        Closed,

        [Description("Invoiced")]
        Invoiced,

        [Description("Payment")]
        Payment,
    }

    //public enum AccountManagerPipeLineFlowPhrase
    //{
    //    [Description("Contract")]
    //    Contract = 1,
    //    [Description("Requirement")]
    //    Requirement = 2,
    //    [Description("Reminder")]
    //    Reminder = 3,
    //    [Description("Expired")]
    //    Expired = 4,
    //    [Description("Renewal")]
    //    Renewal = 5,
    //    [Description("VerbalAgreement")]
    //    VerbalAgreement = 6,
    //    [Description("Quotation")]
    //    Quotation = 7,
    //    [Description("WonLostClosed")]
    //    WonLostClosed = 8,
    //    [Description("Invoiced")]
    //    Invoiced = 9,
    //    [Description("Payment")]
    //    Payment = 9
    //}

    public static class WonLostClosed
    {
        public const string Won = "Won";
        public const string Lost = "Lost";
        public const string Closed = "Closed";
    }

    public static class CompanyNamePredefined
    {
        public const string ImportedFromPhone = "{Imported From Phone}";
    }

    public enum SubscriptionPlan
    {
        [Description("None")]
        None = 0,

        [Description("Essential")]
        Essential = 1,

        [Description("Advanced")]
        Advanced = 2,

        [Description("Premium")]
        Premium = 3
    }

    /// <summary>
    /// This week, this month, new ,made,total
    /// </summary>
    //public enum DefaultStageSummaryType
    //{
    //    [Description("Started")]
    //    Started,
    //    [Description("Follow Up")]
    //    FollowUp,
    //    [Description("Achieved")]
    //    Achieved,
    //    [Description("UnAchieved")]
    //    UnAchieved,
    //    [Description("High")]
    //    High,
    //    [Description("Low")]
    //    Low,
    //    [Description("New")]
    //    New,
    //    [Description("On Hand")]
    //    OnHand,
    //    [Description("This Month")]
    //    ThisMonth,
    //    [Description("This Week")]
    //    ThisWeek,
    //    [Description("This Quarter")]
    //    ThisQuarter,
    //    [Description("Total")]
    //    Total,
    //    [Description("Upcoming")]
    //    Upcoming,
    //    [Description("Review")]
    //    Review,
    //    [Description("Completed")]
    //    Completed,
    //    [Description("Expired")]
    //    Expired,
    //    [Description("In Progress")]
    //    InProgress,
    //    [Description("Waiting")]
    //    Waiting,
    //    [Description("Sent")]
    //    Sent,
    //    [Description("Unsuccess")]
    //    Unsuccess,
    //    [Description("Made")]
    //    Made,
    //    [Description("Success")]
    //    Success
    //}

    public static class MainSectionType
    {
        public const string Calendar = "Calendar";
        public const string Pipeline = "Pipeline";
        public const string Stages = "Stages";
        public const string Schedule = "Schedule";
        public const string Contact = "Contacts";
        public const string NewsFeed = "News Feed";
    }

    public static class MenuSectionType
    {
        public const string Home = "Home";
        public const string Expense = "Expense";
        public const string Nearby = "Nearby";
        public const string Settings = "Settings";
        public const string OfflineMode = "OfflineMode";
        public const string Logout = "Logout";
    }

    public static class DefaultStageSummaryNames
    {
        public const string Started = "Started";
        public const string FollowUp = "Follow Up";
        public const string Achieved = "Achieved";
        public const string UnAchieved = "UnAchieved";
        public const string High = "High";
        public const string Low = "Low";
        public const string New = "New";
        public const string OnHand = "OnHand";
        public const string ThisMonth = "This Month";
        public const string ThisWeek = "This Week";
        public const string ThisQuarter = "This Quarter";
        public const string Total = "Total";
        public const string Upcoming = "Upcoming";
        public const string Review = "Review";
        public const string Completed = "Completed";
        public const string Expired = "Expired";
        public const string InProgess = "In Progress";
        public const string Waiting = "Waiting";
        public const string Sent = "Sent";
        public const string Unsuccess = "Unsuccess";
        public const string Made = "Made";
        public const string Success = "Success";
    }

    public class MainSections
    {
        public List<string> BottomSections { get; set; }
        public List<string> MenuSections { get; set; }
    }

    public static class DashboardSectionsSettings
    {
        public static List<string> GeneralPipelineMainSections => new List<string>
        {
             MainSectionType.Calendar, MainSectionType.Contact,
             MainSectionType.Contact, MainSectionType.NewsFeed,
             MainSectionType.Pipeline, MainSectionType.Schedule,
             MainSectionType.Stages
        };

        public static List<string> GeneralPipelineMenuSections => new List<string>
        {
            MenuSectionType.Home, MenuSectionType.Expense,
            MenuSectionType.Nearby, MenuSectionType.Settings,
            MenuSectionType.OfflineMode, MenuSectionType.Logout
        };
    }
    public static class UploadFileTypes
    {
        public const string DealFiles = "DealFiles";
    }
}