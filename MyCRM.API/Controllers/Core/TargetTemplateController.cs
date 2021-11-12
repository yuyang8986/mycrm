using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Services.Repository.TargetTemplateRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.TargetTemplate;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.TargetTemplate;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.API.Controllers.Core
{
    [Route("api/targettemplate")]
    [Authorize(Policy = "manager")]
    [ApiController]
    public class TargetTemplateController : BaseController
    {
        private readonly ITargetTemplateRepository _targetTemplateRepository;
        private readonly ILogger _logger;

        public TargetTemplateController(IMapper mapper, ITargetTemplateRepository targetTemplateRepository, ILogger<TargetTemplateController> logger) : base(mapper)
        {
            _targetTemplateRepository = targetTemplateRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.GetItem, "Getting Target Template{id}", id);
            var result = await _targetTemplateRepository.GetById(id, cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all Target Template");
            var result = await _targetTemplateRepository.GetAll(cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(TargetTemplateAddRequest request)
        {
            var target = Mapper.Map<TargetTemplate>(request);
            var result = await _targetTemplateRepository.Add(target);
            _logger.LogInformation(LoggingEvents.InsertItem, "Created Target Template {id}", target.Id);
            return await CheckResultAndReturn(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, TargetTemplatePutRequest request)
        {
            var target = Mapper.Map<TargetTemplate>(request);
            var result = await _targetTemplateRepository.Update(id, target);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Updated Target Template {id}", id);
            return await CheckResultAndReturn(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _targetTemplateRepository.Delete(id);
            _logger.LogInformation(LoggingEvents.DeleteItem, "Deleted Target Template {id}", id);
            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        [Route("recover/{id}")]
        public async Task<IActionResult> Recover(Guid id)
        {
            var result = await _targetTemplateRepository.Recover(id);
            _logger.LogInformation(LoggingEvents.RecoverItem, "Recovered Target Template {id}", id);
            return await CheckResultAndReturn(result);
        }
    }
}