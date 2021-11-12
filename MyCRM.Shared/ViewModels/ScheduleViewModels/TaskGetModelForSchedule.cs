using MyCRM.Shared.Models.Activities;
using System;
using System.Collections.Generic;
using System.Text;
using MyCRM.Shared.Models;

namespace MyCRM.Shared.ViewModels.ScheduleViewModels
{
    public class TaskGetModelForSchedule : EventBase, IReminder
    {
        public string ApplicationUserId { get; set; }

        public ActivityViewModel Activity { get; set; }
        public Guid? ActivityId { get; set; }
        public PipelineGetModelForSchedule Pipeline { get; set; }
        public Guid? PipelineId { get; set; }

        public bool IsReminderOn { get; set; }
    }
}