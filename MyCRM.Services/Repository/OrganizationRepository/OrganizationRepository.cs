using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ETLib.Models.QueryResponse;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Managements;
using System.Linq;
using MyCRM.Persistence;
using MyCRM.Shared.ViewModels.Contact.EmployeeViewModel;

namespace MyCRM.Services.Repository.OrganizationRepository
{
    public class OrganizationRepository : RepositoryBase, IOrganizationRepository
    {
        private readonly ILogger _logger;
        private readonly IAccountUserService _accountUserService;
        private readonly IAccountManager _accountManager;

        public Task<ResponseBaseModel<Organization>> GetById(int id, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseBaseModel<IEnumerable<Organization>>> GetAll(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBaseModel<Organization>> Add(Organization organization)
        {
            Context.Organizations.Add(organization);
            if (!await Save())
            {
                _logger.LogWarning(LoggingEvents.InsertItemFailed, "Create Organization({id}) Failed", organization.Id);
                return ResponseBaseModel<Organization>.GetDbSaveFailedResponse();
            }
            try
            {
                organization.Stages = Organization.DefaultStages;
                foreach (var organizationStage in organization.Stages)
                {
                    organizationStage.OrganizationId = organization.Id;
                }
                Context.Organizations.Update(organization);
                if (!await Save())
                {
                    _logger.LogWarning(LoggingEvents.InsertItemFailed, "Create Organization({id}) Failed", organization.Id);
                    return ResponseBaseModel<Organization>.GetDbSaveFailedResponse();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return ResponseBaseModel<Organization>.GetSuccessResponse(organization);
        }
        public async Task<ResponseBaseModel<EmployeeCountViewModel>> GetEmployeeCount(CancellationToken cancellationToken)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
            var roles = await _accountManager.GetUserRolesAsync(user);
            if (roles.Contains("admin"))
            {
                int activeEmployee = user.Organization.ApplicationUsers.Count(x => x.IsActive);
                int totalEmployee = user.Organization.SubscriptionQuantity;
                EmployeeCountViewModel employeeCountView = new EmployeeCountViewModel();
                employeeCountView.ActiveEmployeeCount = activeEmployee;
                employeeCountView.TotalEmployeeCount = totalEmployee;
                return ResponseBaseModel<EmployeeCountViewModel>.GetSuccessResponse(employeeCountView);
            }
            else
            {
              return ResponseBaseModel<EmployeeCountViewModel>.GetNotAuthorizedResponse();
            }
        }

        public Task<ResponseBaseModel<Organization>> Update(int id, Organization request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBaseModel<Organization>> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBaseModel<IEnumerable<Organization>>> GetAll(CancellationToken cancellationToken, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public OrganizationRepository(ApplicationDbContext context, ILogger<OrganizationRepository> logger, IAccountUserService accountUserService, IAccountManager accountManager) : base(context)
        {
            _logger = logger;
            _accountUserService = accountUserService;
            _accountManager = accountManager;
        }
    }
}