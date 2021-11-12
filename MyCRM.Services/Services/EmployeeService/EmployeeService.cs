using MyCRM.Services.Repository.EmployeeRepository;

namespace MyCRM.Services.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IApplicationUserRepository _applicationUserRepository;

        public EmployeeService(IApplicationUserRepository applicationUserRepository)
        {
            _applicationUserRepository = applicationUserRepository;
        }
    }
}