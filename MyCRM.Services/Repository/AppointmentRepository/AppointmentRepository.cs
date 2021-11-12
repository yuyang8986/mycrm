using AutoMapper;
using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.ViewModels.ScheduleViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Services.Repository.AppointmentRepository
{
    public class AppointmentRepository : RepositoryBase, IAppointmentRepository
    {
        private readonly IAccountUserService _accountUserService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AppointmentRepository(ApplicationDbContext context, IAccountUserService accountUserService, ILogger<AppointmentRepository> logger,IMapper mapper) : base(context)
        {
            _accountUserService = accountUserService;
            _logger = logger;
            _mapper = mapper;
        }

        public Task<ResponseBaseModel<Appointment>> GetById(Guid id, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ResponseBaseModel<IEnumerable<AppointmentGetModelForSchedule>>> GetAll(CancellationToken cancellationToken)
        {
            var user = await _accountUserService.GetCurrentUserWithEmployeAllEvents();
            var appointments = user.Appointments;
            List<AppointmentGetModelForSchedule> appointmentGetModels = new List<AppointmentGetModelForSchedule>();
            foreach (var appointment in appointments)
            {
                AppointmentGetModelForSchedule appointmentGetModel = _mapper.Map<AppointmentGetModelForSchedule>(appointment);
                appointmentGetModels.Add(appointmentGetModel);
            }

            return ResponseBaseModel<IEnumerable<AppointmentGetModelForSchedule>>.GetSuccessResponse(appointmentGetModels);
        }

        public async Task<ResponseBaseModel<Appointment>> Add(Appointment appointment)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();

            appointment.ApplicationUserId = user.Id;

            Context.Appointments.Add(appointment);

            return await SaveDbAndReturnReponse(appointment);
        }

        public async Task<ResponseBaseModel<Appointment>> Update(Guid id, Appointment appointment)
        {
            var appointmentToUpdate = await Context.Appointments.FindAsync(id);

            if (appointmentToUpdate == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Appointment{id} NOT FOUND", id);
                return ResponseBaseModel<Appointment>.GetNotFoundResponse();
            }

            appointmentToUpdate.ActivityId = appointment.ActivityId;
            appointmentToUpdate.IsReminderOn = appointment.IsReminderOn;
            appointmentToUpdate.PipelineId = appointment.PipelineId;
            appointmentToUpdate.Location = appointment.Location;
            appointmentToUpdate.Summary = appointment.Summary;
            appointmentToUpdate.EventStartDateTime = appointment.EventStartDateTime;
            appointmentToUpdate.Note = appointment.Note;
            Context.Entry(appointment).State = EntityState.Detached;
            Context.Appointments.Update(appointmentToUpdate);

            return await SaveDbAndReturnReponse(appointmentToUpdate);
        }

        public async Task<ResponseBaseModel<Appointment>> ChangeState(Guid id)
        {
            var appointment = await Context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Appointment{id} NOT FOUND", id);
                return ResponseBaseModel<Appointment>.GetNotFoundResponse();
            }
            appointment.IsCompleted = true;

            return await SaveDbAndReturnReponse(appointment);
        }

        public async Task<ResponseBaseModel<Appointment>> Delete(Guid id)
        {
            var appointment = await Context.Appointments.FindAsync(id);

            Context.Appointments.Remove(appointment);

            return await SaveDbAndReturnReponse(appointment);
        }

        public Task<ResponseBaseModel<IEnumerable<Appointment>>> GetAll(CancellationToken cancellationToken, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        Task<ResponseBaseModel<IEnumerable<Appointment>>> IRepository<Appointment, Guid>.GetAll(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}