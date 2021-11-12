using System;
using System.Collections.Generic;
using System.Text;
using MyCRM.Shared.Models;

namespace MyCRM.Shared.ViewModels.Contact.CompanyViewModel
{
    public class PeopleForCompanyGetModel : PersonBase
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name => $"{FirstName} {LastName}";

        public ICollection<PipelineForCompanyGetModel> Pipelines { get; set; }

        public bool IsDeleted { get; set; }
    }
}