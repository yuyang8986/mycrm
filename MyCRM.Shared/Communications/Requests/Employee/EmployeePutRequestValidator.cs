using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Shared.Communications.Requests.Employee
{
    public class EmployeePutRequestValidator : AbstractValidator<EmployeePutRequest>
    {
        public EmployeePutRequestValidator()
        {
            RuleFor(x => x.FirstName).NotNull();
            RuleFor(x => x.LastName).NotNull();
        }
    }
}
