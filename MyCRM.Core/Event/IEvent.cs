using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyCRM.Core.Constants;

namespace MyCRM.Core.Event
{
    interface IEvent<TPhrase,TEvent> where TPhrase:struct 
        where TEvent:class
    {

        Guid Id { get; set; }
        TPhrase CurrentPhrase { get; set; }

        Task<bool> CanChangePhrase();

        Task ChangePhrase(TPhrase eventPhaseTo);
    }
}
