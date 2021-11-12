using FluentValidation;

namespace MyCRM.Shared.Communications.Requests.Stage
{
    public class StageAddRequestValidator : AbstractValidator<StageAddRequest>
    {
        public StageAddRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.DisplayIndex).NotEmpty();
        }
    }
}