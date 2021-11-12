using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.ViewModels.Contact.PersonViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.Contact.CompanyViewModel
{
    public class CompanyGetModel : CompanyBase
    {
        public CompanyGetModel(string name) : base(name)
        {
        }

        public int Id { get; set; }
        public string Location { get; set; }

        public ApplicatoinUserForCompanyGetModel ApplicationUser { get; set; }//a contact person need to be exclusive to the current user

        public string ApplicationUserId { get; set; }

        public ICollection<PersonGetModel> Peoples { get; set; }//TODO confirm np

        public ICollection<PipelineForCompanyGetModel> Pipelines { get; set; }//TODO confirm relationship

        public bool IsDeleted { get; set; } = false;//when delete, will delete data, instead will mark this as ture
    }
}