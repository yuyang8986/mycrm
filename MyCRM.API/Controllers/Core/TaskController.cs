using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Services.Repository.TaskRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Logging;
using System;
using System.Threading.Tasks;
using MyCRM.Shared.Communications.Requests.Task;
using Task = MyCRM.Shared.Models.Tasks.Task;

namespace MyCRM.API.Controllers.Core
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : BaseController
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger _logger;

        [HttpPost]
        public async Task<IActionResult> Post(Task request)
        {
            var task = Mapper.Map<Task>(request);

            var result = await _taskRepository.Add(task);
            _logger.LogInformation(LoggingEvents.InsertItem, "Created Task{id}", task.Id);
            return await CheckResultAndReturn(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, TaskPutRequest request)
        {
            var result = await _taskRepository.Update(id, request);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Updated Task{id}", id);
            return await CheckResultAndReturn(result);
        }

        [HttpPut]
        [Route("changeState/{id}")]
        public async Task<IActionResult> ChangeState(Guid id)
        {
            var result = await _taskRepository.ChangeState(id);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Changed state for Task{id}", id);
            return await CheckResultAndReturn(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var result = await _taskRepository.Delete(id);
            _logger.LogInformation(LoggingEvents.DeleteItem, "Deleted Task{id}", id);
            return await CheckResultAndReturn(result);
        }

        public TaskController(IMapper mapper, ITaskRepository taskRepository, ILogger<TaskController> logger) : base(mapper)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }
    }
}