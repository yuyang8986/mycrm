using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence.Data;
using MyCRM.Services.Repository.ActivityRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.Activity;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Activities;

namespace MyCRM.API.Controllers.Core
{
    [Route("api/activity")]
    [ApiController]
    public class ActivityController : BaseController
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ILogger _logger;

        public ActivityController(IActivityRepository activityRepository, IMapper mapper, ILogger<ActivityController> logger) : base(mapper)
        {
            _activityRepository = activityRepository;
            _logger = logger;
        }

        [HttpPost]
        [Authorize("manager")]
        public async Task<IActionResult> Post(ActivityAddRequest request)
        {
            var newActivity = new Activity { Name = request.Name, ActivityType = request.ActivityType };
            _logger.LogInformation(LoggingEvents.InsertItem, "Activity{id} Created", newActivity.Id);
            var response = await _activityRepository.Add(newActivity);
            return await CheckResultAndReturn(response);
        }

        [HttpPut("{id}")]
        [Authorize("manager")]
        public async Task<IActionResult> Put(Guid id, ActivityPutRequest request)
        {
            var response = await _activityRepository.Update(id, request);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Activity{Id} Updated", id);
            return await CheckResultAndReturn(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LoggingEvents.ListItems, "Listing all Activities");
            var result = await _activityRepository.GetAll(cancellationToken);
            return await CheckResultAndReturn(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _activityRepository.Delete(id);
            _logger.LogInformation(LoggingEvents.DeleteItem, "Activity{id} Deleted", id);
            return await CheckResultAndReturn(result);
        }
    }
}