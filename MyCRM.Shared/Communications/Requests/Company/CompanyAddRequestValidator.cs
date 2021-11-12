using FluentValidation;

namespace MyCRM.Shared.Communications.Requests.Company
{
    public class CompanyAddRequestValidator : AbstractValidator<CompanyAddRequest>
    {
        public CompanyAddRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}