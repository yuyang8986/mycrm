using MyCRM.Shared.Models.Stages;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.ApplicationUser
{
    public class PipelineGetModelForAppUser
    {
        public Guid Id { get; set; }
        public string ApplicationUserId { get; set; }//can add later?

        public string DealName { get; set; }
        public double DealAmount { get; set; }

        public Stage Stage { get; set; }
        public int StageId { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsStarred { get; set; }
        public PeopleGetModelForAppUser People { get; set; }

        //this is for display in pipline because json loopreference ignored

        //optional, mutual exclusive with company, if set people then will not set company, even can has a indirect company
        public int? PeopleId { get; set; }

        public CompanyGetModelForAppUser Company { get; set; }
        public int? CompanyId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public int StayedTime { get; set; }
    }
}
