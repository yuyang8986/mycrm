using FluentValidation;

namespace MyCRM.Shared.Communications.Requests.Employee
{
    public class EmployeeAddRequestValidator : AbstractValidator<EmployeeAddRequest>
    {
        public EmployeeAddRequestValidator()
        {
            RuleFor(x => x.FirstName).NotNull();
            RuleFor(x => x.LastName).NotNull();
        }
    }
}