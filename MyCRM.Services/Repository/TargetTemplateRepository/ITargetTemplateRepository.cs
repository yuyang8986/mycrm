using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using MyCRM.Shared.Models.TargetTemplate;
using MyCRM.Shared.ViewModels.TargetTemplateViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Services.Repository.TargetTemplateRepository
{
    public interface ITargetTemplateRepository : IRepository<TargetTemplate, Guid>
    {
        Task<ResponseBaseModel<TargetTemplate>> Recover(Guid id);
        Task<ResponseBaseModel<IEnumerable<TargetTemplateGetModel>>> GetAll(CancellationToken cancellationToken);
    }
}
