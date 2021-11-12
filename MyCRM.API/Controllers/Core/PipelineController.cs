using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using MyCRM.Services.Repository.PipelineRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.Pipeline;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Pipelines;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.API.Controllers.Core
{
    /// <summary>
    /// pipe line resource
    /// </summary>
    [Route("api/pipeline")]
    [ApiController]
    public class PipelineController : BaseController
    {
        private readonly IPipelineRepository _pipelineRepository;
        private readonly ILogger _logger;

        public PipelineController(IMapper mapper, IPipelineRepository pipelineRepository, ILogger<PipelineController> logger) : base(mapper)
        {
            _pipelineRepository = pipelineRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Post(PipelineAddRequest request)
        {
            var pipeline = Mapper.Map<Pipeline>(request);

            var result = await _pipelineRepository.Add(pipeline, request.ApplicationUserId, request.StageId, request.PeopleId, request.CompanyId);
            _logger.LogInformation(LoggingEvents.InsertItem, "Created Pipeline{id}", pipeline.Id);

            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all Pipelines");
            var result = await _pipelineRepository.GetAll(cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        [Route("stage")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromQuery] string stageName)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all Pipelines by Stage");
            var result = await _pipelineRepository.GetAllByStage(stageName, cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        [Route("employee")]
        public async Task<IActionResult> GetByEmployee(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all Pipelines by Employee");
            var result = await _pipelineRepository.GetPipelineForEmployee(cancellationToken);
            return await CheckResultAndReturn(result);
        }

        //[HttpGet]
        //[Route("filter")]
        //public async Task<IActionResult> GetByFilter([BindRequired, FromQuery] string[] filter)
        //{
        //    var result = await _pipelineRepository.GetAllByFilter(filter);
        //    return await CheckResultAndReturn(result);
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _pipelineRepository.Delete(id);
            _logger.LogInformation(LoggingEvents.DeleteItem, "Deleted Pipeline{id}", id);
            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        [Route("linkperson/{id}")]
        public async Task<IActionResult> LinkPerson(Guid id, [FromQuery]int personId)
        {
            var result = await _pipelineRepository.LinkPerson(id, personId);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Updated Pipeline{id}", id);
            return await CheckResultAndReturn(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromQuery] string stageName, Guid id, PipelinePutRequest request)
        {

            if (string.IsNullOrWhiteSpace(stageName))
            {
                var result = await _pipelineRepository.Update(id, request);
                _logger.LogInformation(LoggingEvents.UpdateItem, "Updated Pipeline{id}", id);
                return await CheckResultAndReturn(result);
            }
            else
            {
                var result = await _pipelineRepository.Update(stageName, id);
                _logger.LogInformation(LoggingEvents.UpdateItem, "Updated Pipeline{id}", id);
                return await CheckResultAndReturn(result);
            }
        }

        [HttpGet]
        [Route("starred/{id}")]
        public async Task<IActionResult> Starred(Guid id)
        {
            var result = await _pipelineRepository.Starred(id);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Updated Pipeline{id}", id);
            return await CheckResultAndReturn(result);
        }
    }
}