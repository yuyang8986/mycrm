using System;
using System.Collections.Generic;
using System.Text;
using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.Models.Events;
using MyCRM.Shared.Models.Tasks;

namespace MyCRM.Shared.Communications.Responses.Schedule
{
    public class ScheduleResponseModel
    {
        public List<Models.Appointments.Appointment> Appointments { get; set; }
        public List<Event> Events { get; set; }
        public List<Task> Tasks { get; set; }
    }
}