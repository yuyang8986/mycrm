using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Requests.Event
{
    public class EventAddRequestValidator: AbstractValidator<EventAddRequest>
    {
        public EventAddRequestValidator()
        {
            RuleFor(x => x.EventStartDateTime).NotEmpty();
            RuleFor(x => x.DurationMinutes).NotEmpty();
            RuleFor(x => x.ActivityId).NotNull();
            RuleFor(x => x.CompanyId).NotNull();
            RuleFor(x => x.Summary).MaximumLength(30);
        }
    }
}
