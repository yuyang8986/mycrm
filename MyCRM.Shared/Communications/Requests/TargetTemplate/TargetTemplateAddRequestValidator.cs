using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Requests.TargetTemplate
{
    public class TargetTemplateAddRequestValidator : AbstractValidator<TargetTemplateAddRequest>
    {
        public TargetTemplateAddRequestValidator()
        {
            RuleFor(x => x.Name).NotNull();
            
        }
    }
}
