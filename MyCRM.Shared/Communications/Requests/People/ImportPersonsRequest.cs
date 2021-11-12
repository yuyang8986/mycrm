using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MyCRM.Shared.Models;
using MyCRM.Shared.ViewModels.Contact.CompanyViewModel;

namespace MyCRM.Shared.Communications.Requests.People
{
    public class ImportPersonsRequest : PersonBase
    {
        public bool IsCustomer { get; set; }

        //NP
        public string CompanyName { get; set; }

        public int CompanyId { get; set; }

        public bool IsDeleted { get; set; }
    }
}