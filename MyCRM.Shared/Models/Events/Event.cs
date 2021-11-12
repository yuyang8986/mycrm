using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.Models.Pipelines;
using System;
using MyCRM.Shared.Models.Activities;
using MyCRM.Shared.Models.Managements;

namespace MyCRM.Shared.Models.Events
{
    /// <summary>
    /// things gonna happen, not same to task
    /// </summary>
    public class Event : EventBase, IReminder
    {
        public bool IsReminderOn { get; set; }

        public Activity Activity { get; set; }
        public Guid ActivityId { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public Company Company { get; set; }
        public int? CompanyId { get; set; }
    }
}