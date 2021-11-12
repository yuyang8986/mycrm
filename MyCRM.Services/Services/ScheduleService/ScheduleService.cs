using AutoMapper;
using ETLib.Models.QueryResponse;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Responses.Schedule;
using MyCRM.Shared.ViewModels.ScheduleViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Services.Services.ScheduleService
{
    public class ScheduleService : IScheduleService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountUserService _accountUserService;
        private readonly IMapper _mapper;

        public ScheduleService(ApplicationDbContext dbContext, IAccountUserService accountUserService, IMapper mapper)
        {
            _dbContext = dbContext;
            _accountUserService = accountUserService;
            _mapper = mapper;
        }

        public async Task<ResponseBaseModel<IEnumerable<ScheduleGetModel>>> GetAllEmployeeEvents(CancellationToken cancellationToken)
        {
            var user = await _accountUserService.GetCurrentUserWithEmployeAllEvents();

            var scheduleEventModels = user.Appointments.Select(model => new ScheduleEventModel { Appointment = model, EventDateTime = model.EventStartDateTime }).ToList();
            scheduleEventModels.AddRange(user.Organization.Events.Select(employeeEvent => new ScheduleEventModel { Event = employeeEvent, EventDateTime = employeeEvent.EventStartDateTime }));
            scheduleEventModels.AddRange(user.Tasks.Select(task => new ScheduleEventModel { Task = task, EventDateTime = task.EventStartDateTime }));
            List<ScheduleGetModel> scheduleGetModels = new List<ScheduleGetModel>();
            foreach (var scheduleEventModel in scheduleEventModels)
            {
                ScheduleGetModel scheduleGetModel = _mapper.Map<ScheduleGetModel>(scheduleEventModel);
                scheduleGetModels.Add(scheduleGetModel);
            }
            return ResponseBaseModel<IEnumerable<ScheduleGetModel>>.GetSuccessResponse(scheduleGetModels);
        }
    }
}