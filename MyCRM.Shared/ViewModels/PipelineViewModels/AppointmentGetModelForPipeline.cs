using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MyCRM.Shared.Models.Activities;
using MyCRM.Shared.Models.User;
using MyCRM.Shared.ViewModels.ScheduleViewModels;

namespace MyCRM.Shared.ViewModels.PipelineViewModels
{
    public class AppointmentGetModelForPipeline
    {
        public bool IsReminderOn { get; set; }

        //public Models.Pipelines.Pipeline Pipeline { get; set; }
        //public Guid? PipelineId { get; set; }

        public ActivityViewModel Activity { get; set; }
        public Guid? ActivityId { get; set; }

        public Guid Id { get; set; }

        public string Summary { get; set; }

        public bool IsOverdue => EventStartDateTime < DateTime.Now;
        public string Note { get; set; }
        public string Location { get; set; }
        public DateTime EventStartDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompleteTime { get; set; }
        //public ApplicationUser ApplicationUser { get; set; }
        //public string ApplicationUserId { get; set; }
    }
}