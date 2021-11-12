using MyCRM.Shared.Models;
using MyCRM.Shared.Models.Activities;
using System;
using System.Collections.Generic;
using System.Text;
using MyCRM.Shared.ViewModels.ScheduleViewModels;

namespace MyCRM.Shared.ViewModels.PipelineViewModels

{
    public class TaskGetModelForPipeline : EventBase, IReminder
    {
        public string ApplicationUserId { get; set; }

        public ActivityViewModel Activity { get; set; }
        public Guid? ActivityId { get; set; }

        public Guid? PipelineId { get; set; }

        public bool IsReminderOn { get; set; }
    }
}