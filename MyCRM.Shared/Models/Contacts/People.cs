using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.User;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCRM.Shared.Models.Contacts
{
    /// <summary>
    /// client
    /// </summary>
    public class People : PersonBase
    {
        public People()
        {
        }

        public People(bool isCustomer)
        {
            IsCustomer = isCustomer;
        }

        public bool IsCustomer { get; set; }

        //NP
        public Company Company { get; set; }

        //people does not directly np to applicationUser
        [NotMapped]
        public ApplicationUser ApplicationUser => Company?.ApplicationUser;

        public int CompanyId { get; set; }

        //no need for this as we know company should be linked to a employee and a people should always link to a company
        //[JsonIgnore]
        //public Employee Employee { get; set; }//a contact person need to be exclusive to the current user

        //public int EmployeeId { get; set; }

        public ICollection<Pipeline> Pipelines { get; set; }

        public bool IsDeleted { get; set; } = false;//when delete, will delete data, instead will mark this as ture
    }
}