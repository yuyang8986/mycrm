using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Services.Services.DashboardService;
using MyCRM.Shared.Logging;

namespace MyCRM.API.Controllers.Core
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger _logger;

        public DashboardController(IMapper mapper, IDashboardService dashboardService, ILogger<DashboardController> logger) : base(mapper)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "List all items");
            var result = await _dashboardService.GetDashboard(cancellationToken);

            return await CheckResultAndReturn(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.GetItem, "Getting Dashboard{id}", id);
            var result = await _dashboardService.GetDashboardById(id, cancellationToken);
            return await CheckResultAndReturn(result);
        }
    }
}