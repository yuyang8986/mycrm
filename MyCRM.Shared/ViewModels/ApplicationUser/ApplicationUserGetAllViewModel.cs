using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;
using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.Tasks;
using MyCRM.Shared.ViewModels.Contact.CompanyViewModel;
using MyCRM.Shared.ViewModels.PipelineViewModels;

namespace MyCRM.Shared.ViewModels.ApplicationUser
{
    public class ApplicationUserGetAllViewModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public string LastName { get; set; }

        public string Name => $"{FirstName} {LastName}";

        public bool IsActive { get; set; }

        public ICollection<PipelineGetModelForAppUser> PipeLineFlows { get; set; }

        public IList<PeopleGetModelForAppUser> Peoples { get; set; }

        public ICollection<CompanyGetModelForAppUser> Companies { get; set; }
        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public IList<string> RoleStrings { get; set; }
    }
}