using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Services.Repository.PeopleRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.People;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Contacts;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyCRM.API.Controllers.Core
{
    [Route("api/people")]
    [ApiController]
    public class PeopleController : BaseController
    {
        private readonly IPeopleRepository _peopleRepository;
        private readonly ILogger _logger;

        public PeopleController(IMapper mapper, IPeopleRepository peopleRepository, ILogger<PeopleController> logger) : base(mapper)
        {
            _peopleRepository = peopleRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(PeopleAddRequest request)
        {
            var people = Mapper.Map<People>(request);

            //pipeline id is needed to add the pipeline to people
            var result = await _peopleRepository.Add(people, request.CompanyId, request.PipelineId);
            _logger.LogInformation(LoggingEvents.InsertItem, "Created People{id}", people.Id);
            return await CheckResultAndReturn(result);
        }

        [HttpPost]
        [Route("scan")]
        public async Task<IActionResult> ScanPerson(CreatePersonWithCompanyRequest request)
        {
            var result = await _peopleRepository.ScanPerson(request);
            _logger.LogInformation(LoggingEvents.InsertItem, "Created People{id}");
            return await CheckResultAndReturn(result);
        }

        [HttpPost]
        [Route("import")]
        public async Task<IActionResult> ImportPersons([FromBody]ImportRequest requests)
        {
            //pipeline id is needed to add the pipeline to people
            var result = await _peopleRepository.ImportMultiplePersons(requests);
            _logger.LogInformation(LoggingEvents.InsertItem, "Created Peoples");
            return await CheckResultAndReturn(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.GetItem, "Getting People{id}", id);
            var result = await _peopleRepository.GetById(id, cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all People");
            var result = await _peopleRepository.GetAll(cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        [Route("employee")]
        public async Task<IActionResult> GetPeoplesForCurrentEmployee(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all People for current Employee");
            var result = await _peopleRepository.GetPeoplesForCurrentEmployee(cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PeoplePutRequest request)
        {
            var result = await _peopleRepository.Update(id, request);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Updated People{id}", id);
            return await CheckResultAndReturn(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _peopleRepository.Delete(id);
            _logger.LogInformation(LoggingEvents.DeleteItem, "Deleted Employee{id}", id);
            return await CheckResultAndReturn(result);
        }
    }
}