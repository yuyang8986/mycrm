using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.TargetTemplateViewModels
{
    public class TargetTemplateGetModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Q1 { get; set; }
        public double Q2 { get; set; }
        public double Q3 { get; set; }
        public double Q4 { get; set; }
        public bool IsArchive { get; set; } = false;
        //public Organization Organization { get; set; }
        //public int OrganizationId { get; set; }
        public ICollection<ApplicationUserForTargetTemplate> Employees { get; set; }

        public IList<ApplicationUserForTargetTemplate> EmployeesNotInThisTemplate { get; set; }
          
    }
}
