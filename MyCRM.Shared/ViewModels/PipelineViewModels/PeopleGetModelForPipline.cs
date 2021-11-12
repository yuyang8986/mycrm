using MyCRM.Shared.Communications.Responses.Company;
using MyCRM.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.PipelineViewModels
{
    public class PeopleGetModelForPipeline : PersonBase
    {
        public int Id { get; set; }
        public bool IsCustomer { get; set; }

        public CompanyGetModelForPipelineGetAll Company { get; set; }

        public int CompanyId { get; set; }

        public bool IsDeleted { get; set; }
    }
}