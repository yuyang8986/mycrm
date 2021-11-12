using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using MyCRM.Shared.Communications.Requests.Activity;
using MyCRM.Shared.Models.Activities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCRM.Services.Repository.ActivityRepository
{
   public interface IActivityRepository: IRepository<Activity,Guid>
    {
        Task<ResponseBaseModel<Activity>> Update(Guid id, ActivityPutRequest request);
    }
}
