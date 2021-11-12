using ETLib.Models;
using System.ComponentModel.DataAnnotations;

namespace MyCRM.Shared.Models.Contacts
{
    public abstract class CompanyBase : BaseEntity<CompanyBase>
    {
        protected CompanyBase(string name) : base(name)
        {
        }
     

        [EmailAddress]
        public string Email { get; set; } //Primary

        [EmailAddress]
        public string SecondaryEmail { get; set; }

        [Phone]
        public string Phone { get; set; } //Primary

        [Phone]
        public string SecondaryPhone { get; set; } 
    }
}