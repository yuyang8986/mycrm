using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCRM.Core.Management
{
    public class Company:BaseEntity<Company>,ICompany<CompanyOwnedTemplate>
    {
        public Company(string name) : base(name)
        {
        }


        public IEnumerable<CompanyOwnedTemplate> CompanyOwnedTemplate {
            get { return CompanyCompanyTemplates.Select(s => s.CompanyOwnedTemplate).AsEnumerable(); }
        }

        public ICollection<CompanyCompanyOwnedTemplates> CompanyCompanyTemplates { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
