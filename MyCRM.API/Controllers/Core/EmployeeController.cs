using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Services.Repository.EmployeeRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.Employee;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.API.Controllers.Core
{
    /// <summary>
    /// Employee resource
    /// </summary>
    [Route("api/employee")]
    [Authorize(Policy = "manager")]
    [ApiController]
    public class EmployeeController : BaseController
    {
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly ILogger _logger;

        public EmployeeController(IApplicationUserRepository applicationUserRepository,
            IMapper mapper,
            ILogger<EmployeeController> logger) : base(mapper)
        {
            _applicationUserRepository = applicationUserRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.GetItem, "Getting Employee{id}", id);
            var result = await _applicationUserRepository.GetById(id, cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all Employees}");
            var result = await _applicationUserRepository.GetAll(cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]EmployeeAddRequest request)
        {
            var employee = Mapper.Map<ApplicationUser>(request);
            var result = await _applicationUserRepository.Add(employee, request.IsManager);
            _logger.LogInformation(LoggingEvents.InsertItem, "Created Employee{id}", employee.Id);
            return await CheckResultAndReturn(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [Required]EmployeePutRequest request)
        {
            var employee = Mapper.Map<ApplicationUser>(request);
            var result = await _applicationUserRepository.Update(id, employee, request.IsManager);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Updated Employee{id}", id);
            return await CheckResultAndReturn(result);
        }

        [HttpPut]
        [Route("template/{id}")]
        public async Task<IActionResult> AddEmployeeToTemplate(string id, [FromQuery]Guid templateId)
        {
            var result = await _applicationUserRepository.UpdateEmployeeTemplate(id, templateId);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Added Employee{id} to Template{templateId}", id, templateId);
            return await CheckResultAndReturn(result);
        }

        [HttpDelete]
        [Route("template/{id}")]
        public async Task<IActionResult> RemoveEmployeeFromTemplate(string id)
        {
            var result = await _applicationUserRepository.RemoveEmployeeFromTemplate(id);
            _logger.LogInformation(LoggingEvents.DeleteItem, "Deleted Employee{id} from Template", id);
            return await CheckResultAndReturn(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _applicationUserRepository.Delete(id);
            _logger.LogInformation(LoggingEvents.DeleteItem, "Deleted Employee{id}", id);
            return await CheckResultAndReturn(result);
        }
    }
}