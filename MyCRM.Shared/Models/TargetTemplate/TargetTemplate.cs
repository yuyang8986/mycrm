using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MyCRM.Shared.Models.TargetTemplate
{
    public class TargetTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Q1 { get; set; }
        public double Q2 { get; set; }
        public double Q3 { get; set; }
        public double Q4 { get; set; }
        public bool IsArchive { get; set; } = false;
        public Organization Organization { get; set; }
        public int OrganizationId { get; set; }
        public ICollection<ApplicationUser> Employees { get; set; }

        [NotMapped]
        public IList<ApplicationUser> EmployeesNotInThisTemplate { get; set; }
    }
}