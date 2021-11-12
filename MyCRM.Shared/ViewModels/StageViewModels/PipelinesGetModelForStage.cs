using System;
using System.Collections.Generic;
using MyCRM.Shared.ViewModels.PipelineViewModels;
using MyCRM.Shared.ViewModels.StageViewModels;

namespace MyCRM.Shared.ViewModels.StageViewModels
{
    public class PipelinesGetModelForStage
    {
        public Guid Id { get; set; }
        public ApplicationUserForStage ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }//can add later?
        public string DealName { get; set; }
        public double DealAmount { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<AppointmentGetModelForPipeline> Appointments { get; set; }

        public ICollection<TaskGetModelForPipeline> Tasks { get; set; }

        //public Guid? AppointmentId { get; set; }
        public PeopleGetModelForPipeline People { get; set; }

        //public People People { get; set; }
        public CompanyGetModelForStage Company { get; set; }

        //public Appointment Appointment { get; set; }
        public Models.Stages.Stage Stage { get; set; }

        public bool IsStarred { get; set; }
        //public DateTime? NextFollowUpDate { get; set; } //next follow update date for activity

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}