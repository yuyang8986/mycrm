using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Responses.Company
{
    public class CompanyGetModelForPipelineGetAll
    {
        public string Location { get; set; }

        //[JsonIgnore]
        //public ApplicationUser ApplicationUser { get; set; }//a contact person need to be exclusive to the current user
        public string Name { get; set; }

        public string ApplicationUserId { get; set; }

        //public ICollection<People> Peoples { get; set; }//TODO confirm np

        //public ICollection<Pipeline> Pipelines { get; set; }//TODO confirm relationship

        //public ICollection<Appointment> Appointments { get; set; }//TODO confirm relationship

        //public ICollection<Event> Events { get; set; }

        //public ICollection<Task> Tasks { get; set; }

        public bool IsDeleted { get; set; } = false;//when delete, will delete data, instead will mark this as ture
    }
}