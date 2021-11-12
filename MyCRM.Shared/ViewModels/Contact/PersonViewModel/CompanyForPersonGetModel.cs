using MyCRM.Shared.Models.Contacts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.Contact.PersonViewModel
{
    public class CompanyForPersonGetModel : CompanyBase
    {
        public CompanyForPersonGetModel(string name) : base(name)
        {
        }

        public string Location { get; set; }

        public bool IsDeleted { get; set; }

        public ApplicationUserViewModelForPeople ApplicationUser { get; set; }
    }
}