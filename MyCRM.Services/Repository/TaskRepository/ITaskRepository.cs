using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using MyCRM.Shared.ViewModels.ScheduleViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyCRM.Shared.Communications.Requests.Task;
using Task = MyCRM.Shared.Models.Tasks.Task;

namespace MyCRM.Services.Repository.TaskRepository
{
    public interface ITaskRepository : IRepository<Task, Guid>
    {
        Task<ResponseBaseModel<Task>> ChangeState(Guid id);

        Task<ResponseBaseModel<IEnumerable<TaskGetModelForSchedule>>> GetAll(CancellationToken cancellationToken);

        Task<ResponseBaseModel<Task>> Update(Guid id, TaskPutRequest request);
    }
}