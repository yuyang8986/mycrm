using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Core.Constants
{
   public class Types
    {
        public enum SalesEventPhase
        {
            LeadIn,
            ContactMade,
            Propect,
            NeedsDefined,
            ProposalMade,
            NegotiationsStarted,
            QuotationSent,
            WonLostDecision,
            InvoiceSent
        }
    }
}
