using MyCRM.Shared.Models;
using MyCRM.Shared.Models.Activities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.ScheduleViewModels
{
    public class EventGetModelForSchedule : EventBase, IReminder
    {
        public bool IsReminderOn { get; set; }

        public Guid Id { get; set; }
        public ActivityViewModel Activity { get; set; }
        public Guid ActivityId { get; set; }
        public CompanyGetModelForSchedule Company { get; set; }
        public int? CompanyId { get; set; }

        public string Summary { get; set; }
    }
}