using MyCRM.Shared.Models.Events;
using System;
using ETLib.Interfaces.Repository;

namespace MyCRM.Services.Repository.EventRepository
{
    public interface IEventRepository : IRepository<Event, Guid>
    {
    }
}