using MyCRM.Shared.Models;
using System.Collections.Generic;

namespace MyCRM.Shared.ViewModels.Contact.PersonViewModel
{
    public class PersonGetModel : PersonBase

    {
        public CompanyForPersonGetModel Company { get; set; }

        public ApplicationUserViewModelForPeople ApplicationUser => Company.ApplicationUser;

        public int CompanyId { get; set; }

        public ICollection<PipelineForPersonGetModel> Pipelines { get; set; }

        public bool IsDeleted { get; set; } = false;//when delete, will delete data, instead will mark this as ture
    }
}