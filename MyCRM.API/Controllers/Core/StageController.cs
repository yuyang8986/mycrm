using System;
using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Services.Repository.StageRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Services.Services.StageService;
using MyCRM.Shared.Communications.Requests.Stage;
using MyCRM.Shared.Models.Stages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MyCRM.Shared.Logging;

namespace MyCRM.API.Controllers.Core
{
    [Route("api/stage")]
    [ApiController]
    public class StageController : BaseController
    {
        private readonly IStageRepository _stageRepository;
        private readonly ILogger _logger;

        public StageController(IMapper mapper, IStageRepository stageRepository, ILogger<StageController> logger) : base(mapper)
        {
            _stageRepository = stageRepository;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Policy = "manager")]
        public async Task<IActionResult> Post(StageAddRequest request)
        {
            var stage = Mapper.Map<Stage>(request);

            var result = await _stageRepository.Add(stage);
            _logger.LogInformation(LoggingEvents.InsertItem, "Created Stage{id}", stage.Id);
            return await CheckResultAndReturn(result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "manager")]
        public async Task<IActionResult> Put(int id, StagePutRequest request)
        {
            var result = await _stageRepository.Update(id, request);
            _logger.LogInformation(LoggingEvents.InsertItem, "Updated Pipeline{id}", id);
            return await CheckResultAndReturn(result);
        }

        /// <summary>
        /// get all stage names only
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all Stages");
            var result = await _stageRepository.GetAllWithPipelines(cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpGet("{employeeId}")]
        [Authorize(Policy = "manager")]
        public async Task<IActionResult> Get(string employeeId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.GetItem, "Getting Stage by Employee{employeeId}", employeeId);
            var result = await _stageRepository.GetAllWithPipelinesById(employeeId, cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        [Route("{pipelineId}/{stageId}")]
        public async Task<IActionResult> UpdatePipelineStage(Guid pipelineId, int stageId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all Stages");
            var result = await _stageRepository.UpdatePipelineStage(pipelineId, stageId);
            return await CheckResultAndReturn(result);
        }

        /// <summary>
        /// get all stage and data for current employee
        /// </summary>
        /// <returns></returns>
        //[HttpGet("employee")]
        //public async Task<IActionResult> GetEmployeeStagesAndData()
        //{
        //    var result = await _stageService.GetStagesWithSummariesData();
        //    return await CheckResultAndReturn(result);
        //}

        [HttpPut("reorder")]
        public async Task<IActionResult> Reorder([FromQuery] int id, [FromQuery]int displayIndex)
        {
            var stageReorderRequest = new StageReorderRequest { DisplayIndex = displayIndex, Id = id };
            var result = await _stageRepository.Reorder(stageReorderRequest);
            return await CheckResultAndReturn(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _stageRepository.Delete(id);
            _logger.LogInformation(LoggingEvents.DeleteItem, "Deleted Stage{id}", id);
            return await CheckResultAndReturn(result);
        }
    }
}