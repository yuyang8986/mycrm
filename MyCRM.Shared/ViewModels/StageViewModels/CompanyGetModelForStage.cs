using MyCRM.Shared.Models.Contacts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.ViewModels.StageViewModels
{
    public class CompanyGetModelForStage
    {
        public int Id { get; set; }
        public string Location { get; set; }

        public string Name { get; set; }

        public string ApplicationUserId { get; set; }

        public bool IsDeleted { get; set; } = false;//when delete, will delete data, instead will mark this as ture
    }
}