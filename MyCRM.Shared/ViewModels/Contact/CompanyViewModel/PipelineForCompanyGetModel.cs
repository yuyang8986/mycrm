using MyCRM.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCRM.Shared.ViewModels.Contact.CompanyViewModel
{
    public class PipelineForCompanyGetModel
    {
        public Guid Id { get; set; }
        public string DealName { get; set; }
        public double DealAmount { get; set; }

        public Models.Stages.Stage Stage { get; set; }
        public int StageId { get; set; }

        //public Guid? AppointmentId { get; set; }
        public PeopleForPipelineGetModel People { get; set; }

        public int? PeopleId { get; set; }

        public bool IsDeleted { get; set; }
    }
}