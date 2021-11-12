using AutoMapper;
using ETLib.Helpers;
using ETLib.Models.QueryResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.Stage;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.Stages;
using MyCRM.Shared.ViewModels.StageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using MyCRM.Shared.Models.Pipelines;

namespace MyCRM.Services.Repository.StageRepository
{
    public class StageRepository : RepositoryBase, IStageRepository
    {
        private readonly IMapper _mapper;
        private readonly IAccountUserService _accountUserService;
        private readonly IAccountManager _accountManager;
        private readonly ILogger _logger;

        public StageRepository(ApplicationDbContext context, IMapper mapper, IAccountManager accountManager, ILogger<StageRepository> logger,
         IAccountUserService accountUserService) : base(context)
        {
            _mapper = mapper;
            _accountUserService = accountUserService;
            _accountManager = accountManager;
            _logger = logger;
        }

        public Task<ResponseBaseModel<Stage>> GetById(int id, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseBaseModel<IEnumerable<Stage>>> GetAll(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBaseModel<Pipeline>> UpdatePipelineStage(Guid id, int stageId)
        {
            var pipeline = await Context.Pipelines.FindAsync(id);

            if (pipeline == null) throw new NullReferenceException(nameof(Pipeline));

            var stage = await Context.Stages.FindAsync(stageId);

            if (stage == null) throw new NullReferenceException(nameof(Stage));

            pipeline.StageId = stageId;

            Context.Pipelines.Update(pipeline);
            return await SaveDbAndReturnReponse(pipeline);
        }

        /// <summary>
        /// get all stage names
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseBaseModel<IEnumerable<StageGetModel>>> GetAllWithPipelines(CancellationToken cancellationToken)
        {
            try
            {
                var user = await _accountUserService.GetUserWithEmployeeOrganizationStagesPipelinesData();
                var roles = await _accountManager.GetUserRolesAsync(user);
                var months = CheckTimeHelper.GetQuarter(DateTime.Now.Month);

                if (roles.Contains("manager"))
                {
                    var stages = user?.Organization?.Stages?.Where(s => s.IsDeleted == false).OrderBy(s => s.DisplayIndex).ToList();
                    if (stages == null || stages.Count == 0) return ResponseBaseModel<IEnumerable<StageGetModel>>.GetSuccessResponse(null);
                    List<StageGetModel> stageGetModels = new List<StageGetModel>();

                    foreach (Stage stage in stages)
                    {
                        var model = _mapper.Map<StageGetModel>(stage);
                        Func<PipelinesGetModelForStage, bool> findPipelineInMonthFunc = (s) =>
                        {
                            var date = s.UpdatedDate == default ? s.CreatedDate : s.UpdatedDate;
                            return date.Month == DateTime.Now.Month;
                        };
                        Func<PipelinesGetModelForStage, bool> findPipelineInQuarterFunc = (s) =>
                        {
                            var date = s.UpdatedDate == default(DateTime) ? s.CreatedDate : s.UpdatedDate;
                            return months.Contains(date.Month);
                        };
                        model.ThisMonthNumber = model.Pipelines.Where(findPipelineInMonthFunc).Count();
                        model.ThisQuarterNumber = model.Pipelines.Where(findPipelineInQuarterFunc).Count();
                        stageGetModels.Add(model);
                    }
                    return ResponseBaseModel<IEnumerable<StageGetModel>>.GetSuccessResponse(stageGetModels);
                }
                else
                {
                    //foreach (var organizationStage in user.Organization.Stages)
                    //{
                    //    var stageEntry = Context.Entry(organizationStage);
                    //    await stageEntry.Collection(s => s.Pipelines).LoadAsync();
                    //}
                    var stages = user?.Organization?.Stages?.Where(s => s.IsDeleted == false).OrderBy(s => s.DisplayIndex).ToList();
                    if (stages == null || stages.Count == 0) return ResponseBaseModel<IEnumerable<StageGetModel>>.GetSuccessResponse(null);
                    List<StageGetModel> stageGetModels = new List<StageGetModel>();

                    foreach (Stage stage in stages)
                    {
                        var model = _mapper.Map<StageGetModel>(stage);
                        Func<PipelinesGetModelForStage, bool> findPipelineInMonthFunc = (s) =>
                        {
                            var date = s.UpdatedDate == default(DateTime) ? s.CreatedDate : s.UpdatedDate;
                            return date.Month == DateTime.Now.Month;
                        };
                        Func<PipelinesGetModelForStage, bool> findPipelineInQuarterFunc = (s) =>
                        {
                            var date = s.UpdatedDate == default(DateTime) ? s.CreatedDate : s.UpdatedDate;
                            return months.Contains(date.Month);
                        };
                        var piplines = model.Pipelines.Where(x => x.ApplicationUserId == user.Id);

                        model.Pipelines = piplines.ToList();
                        model.ThisMonthNumber = piplines.Where(findPipelineInMonthFunc).Count();
                        model.ThisQuarterNumber = piplines.Where(findPipelineInQuarterFunc).Count();
                        stageGetModels.Add(model);
                    }
                    return ResponseBaseModel<IEnumerable<StageGetModel>>.GetSuccessResponse(stageGetModels);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.LogWarning(LoggingEvents.ListItemsFailed, e, "List stage with pipeline failed");
                throw;
            }
        }

        public async Task<ResponseBaseModel<IEnumerable<StageGetModel>>> GetAllWithPipelinesById(string employeeId, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await Context.Users.Include(x => x.Organization)
               .Include(s => s.PipeLineFlows).Where(s => s.Id == employeeId).FirstOrDefaultAsync(cancellationToken);

                var organzationEntry = Context.Entry(employee.Organization);
                await organzationEntry.Collection(s => s.Stages).LoadAsync(cancellationToken);
                var months = CheckTimeHelper.GetQuarter(DateTime.Now.Month);
                var allStages = employee.Organization.Stages.Where(s => s.IsDeleted == false);

                foreach (var organizationStage in allStages)
                {
                    var stageEntry = Context.Entry(organizationStage);
                    await stageEntry.Collection(s => s.Pipelines).LoadAsync(cancellationToken);
                }
                var stages = employee?.Organization?.Stages?.Where(s => s.IsDeleted == false).OrderBy(s => s.DisplayIndex).ToList();
                if (stages.Count == 0) return ResponseBaseModel<IEnumerable<StageGetModel>>.GetSuccessResponse(null);
                List<StageGetModel> stageGetModels = new List<StageGetModel>();

                foreach (Stage stage in stages)
                {
                    var model = _mapper.Map<StageGetModel>(stage);
                    Func<PipelinesGetModelForStage, bool> findPipelineInMonthFunc = (s) =>
                    {
                        var date = s.UpdatedDate == default(DateTime) ? s.CreatedDate : s.UpdatedDate;
                        return date.Month == DateTime.Now.Month;
                    };
                    Func<PipelinesGetModelForStage, bool> findPipelineInQuarterFunc = (s) =>
                    {
                        var date = s.UpdatedDate == default(DateTime) ? s.CreatedDate : s.UpdatedDate;
                        return months.Contains(date.Month);
                    };
                    var piplines = model.Pipelines.Where(x => x.ApplicationUserId == employeeId);
                    model.ThisMonthNumber = piplines.Where(findPipelineInMonthFunc).Count();
                    model.ThisQuarterNumber = piplines.Where(findPipelineInQuarterFunc).Count();
                    stageGetModels.Add(model);
                }
                return ResponseBaseModel<IEnumerable<StageGetModel>>.GetSuccessResponse(stageGetModels);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.LogWarning(LoggingEvents.ListItemsFailed, e, "List stage with pipeline by Employee({id}) failed", employeeId);
                throw;
            }
        }

        public async Task<ResponseBaseModel<Stage>> Add(Stage stage)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
            //var stages = Context.Stages.Where(x => x.OrganizationId == user.OrganizationId);
            try
            {
                //if (stages.Any(s => s.Name == stage.Name))
                //{
                //    _logger.LogWarning(LoggingEvents.InsertItemFailed, "Stage({Name}) already exist", stage.Name);
                //    return ResponseBaseModel<Stage>.GetDbSaveFailedResponse();
                //}

                //stage must belong to a organization
                if (user.Organization == null)
                {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, "User's organization NOT FOUND.");
                    return ResponseBaseModel<Stage>.GetNotFoundResponse(typeof(Organization));
                }
                stage.OrganizationId = user.Organization.Id;

                //await OrderExistingStagesAndSave(stage, null);
                stage.DisplayIndex = Context.Stages.Where(s => s.OrganizationId == stage.OrganizationId && s.IsDeleted == false).Count() + 1;

                Context.Stages.Add(stage);
                return await SaveDbAndReturnReponse(stage);
            }
            catch (Exception e)
            {
                if (e is AuthenticationException ae)
                {
                    _logger.LogWarning(LoggingEvents.InsertItemFailed, e, "Create stage({name}) failed", stage.Name);
                    return ResponseBaseModel<Stage>.GetNotAuthorizedResponse();
                }
                _logger.LogError(LoggingEvents.InsertItemFailed, e, "Create stage({name}) failed", stage.Name);
                return ResponseBaseModel<Stage>.GetUnexpectedErrorResponse(e);
            }
        }

        private async Task OrderExistingStagesAndSave(Stage stage, int? oldDisplayIndex, bool isReorder = false)
        {
            //if new stage display index is first, then increase all other stage display index by 1 and save
            var stages = Context.Stages.Where(s => s.OrganizationId == stage.OrganizationId).ToList();

            //exclude the stage which is updating
            stages.Remove(stage);

            //if (!isReorder)
            //{
            //    stage.DisplayIndex = stages.Count() + 1;
            //    Context.Stages.Update(stage);
            //    await Save();
            //if (stage.DisplayIndex == 1 && stages.Count != 1)
            //{
            //    foreach (var stageFromDb in stages)
            //    {
            //        stageFromDb.DisplayIndex += 1;
            //        Context.Stages.Update(stageFromDb);
            //    }
            //    await Save();
            //}

            //put displayindex to last
            //else if (stage.DisplayIndex == stages.Count())
            //{
            //    var largestDisplayIndexStage = stages.OrderByDescending(x => x.DisplayIndex).FirstOrDefault();
            //    if (largestDisplayIndexStage != null)
            //    {
            //        stage.DisplayIndex = largestDisplayIndexStage.DisplayIndex + 1;
            //    }
            //    else
            //    {
            //        stage.DisplayIndex = 1;
            //    }
            //}
            //else
            //{
            //    //manual set display index, and need to increase other stage index which are after this new index by 1
            //    var stagesAfterThisIndex = stages.Where(s => s.DisplayIndex >= stage.DisplayIndex).ToList();
            //    foreach (var stageAfter in stagesAfterThisIndex)
            //    {
            //        stageAfter.DisplayIndex += 1;
            //        Context.Stages.Update(stageAfter);
            //    }
            //    if (Context.Stages.Any(x => x.DisplayIndex == stage.DisplayIndex))
            //    {
            //        var stagesWithDisplayIndexBiggerThenNewDisplay =
            //           stages.Where(s => s.DisplayIndex >= stage.DisplayIndex).ToList();

            //        foreach (var stageSmaller in stagesWithDisplayIndexBiggerThenNewDisplay)
            //        {
            //            stageSmaller.DisplayIndex += 1;
            //        }
            //    }
            //    await Save();
            //}
            //}
            //else
            //{
            //reorder stages
            if (stage.DisplayIndex > oldDisplayIndex)
            {
                var stagesWithDisplayIndexSmallerThenNewDisplay =
                    stages.Where(s => s.DisplayIndex <= stage.DisplayIndex && s.DisplayIndex > oldDisplayIndex).ToList();

                foreach (var stageSmaller in stagesWithDisplayIndexSmallerThenNewDisplay)
                {
                    stageSmaller.DisplayIndex -= 1;
                }
            }
            else if (stage.DisplayIndex < oldDisplayIndex)
            {
                var stagesWithDisplayIndexBiggerThenNewDisplay =
                    stages.Where(s => s.DisplayIndex >= stage.DisplayIndex && s.DisplayIndex < oldDisplayIndex).ToList();

                foreach (var stageSmaller in stagesWithDisplayIndexBiggerThenNewDisplay)
                {
                    stageSmaller.DisplayIndex += 1;
                }
            }
            await Save();
        }

        //}

        public async Task<ResponseBaseModel<Stage>> Update(int id, StagePutRequest request)
        {
            try
            {
                Stage stage = await Context.Stages.FindAsync(id);
                if (stage == null)
                {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, "Stage({id}) NOT FOUND.", id);
                    return ResponseBaseModel<Stage>.GetNotFoundResponse();
                }

                stage.DisplayIndex = request.DisplayIndex;
                stage.IconIndex = request.IconIndex;
                stage.Name = request.Name;
                //stage.ThisMonth = request.ThisMonth;
                //stage.ThisQuater = request.ThisQuater;

                await OrderExistingStagesAndSave(stage, null);
                if (stage.DisplayIndex == 0)
                {
                    _logger.LogError(LoggingEvents.ReorderItemFailed, "DisplayIndex can not be 0.");
                    return ResponseBaseModel<Stage>.GetDbSaveFailedResponse();
                }

                Context.Stages.Update(stage);
                return await SaveDbAndReturnReponse(stage);

                _logger.LogWarning(LoggingEvents.UpdateItemFailed, "Update stage({id}) failed", id);
                return ResponseBaseModel<Stage>.GetDbSaveFailedResponse();
            }
            catch (Exception e)
            {
                if (e is AuthenticationException ae)
                {
                    _logger.LogWarning(LoggingEvents.InsertItemFailed, e, "Create stage({name}) failed");
                    return ResponseBaseModel<Stage>.GetNotAuthorizedResponse();
                }
                _logger.LogError(LoggingEvents.InsertItemFailed, e, "Create stage({name}) failed");
                return ResponseBaseModel<Stage>.GetUnexpectedErrorResponse(e);
            }
        }

        public async Task<ResponseBaseModel<Stage>> Delete(int id)
        {
            var stage = await Context.Stages.FindAsync(id);
            if (stage == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Stage({id}) NOT FOUND.", id);
                return ResponseBaseModel<Stage>.GetNotFoundResponse();
            }

            //move all stages which has larger displayindex than the one being deleted by minus 1
            var stagesToDecreaseDisplayIndex = Context.Stages.Where(s => s.OrganizationId == stage.OrganizationId && s.DisplayIndex > stage.DisplayIndex).ToList();
            foreach (var stageChanging in stagesToDecreaseDisplayIndex)
            {
                stageChanging.DisplayIndex -= 1;
                Context.Stages.Update(stageChanging);
            }

            stage.IsDeleted = true;
            stage.DisplayIndex = null;
            Context.Stages.Update(stage);
            return await SaveDbAndReturnReponse(stage);
        }

        public async Task<ResponseBaseModel<Stage>> Reorder(StageReorderRequest request)
        {
            try
            {
                int oldIndex = 0;

                var stage = await Context.Stages.
                    FirstOrDefaultAsync(s => s.Id == request.Id);
                if (stage == null)
                {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, "Stage({id}) NOT FOUND.", request.Id);
                    return ResponseBaseModel<Stage>.GetNotFoundResponse();
                }
                oldIndex = stage.DisplayIndex.Value;
                stage.DisplayIndex = request.DisplayIndex;
                if (stage.DisplayIndex == 0)
                {
                    _logger.LogError(LoggingEvents.ReorderItemFailed, "DisplayIndex can not be 0.");
                    return ResponseBaseModel<Stage>.GetDbSaveFailedResponse();
                }
                await OrderExistingStagesAndSave(stage, oldIndex, true);

                Context.Stages.Update(stage);
                await Save();

                return ResponseBaseModel<Stage>.GetSuccessResponse(stage);
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.ReorderItemFailed, e, "Reorder stage({id}) failed", request.Id);
                return ResponseBaseModel<Stage>.GetUnexpectedErrorResponse(e);
            }
        }

        public Task<ResponseBaseModel<IEnumerable<Stage>>> GetAll(CancellationToken cancellationToken, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBaseModel<Stage>> Update(int id, Stage request)
        {
            throw new NotImplementedException();
        }
    }
}