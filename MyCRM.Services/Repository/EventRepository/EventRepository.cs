using ETLib.Models.QueryResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Services.Repository.EventRepository
{
    public class EventRepository : RepositoryBase, IEventRepository
    {
        private readonly IAccountUserService _accountUserService;
        private readonly ILogger _logger;

        public EventRepository(ApplicationDbContext context, IAccountUserService accountUserService, ILogger<EventRepository> logger) : base(context)
        {
            _accountUserService = accountUserService;
            _logger = logger;
        }

        public Task<ResponseBaseModel<Event>> GetById(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBaseModel<IEnumerable<Event>>> GetAll(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBaseModel<Event>> Add(Event evt)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();

            evt.OrganizationId = user.OrganizationId;
            Context.Events.Add(evt);

            return await SaveDbAndReturnReponse(evt);
        }

        public async Task<ResponseBaseModel<Event>> Update(Guid id, Event request)
        {
            var evt = await Context.Events.FindAsync(id);

            if (evt == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Event{id} NOT FOUND", id);
                return ResponseBaseModel<Event>.GetNotFoundResponse();
            }

            evt.ActivityId = request.ActivityId;
            evt.CompanyId = request.CompanyId;
            evt.IsReminderOn = request.IsReminderOn;
            evt.Location = request.Location;
            evt.Summary = request.Summary;
            evt.EventStartDateTime = request.EventStartDateTime;
            evt.DurationMinutes = request.DurationMinutes;
            evt.Note = request.Note;
            Context.Entry(request).State = EntityState.Detached;
            Context.Events.Update(evt);

            return await SaveDbAndReturnReponse(evt);
        }

        public async Task<ResponseBaseModel<Event>> Delete(Guid id)
        {
            var evt = await Context.Events.FindAsync(id);

            Context.Events.Remove(evt);

            return await SaveDbAndReturnReponse(evt);
        }

        public Task<ResponseBaseModel<IEnumerable<Event>>> GetAll(CancellationToken cancellationToken, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}