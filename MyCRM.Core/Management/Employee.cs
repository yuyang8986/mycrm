using System;
using System.Collections.Generic;
using System.Text;
using MyCRM.Core.Event;

namespace MyCRM.Core.Management
{
    public class Employee:BaseEntity<Employee>, IEmployee<EmployeeRole>
    {
        public Employee(string name, bool isSupervisor = false) : base(name)
        {
            IsSupervisor = isSupervisor;
        }



        public bool IsSupervisor { get; set; }

        public EmployeeRole EmployeeRole { get; set; } = EmployeeRole.General;

        public Company Company { get; set; }

        public ICollection<SalesEvent> SalesEvents { get; set; }
    }
}
