using AutoMapper;
using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Tasks;
using MyCRM.Shared.ViewModels.ScheduleViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyCRM.Shared.Communications.Requests.Task;
using Task = MyCRM.Shared.Models.Tasks.Task;

namespace MyCRM.Services.Repository.TaskRepository
{
    public class TaskRepository : RepositoryBase, ITaskRepository
    {
        private readonly IAccountUserService _accountUserService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public TaskRepository(ApplicationDbContext context, IAccountUserService accountUserService, ILogger<TaskRepository> logger, IMapper mapper) : base(context)
        {
            _accountUserService = accountUserService;
            _logger = logger;
            _mapper = mapper;
        }

        public Task<ResponseBaseModel<Task>> GetById(Guid id, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ResponseBaseModel<IEnumerable<TaskGetModelForSchedule>>> GetAll(CancellationToken cancellationToken)
        {
            var user = await _accountUserService.GetCurrentUserWithEmployeAllEvents();
            var tasks = user.Tasks;
            List<TaskGetModelForSchedule> taskGetModels = new List<TaskGetModelForSchedule>();
            foreach (var task in tasks)
            {
                TaskGetModelForSchedule taskGetModel = _mapper.Map<TaskGetModelForSchedule>(task);
                taskGetModels.Add(taskGetModel);
            }

            return ResponseBaseModel<IEnumerable<TaskGetModelForSchedule>>.GetSuccessResponse(taskGetModels);
        }

        public async Task<ResponseBaseModel<Task>> Add(Task task)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();

            ////pipeline can have only on task or appointment at a time
            //if (Context.Tasks.Any(s => s.PipelineId == task.PipelineId))
            //{
            //    _logger.LogWarning(LoggingEvents.InsertItemFailed, "Task already exist in this pipline.");
            //    return ResponseBaseModel<Task>.GetDbSaveFailedResponse();
            //}

            //if (Context.Pipelines.Include(s => s.Appointment).Any(s => s.Id == task.PipelineId && s.Appointment != null))
            //{
            //    _logger.LogWarning(LoggingEvents.InsertItemFailed, "Appointment already exist in this pipline.");
            //    return ResponseBaseModel<Task>.GetDbSaveFailedResponse();
            //}

            task.ApplicationUserId = user.Id;

            Context.Tasks.Add(task);

            return await SaveDbAndReturnReponse(task);
        }

        public Task<ResponseBaseModel<Task>> Update(Guid id, Task request)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBaseModel<Task>> Update(Guid id, TaskPutRequest request)
        {
            var task = await Context.Tasks.FindAsync(id);

            if (task == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Task{id} NOT FOUND", id);
                return ResponseBaseModel<Task>.GetNotFoundResponse();
            }

            task.ActivityId = request.ActivityId;
            task.IsReminderOn = request.IsReminderOn;
            task.PipelineId = request.PipelineId;
            task.Location = request.Location;
            task.Summary = request.Summary;
            task.EventStartDateTime = request.EventStartDateTime;
            task.DurationMinutes = request.DurationMinutes;
            task.Note = request.Note;
            //Context.Entry(request).State = EntityState.Detached;
            Context.Tasks.Update(task);

            return await SaveDbAndReturnReponse(task);
        }

        public async Task<ResponseBaseModel<Task>> ChangeState(Guid id)
        {
            var task = await Context.Tasks.FindAsync(id);

            task.IsCompleted = !task.IsCompleted;

            return await SaveDbAndReturnReponse(task);
        }

        public async Task<ResponseBaseModel<Task>> Delete(Guid id)
        {
            var task = await Context.Tasks.FindAsync(id);

            Context.Tasks.Remove(task);

            return await SaveDbAndReturnReponse(task);
        }

        public Task<ResponseBaseModel<IEnumerable<Task>>> GetAll(CancellationToken cancellationToken, bool includeDeleted = false)
        {
            throw new System.NotImplementedException();
        }

        Task<ResponseBaseModel<IEnumerable<Task>>> IRepository<Task, Guid>.GetAll(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}