using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCRM.Service.Repository.EmployeeRepository
{
    public class EmployeeRepository: IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context)
        {
            Context = context;
        }
        public ApplicationDbContext Context { get; }
        public async Task<Employee> GetById(int id)
        {
            return await Context.Employees.FindAsync(id);
        }

        public Task<IEnumerable<Employee>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task Add(Employee t)
        {
            throw new NotImplementedException();
        }

        public Task AddAll(IEnumerable<Employee> ts)
        {
            throw new NotImplementedException();
        }

        public Task Update(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save()
        {
            throw new NotImplementedException();
        }

        public Task<Employee> GetEmployeesByCompany(Company company)
        {
            throw new NotImplementedException();
        }
    }
}
