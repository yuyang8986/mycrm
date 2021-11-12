using System;
using MyCRM.Shared.Models.Contacts;

namespace MyCRM.Shared.ViewModels.PipelineViewModels
{
    public class EmployeePipelineGetModel
    {
        public Guid Id { get; set; }
        public string DealName { get; set; }
        public double DealAmount { get; set; }
        public bool IsDeleted { get; set; }
        public string Type { get; set; }
        public double? CogsAmount { get; set; }
        public double? Margin { get; set; }

        public DateTime AttainDate { get; set; }
        public PeopleGetModelForPipeline People { get; set; }
        public CompanyGetModelForPipeline Company { get; set; }

        //public Appointment Appointment { get; set; }
        public Models.Stages.Stage Stage { get; set; }

        public bool IsStarred { get; set; }
    }
}