using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.Contact.CompanyViewModel
{
    public class PeopleForPipelineGetModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name => $"{FirstName} {LastName}";

        public bool IsDeleted { get; set; }
    }
}