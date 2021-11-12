using FluentValidation;
using MyCRM.Shared.Communications.Requests.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Requests.TargetTemplate
{
    public class TargetTemplatePutRequestValidator : AbstractValidator<TargetTemplatePutRequest>
    {
        public TargetTemplatePutRequestValidator()
        {
            RuleFor(x => x.Name).NotNull();

        }
    }
}
