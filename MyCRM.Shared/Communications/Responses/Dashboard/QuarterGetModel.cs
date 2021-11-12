using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Responses.Dashboard
{
    public class QuarterGetModel
    {
        public int Q1 { get; set; }
        public int Q2 { get; set; }
        public int Q3 { get; set; }
        public int Q4 { get; set; }
        public double Q1Amount { get; set; }
        public double Q2Amount { get; set; }
        public double Q3Amount { get; set; }
        public double Q4Amount { get; set; }
    }
}
