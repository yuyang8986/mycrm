using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ETLib.Models.QueryResponse;

namespace ETLib.Interfaces.Repository
{
    public interface IRepository<T, in TId> where T : class
    {
        Task<ResponseBaseModel<T>> GetById(TId id, CancellationToken cancellationToken);

        Task<ResponseBaseModel<IEnumerable<T>>> GetAll(CancellationToken cancellationToken);

        Task<ResponseBaseModel<IEnumerable<T>>> GetAll(CancellationToken cancellationToken, bool includeDeleted = false);

        Task<ResponseBaseModel<T>> Add(T request);

        Task<ResponseBaseModel<T>> Update(TId id, T request);

        Task<ResponseBaseModel<T>> Delete(TId id);
    }
}