using System;
using System.Collections.Generic;
using System.Text;
using MyCRM.Shared.Models.Activities;

namespace MyCRM.Shared.Communications.Requests.Activity
{
    public class ActivityAddRequest
    {
        public string Name { get; set; }
        public ActivityType ActivityType { get; set; }
    }
}