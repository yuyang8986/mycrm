using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.ApplicationUser
{
    public class CompanyGetModelForAppUser
    {
        public int Id { get; set; }
        public string Location { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; } = false;//when delete, will delete data, instead will mark this as ture
    }
}
