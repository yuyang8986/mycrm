using ETLib.Models.QueryResponse;
using MyCRM.Shared.Constants;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.User;
using System;
using System.Threading.Tasks;

namespace MyCRM.Services.Services.AccountUserService
{
    public interface IAccountUserService
    {
        //Task<AppUser> GetCurrentApplicationUserWithData(bool requiredSuperAdmin = false);

        Task<ApplicationUser> GetCurrentApplicationUserWithOutData(bool requiredSuperAdmin = false);

        Task<ApplicationUser> GetCurrentApplicationUserWithPipelines(bool requireSuperAdmin = false);

        //Task<ResponseBaseModel<UserDataResponseModel>> FetchUserDashboardData();

        Task<ApplicationUser> GetUserWithoutNPData(bool requireAdmin = false);

        Task<ApplicationUser> GetUserWithEmployeeOrganizationData(bool requireSuperAdmin = false);

        Task<ApplicationUser> GetUserWithEmployeeOrganizationActivitiesData(bool requireAdmin = false);

        Task<ApplicationUser> GetUserWithEmployeeOrganizationStagesPipelinesData(bool requireAdmin = false);

        Task<ApplicationUser> GetUserWithOrganizationTemplateData(bool requireSuperAdmin = false);

        Task<ResponseBaseModel<ApplicationUser>> AddNewOrganizationToUserAccount(Organization organization, ApplicationUser user);

        Task<ResponseBaseModel<ApplicationUser>> SetSubscriptionPlan(ApplicationUser user, SubscriptionPlan plan, DateTime subscriptionEndDate);

        Task<bool> Save();

        //Task<ResponseBaseModel<AppUser>> GetCurrentUserBasicInfo();

        Task<ApplicationUser> GetCurrentUserWithEmployeAllEvents();
    }
}