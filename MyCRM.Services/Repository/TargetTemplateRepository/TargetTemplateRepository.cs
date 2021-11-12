using AutoMapper;
using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.TargetTemplate;
using MyCRM.Shared.Models.User;
using MyCRM.Shared.ViewModels.TargetTemplateViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Services.Repository.TargetTemplateRepository
{
    public class TargetTemplateRepository : RepositoryBase, ITargetTemplateRepository
    {
        private readonly IAccountUserService _accountUserService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public TargetTemplateRepository(ApplicationDbContext context, IAccountUserService accountUserService, ILogger<TargetTemplateRepository> logger, IMapper mapper) : base(context)
        {
            _accountUserService = accountUserService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseBaseModel<TargetTemplate>> Add(TargetTemplate request)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
            //var targetTemplates = Context.TargetTemplates.Where(x => x.OrganizationId == request.OrganizationId);
            //if (targetTemplates.Any(s => s.Name == request.Name))
            //{
            //    _logger.LogWarning(LoggingEvents.InsertItemFailed, "TargetTemplate{name} already exists", request.Name);
            //    return ResponseBaseModel<TargetTemplate>.GetDbSaveFailedResponse();
            //}

            request.Organization = user.Organization;
            Context.TargetTemplates.Add(request);

            return await SaveDbAndReturnReponse(request);
        }

        public async Task<ResponseBaseModel<TargetTemplate>> Delete(Guid id)
        {
            var target = await Context.TargetTemplates.FindAsync(id);
            if (target == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "TargetTemplate({id}) NOT FOUND.", id);
                return ResponseBaseModel<TargetTemplate>.GetNotFoundResponse();
            }
            target.IsArchive = true;
            Context.Update(target);
            return await SaveDbAndReturnReponse(target);
        }

        public async Task<ResponseBaseModel<TargetTemplate>> Recover(Guid id)
        {
            var target = await Context.TargetTemplates.FindAsync(id);
            if (target == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "TargetTemplate({id}) NOT FOUND.", id);
                return ResponseBaseModel<TargetTemplate>.GetNotFoundResponse();
            }
            target.IsArchive = false;
            Context.Update(target);
            return await SaveDbAndReturnReponse(target);
        }

        public async Task<ResponseBaseModel<IEnumerable<TargetTemplateGetModel>>> GetAll(CancellationToken cancellationToken)
        {
            var user = await _accountUserService.GetUserWithOrganizationTemplateData();
            var targets = user.Organization.TargetTemplates;
            List<TargetTemplateGetModel> targetTemplates = new List<TargetTemplateGetModel>();
            foreach (var target in targets)
            {
                var employeesNotInTemplate = user.Organization?.ApplicationUsers?.Where(s => s.TargetTemplateId != target.Id).ToList();
                target.EmployeesNotInThisTemplate = employeesNotInTemplate;
                TargetTemplateGetModel targetTemplateGetModel = _mapper.Map<TargetTemplateGetModel>(target);
                targetTemplates.Add(targetTemplateGetModel);
            }
            return ResponseBaseModel<IEnumerable<TargetTemplateGetModel>>.GetSuccessResponse(targetTemplates);
        }

        public Task<ResponseBaseModel<IEnumerable<TargetTemplate>>> GetAll(CancellationToken cancellationToken, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBaseModel<TargetTemplate>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var target = await Context.TargetTemplates.Include(x => x.Employees).Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (target == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "TargetTemplate({id}) NOT FOUND.", id);
                return ResponseBaseModel<TargetTemplate>.GetNotFoundResponse();
            }
            return ResponseBaseModel<TargetTemplate>.GetSuccessResponse(target);
        }

        public async Task<ResponseBaseModel<TargetTemplate>> Update(Guid id, TargetTemplate request)
        {
            var target = await Context.TargetTemplates.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (target == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "TargetTemplate({id}) NOT FOUND.", id);
                return ResponseBaseModel<TargetTemplate>.GetNotFoundResponse();
            }
            target.Name = request.Name;
            target.Q1 = request.Q1;
            target.Q2 = request.Q2;
            target.Q3 = request.Q3;
            target.Q4 = request.Q4;

            Context.Update(target);

            return await SaveDbAndReturnReponse<TargetTemplate>(target);
        }

        Task<ResponseBaseModel<IEnumerable<TargetTemplate>>> IRepository<TargetTemplate, Guid>.GetAll(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //public async Task<ResponseBaseModel<IEnumerable<ApplicationUser>>> GetEmployeesNotInTemplate(Guid templateId, CancellationToken cancellationToken)
        //{
        //    var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
        //    var allEmployees = user.Organization.ApplicationUsers;
        //    return ResponseBaseModel<IEnumerable<ApplicationUser>>.GetSuccessResponse(allEmployees.Where(x => x.TargetTemplateId != templateId));
        //}
    }
}