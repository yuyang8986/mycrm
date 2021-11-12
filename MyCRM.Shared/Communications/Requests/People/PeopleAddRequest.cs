using System;
using System.ComponentModel.DataAnnotations;

namespace MyCRM.Shared.Communications.Requests.People
{
    public class PeopleAddRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        public string WorkEmail { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string WorkPhone { get; set; }

        [Phone]
        public string Phone { get; set; }

        public int CompanyId { get; set; }
        public Guid? PipelineId { get; set; }
        public bool IsCustomer { get; set; }

        public string ApplicationUserId { get; set; }
    }
}