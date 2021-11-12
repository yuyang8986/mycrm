using MyCRM.Shared.ViewModels.ScheduleViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Responses.Dashboard
{
    public class DashboardResponseModel
    {
        //public OverallSummary WeekOverallSummary { get; set; }
        public List<OverallSummary>  MonthOverallSummary { get; set; }
        public List<OverallSummary> QuarterOverallSummary { get; set; }
        public List<OverallSummary> YearOverallSummary { get; set; }
        public LeadAmountAnalysis LeadAmountAnalysis { get; set; }
        public LeadStarredOverdue LeadStarredOverdue { get; set; }
        public TargetAchieved TargetAchieved { get; set; }
        public Performance Performance { get; set; }
        public List<ScheduleGetModel> TodaySchedule { get; set; }
        public CountSummary CountSummary { get; set; }

    }
}
