using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Requests.Pipeline
{
    public class PiplineAddRequestValidator : AbstractValidator<PipelineAddRequest>
    {
        public PiplineAddRequestValidator()
        {
            RuleFor(x => x.DealName).NotNull();
            RuleFor(x => x.DealAmount).NotEmpty();
            RuleFor(x => x.StageId).NotEmpty();
        }
    }
}