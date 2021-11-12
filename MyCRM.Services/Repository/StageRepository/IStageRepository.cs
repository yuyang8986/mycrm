using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using MyCRM.Shared.Communications.Requests.Stage;
using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.Stages;
using MyCRM.Shared.ViewModels.StageViewModels;

namespace MyCRM.Services.Repository.StageRepository
{
    public interface IStageRepository : IRepository<Stage, int>
    {
        Task<ResponseBaseModel<Stage>> Reorder(StageReorderRequest request);

        Task<ResponseBaseModel<Stage>> Update(int id, StagePutRequest request);

        Task<ResponseBaseModel<Pipeline>> UpdatePipelineStage(Guid id, int stageId);

        Task<ResponseBaseModel<IEnumerable<StageGetModel>>> GetAllWithPipelines(CancellationToken cancellationToken);

        Task<ResponseBaseModel<IEnumerable<StageGetModel>>> GetAllWithPipelinesById(string employeeId, CancellationToken cancellationToken);
    }
}