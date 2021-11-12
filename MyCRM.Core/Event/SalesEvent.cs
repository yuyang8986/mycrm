using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;
using MyCRM.Core.Constants;
using MyCRM.Core.Management;

namespace MyCRM.Core.Event
{
    [Table("Sales")]
    public class SalesEvent:EventBase<SalesEvent, Types.SalesEventPhase>
    {
        
        public SalesEvent()
        {
            CurrentPhrase = Types.SalesEventPhase.LeadIn;
        }

        public SalesEvent(Types.SalesEventPhase salesEventPhase)
        {
            CurrentPhrase = salesEventPhase;
        }






        public Employee Employee { get; set; }
        public override async Task<bool> CanChangePhrase()
        {
            return true;
        }
    }
}
