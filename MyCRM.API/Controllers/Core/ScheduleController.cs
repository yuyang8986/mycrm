using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Services.Services.ScheduleService;
using MyCRM.Shared.Logging;
using System.Threading.Tasks;

namespace MyCRM.API.Controllers.Core
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILogger _logger;

        public ScheduleController(IScheduleService scheduleService,
         IMapper mapper, ILogger<ScheduleController> logger) : base(mapper)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        /// <summary>
        /// get all events, including appointment, event, and tasks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all Employee's Events");
            var result = await _scheduleService.GetAllEmployeeEvents(cancellationToken);
            return await CheckResultAndReturn(result);
        }
    }
}