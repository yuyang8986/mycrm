using MyCRM.Shared.Models;
using MyCRM.Shared.Models.Activities;
using MyCRM.Shared.Models.Appointments;
using System;

using System.Text;

namespace MyCRM.Shared.ViewModels.ScheduleViewModels
{
    public class AppointmentGetModelForSchedule : EventBase, IReminder
    {
        public bool IsReminderOn { get; set; }

        public Guid Id { get; set; }
        public PipelineGetModelForSchedule Pipeline { get; set; }
        public Guid? PipelineId { get; set; }

        public ActivityViewModel Activity { get; set; }
        public Guid? ActivityId { get; set; }

        public string ApplicationUserId { get; set; }
        public string Summary { get; set; }
    }
}