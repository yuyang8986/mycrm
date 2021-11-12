using ETLib.Models.QueryResponse;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Models.Stages;
using MyCRM.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCRM.Services.Services.StageService
{
    public class StageService : IStageService
    {
        private readonly IAccountUserService _accountUserService;
        private readonly ApplicationDbContext _context;

        public StageService(IAccountUserService accountUserService, ApplicationDbContext context)
        {
            _accountUserService = accountUserService;
            _context = context;
        }

        /// <summary>
        /// get current user company's stages with summary data for this employee
        /// </summary>
        /// <returns></returns>
        //public async Task<ResponseBaseModel<List<Stage>>> GetStagesWithSummariesData()
        //{
        //    try
        //    {
        //        var user = await _accountUserService.GetUserWithEmployeeOrganizationData();

        //        var organzationEntry = _context.Entry(user.Organization);
        //        await organzationEntry.Collection(s => s.Stages).LoadAsync();
        //        var stages = user?.Organization?.Stages;

        //        if (stages != null)
        //        {
        //            foreach (var stage in stages)
        //            {
        //                stage.ThisMonthNumber = CountStageSummaryData(user, stage.Name);
        //                stage.ThisQuaterNumber = CountStageSummaryData(user, stage.Name);
        //            }

        //            return ResponseBaseModel<List<Stage>>.GetSuccessResponse(stages?.ToList());
        //        }

        //        return ResponseBaseModel<List<Stage>>.GetNotFoundResponse();
        //    }
        //    catch (Exception e)
        //    {
        //        return ResponseBaseModel<List<Stage>>.GetUnexpectedErrorResponse(e);
        //    }
        //}

        public int CountStageSummaryData(ApplicationUser currentUser, string targetStageName)
        {
            switch (targetStageName)
            {
                //case DefaultStageSummaryNames.ThisMonth:
                //    return currentUser.Employee.PipeLineFlows.Count(s =>
                //        s.Stage.Name == targetStageName && s.CreatedTime.IsInCurrentMonth());
                //case DefaultStageSummaryNames.ThisQuarter:
                //    return currentUser.Employee.PipeLineFlows.Count(s => s.CreatedTime.IsInQuarter());
                ////case DefaultStageSummaryType.New:
            }

            return 0;
        }
    }
}