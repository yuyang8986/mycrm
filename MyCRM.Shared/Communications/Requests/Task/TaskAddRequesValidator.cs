using FluentValidation;

namespace MyCRM.Shared.Communications.Requests.Task
{
    public class TaskAddRequesValidator : AbstractValidator<TaskAddRequest>
    {
        public TaskAddRequesValidator()
        {
            RuleFor(x => x.Summary).MaximumLength(30);
        }
    }
}