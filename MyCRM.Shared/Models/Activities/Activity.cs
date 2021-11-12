using System;
using System.Collections.Generic;
using System.Text;
using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.Tasks;
using Newtonsoft.Json;

namespace MyCRM.Shared.Models.Activities
{
    /// <summary>
    /// specific for follow up activity
    /// </summary>
    public class Activity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ActivityType ActivityType { get; set; }

        [JsonIgnore]
        public Organization Organization { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Task> Tasks { get; set; }

        public int OrganizationId { get; set; }
    }

    public enum ActivityType
    {
        Appointment = 0,
        Event = 1,
        Task = 2
    }
}