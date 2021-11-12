using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ETLib.Models.QueryResponse;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.Activity;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Activities;

namespace MyCRM.Services.Repository.ActivityRepository
{
    public class ActivityRepository : RepositoryBase, IActivityRepository
    {
        private readonly IAccountUserService _accountUserService;
        private readonly ILogger _logger;

        public ActivityRepository(IAccountUserService accountUserService, ApplicationDbContext context, ILogger<ActivityRepository> logger) : base(context)
        {
            _accountUserService = accountUserService;
            _logger = logger;
        }

        public async Task<ResponseBaseModel<Activity>> Add(Activity activity)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
            //var activities = Context.Activities.Where(x => x.OrganizationId == user.OrganizationId);
            //if (activities.Any(s => s.Name == activity.Name))
            //{
            //    _logger.LogWarning(LoggingEvents.InsertItemFailed, "Activity{name} already exists", activity.Name);
            //    return ResponseBaseModel<Activity>.GetDbSaveFailedResponse();
            //}

            activity.OrganizationId = user.OrganizationId;

            Context.Activities.Add(activity);

            return await SaveDbAndReturnReponse<Activity>(activity);
        }

        public async Task<ResponseBaseModel<Activity>> Delete(Guid id)
        {
            try
            {
                var activity = await Context.Activities.FindAsync(id);
                if (activity == null)
                {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, "Activity({Id}) NOT FOUND", id);
                    return ResponseBaseModel<Activity>.GetNotFoundResponse();
                }

                Context.Activities.Remove(activity);

                return await SaveDbAndReturnReponse(activity);
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.DeleteItemFailed, e, "Delete Acticity({Id}) Failed", id);
                return ResponseBaseModel<Activity>.GetUnexpectedErrorResponse(e);
            }
        }

        public async Task<ResponseBaseModel<IEnumerable<Activity>>> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var user = await _accountUserService.GetUserWithEmployeeOrganizationActivitiesData();
                return ResponseBaseModel<IEnumerable<Activity>>.GetSuccessResponse(user?.Organization?.Activities);
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.ListItemsFailed, e, "List Acticitys Failed");
                return ResponseBaseModel<IEnumerable<Activity>>.GetUnexpectedErrorResponse(e);
            }
        }

        public Task<ResponseBaseModel<IEnumerable<Activity>>> GetAll(CancellationToken cancellationToken, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBaseModel<Activity>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBaseModel<Activity>> GetById(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBaseModel<Activity>> Update(Guid id, Activity request)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBaseModel<Activity>> Update(Guid id, ActivityPutRequest request)
        {
            var activity = await Context.Activities.FindAsync(id);
            if (activity == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "GetById({Id}) NOT FOUND", id);
                return ResponseBaseModel<Activity>.GetNotFoundResponse();
            }

            activity.Name = request.Name;
            activity.ActivityType = request.ActivityType;
            Context.Activities.Update(activity);

            return await SaveDbAndReturnReponse(activity);
        }
    }
}