using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Responses.Dashboard
{
    public class OverallSummary
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public int OpenLead { get; set; }
        public double OpenLeadAmount { get; set; }
        public int Won { get; set; }
        public double WonAmount { get; set; }
        public int Lost { get; set; }
        public double LostAmount { get; set; }

        public double SumAmount => OpenLeadAmount + WonAmount + LostAmount;
    }
}