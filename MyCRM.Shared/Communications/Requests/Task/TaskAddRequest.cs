using System;

namespace MyCRM.Shared.Communications.Requests.Task
{
    public class TaskAddRequest
    {
        public string Summary { get; set; }

        public string Note { get; set; }

        public string Location { get; set; }

        public DateTime EventStartDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public Guid ActivityId { get; set; }
        public Guid PipelineId { get; set; }

        public bool IsReminderOn { get; set; }
    }
}