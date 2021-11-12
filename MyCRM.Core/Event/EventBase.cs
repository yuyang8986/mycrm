using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCRM.Core.Event
{
    public abstract class EventBase<TEvent,TPhrase>:IEvent<TPhrase,TEvent> 
        where TPhrase : struct
        where TEvent:class
    {
        public Guid Id { get; set; }
        public TPhrase CurrentPhrase { get; set; }

        public bool IsEventLocked { get; set; }
        public abstract Task<bool> CanChangePhrase();
       

        public async Task ChangePhrase(TPhrase eventPhaseTo)
        {
            if(await CanChangePhrase()) throw new Exception("Can not Change Event Status");

            await Task.Run((() => CurrentPhrase = eventPhaseTo));
        }
    }
}
