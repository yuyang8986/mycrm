using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.ScheduleViewModels
{
    public class ScheduleGetModel
    {
        public DateTime EventDateTime { get; set; }
        public AppointmentGetModelForSchedule Appointment { get; set; }
        public EventGetModelForSchedule Event { get; set; }
        public TaskGetModelForSchedule Task { get; set; }

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