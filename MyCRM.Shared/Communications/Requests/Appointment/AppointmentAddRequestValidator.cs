using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Requests.Appointment
{
    public class AppointmentAddRequestValidator: AbstractValidator<AppointmentAddRequest>
    {
        public AppointmentAddRequestValidator()
        {
            RuleFor(x => x.EventStartDateTime).NotEmpty();
            RuleFor(x => x.DurationMinutes).NotEmpty();
            RuleFor(x => x.PipelineId).NotNull();
            RuleFor(x => x.ActivityId).NotNull();
            RuleFor(x => x.Summary).MaximumLength(30);
        }
    }
}
