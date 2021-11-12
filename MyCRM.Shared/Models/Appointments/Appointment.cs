using MyCRM.Shared.Models.Activities;
using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.User;
using System;

namespace MyCRM.Shared.Models.Appointments
{
    public class Appointment : EventBase, IReminder
    {
        public bool IsReminderOn { get; set; }

        public Pipeline Pipeline { get; set; }
        public Guid? PipelineId { get; set; }

        public Activity Activity { get; set; }
        public Guid? ActivityId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }
    }
}