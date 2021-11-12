using ETLib.Models;
using Microsoft.AspNetCore.Identity;
using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.Tasks;
using MyCRM.Shared.ViewModels.PipelineViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCRM.Shared.Models.User
{
    public class ApplicationUser : IdentityUser, IAuditableEntity

    {
        public ApplicationUser()
        {
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name => $"{FirstName} {LastName}";

        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }

        public ICollection<Pipeline> PipeLineFlows { get; set; }

        //[NotMapped]
        //public IList<EmployeePipelineGetModel> PipelinesForDisplayInEmployee
        //{
        //    get
        //    {
        //        if (PipeLineFlows == null) return null;
        //        List<EmployeePipelineGetModel> employeePipelineGetModels = new List<EmployeePipelineGetModel>();

        //        foreach (var pipeLineFlow in PipeLineFlows)
        //        {
        //            employeePipelineGetModels.Add(new EmployeePipelineGetModel
        //            {
        //                Company = pipeLineFlow.Company,
        //                People = pipeLineFlow.People,
        //                DealAmount = pipeLineFlow.DealAmount,
        //                DealName = pipeLineFlow.DealName,
        //                Id = pipeLineFlow.Id,
        //                IsStarred = pipeLineFlow.IsStarred,
        //                IsDeleted = pipeLineFlow.IsDeleted,
        //                Stage = pipeLineFlow.Stage
        //            });
        //        }

        //        return employeePipelineGetModels;
        //    }
        //}

        public ICollection<Task> Tasks { get; set; }

        /// <summary>
        /// all peoples belongs to organization
        /// </summary>
        /// TODO set checking on role to only allow manager to access
        [NotMapped]
        public IList<People> Peoples
        {
            get
            {
                if (Companies == null) return null;
                if (Companies.Count <= 0) return null;
                var peoples = new List<People>();
                foreach (var company in Companies)
                {
                    if (company.Peoples == null) continue;
                    peoples.AddRange(company.Peoples);
                }
                return peoples;
            }
        }

        public ICollection<Company> Companies { get; set; }
        public ICollection<Appointment> Appointments { get; set; }

        //public ICollection<Event> Events { get; set; }

        public bool IsActive { get; set; } = true;

        public TargetTemplate.TargetTemplate TargetTemplate { get; set; }
        public Guid? TargetTemplateId { get; set; }
        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        [NotMapped]
        public IList<string> RoleStrings { get; set; }

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public string VerifyCode { get; set; }
        public DateTime VerifyExpiredDateTime { get; set; }
    }
}