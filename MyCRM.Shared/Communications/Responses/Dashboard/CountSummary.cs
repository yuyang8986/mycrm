using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Responses.Dashboard
{
    public class CountSummary
    {
        public int DealCount { get; set; }
        public int AppointmentCount { get; set; }
        public int EventCount { get; set; }
        public int TaskCount { get; set; }
    }
}
