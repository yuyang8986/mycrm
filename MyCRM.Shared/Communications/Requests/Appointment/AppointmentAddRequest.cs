using System;

namespace MyCRM.Shared.Communications.Requests.Appointment
{
    public class AppointmentAddRequest
    {
        public string Summary { get; set; }

        public string Note { get; set; }

        public string Location { get; set; }

        public DateTime EventStartDateTime { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsReminderOn { get; set; }

        public Guid PipelineId { get; set; }

        public Guid ActivityId { get; set; }
    }
}