using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Requests.Company
{
    class CompanyPutRequestValidator:AbstractValidator<CompanyPutRequest>
    {
        public CompanyPutRequestValidator()
        {
            RuleFor(s => s.Name).NotEmpty();
        }
    }
}
