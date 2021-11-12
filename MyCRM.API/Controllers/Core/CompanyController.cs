using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Services.Repository.CompanyRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.Company;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Contacts;
using System.Threading.Tasks;

namespace MyCRM.API.Controllers.Core
{
    [Route("api/company")]
    [ApiController]
    public class CompanyController : BaseController
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger _logger;

        public CompanyController(IMapper mapper,
            ICompanyRepository companyRepository,
            ILogger<CompanyController> logger) : base(mapper)
        {
            _companyRepository = companyRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CompanyAddRequest request)
        {
            var company = Mapper.Map<Company>(request);

            var result = await _companyRepository.Add(company, request.PipelineId, request.PeopleId);
            _logger.LogInformation(LoggingEvents.InsertItem, "Company{Id} Created", company.Id);
            return await CheckResultAndReturn(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.GetItem, "Getting Company{id}", id);
            var result = await _companyRepository.GetById(id, cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        [Route("employee")]
        public async Task<IActionResult> GetForCurrentEmployee(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.GetItem, "Getting Current Employee's Company");
            var result = await _companyRepository.GetAllForCurrentEmployee(cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromQuery] bool includeDeleted = false)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all Company");
            var result = await _companyRepository.GetAll(cancellationToken, includeDeleted);
            return await CheckResultAndReturn(result);
        }

        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    var result = await _companyRepository.GetAll();
        //    return await CheckResultAndReturn(result);
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CompanyPutRequest request)
        {
            var result = await _companyRepository.Update(id, request);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Company{Id} Updated", id);
            return await CheckResultAndReturn(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _companyRepository.Delete(id);
            _logger.LogInformation(LoggingEvents.DeleteItem, "Company{Id} Deleted", id);
            return await CheckResultAndReturn(result);
        }
    }
}