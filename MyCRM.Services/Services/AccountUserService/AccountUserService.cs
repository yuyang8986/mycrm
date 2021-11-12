using AutoMapper;
using ETLib.Models.QueryResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence;
using MyCRM.Persistence.Data;
using MyCRM.Shared.Constants;
using MyCRM.Shared.Exceptions;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.User;
using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace MyCRM.Services.Services.AccountUserService
{
    public class AccountUserService : IAccountUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountManager _accountManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public AccountUserService(UserManager<ApplicationUser> userManager, IAccountManager accountManager,
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AccountUserService> logger)
        {
            _userManager = userManager;
            _accountManager = accountManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApplicationUser> GetUserWithEmployeeOrganizationData(bool requireAdmin = false)
        {
            var (applicationUser, roles) = await GetUserAndRolesByPrincipal();
            await EnsureIsAdmin(requireAdmin, roles);
            var user = await _context.Users
                .Include(s => s.Organization).ThenInclude(s => s.ApplicationUsers).ThenInclude(s => s.Companies).ThenInclude(s => s.Peoples)
                .FirstOrDefaultAsync(s => s.Id == applicationUser.Id);
            user.RoleStrings = roles;
            await EnsureOrganizationNotNull(user);
            return user;
        }

        public async Task<ApplicationUser> GetUserWithEmployeeOrganizationActivitiesData(bool requireAdmin = false)
        {
            var (applicationUser, roles) = await GetUserAndRolesByPrincipal();
            await EnsureIsAdmin(requireAdmin, roles);
            var user = await _context.Users
                .Include(s => s.Organization).
                ThenInclude(s => s.ApplicationUsers).ThenInclude(s => s.Companies).ThenInclude(s => s.Peoples)
                .Include(s => s.Organization).ThenInclude(s => s.Activities)
                .FirstOrDefaultAsync(s => s.Id == applicationUser.Id);
            user.RoleStrings = roles;
            await EnsureOrganizationNotNull(user);
            return user;
        }

        public async Task<ApplicationUser> GetUserWithEmployeeOrganizationStagesPipelinesData(bool requireAdmin = false)
        {
            var (applicationUser, roles) = await GetUserAndRolesByPrincipal();
            await EnsureIsAdmin(requireAdmin, roles);
            var user = await _context.Users
                .Include(s => s.Organization).
                ThenInclude(s => s.ApplicationUsers).ThenInclude(s => s.Companies).ThenInclude(s => s.Peoples)
                .Include(s => s.Organization).ThenInclude(s => s.Stages).ThenInclude(s => s.Pipelines)
                .FirstOrDefaultAsync(s => s.Id == applicationUser.Id);
            user.RoleStrings = roles;
            await EnsureOrganizationNotNull(user);
            return user;
        }

        public async Task<ApplicationUser> GetUserWithoutNPData(bool requireAdmin = false)
        {
            var (applicationUser, roles) = await GetUserAndRolesByPrincipal();
            await EnsureIsAdmin(requireAdmin, roles);
            var user = await _context.Users
                .FirstOrDefaultAsync(s => s.Id == applicationUser.Id);
            user.RoleStrings = roles;
            await EnsureOrganizationNotNull(user);
            return user;
        }

        private Task EnsureIsAdmin(bool requireAdmin, string[] roles)
        {
            if (!requireAdmin || roles.Contains("admin")) return Task.CompletedTask;
            throw new AuthenticationException();
        }

        private async Task<(ApplicationUser, string[] roles)> GetUserAndRolesByPrincipal()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            if (principal == null)
            {
                var e = new AuthenticationException();
                _logger.LogWarning(LoggingEvents.GetItemNotFound, e, "Principal NOT FOUND");
                throw e;
            }

            var appUser = await _userManager.GetUserAsync(principal);
            if (appUser == null)
            {
                var e = new AuthenticationException();
                _logger.LogWarning(LoggingEvents.GetItemNotFound, e, "AppUser NOT FOUND");
                throw e;
            }
            var result = await _accountManager.GetUserAndRolesAsync(appUser.Id);
            return (result.Value.User, result.Value.Roles);
        }

        public async Task<ApplicationUser> GetUserWithOrganizationTemplateData(bool requireSuperAdmin = false)
        {
            var (applicationUser, roles) = await GetUserAndRolesByPrincipal();
            await EnsureIsAdmin(requireSuperAdmin, roles);
            var user = await _context.Users
                .Include(s => s.Organization).ThenInclude(s => s.TargetTemplates)
                .Include(s => s.Organization).ThenInclude(s => s.ApplicationUsers)
                .FirstOrDefaultAsync(s => s.Id == applicationUser.Id);
            user.RoleStrings = roles;
            await EnsureOrganizationNotNull(user);
            return user;
        }

        private async Task EnsureOrganizationNotNull(ApplicationUser applicationUser)
        {
            var organization =
                await _context.Organizations.FirstOrDefaultAsync(s => s.Id == applicationUser.OrganizationId);
            if (organization == null) throw new NotFoundException(nameof(Organization), applicationUser.Id);
        }

        public async Task<ApplicationUser> GetCurrentApplicationUserWithOutData(bool requireSuperAdmin = false)
        {
            var (applicationUser, roles) = await GetUserAndRolesByPrincipal();
            applicationUser.RoleStrings = roles;
            await EnsureIsAdmin(requireSuperAdmin, roles);

            return applicationUser;
        }

        public async Task<ApplicationUser> GetCurrentApplicationUserWithPipelines(bool requireSuperAdmin = false)
        {
            var (applicationUser, roles) = await GetUserAndRolesByPrincipal();
            await EnsureIsAdmin(requireSuperAdmin, roles);
            var user = await _context.Users.Include(s => s.PipeLineFlows)
                .Include(s => s.Organization).ThenInclude(s => s.ApplicationUsers).ThenInclude(s => s.Companies)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == applicationUser.Id);
            user.RoleStrings = roles;
            user.PipeLineFlows = user.PipeLineFlows.Where(s => !s.IsDeleted).ToList();
            await EnsureOrganizationNotNull(user);
            return user;
        }

        /// <summary>
        /// Set another user pipeflow template from a superadmin account
        /// </summary>
        /// <param name="pipeLineFlowType"></param>
        /// <param name="employeeId"></param>
        /// <returns>true/false</returns>

        public async Task<ResponseBaseModel<ApplicationUser>> AddNewOrganizationToUserAccount(Organization organization, ApplicationUser user)
        {
            try
            {
                var (applicationUser, roles) = await GetUserAndRolesByPrincipal();
                await EnsureIsAdmin(true, roles);
                _context.Organizations.Add(organization);
                if (await Save())
                {
                    user.Organization = organization;
                    _context.Users.Update(user);
                    await Save();
                    return ResponseBaseModel<ApplicationUser>.GetSuccessResponse(user);
                }
                _logger.LogWarning(LoggingEvents.InsertItemFailed, "Add New Organization To User Account Failed");
                return ResponseBaseModel<ApplicationUser>.GetDbSaveFailedResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.InsertItemFailed, e, "Add New Organization To User Account Failed");
                return ResponseBaseModel<ApplicationUser>.GetUnexpectedErrorResponse(e);
            }
        }

        public async Task<ResponseBaseModel<ApplicationUser>> SetSubscriptionPlan(ApplicationUser user, SubscriptionPlan plan, DateTime subscriptionEndDate)
        {
            //user.SubscriptionPlan = plan;
            //user.SubscriptionStartDate = DateTime.Now;
            //user.SubscriptionExpirationDate = plan == SubscriptionPlan.Trial ? user.SubscriptionStartDate?.AddDays(30) : subscriptionEndDate;
            //var result = await Save();
            //if (!result) return ResponseBaseModel<AppUser>.GetDbSaveFailedResponse();
            //return ResponseBaseModel<AppUser>.GetSuccessResponse(user);
            throw new NotImplementedException();
        }

        public async Task<bool> Save()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning(LoggingEvents.SaveToDatabaseFailed, "Save to database failed.");
                return false;
            }
        }

        public async Task<ApplicationUser> GetCurrentUserWithEmployeAllEvents()
        {
            try
            {
                var principal = _httpContextAccessor.HttpContext.User;
                if (principal == null)
                {
                    var e = new AuthenticationException();
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, e, "Principal NOT FOUND");
                    throw e;
                }
                var appUser = await _userManager.GetUserAsync(principal);
                if (appUser == null)
                {
                    var e = new AuthenticationException();
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, e, "AppUser NOT FOUND");
                    throw e;
                }
                _context.Entry(appUser).State = EntityState.Detached;
                var user = await _context.Users.
                    Include(s => s.Appointments).ThenInclude(s => s.Activity).
                    Include(s => s.Appointments).ThenInclude(s => s.Pipeline).ThenInclude(s => s.People).ThenInclude(s => s.Company).
                    Include(s => s.Appointments).ThenInclude(s => s.Pipeline).ThenInclude(x => x.Company).

                    Include(s => s.Organization).ThenInclude(s => s.Events).ThenInclude(x => x.Activity).

                    Include(s => s.Tasks).ThenInclude(s => s.Activity).
                    Include(s => s.Tasks).ThenInclude(s => s.Pipeline).ThenInclude(x => x.People).ThenInclude(s => s.Company).
                    Include(s => s.Tasks).ThenInclude(s => s.Pipeline).ThenInclude(x => x.Company)

                    .Include(s => s.Organization).ThenInclude(s => s.Events).ThenInclude(s => s.Company).
                    Include(s => s.PipeLineFlows).
                    FirstOrDefaultAsync(s => s.Id == appUser.Id);
                var roleNames = await _accountManager.GetUserRolesAsync(user);
                user.RoleStrings = roleNames;
                await EnsureOrganizationNotNull(user);
                return user;
            }
            catch (Exception e)
            {
                _logger.LogWarning(LoggingEvents.ListItemsFailed, e, "Get all events for current user failed.");
                throw e;
            }
        }
    }
}