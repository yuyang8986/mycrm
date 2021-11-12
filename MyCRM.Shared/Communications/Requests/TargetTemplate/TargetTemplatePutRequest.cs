using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Requests.TargetTemplate
{
    public class TargetTemplatePutRequest
    {
        public string Name { get; set; }
        public double Q1 { get; set; }
        public double Q2 { get; set; }
        public double Q3 { get; set; }
        public double Q4 { get; set; }
    }
}