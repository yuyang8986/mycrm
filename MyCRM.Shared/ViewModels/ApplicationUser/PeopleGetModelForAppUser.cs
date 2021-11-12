using MyCRM.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.ApplicationUser
{
    public class PeopleGetModelForAppUser : PersonBase
    {
        public int Id { get; set; }
        public bool IsCustomer { get; set; }

        public CompanyGetModelForAppUser Company { get; set; }

        public int CompanyId { get; set; }

        public bool IsDeleted { get; set; }

    }
}
