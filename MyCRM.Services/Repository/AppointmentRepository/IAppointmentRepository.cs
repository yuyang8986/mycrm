using MyCRM.Shared.Models.Appointments;
using System;
using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using System.Threading.Tasks;
using MyCRM.Shared.ViewModels.ScheduleViewModels;
using System.Collections.Generic;
using System.Threading;

namespace MyCRM.Services.Repository.AppointmentRepository
{
    public interface IAppointmentRepository : IRepository<Appointment, Guid>
    {
        Task<ResponseBaseModel<Appointment>> ChangeState(Guid id);
        Task<ResponseBaseModel<IEnumerable<AppointmentGetModelForSchedule>>> GetAll(CancellationToken cancellationToken);
    }
}