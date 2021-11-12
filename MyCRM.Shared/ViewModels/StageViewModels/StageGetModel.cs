using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.Pipelines;
using Newtonsoft.Json;

namespace MyCRM.Shared.ViewModels.StageViewModels
{
    public class StageGetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? DisplayIndex { get; set; }

        public int? IconIndex { get; set; }

        //public string ThisMonth { get; set; }
        //public string ThisQuater { get; set; }

        [NotMapped]
        public int ThisMonthNumber { get; set; }

        [NotMapped]
        public int ThisQuarterNumber { get; set; }

        //public Organization Organization { get; set; }

        //public int OrganizationId { get; set; }

        public ICollection<PipelinesGetModelForStage> Pipelines { get; set; }

        //public int PipelineCount { get; set; }

        //public ICollection<Appointment> Appointments { get; set; }

        public bool IsDeleted { get; set; }
    }
}