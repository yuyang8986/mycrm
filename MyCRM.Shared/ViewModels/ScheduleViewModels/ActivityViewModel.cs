using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.ScheduleViewModels
{
    public class ActivityViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ActivityType ActivityType { get; set; }

        public int OrganizationId { get; set; }
    }
        public enum ActivityType
        {
            Appointment = 0,
            Event = 1,
            Task = 2
        }
    
}
