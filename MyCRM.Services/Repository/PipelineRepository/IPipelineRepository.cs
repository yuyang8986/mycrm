using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using MyCRM.Shared.Models.Pipelines;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyCRM.Shared.ViewModels.PipelineViewModels;
using MyCRM.Shared.Communications.Requests.Pipeline;

namespace MyCRM.Services.Repository.PipelineRepository
{
    public interface IPipelineRepository : IRepository<Pipeline, Guid>
    {
        Task<ResponseBaseModel<Pipeline>> Add(Pipeline pipeline, string userId, int stageId, int? peopleId,
            int? organizationId);
        Task<ResponseBaseModel<Pipeline>> Update(Guid id, PipelinePutRequest request);
        Task<ResponseBaseModel<Pipeline>> Update(string stageName, Guid id);

        Task<ResponseBaseModel<Pipeline>> LinkPerson(Guid id, int personId);

        Task<ResponseBaseModel<IEnumerable<PipelineGetAllModel>>> GetAll(CancellationToken cancellationToken);

        //Task<ResponseBaseModel<IEnumerable<Pipeline>>> GetAllByFilter(string[] filter);

        Task<ResponseBaseModel<IEnumerable<Pipeline>>> GetAllByStage(string stageName, CancellationToken cancellationToken);

        Task<ResponseBaseModel<Pipeline>> Starred(Guid id);

        Task<ResponseBaseModel<IEnumerable<EmployeePipelineGetModel>>> GetPipelineForEmployee(CancellationToken cancellationToken);
    }
}