using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ETLib.Models.QueryResponse;
using MyCRM.Shared.Communications.Responses.Schedule;
using MyCRM.Shared.ViewModels.ScheduleViewModels;

namespace MyCRM.Services.Services.ScheduleService
{
    public interface IScheduleService
    {
        Task<ResponseBaseModel<IEnumerable<ScheduleGetModel>>> GetAllEmployeeEvents(CancellationToken cancellationToken);
    }
}