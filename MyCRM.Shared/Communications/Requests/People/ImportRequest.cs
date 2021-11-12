using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Requests.People
{
    public class ImportRequest
    {
        public List<ImportPersonsRequest> PeopleList { get; set; }
    }
}