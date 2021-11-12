using System;
using System.ComponentModel.DataAnnotations;

namespace MyCRM.Shared.Communications.Requests.Company
{
    public class CompanyPutRequest
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

        public int? PeopleId { get; set; }

        public Guid PipelineId { get; set; }
    }
}