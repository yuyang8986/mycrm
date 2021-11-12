using System;

namespace MyCRM.Shared.Communications.Requests.Event
{
    public class EventPutRequest
    {
        public string Summary { get; set; }

        public string Note { get; set; }

        public string Location { get; set; }

        public DateTime EventStartDateTime { get; set; }
        public bool IsReminderOn { get; set; }
        public int DurationMinutes { get; set; }
        public Guid ActivityId { get; set; }
        public int CompanyId { get; set; }

        public int OrganizationId { get; set; }
    }
}