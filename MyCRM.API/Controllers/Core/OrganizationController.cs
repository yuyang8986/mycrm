using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Services.Repository.OrganizationRepository;
using MyCRM.Shared.Logging;

namespace MyCRM.API.Controllers.Core
{
    [Route("api/organization")]
    [ApiController]
    public class OrganizationController : BaseController
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ILogger _logger;
        public OrganizationController(IMapper mapper, IOrganizationRepository organizationRepository,ILogger<OrganizationController> logger):base(mapper)
        {
            _organizationRepository = organizationRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("employeecount")]
        public async Task<IActionResult> GetEmployeeCount(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.GetItem, "Getting EmployeeCount");
            var result = await _organizationRepository.GetEmployeeCount(cancellationToken);
            return await CheckResultAndReturn(result);
        }
    }
}