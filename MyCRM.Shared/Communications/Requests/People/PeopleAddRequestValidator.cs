using FluentValidation;

namespace MyCRM.Shared.Communications.Requests.People
{
    public class PeopleAddRequestValidator : AbstractValidator<PeopleAddRequest>
    {
        public PeopleAddRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();

            //RuleFor(x => x.EmployeeId).NotEmpty();
        }
    }
}