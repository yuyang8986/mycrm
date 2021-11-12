using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCRM.Service.Repository
{
    public interface IRepository<T, in TId> where T:class
    {
        ApplicationDbContext Context { get; }
        Task<T> GetById(TId id);
        Task<IEnumerable<T>> GetAll();

        Task Add(T t);
        Task AddAll(IEnumerable<T> ts);

        Task Update(TId id);

        Task<bool> Save();
    }
}