using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.Contact.PersonViewModel
{
    public class PipelineForPersonGetModel
    {
        public Guid Id { get; set; }
        public string DealName { get; set; }
        public double DealAmount { get; set; }
        public PersonForPipelineGetModel People { get; set; }
        public Models.Stages.Stage Stage { get; set; }
        public int StageId { get; set; }

        public bool IsDeleted { get; set; }
    }
}