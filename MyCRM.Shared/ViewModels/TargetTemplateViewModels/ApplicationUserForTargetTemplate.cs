using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.TargetTemplateViewModels
{
    public class ApplicationUserForTargetTemplate
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name => $"{FirstName} {LastName}";

        public bool IsActive { get; set; }
    }
}