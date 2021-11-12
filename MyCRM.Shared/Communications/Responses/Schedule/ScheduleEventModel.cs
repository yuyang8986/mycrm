using System;
using System.Collections.Generic;
using System.Text;
using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.Models.Events;
using MyCRM.Shared.Models.Tasks;
using MyCRM.Shared.ViewModels.ScheduleViewModels;

namespace MyCRM.Shared.Communications.Responses.Schedule
{
    /// <summary>
    /// a wrap for one of the event model
    /// </summary>
    public class ScheduleEventModel
    {
        public DateTime EventDateTime { get; set; }
        public Appointment Appointment { get; set; }
        public Event Event { get; set; }
        public Task Task { get; set; }

        public string EventType
        {
            get
            {
                if (Appointment != null) return "Appointment";
                if (Event != null) return "Event";

                if (Task != null) return "Task";

                return "";
            }
        }
    }
}