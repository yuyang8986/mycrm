using MyCRM.Shared.Constants;
using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.Pipelines;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MyCRM.Shared.Models.Stages
{
    /// <summary>
    /// stages defined for current user belonged company
    /// </summary>
    public class Stage
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? DisplayIndex { get; set; }

        public int? IconIndex { get; set; }

        //public string ThisMonth { get; set; }
        //public string ThisQuater { get; set; }

        //[NotMapped]
        //public int ThisMonthNumber { get; set; }

        //[NotMapped]
        //public int ThisQuaterNumber { get; set; }

        [JsonIgnore]
        public Organization Organization { get; set; }

        public int OrganizationId { get; set; }

        [JsonIgnore]
        public ICollection<Pipeline> Pipelines { get; set; }

        //public int PipelineCount { get; set; }

        //[JsonIgnore]
        //public ICollection<Appointment> Appointments { get; set; }

        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        [JsonIgnore]
        public IList<string> DefaultSummaryNames
        {
            get
            {
                var summaryNames = typeof(DefaultStageSummaryNames).GetFields();
                return summaryNames.Select(s => s.GetValue(s).ToString()).ToList();
            }
        }
    }
}