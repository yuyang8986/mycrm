using System.Threading.Tasks;

namespace MyCRM.Service.Repository.EmployeeRepository
{
    public interface IEmployeeRepository : IRepository<Core.Management.Employee, int>
    {
        Task<Employee> GetEmployeesByCompany(Company company);
    }
}