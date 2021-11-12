using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCRM.Service.Repository
{
    public abstract class RepositoryBase<T,TId>:IRepository<T,TId> where T:class
    {
        protected RepositoryBase(ApplicationDbContext context)
        {
            Context = context;
        }
        public ApplicationDbContext Context { get; }
        public abstract Task<T> GetById(TId id);

        public abstract Task<IEnumerable<T>> GetAll();

        public abstract Task Add(T t);

        public abstract Task AddAll(IEnumerable<T> ts);

        public abstract Task Update(TId id);

        public virtual async Task<bool> Save()
        {
            return await Context.SaveChangesAsync()>0;
        }
    }
}
