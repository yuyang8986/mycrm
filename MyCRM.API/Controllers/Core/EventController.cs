using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCRM.Services.Repository.EventRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.Event;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Events;
using System;
using System.Threading.Tasks;

namespace MyCRM.API.Controllers.Core
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : BaseController
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger _logger;
        public EventController(IMapper mapper, IEventRepository eventRepository,ILogger<EventController>logger) : base(mapper)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, EventPutRequest request)
        {
            var evt = Mapper.Map<Event>(request);
            var result = await _eventRepository.Update(id, evt);
            _logger.LogInformation(LoggingEvents.UpdateItem, "Updated Event{id}", id);
            return await CheckResultAndReturn(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(EventAddRequest request)
        {
            var evt = Mapper.Map<Event>(request);

            var result = await _eventRepository.Add(evt);
            _logger.LogInformation(LoggingEvents.InsertItem, "Insert Event{id}", evt.Id);
            return await CheckResultAndReturn(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var result = await _eventRepository.Delete(id);
            _logger.LogInformation(LoggingEvents.DeleteItem, "Delete Event{id}", id);
            return await CheckResultAndReturn(result);
        }
    }
}