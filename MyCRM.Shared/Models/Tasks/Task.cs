using MyCRM.Shared.Models.Activities;
using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.User;
using System;

namespace MyCRM.Shared.Models.Tasks
{
    /// <summary>
    /// employee task need to be completed
    /// </summary>
    public class Task : EventBase, IReminder
    {
        public ApplicationUser ApplicationUser { get; set; }//assignee
        public string ApplicationUserId { get; set; }

        public Activity Activity { get; set; }
        public Guid? ActivityId { get; set; }
        public Pipeline Pipeline { get; set; }
        public Guid? PipelineId { get; set; }
        
        public bool IsReminderOn { get; set; }
    }
}