using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Services.Repository.AppointmentRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.Appointment;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Appointments;
using System;
using System.Threading.Tasks;

namespace MyCRM.API.Controllers.Core
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : BaseController
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger _logger;

        public AppointmentController(IMapper mapper, IAppointmentRepository appointmentRepository, ILogger<AppointmentController>logger) : base(mapper)
        {
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AppointmentAddRequest request)
        {
            var appointment = Mapper.Map<Appointment>(request);

            var result = await _appointmentRepository.Add(appointment);
            _logger.LogInformation(LoggingEvents.InsertItem, "Appointment{Id} Created", appointment.Id);

            return await CheckResultAndReturn(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, AppointmentPutRequest request)
        {
            var appointment = Mapper.Map<Appointment>(request);
            var result = await _appointmentRepository.Update(id, appointment);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Appointment{Id} Updated", appointment.Id);
            return await CheckResultAndReturn(result);
        }
        [HttpPut]
        [Route("changeState/{id}")]
        public async Task<IActionResult> Put(Guid id)
        {
            var result = await _appointmentRepository.ChangeState(id);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Appointment{Id} Changed State", id);
            return await CheckResultAndReturn(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _appointmentRepository.Delete(id);
            _logger.LogInformation(LoggingEvents.DeleteItem, "Appointment{Id} Deleted", id);
            return await CheckResultAndReturn(result);
        }
    }
}