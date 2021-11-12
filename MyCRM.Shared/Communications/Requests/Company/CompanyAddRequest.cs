using System;
using System.ComponentModel.DataAnnotations;

namespace MyCRM.Shared.Communications.Requests.Company
{
    public class CompanyAddRequest
    {
        public string Name { get; set; }
        public string Location { get; set; }

        [EmailAddress]
        public string Email { get; set; } //Primary

        [EmailAddress]
        public string SecondaryEmail { get; set; }

        [Phone]
        public string Phone { get; set; } //Primary

        [Phone]
        public string SecondaryPhone { get; set; }

        public int? PeopleId { get; set; } //optional
        public Guid? PipelineId { get; set; } //optional

        public string ApplicationUserId { get; set; }
    }
}