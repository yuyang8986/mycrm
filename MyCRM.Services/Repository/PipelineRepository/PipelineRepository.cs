using ETLib.Models.QueryResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence;
using MyCRM.Persistence.Data;
using MyCRM.Services.Repository.CompanyRepository;
using MyCRM.Services.Repository.EmployeeRepository;
using MyCRM.Services.Repository.PeopleRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.Stages;
using MyCRM.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ETLib.Interfaces.Repository;
using MyCRM.Shared.ViewModels.PipelineViewModels;
using MyCRM.Shared.ViewModels.ScheduleViewModels;
using MyCRM.Shared.Communications.Requests.Pipeline;

namespace MyCRM.Services.Repository.PipelineRepository
{
    public class PipelineRepository : RepositoryBase, IPipelineRepository
    {
        private readonly IAccountUserService _accountUserService;
        private readonly IAccountManager _accountManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PipelineRepository(ApplicationDbContext context, IAccountUserService accountUserService, IAccountManager accountManager,
            IMapper mapper,
            ILogger<PipelineRepository> logger) : base(context)
        {
            _accountUserService = accountUserService;
            _accountManager = accountManager;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<ResponseBaseModel<Pipeline>> GetById(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ResponseBaseModel<IEnumerable<Pipeline>>> IRepository<Pipeline, Guid>.GetAll(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBaseModel<IEnumerable<PipelineGetAllModel>>> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var findPipelinesFunc = await FindPipelinesFunc();

                List<Pipeline> pipelines = Context.Pipelines.IgnoreQueryFilters()
                    .Include(s => s.ApplicationUser)
                    .Include(s => s.People).ThenInclude(z=>z.Company)
                    .Include(x=>x.Company)
                    .Include(x=>x.Stage)
                    .Include(x=>x.Appointments).ThenInclude(x=>x.Activity)
                    .Include(x=>x.Tasks).ThenInclude(x => x.Activity)
                    .Where(findPipelinesFunc)
                    .ToList();

                List<PipelineGetAllModel> pipelineGetAllModels = new List<PipelineGetAllModel>();
                foreach (var pipeline in pipelines)
                {
                    var nextActivity = new NextActivity();
                    var startDateTime = DateTime.MaxValue;
                    if (pipeline.Appointments != null)
                    {
                        foreach (var pipelineAppointment in pipeline.Appointments)
                        {
                            if (startDateTime > pipelineAppointment.EventStartDateTime)
                            {
                                nextActivity.Name = pipelineAppointment.Activity.Name;
                                nextActivity.StartTime = pipelineAppointment.EventStartDateTime;
                            }
                        }
                    }

                    if (pipeline.Tasks != null)
                    {
                        foreach (var task in pipeline.Tasks)
                        {
                            if (startDateTime > task.EventStartDateTime)
                            {
                                nextActivity.Name = task.Activity.Name;
                                nextActivity.StartTime = task.EventStartDateTime;
                            }
                        }
                    }

                    PipelineGetAllModel pipelineGetAllModel = _mapper.Map<PipelineGetAllModel>(pipeline);
                    if (nextActivity.Name != null) pipelineGetAllModel.NextActivity = nextActivity;
                    double ts = DateTime.Now.Subtract(pipeline.ChangeStageDate).TotalDays;
                    pipelineGetAllModel.StayedTime = (int)ts;

                    pipelineGetAllModels.Add(pipelineGetAllModel);
                }

                return ResponseBaseModel<IEnumerable<PipelineGetAllModel>>.GetSuccessResponse(pipelineGetAllModels.OrderByDescending(x => x.CreatedDate));
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.ListItems, e, "List all pipelines failed.");
                return ResponseBaseModel<IEnumerable<PipelineGetAllModel>>.GetUnexpectedErrorResponse(e);
            }
        }

        private async Task<Func<Pipeline, bool>> FindPipelinesFunc()
        {
            var user = await _accountUserService.GetUserWithoutNPData();
            var roles = await _accountManager.GetUserRolesAsync(user);
            Func<Pipeline, bool> findPipelinesFunc;
            if (roles.Contains("manager"))
            {
                findPipelinesFunc = (p) => p.ApplicationUser?.OrganizationId == user.OrganizationId && p.IsDeleted == false;
            }
            else
            {
                findPipelinesFunc = (p) => p.ApplicationUser.Id == user.Id && p.IsDeleted == false;
            }

            return findPipelinesFunc;
        }

        public Task<ResponseBaseModel<Pipeline>> Add(Pipeline request)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBaseModel<Pipeline>> Update(Guid id, PipelinePutRequest request)
        {
            var pipeline = await Context.Pipelines.FindAsync(id);
            _mapper.Map(request, pipeline);
            if (!Context.Pipelines.Any(s => s.Id == id))
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Pipeline({id}) NOT FOUND.", id);
                return ResponseBaseModel<Pipeline>.GetNotFoundResponse();
            }
            pipeline.Id = id;
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
            pipeline.UpdatedBy = user.Name;
            pipeline.UpdatedDate = DateTime.Now;
            Context.Pipelines.Update(pipeline);

            return await SaveDbAndReturnReponse(pipeline);
        }

        public async Task<ResponseBaseModel<Pipeline>> Update(string stageName, Guid id)
        {
            var user = await _accountUserService.GetCurrentApplicationUserWithOutData();
            var pipeline = await Context.Pipelines.FindAsync(id);
            if (pipeline == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Pipeline({id}) NOT FOUND.", id);
                return ResponseBaseModel<Pipeline>.GetNotFoundResponse();
            }
            var stage = Context.Stages.FirstOrDefault(s => s.Name == stageName && s.OrganizationId == user.OrganizationId);
            if (stage == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Stage({id}) NOT FOUND.", id);
                return ResponseBaseModel<Pipeline>.GetNotFoundResponse(typeof(Stage));
            }

            pipeline.Stage = stage;

            pipeline.UpdatedBy = user.Name;
            pipeline.UpdatedDate = DateTime.Now;
            pipeline.ChangeStageDate = DateTime.Now;
            Context.Pipelines.Update(pipeline);

            return await SaveDbAndReturnReponse(pipeline);
        }

        public async Task<ResponseBaseModel<Pipeline>> LinkPerson(Guid id, int personId)
        {
            var pipeline = await Context.Pipelines.FindAsync(id);
            if (pipeline == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Pipeline({id}) NOT FOUND.", id);
                return ResponseBaseModel<Pipeline>.GetNotFoundResponse();
            }

            //remove the company linked
            pipeline.CompanyId = null;
            pipeline.PeopleId = personId;
            var user = await _accountUserService.GetCurrentApplicationUserWithOutData();
            pipeline.UpdatedBy = user.Name;
            pipeline.UpdatedDate = DateTime.Now;
            Context.Pipelines.Update(pipeline);

            return await SaveDbAndReturnReponse(pipeline);
        }

        public async Task<ResponseBaseModel<Pipeline>> Delete(Guid id)
        {
            var pipeline = await Context.Pipelines.FindAsync(id);
            if (pipeline == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Pipeline({id}) NOT FOUND.", id);
                return ResponseBaseModel<Pipeline>.GetNotFoundResponse();
            }

            pipeline.IsDeleted = true;

            if (pipeline.Appointments != null)
            {
                Context.Appointments.RemoveRange(pipeline.Appointments);
                foreach (var pipelineAppointment in pipeline.Appointments)
                {
                    _logger.LogInformation(LoggingEvents.DeleteItem, "Deleted Appointment{id}", pipelineAppointment.Id);
                }
            }

            if (pipeline.Tasks != null)
            {
                Context.Tasks.RemoveRange(pipeline.Tasks);
                foreach (var task in pipeline.Tasks)
                {
                    _logger.LogInformation(LoggingEvents.DeleteItem, "Deleted Appointment{id}", task.Id);
                }
            }

            Context.Pipelines.Update(pipeline);

            return await SaveDbAndReturnReponse(pipeline);
        }

        public async Task<ResponseBaseModel<Pipeline>> Add(Pipeline pipeline, string userId, int stageId, int? peopleId, int? companyId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                //manager action -manual assign
                //var result = await _applicationUserRepository.GetById(userId,cancellationToken);
                //if (result.Success)
                //{
                //    pipeline.ApplicationUserId = result.Model.Id;
                //}
                //else
                //{
                //    _logger.LogWarning(LoggingEvents.GetItemNotFound, "User({id}) NOT FOUND", userId);
                //    return ResponseBaseModel<Pipeline>.GetNotFoundResponse(typeof(ApplicationUser));
                //}
            }
            else
            {
                //auto assign to current normal employee
                var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
                pipeline.ApplicationUserId = user.Id;
                pipeline.CreatedBy = user.Name;
            }

            //add both people and company link is not allowed
            if (peopleId.HasValue && companyId.HasValue)
            {
                _logger.LogWarning(LoggingEvents.SaveToDatabaseFailed, "People and company both exist, Create pipeline({id}) failed.", pipeline.Id);
                return ResponseBaseModel<Pipeline>.GetDbSaveFailedResponse();
            }
            if (!peopleId.HasValue && !companyId.HasValue)
            {
                _logger.LogWarning(LoggingEvents.SaveToDatabaseFailed, "People and company both empty, Create pipeline({id}) failed.", pipeline.Id);
                return ResponseBaseModel<Pipeline>.GetDbSaveFailedResponse();
            }

            if (peopleId.HasValue)
            {
                var people = await Context.Peoples.FindAsync(peopleId);
                if (people != null)
                {
                    pipeline.PeopleId = peopleId;
                }
                else
                {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, "People({id}) NOT FOUND.", peopleId);
                    return ResponseBaseModel<Pipeline>.GetNotFoundResponse(typeof(People));
                }
            }

            if (companyId.HasValue)
            {
                var company = await Context.Companies.FindAsync(companyId);
                if (company != null)
                {
                    pipeline.CompanyId = companyId;
                }
                else
                {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, "Company({id}) NOT FOUND.", companyId);
                    return ResponseBaseModel<Pipeline>.GetNotFoundResponse(typeof(Organization));
                }
            }
            pipeline.CreatedDate = DateTime.Now;
            pipeline.ChangeStageDate = DateTime.Now;

            Context.Pipelines.Add(pipeline);

            return await SaveDbAndReturnReponse(pipeline);
        }

        public Task<ResponseBaseModel<IEnumerable<Pipeline>>> GetAll(CancellationToken cancellationToken, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        //public async Task<ResponseBaseModel<IEnumerable<Pipeline>>> GetAllByFilter(string[] filter)
        //{
        //    var response = await GetAll();

        //    var pipelines = response.Model;

        //    if (filter.Contains("starred"))
        //    {
        //        pipelines = pipelines.Where(s => s.IsStarred);
        //    }

        //    if (filter.Contains("overdue"))
        //    {
        //        pipelines = pipelines.Where(s => s.IsOverdue);
        //    }

        //    if (filter.Contains("todayOnly"))
        //    {
        //        pipelines = pipelines.Where(s =>
        //            {
        //                if (s.Appointment == null) return false;
        //                return s.Appointment.EventStartDateTime.Day == DateTime.Now.Day;
        //            }
        //        );
        //    }

        //    if (filter.Contains("biggestAmount"))
        //    {
        //        pipelines = pipelines.OrderByDescending(s => s.DealAmount);
        //    }

        //    return ResponseBaseModel<IEnumerable<Pipeline>>.GetSuccessResponse(pipelines);
        //}

        public async Task<ResponseBaseModel<IEnumerable<Pipeline>>> GetAllByStage(string stageName, CancellationToken cancellationToken)
        {
            try
            {
                var findPipelinesFunc = await FindPipelinesFunc();

                var pipelines = Context.Pipelines
                    .Include(s => s.ApplicationUser)
                    .Where(findPipelinesFunc).ToList();

                foreach (var pipeline in pipelines)
                {
                    var pipelineEntry = Context.Entry(pipeline);
                    await pipelineEntry.Reference(s => s.People).LoadAsync(cancellationToken);
                    await pipelineEntry.Reference(s => s.Stage).LoadAsync(cancellationToken);
                    await pipelineEntry.Collection(s => s.Appointments).LoadAsync(cancellationToken);
                }

                pipelines = pipelines.Where(s => s.Stage.Name == stageName).ToList();
                return ResponseBaseModel<IEnumerable<Pipeline>>.GetSuccessResponse(pipelines);
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.ListItemsFailed, e, "List pipleline by stage failed.");
                return ResponseBaseModel<IEnumerable<Pipeline>>.GetUnexpectedErrorResponse(e);
            }
        }

        public async Task<ResponseBaseModel<IEnumerable<EmployeePipelineGetModel>>> GetPipelineForEmployee(CancellationToken cancellationToken)
        {
            try
            {
                var user = await _accountUserService.GetUserWithEmployeeOrganizationData();

                var pipelines = await Context.Pipelines
                    .Include(s => s.ApplicationUser)
                    .Where(p => p.ApplicationUserId == user.Id).ToListAsync(cancellationToken);
                List<EmployeePipelineGetModel> employeePipelineGetModels = new List<EmployeePipelineGetModel>();
                foreach (var pipeline in pipelines)
                {
                    var pipelineEntry = Context.Entry(pipeline);
                    await pipelineEntry.Reference(s => s.People).LoadAsync(cancellationToken);
                    await pipelineEntry.Reference(s => s.Stage).LoadAsync(cancellationToken);
                    await pipelineEntry.Collection(s => s.Appointments).LoadAsync(cancellationToken);
                    EmployeePipelineGetModel employeePipelineGetModel = _mapper.Map<EmployeePipelineGetModel>(pipeline);
                    employeePipelineGetModels.Add(employeePipelineGetModel);
                }

                return ResponseBaseModel<IEnumerable<EmployeePipelineGetModel>>.GetSuccessResponse(employeePipelineGetModels.OrderBy(x => x.DealName));
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.ListItemsFailed, e, "List pipleline by for current employee failed.");
                return ResponseBaseModel<IEnumerable<EmployeePipelineGetModel>>.GetUnexpectedErrorResponse(e);
            }
        }

        public async Task<ResponseBaseModel<Pipeline>> Starred(Guid id)
        {
            var pipeline = await Context.Pipelines.FindAsync(id);
            if (pipeline == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Pipeline({id}) NOT FOUND.", id);
                return ResponseBaseModel<Pipeline>.GetNotFoundResponse();
            }

            if (pipeline.IsStarred == true)
            {
                pipeline.IsStarred = false;
            }
            if (pipeline.IsStarred == false)
            {
                pipeline.IsStarred = true;
            }
            Context.Pipelines.Update(pipeline);

            return await SaveDbAndReturnReponse(pipeline);
        }

        public Task<ResponseBaseModel<Pipeline>> Update(Guid id, Pipeline request)
        {
            throw new NotImplementedException();
        }
    }
}