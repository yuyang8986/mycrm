using MyCRM.Shared.Models.Managements;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyCRM.Shared.Communications.Requests.Employee
{
    public class EmployeePutRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public bool IsManager { get; set; }

        public bool IsActive { get; set; }

        public Guid? TargetTemplateId { get; set; }
    }
}