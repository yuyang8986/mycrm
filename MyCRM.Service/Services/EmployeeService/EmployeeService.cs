using System;
using System.Threading.Tasks;
using MyCRM.Service.Repository.EmployeeRepository;

namespace MyCRM.Service.Services.EmployeeService
{
    public class EmployeeService:BaseService<Employee>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }


        public Task<bool> IsEmployeeSalesTargetPassedThisMonth(Employee employee)
        {
            throw new NotImplementedException();
            //var salesEvent = _employeeRepository.
        }
    }
}
