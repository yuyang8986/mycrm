using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.ViewModels.PipelineViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCRM.Shared.ViewModels.ScheduleViewModels
{
    public class PipelineGetModelForSchedule
    {
        public Guid Id { get; set; }
        public string DealName { get; set; }
        public double DealAmount { get; set; }

        public bool IsDeleted { get; set; }
        public PeopleGetModelForPipeline People { get; set; }

        public int? PeopleId { get; set; }

        public CompanyGetModelForSchedule Company { get; set; }
        public int? CompanyId { get; set; }
       
    }
}
