using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Responses.Dashboard
{
    public class TargetAchieved
    {
        public TargetAndAchievedModel Q1 { get; set; }
        public TargetAndAchievedModel Q2 { get; set; }
        public TargetAndAchievedModel Q3 { get; set; }
        public TargetAndAchievedModel Q4 { get; set; }
    }
}
