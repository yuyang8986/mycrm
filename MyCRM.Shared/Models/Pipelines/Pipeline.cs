using ETLib.Models;
using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.Models.Stages;
using MyCRM.Shared.Models.Tasks;
using MyCRM.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCRM.Shared.Models.Pipelines
{
    /// <summary>
    /// represent a pipeline workflow tracking from an employee serving a potential customer or new customer
    /// </summary>
    [Table("Pipelines")]
    public class Pipeline : IAuditableEntity
    {
        public Pipeline()
        {
        }

        /// <summary>
        /// Create a new pipeline flow assign to employee
        /// </summary>
        /// <param name="applicationUser"></param>
        public Pipeline(ApplicationUser applicationUser) { ApplicationUser = applicationUser; }

        //NP
        public Guid Id { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }//can add later?

        public string DealName { get; set; }
        public double DealAmount { get; set; }

        public Stage Stage { get; set; }
        public int StageId { get; set; }
        public string Type { get; set; }
        public double? CogsAmount { get; set; }
        public double? Margin { get; set; }

        public DateTime? AttainDate { get; set; }
        public string Note { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsStarred { get; set; }

        public ICollection<Appointment> Appointments { get; set; }

        public ICollection<Task> Tasks { get; set; }

        //public Guid? AppointmentId { get; set; }
        public People People { get; set; }

        //this is for display in pipline because json loopreference ignored
        [NotMapped]
        public PersonBase PersonForDisplayInPipeline
        {
            get
            {
                if (People == null) return null;
                return new PersonBase
                {
                    FirstName = People.FirstName,
                    LastName = People.LastName
                };
            }
        }

        //optional, mutual exclusive with company, if set people then will not set company, even can has a indirect company
        public int? PeopleId { get; set; }

        public Company Company { get; set; }
        public int? CompanyId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public DateTime ChangeStageDate { get; set; }
    }
}