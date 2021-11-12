using MyCRM.Shared.ViewModels.StageViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCRM.Shared.ViewModels.PipelineViewModels
{
    public class PipelineGetAllModel
    {
        public Guid Id { get; set; }

        public ApplicationUserForStage ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }//can add later?

        public string DealName { get; set; }
        public double DealAmount { get; set; }

        public Models.Stages.Stage Stage { get; set; }
        public int StageId { get; set; }
        public double EstimatedCost { get; set; }
        public DateTime? AttainDate { get; set; }
        public string Note { get; set; }
        public string Type { get; set; }
        public double? CogsAmount { get; set; }
        public double? Margin { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsStarred { get; set; }

        public NextActivity NextActivity { get; set; }

        //public ICollection<AppointmentGetModelForPipeline> Appointments { get; set; }

       // public ICollection<TaskGetModelForPipeline> Tasks { get; set; }

        //public Guid? AppointmentId { get; set; }
        public PeopleGetModelForPipeline People { get; set; }

        //this is for display in pipline because json loopreference ignored
       
        //optional, mutual exclusive with company, if set people then will not set company, even can has a indirect company
        public int? PeopleId { get; set; }

        public CompanyGetModelForPipeline Company { get; set; }
        public int? CompanyId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public int StayedTime { get; set; }
    }
}