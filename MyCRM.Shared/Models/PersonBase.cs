using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyCRM.Shared.Models
{
    public class PersonBase
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name => $"{FirstName} {LastName}";

        [EmailAddress]
        public string WorkEmail { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string WorkPhone { get; set; }

        [Phone]
        public string Phone { get; set; }
    }
}
