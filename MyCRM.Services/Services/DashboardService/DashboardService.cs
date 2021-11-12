using AutoMapper;
using ETLib.Helpers;
using ETLib.Models.QueryResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence;
using MyCRM.Persistence.Data;
using MyCRM.Services.Repository.PipelineRepository;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Services.Services.DashboardService;
using MyCRM.Services.Services.ScheduleService;
using MyCRM.Shared.Communications.Responses.Dashboard;
using MyCRM.Shared.Communications.Responses.Schedule;
using MyCRM.Shared.Constants;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.Stages;
using MyCRM.Shared.Models.TargetTemplate;
using MyCRM.Shared.Models.User;
using MyCRM.Shared.ViewModels.ScheduleViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Services.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly IAccountUserService _accountUserService;
        private readonly ApplicationDbContext _context;
        private readonly IAccountManager _accountManager;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private List<Pipeline> _allPipelines;

        private string[] Months = new string[12]
        {
          "January",
          "February",
          "March",
          "April",
          "May",
          "June",
          "July",
          "August",
          "September",
          "October",
          "November",
          "December"
        };

        private string[] Quarter = new string[4] { "Quarter1", "Quarter2", "Quarter3", "Quarter4" };
        private int CurrentYear = DateTime.Now.Year;

        public DashboardService(ApplicationDbContext context, IAccountUserService accountUserService,
            IAccountManager accountManager, ILogger<DashboardService> logger, IMapper mapper)
        {
            _accountUserService = accountUserService;
            _accountManager = accountManager;
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _allPipelines = _context.Pipelines.Include(s => s.ApplicationUser)
                .Include(s => s.Stage).ToList();
        }

        public async Task<ResponseBaseModel<DashboardResponseModel>> GetDashboard(CancellationToken cancellationToken)
        {
            try
            {
                var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
                var roles = await _accountManager.GetUserRolesAsync(user);
                var organizationEntry = _context.Entry(user.Organization);
                await organizationEntry.Collection(s => s.Stages).LoadAsync(cancellationToken);
                var organizationPipelines = _allPipelines
                    .Where(x => x.ApplicationUser.OrganizationId == user.OrganizationId).ToList();

                var employeePipelines = _allPipelines
                    .Where(x => x.ApplicationUserId == user.Id).ToList();

                var dashboard = new DashboardResponseModel();
                //dashboard.WeekOverallSummary = await GetWeekhOverall();
                dashboard.MonthOverallSummary = GetMonthOverall(roles, organizationPipelines, employeePipelines);
                dashboard.QuarterOverallSummary = GetQuarterOverall(roles, organizationPipelines, employeePipelines);
                dashboard.YearOverallSummary = GetYearOverall(roles, organizationPipelines, employeePipelines);
                dashboard.LeadAmountAnalysis = GetLeadAmountAnalysis(roles, organizationPipelines, employeePipelines);
                dashboard.LeadStarredOverdue = GetLeadStarredOverdue(roles, organizationPipelines, employeePipelines);
                dashboard.TargetAchieved = await GetTargetAchieved();
                dashboard.Performance = await GetPerformance();
                dashboard.TodaySchedule = await GetTodaySchedule();
                dashboard.CountSummary = await GetCountSummary(cancellationToken);
                return ResponseBaseModel<DashboardResponseModel>.GetSuccessResponse(dashboard);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ResponseBaseModel<DashboardResponseModel>> GetDashboardById(string employeeId, CancellationToken cancellationToken)
        {
            if (!_context.Users.Any(x => x.Id == employeeId)) return ResponseBaseModel<DashboardResponseModel>.GetNotFoundResponse();
            var dashboard = new DashboardResponseModel();
            //dashboard.WeekOverallSummary = await GetWeekOverallById(employeeId);
            dashboard.MonthOverallSummary = await GetMonthOverallById(employeeId);
            dashboard.QuarterOverallSummary = await GetQuarterOverallById(employeeId);
            dashboard.YearOverallSummary = await GetYearOverallById(employeeId);
            dashboard.LeadAmountAnalysis = await GetLeadAmountAnalysisById(employeeId);
            dashboard.LeadStarredOverdue = await GetLeadStarredOverdueById(employeeId);
            dashboard.TargetAchieved = await GetTargetAchievedById(employeeId);
            dashboard.Performance = await GetPerformanceById(employeeId);
            dashboard.TodaySchedule = await GetTodaySchedule();
            dashboard.CountSummary = await GetCountSummaryById(employeeId);
            return ResponseBaseModel<DashboardResponseModel>.GetSuccessResponse(dashboard);
        }

        //public async Task<OverallSummary> GetWeekhOverall(int? weekNumber)
        //{
        //    var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
        //    var roles = await _accountManager.GetUserRolesAsync(user);
        //    var organzationEntry = _context.Entry(user.Organization);
        //    await organzationEntry.Collection(s => s.Stages).LoadAsync();

        //    CultureInfo culture = new CultureInfo("en-AU");
        //    CalendarWeekRule rule = culture.DateTimeFormat.CalendarWeekRule;
        //    Calendar calendar = culture.Calendar;
        //    DayOfWeek firstDOW = culture.DateTimeFormat.FirstDayOfWeek;
        //    if (weekNumber == null) weekNumber = calendar.GetWeekOfYear(DateTime.Now, rule, firstDOW);

        //    if (roles.Contains("manager"))
        //    {
        //        List<Pipeline> pipelines = _context.Pipelines.Include(s => s.ApplicationUser).Where(x => x.ApplicationUser.OrganizationId == user.OrganizationId).ToList();
        //        List<Pipeline> pipelinesInWeek = new List<Pipeline>();
        //        foreach (var pipeline in pipelines)
        //        {
        //            int pipelineWeekNumber = calendar.GetWeekOfYear(pipeline.CreatedDate, rule, firstDOW);
        //            if (pipelineWeekNumber == weekNumber) pipelinesInWeek.Add(pipeline);
        //        }
        //        var thisWeekOverall = GetSummary(pipelinesInWeek);
        //        return thisWeekOverall;
        //    }
        //    else
        //    {
        //        List<Pipeline> pipelines = _context.Pipelines.Include(s => s.ApplicationUser).Where(x => x.ApplicationUserId == user.Id ).ToList();
        //        List<Pipeline> pipelinesInWeek = new List<Pipeline>();
        //        foreach (var pipeline in pipelines)
        //        {
        //            int pipelineWeekNumber = calendar.GetWeekOfYear(pipeline.CreatedDate, rule, firstDOW);
        //            if (pipelineWeekNumber == weekNumber) pipelinesInWeek.Add(pipeline);
        //        }
        //        var thisWeekOverall = GetSummary(pipelines);
        //        return thisWeekOverall;
        //    }

        //}
        //public async Task<OverallSummary> GetWeekOverallById(string employeeId, int? weekNumber)
        //{
        //    CultureInfo culture = new CultureInfo("en-AU");
        //    CalendarWeekRule rule = culture.DateTimeFormat.CalendarWeekRule;
        //    Calendar calendar = culture.Calendar;
        //    DayOfWeek firstDOW = culture.DateTimeFormat.FirstDayOfWeek;
        //    if (weekNumber == null) weekNumber = calendar.GetWeekOfYear(DateTime.Now, rule, firstDOW);
        //    var pipelines = await _context.Pipelines.Include(s => s.ApplicationUser).Include(s => s.Stage).Where(x => x.ApplicationUserId == employeeId).ToListAsync();
        //    List<Pipeline> pipelinesInWeek = new List<Pipeline>();
        //    foreach (var pipeline in pipelines)
        //    {
        //        int pipelineWeekNumber = calendar.GetWeekOfYear(pipeline.CreatedDate, rule, firstDOW);
        //        if (pipelineWeekNumber == weekNumber) pipelinesInWeek.Add(pipeline);
        //    }
        //    var thisWeekOverall = GetSummary(pipelinesInWeek);
        //    return thisWeekOverall;

        //}
        public async Task<CountSummary> GetCountSummary(CancellationToken cancellationToken)
        {
            var user = await _accountUserService.GetCurrentUserWithEmployeAllEvents();
            var roles = await _accountManager.GetUserRolesAsync(user);
            var organizationEntry = _context.Entry(user.Organization);
            await organizationEntry.Collection(s => s.Stages).LoadAsync(cancellationToken);
            if (roles.Contains("manager"))
            {
                var organizationPipelines = _allPipelines.Where(x => x.ApplicationUser.OrganizationId == user.OrganizationId && !x.Stage.Name.Equals("Won") && !x.Stage.Name.Equals("Lost") && !x.Stage.Name.Equals("Closed")).ToList();
                var organizationAppointment = _context.Appointments.Include(s => s.ApplicationUser).Where(x => x.ApplicationUser.OrganizationId == user.OrganizationId && x.EventStartDateTime.Day == DateTime.UtcNow.Day).ToList();
                var organizationEvent = _context.Events.AsNoTracking().Where(x => x.OrganizationId == user.OrganizationId && x.EventStartDateTime.Day == DateTime.UtcNow.Day).ToList();
                var organizationTask = _context.Tasks.Include(s => s.ApplicationUser).Where(x => x.ApplicationUser.OrganizationId == user.OrganizationId && x.EventStartDateTime.Day == DateTime.UtcNow.Day).ToList();
                var countSummary = new CountSummary();
                if (organizationPipelines != null) countSummary.DealCount = organizationPipelines.Count();
                if (organizationAppointment != null) countSummary.AppointmentCount = organizationAppointment.Count();
                if (organizationEvent != null) countSummary.EventCount = organizationEvent.Count();
                if (organizationTask != null) countSummary.TaskCount = organizationTask.Count();
                return countSummary;
            }
            else
            {
                var organizationPipelines = _allPipelines.Where(x => x.ApplicationUserId == user.Id && !x.Stage.Name.Equals("Won") && !x.Stage.Name.Equals("Lost") && !x.Stage.Name.Equals("Closed")).ToList();
                var organizationAppointment = _context.Appointments.Include(s => s.ApplicationUser).Where(x => x.ApplicationUserId == user.Id && x.EventStartDateTime.Day == DateTime.UtcNow.Day).ToList();
                var organizationEvent = _context.Events.Where(x => x.OrganizationId == user.OrganizationId && x.EventStartDateTime.Day == DateTime.UtcNow.Day).AsNoTracking().ToList();
                var organizationTask = _context.Tasks.Include(s => s.ApplicationUser).Where(x => x.ApplicationUserId == user.Id && x.EventStartDateTime.Day == DateTime.UtcNow.Day).ToList();
                var countSummary = new CountSummary();
                countSummary.DealCount = organizationPipelines.Count();
                countSummary.AppointmentCount = organizationAppointment.Count();
                countSummary.EventCount = organizationEvent.Count();
                countSummary.TaskCount = organizationTask.Count();
                return countSummary;
            }
        }

        public async Task<CountSummary> GetCountSummaryById(string employeeId)
        {
            var user = await _context.Users.Include(x => x.TargetTemplate).Include(x => x.PipeLineFlows).ThenInclude(x => x.Stage).Where(s => s.Id == employeeId).FirstOrDefaultAsync();
            var countSummary = new CountSummary();
            if (user.PipeLineFlows != null) countSummary.DealCount = user.PipeLineFlows.Count(x => !x.Stage.Name.Equals("Won") && !x.Stage.Name.Equals("Lost") && !x.Stage.Name.Equals("Closed"));
            if (user.Appointments != null) countSummary.AppointmentCount = user.Appointments.Count(x => x.EventStartDateTime.Day == DateTime.UtcNow.Day);
            if (user.Organization.Events != null) countSummary.EventCount = user.Organization.Events.Count(x => x.EventStartDateTime.Day == DateTime.UtcNow.Day);
            if (user.Tasks != null) countSummary.TaskCount = user.Tasks.Count(x => x.EventStartDateTime.Day == DateTime.UtcNow.Day);
            return countSummary;
        }

        public List<OverallSummary> GetMonthOverall(IList<string> roles, List<Pipeline> organizationPipelines, List<Pipeline> employeePipelines)
        {
            List<OverallSummary> overallsForMonths = new List<OverallSummary>();
            if (roles.Contains("manager"))
            {
                for (int j = CurrentYear - 2; j <= CurrentYear; j++)
                {
                    var pipelinesInYear = organizationPipelines.Where(x => x.UpdatedDate == default ? x.CreatedDate.Year == j : x.UpdatedDate.Year == j);

                    for (int i = 1; i <= 12; i++)
                    {
                        var pipelines = pipelinesInYear.Where(x => x.UpdatedDate == default ? x.CreatedDate.Month == i : x.UpdatedDate.Month == i).ToList();
                        string month = Months[i - 1];
                        var thisMonthOverall = GetSummary(pipelines, month, j);
                        overallsForMonths.Add(thisMonthOverall);
                    }
                }
                return overallsForMonths;
            }
            else
            {
                for (int j = CurrentYear - 2; j <= CurrentYear; j++)
                {
                    var pipelinesInYear = employeePipelines.Where(x => x.UpdatedDate == default ? x.CreatedDate.Year == j : x.UpdatedDate.Year == j);

                    for (int i = 1; i <= 12; i++)
                    {
                        var pipelines = pipelinesInYear.Where(x => x.UpdatedDate == default ? x.CreatedDate.Month == i : x.UpdatedDate.Month == i).ToList();

                        string month = Months[i - 1];
                        var thisMonthOverall = GetSummary(pipelines, month, j);
                        overallsForMonths.Add(thisMonthOverall);
                    }
                }

                return overallsForMonths;
            }
        }

        public async Task<List<OverallSummary>> GetMonthOverallById(string employeeId)
        {
            List<OverallSummary> overallsForMonths = new List<OverallSummary>();
            var pipelines = _allPipelines.Where(x => x.ApplicationUserId == employeeId).ToList();
            for (int j = CurrentYear - 2; j <= CurrentYear; j++)
            {
                var pipelinesInYear = pipelines.Where(x => x.UpdatedDate == default ? x.CreatedDate.Year == j : x.UpdatedDate.Year == j).ToList();

                for (int i = 1; i <= 12; i++)
                {
                    var pipelinesInThisYear = pipelinesInYear.Where(x => x.UpdatedDate == default ? x.CreatedDate.Month == i : x.UpdatedDate.Month == i).ToList();

                    string name = Months[i - 1];
                    var thisMonthOverall = GetSummary(pipelinesInThisYear, name, j);
                    overallsForMonths.Add(thisMonthOverall);
                }
            }

            return overallsForMonths;
        }

        public List<OverallSummary> GetQuarterOverall(IList<string> roles, List<Pipeline> organizationPipelines, List<Pipeline> employeePipelines)
        {
            if (roles.Contains("manager"))
            {
                List<OverallSummary> overallsForQuarters = new List<OverallSummary>();

                for (int j = CurrentYear - 2; j <= CurrentYear; j++)
                {
                    var pipelinesInYear = organizationPipelines.Where(x => x.UpdatedDate == default ? x.CreatedDate.Year == j : x.UpdatedDate.Year == j);

                    for (int i = 1; i <= 4; i++)
                    {
                        List<Pipeline> pipelinesInQuarter = new List<Pipeline>();
                        string quarter = Quarter[i - 1];
                        foreach (var pipeline in pipelinesInYear)
                        {
                            if (getQuarterNumber(pipeline.CreatedDate.Month) == i) pipelinesInQuarter.Add(pipeline);
                        }
                        var thisQuarterOverall = GetSummary(pipelinesInQuarter, quarter, j);
                        overallsForQuarters.Add(thisQuarterOverall);
                    }
                }
                return overallsForQuarters;
            }
            else
            {
                List<OverallSummary> overallsForQuarters = new List<OverallSummary>();
                for (int j = CurrentYear - 2; j <= CurrentYear; j++)
                {
                    var pipelinesInYear = employeePipelines.Where(x => x.UpdatedDate == default ? x.CreatedDate.Year == j : x.UpdatedDate.Year == j);

                    for (int i = 1; i <= 4; i++)
                    {
                        List<Pipeline> pipelinesInQuarter = new List<Pipeline>();
                        string quarter = Quarter[i - 1];
                        foreach (var pipeline in pipelinesInYear)
                        {
                            if (getQuarterNumber(pipeline.CreatedDate.Month) == i) pipelinesInQuarter.Add(pipeline);
                        }
                        var thisQuarterOverall = GetSummary(pipelinesInQuarter, quarter, j);
                        overallsForQuarters.Add(thisQuarterOverall);
                    }
                }
                return overallsForQuarters;
            }
        }

        public async Task<List<OverallSummary>> GetQuarterOverallById(string employeeId)
        {
            List<OverallSummary> overallsForQuarters = new List<OverallSummary>();
            var pipelines = _allPipelines.Where(x => x.ApplicationUserId == employeeId).ToList();
            for (int j = CurrentYear - 2; j <= CurrentYear; j++)
            {
                var pipelinesInYear = pipelines.Where(x => x.UpdatedDate == default ? x.CreatedDate.Year == j : x.UpdatedDate.Year == j).ToList();

                for (int i = 1; i <= 4; i++)
                {
                    List<Pipeline> pipelinesInQuarter = new List<Pipeline>();
                    string quarter = Quarter[i - 1];
                    foreach (var pipeline in pipelinesInYear)
                    {
                        if (getQuarterNumber(pipeline.CreatedDate.Month) == i) pipelinesInQuarter.Add(pipeline);
                    }
                    var thisQuarterOverall = GetSummary(pipelinesInQuarter, quarter, j);
                    overallsForQuarters.Add(thisQuarterOverall);
                }
            }
            return overallsForQuarters;
        }

        public List<OverallSummary> GetYearOverall(IList<string> roles, List<Pipeline> organizationPipelines, List<Pipeline> employeePipelines)
        {
            //int year = DateTime.Now.Year;
            if (roles.Contains("manager"))
            {
                List<OverallSummary> overallsForYears = new List<OverallSummary>();

                for (int j = CurrentYear - 2; j <= CurrentYear; j++)
                {
                    List<Pipeline> pipelines = organizationPipelines.Where(x => x.UpdatedDate == default ? x.CreatedDate.Year == j : x.UpdatedDate.Year == j).ToList();

                    var thisYearOverall = GetSummary(pipelines, j.ToString(), j);
                    overallsForYears.Add(thisYearOverall);
                }
                return overallsForYears;
            }
            else
            {
                List<OverallSummary> overallsForYears = new List<OverallSummary>();
                for (int j = CurrentYear - 2; j <= CurrentYear; j++)
                {
                    List<Pipeline> pipelines = employeePipelines.Where(x => x.UpdatedDate == default ? x.CreatedDate.Year == j : x.UpdatedDate.Year == j).ToList();

                    var thisYearOverall = GetSummary(pipelines, j.ToString(), j);
                    overallsForYears.Add(thisYearOverall);
                }
                return overallsForYears;
            }
        }

        public async Task<List<OverallSummary>> GetYearOverallById(string employeeId)
        {
            List<OverallSummary> overallsForYears = new List<OverallSummary>();
            //int year = DateTime.Now.Year;
            var pipelines = _allPipelines.Where(x => x.ApplicationUserId == employeeId).ToList();
            for (int j = CurrentYear - 2; j <= CurrentYear; j++)
            {
                var pipelinesForYears = pipelines.Where(x => x.UpdatedDate == default ? x.CreatedDate.Year == j : x.UpdatedDate.Year == j).ToList();

                var thisYearOverall = GetSummary(pipelinesForYears, j.ToString(), j);
                overallsForYears.Add(thisYearOverall);
            }
            return overallsForYears;
        }

        public OverallSummary GetSummary(List<Pipeline> pipelines, string name, int year)
        {
            try
            {
                OverallSummary overallSummary = new OverallSummary();
                int won = 0, lost = 0, openLead = 0;
                double wonAmount = 0, lostAmount = 0, openLeadAmount = 0;
                foreach (var pipeline in pipelines)
                {
                    if (pipeline.Stage.Name.Equals("Won"))
                    {
                        wonAmount += pipeline.DealAmount;
                        won++;
                    }
                    if (pipeline.Stage.Name.Equals("Lost"))
                    {
                        lostAmount += pipeline.DealAmount;
                        lost++;
                    }
                    if (!pipeline.Stage.Name.Equals("Won") && !pipeline.Stage.Name.Equals("Lost") && !pipeline.Stage.Name.Equals("Closed"))
                    {
                        openLeadAmount += pipeline.DealAmount;
                        openLead++;
                    }
                }
                overallSummary.Name = name;

                overallSummary.Year = year;
                overallSummary.Won = won;
                overallSummary.WonAmount = wonAmount;
                overallSummary.Lost = lost;
                overallSummary.LostAmount = lostAmount;
                overallSummary.OpenLead = openLead;
                overallSummary.OpenLeadAmount = openLeadAmount;
                return overallSummary;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public LeadAmountAnalysis GetLeadAmountAnalysis(IList<string> roles, List<Pipeline> organizationPipelines, List<Pipeline> employeePipelines)
        {
            if (roles.Contains("manager"))
            {
                var leadAmountAnalysis = GetLeadAmount(organizationPipelines);
                return leadAmountAnalysis;
            }
            else
            {
                var leadAmountAnalysis = GetLeadAmount(employeePipelines);
                return leadAmountAnalysis;
            }
        }

        public async Task<LeadAmountAnalysis> GetLeadAmountAnalysisById(string employeeId)
        {
            var pipelines = _allPipelines.Where(x => x.ApplicationUserId == employeeId).ToList();
            var leadAmountAnalysis = GetLeadAmount(pipelines);
            return leadAmountAnalysis;
        }

        public LeadAmountAnalysis GetLeadAmount(List<Pipeline> pipelines)
        {
            double maxAmount = 0;
            double minAmount = 99999999;
            double sum = 0;
            int count = 0;
            foreach (var pipeline in pipelines)
            {
                sum = sum + pipeline.DealAmount;
                count++;

                if (maxAmount < pipeline.DealAmount) { maxAmount = Math.Round(pipeline.DealAmount, 2); }
                if (minAmount > pipeline.DealAmount) { minAmount = Math.Round(pipeline.DealAmount, 2); }
            }
            double average = sum / count;
            LeadAmountAnalysis leadAmountAnalysis = new LeadAmountAnalysis();
            leadAmountAnalysis.Highest = maxAmount;
            leadAmountAnalysis.Lowest = minAmount;
            leadAmountAnalysis.Average = average;
            return leadAmountAnalysis;
        }

        public LeadStarredOverdue GetLeadStarredOverdue(IList<string> roles, List<Pipeline> organizationPipelines, List<Pipeline> employeePipelines)
        {
            if (roles.Contains("manager"))
            {
                var leadStarredOverdue = GetLeadStarredOverdueAmount(organizationPipelines);
                return leadStarredOverdue;
            }
            else
            {
                var leadStarredOverdue = GetLeadStarredOverdueAmount(employeePipelines);
                return leadStarredOverdue;
            }
        }

        public async Task<LeadStarredOverdue> GetLeadStarredOverdueById(string employeeId)
        {
            var pipelines = _allPipelines.Where(x => x.ApplicationUserId == employeeId).ToList();
            var leadStarredOverdue = GetLeadStarredOverdueAmount(pipelines);
            return leadStarredOverdue;
        }

        public LeadStarredOverdue GetLeadStarredOverdueAmount(List<Pipeline> pipelines)
        {
            int starredCount = 0;
            int overdueCount = 0;
            foreach (var pipeline in pipelines)
            {
                if (pipeline.IsStarred) { starredCount++; }
                //if (pipeline.IsOverdue) { overdueCount++; }
            }
            LeadStarredOverdue leadStarredOverdue = new LeadStarredOverdue();
            leadStarredOverdue.Starred = starredCount;
            leadStarredOverdue.Overdue = overdueCount;
            return leadStarredOverdue;
        }

        public async Task<TargetAchieved> GetTargetAchieved()
        {
            var user = await _accountUserService.GetUserWithOrganizationTemplateData();
            var roles = await _accountManager.GetUserRolesAsync(user);
            var organzationEntry = _context.Entry(user.Organization);
            await organzationEntry.Collection(s => s.Stages).LoadAsync();
            if (roles.Contains("manager"))
            {
                var targets = user.Organization.TargetTemplates;
                double Q1Target = 0;
                double Q2Target = 0;
                double Q3Target = 0;
                double Q4Target = 0;
                if (targets == null)
                {
                }
                else
                {
                    foreach (var target in targets)
                    {
                        var employeesInTemplate = target.Employees;

                        if (employeesInTemplate != null)
                        {
                            Q1Target += target.Q1 * employeesInTemplate.Count();
                            Q2Target += target.Q2 * employeesInTemplate.Count();
                            Q3Target += target.Q3 * employeesInTemplate.Count();
                            Q4Target += target.Q4 * employeesInTemplate.Count();
                        }
                    }
                }

                List<Pipeline> pipelines = _allPipelines.Where(x => x.ApplicationUser.OrganizationId == user.OrganizationId && x.Stage.Name.Equals("Won") && x.UpdatedDate == default ? x.CreatedDate.Year == CurrentYear : x.UpdatedDate.Year == CurrentYear).ToList();

                double? Q1GpSum = 0;
                double? Q2GpSum = 0;
                double? Q3GpSum = 0;
                double? Q4GpSum = 0;
                Q1GpSum = getQuarterGPSummary(pipelines).Q1Amount;
                Q2GpSum = getQuarterGPSummary(pipelines).Q2Amount;
                Q3GpSum = getQuarterGPSummary(pipelines).Q3Amount;
                Q4GpSum = getQuarterGPSummary(pipelines).Q4Amount;
                var Q1targetAndAchievedModel = new TargetAndAchievedModel();
                Q1targetAndAchievedModel.Target = Q1Target;
                Q1targetAndAchievedModel.Achieved = Q1GpSum.Value;
                var Q2targetAndAchievedModel = new TargetAndAchievedModel();
                Q2targetAndAchievedModel.Target = Q2Target;
                Q2targetAndAchievedModel.Achieved = Q2GpSum.Value;
                var Q3targetAndAchievedModel = new TargetAndAchievedModel();
                Q3targetAndAchievedModel.Target = Q3Target;
                Q3targetAndAchievedModel.Achieved = Q3GpSum.Value;
                var Q4targetAndAchievedModel = new TargetAndAchievedModel();
                Q4targetAndAchievedModel.Target = Q4Target;
                Q4targetAndAchievedModel.Achieved = Q4GpSum.Value;
                var targetAchieved = new TargetAchieved();
                targetAchieved.Q1 = Q1targetAndAchievedModel;
                targetAchieved.Q2 = Q2targetAndAchievedModel;
                targetAchieved.Q3 = Q3targetAndAchievedModel;
                targetAchieved.Q4 = Q4targetAndAchievedModel;
                return targetAchieved;
            }
            else
            {
                var target = user.TargetTemplate;
                double Q1Target = target?.Q1 ?? 0;
                double Q2Target = target?.Q2 ?? 0;
                double Q3Target = target?.Q3 ?? 0;
                double Q4Target = target?.Q4 ?? 0;

                List<Pipeline> pipelines = _allPipelines.Where(x => x.ApplicationUserId == user.Id && x.Stage.Name.Equals("Won") && x.UpdatedDate == default ? x.CreatedDate.Year == CurrentYear : x.UpdatedDate.Year == CurrentYear).ToList();

                double Q1GpSum = 0;
                double Q2GpSum = 0;
                double Q3GpSum = 0;
                double Q4GpSum = 0;
                Q1GpSum = getQuarterGPSummary(pipelines).Q1Amount;
                Q2GpSum = getQuarterGPSummary(pipelines).Q2Amount;
                Q3GpSum = getQuarterGPSummary(pipelines).Q3Amount;
                Q4GpSum = getQuarterGPSummary(pipelines).Q4Amount;
                var Q1targetAndAchievedModel = new TargetAndAchievedModel();
                Q1targetAndAchievedModel.Target = Q1Target;
                Q1targetAndAchievedModel.Achieved = Q1GpSum;
                var Q2targetAndAchievedModel = new TargetAndAchievedModel();
                Q2targetAndAchievedModel.Target = Q2Target;
                Q2targetAndAchievedModel.Achieved = Q2GpSum;
                var Q3targetAndAchievedModel = new TargetAndAchievedModel();
                Q3targetAndAchievedModel.Target = Q3Target;
                Q3targetAndAchievedModel.Achieved = Q3GpSum;
                var Q4targetAndAchievedModel = new TargetAndAchievedModel();
                Q4targetAndAchievedModel.Target = Q4Target;
                Q4targetAndAchievedModel.Achieved = Q4GpSum;
                var targetAchieved = new TargetAchieved();
                targetAchieved.Q1 = Q1targetAndAchievedModel;
                targetAchieved.Q2 = Q2targetAndAchievedModel;
                targetAchieved.Q3 = Q3targetAndAchievedModel;
                targetAchieved.Q4 = Q4targetAndAchievedModel;

                return targetAchieved;
            }
        }

        public async Task<TargetAchieved> GetTargetAchievedById(string employeeId)
        {
            var user = await _context.Users.Include(x => x.TargetTemplate).Where(s => s.Id == employeeId).FirstOrDefaultAsync();
            var targets = user.TargetTemplate;
            double Q1Target, Q2Target, Q3Target, Q4Target;
            if (targets == null)
            {
                Q1Target = 0;
                Q2Target = 0;
                Q3Target = 0;
                Q4Target = 0;
            }
            else
            {
                Q1Target = targets.Q1;
                Q2Target = targets.Q2;
                Q3Target = targets.Q3;
                Q4Target = targets.Q4;
            }

            List<Pipeline> pipelines = _allPipelines.Where(x => x.ApplicationUserId == user.Id && x.Stage.Name.Equals("Won")).ToList();

            var quarterAchieved = getQuarterGPSummary(pipelines);
            var Q1targetAndAchievedModel = new TargetAndAchievedModel();
            Q1targetAndAchievedModel.Target = Q1Target;
            Q1targetAndAchievedModel.Achieved = quarterAchieved.Q1Amount;
            var Q2targetAndAchievedModel = new TargetAndAchievedModel();
            Q2targetAndAchievedModel.Target = Q2Target;
            Q2targetAndAchievedModel.Achieved = quarterAchieved.Q2Amount;
            var Q3targetAndAchievedModel = new TargetAndAchievedModel();
            Q3targetAndAchievedModel.Target = Q3Target;
            Q3targetAndAchievedModel.Achieved = quarterAchieved.Q3Amount;
            var Q4targetAndAchievedModel = new TargetAndAchievedModel();
            Q4targetAndAchievedModel.Target = Q4Target;
            Q4targetAndAchievedModel.Achieved = quarterAchieved.Q4Amount;
            var targetAchieved = new TargetAchieved();
            targetAchieved.Q1 = Q1targetAndAchievedModel;
            targetAchieved.Q2 = Q2targetAndAchievedModel;
            targetAchieved.Q3 = Q3targetAndAchievedModel;
            targetAchieved.Q4 = Q4targetAndAchievedModel;

            return targetAchieved;
        }

        //public QuarterGetModel getQuarterSummary(List<Pipeline> pipelines)
        //{
        //    int Q1 = 0;
        //    int Q2 = 0;
        //    int Q3 = 0;
        //    int Q4 = 0;
        //    double Q1Amount = 0;
        //    double Q2Amount = 0;
        //    double Q3Amount = 0;
        //    double Q4Amount = 0;
        //    foreach (var pipeline in pipelines)
        //    {
        //        if (getQuarterNumber(pipeline.UpdatedDate == default ? pipeline.CreatedDate : pipeline.UpdatedDate) == 1) { Q1Amount += pipeline.DealAmount; Q1++; }
        //        else if (getQuarterNumber(pipeline.UpdatedDate == default ? pipeline.CreatedDate : pipeline.UpdatedDate) == 2) { Q2Amount += pipeline.DealAmount; Q2++; }
        //        else if (getQuarterNumber(pipeline.UpdatedDate == default ? pipeline.CreatedDate : pipeline.UpdatedDate) == 3) { Q3Amount += pipeline.DealAmount; Q3++; }
        //        else if (getQuarterNumber(pipeline.UpdatedDate == default ? pipeline.CreatedDate : pipeline.UpdatedDate) == 4) { Q4Amount += pipeline.DealAmount; Q4++; }
        //    }
        //    var quartermodel = new QuarterGetModel();
        //    quartermodel.Q1 = Q1;
        //    quartermodel.Q2 = Q2;
        //    quartermodel.Q3 = Q3;
        //    quartermodel.Q4 = Q4;
        //    quartermodel.Q1Amount = Q1Amount;
        //    quartermodel.Q2Amount = Q2Amount;
        //    quartermodel.Q3Amount = Q3Amount;
        //    quartermodel.Q4Amount = Q4Amount;
        //    return quartermodel;
        //}

        public GpQuarterGetModel getQuarterGPSummary(List<Pipeline> pipelines)
        {
            double? Q1Amount = 0;
            double? Q2Amount = 0;
            double? Q3Amount = 0;
            double? Q4Amount = 0;
            foreach (var pipeline in pipelines)
            {
                if (getQuarterNumberWithinSameYear(pipeline.UpdatedDate == default ? pipeline.CreatedDate : pipeline.UpdatedDate) == 1)
                {
                    if (pipeline.CogsAmount != null)
                    {
                        Q1Amount += pipeline.DealAmount - pipeline.CogsAmount;
                    }
                    else
                    {
                        Q1Amount += pipeline.DealAmount * pipeline.Margin;
                    }
                }
                else if (getQuarterNumberWithinSameYear(pipeline.UpdatedDate == default ? pipeline.CreatedDate : pipeline.UpdatedDate) == 2)
                {
                    if (pipeline.CogsAmount != null)
                    {
                        Q2Amount += pipeline.DealAmount - pipeline.CogsAmount;
                    }
                    else
                    {
                        Q2Amount += pipeline.DealAmount * pipeline.Margin;
                    }
                }
                else if (getQuarterNumberWithinSameYear(pipeline.UpdatedDate == default ? pipeline.CreatedDate : pipeline.UpdatedDate) == 3)
                {
                    if (pipeline.CogsAmount != null)
                    {
                        Q3Amount += pipeline.DealAmount - pipeline.CogsAmount;
                    }
                    else
                    {
                        Q3Amount += pipeline.DealAmount * pipeline.Margin;
                    }
                }
                else if (getQuarterNumberWithinSameYear(pipeline.UpdatedDate == default ? pipeline.CreatedDate : pipeline.UpdatedDate) == 4)
                {
                    if (pipeline.CogsAmount != null)
                    {
                        Q4Amount += pipeline.DealAmount - pipeline.CogsAmount;
                    }
                    else
                    {
                        Q4Amount += pipeline.DealAmount * pipeline.Margin;
                    }
                }
            }
            var gpQuarterModel = new GpQuarterGetModel();
            gpQuarterModel.Q1Amount = Q1Amount.Value;
            gpQuarterModel.Q2Amount = Q2Amount.Value;
            gpQuarterModel.Q3Amount = Q3Amount.Value;
            gpQuarterModel.Q4Amount = Q4Amount.Value;
            return gpQuarterModel;
        }

        public int getQuarterNumber(int month)
        {
            int quarterNumber = 0;
            if (month >= 1 && month <= 3) quarterNumber = 1;
            else if (month > 3 && month <= 6) quarterNumber = 2;
            else if (month > 6 && month <= 9) quarterNumber = 3;
            else if (month > 9 && month <= 12) quarterNumber = 4;
            return quarterNumber;
        }

        public int getQuarterNumberWithinSameYear(DateTime createOrUpdateTime)
        {
            int quarterNumber = 0;
            if (createOrUpdateTime.Month >= 1 && createOrUpdateTime.Month <= 3 && createOrUpdateTime.Year == DateTime.Now.Year) quarterNumber = 1;
            else if (createOrUpdateTime.Month > 3 && createOrUpdateTime.Month <= 6 && createOrUpdateTime.Year == DateTime.Now.Year) quarterNumber = 2;
            else if (createOrUpdateTime.Month > 6 && createOrUpdateTime.Month <= 9 && createOrUpdateTime.Year == DateTime.Now.Year) quarterNumber = 3;
            else if (createOrUpdateTime.Month > 9 && createOrUpdateTime.Month <= 12 && createOrUpdateTime.Year == DateTime.Now.Year) quarterNumber = 4;
            return quarterNumber;
        }

        public async Task<Performance> GetPerformance()
        {
            var targetAchieve = await GetTargetAchieved();
            var performance = new Performance();
            if (targetAchieve.Q1.Target == 0)
            {
                performance.Q1 = 0;
            }
            else
            {
                performance.Q1 = Math.Round(targetAchieve.Q1.Achieved / targetAchieve.Q1.Target, 2);
            }

            if (targetAchieve.Q2.Target == 0)
            {
                performance.Q2 = 0;
            }
            else
            {
                performance.Q2 = Math.Round(targetAchieve.Q2.Achieved / targetAchieve.Q2.Target, 2);
            }
            if (targetAchieve.Q3.Target == 0)
            {
                performance.Q3 = 0;
            }
            else
            {
                performance.Q3 = Math.Round(targetAchieve.Q3.Achieved / targetAchieve.Q3.Target, 2);
            }
            if (targetAchieve.Q4.Target == 0)
            {
                performance.Q4 = 0;
            }
            else
            {
                performance.Q4 = Math.Round(targetAchieve.Q4.Achieved / targetAchieve.Q4.Target, 2);
            }
            return performance;
        }

        public async Task<Performance> GetPerformanceById(string employeeId)
        {
            var targetAchieve = await GetTargetAchievedById(employeeId);
            var performance = new Performance();
            if (targetAchieve.Q1.Target == 0)
            {
                performance.Q1 = 0;
            }
            else
            {
                performance.Q1 = Math.Round(targetAchieve.Q1.Achieved / targetAchieve.Q1.Target, 2);
            }

            if (targetAchieve.Q2.Target == 0)
            {
                performance.Q2 = 0;
            }
            else
            {
                performance.Q2 = Math.Round(targetAchieve.Q2.Achieved / targetAchieve.Q2.Target, 2);
            }
            if (targetAchieve.Q3.Target == 0)
            {
                performance.Q3 = 0;
            }
            else
            {
                performance.Q3 = Math.Round(targetAchieve.Q3.Achieved / targetAchieve.Q3.Target, 2);
            }
            if (targetAchieve.Q4.Target == 0)
            {
                performance.Q4 = 0;
            }
            else
            {
                performance.Q4 = Math.Round(targetAchieve.Q4.Achieved / targetAchieve.Q4.Target, 2);
            }
            return performance;
        }

        public async Task<List<ScheduleGetModel>> GetTodaySchedule()
        {
            var user = await _accountUserService.GetCurrentUserWithEmployeAllEvents();

            var scheduleEventModels = user.Appointments.Where(x => x.EventStartDateTime.Year.Equals(DateTime.UtcNow.Year) && x.EventStartDateTime.Month.Equals(DateTime.UtcNow.Month) && x.EventStartDateTime.Day.Equals(DateTime.UtcNow.Day)).Select(model => new ScheduleEventModel { Appointment = model, EventDateTime = model.EventStartDateTime }).ToList();
            scheduleEventModels.AddRange(user.Organization.Events.Where(x => x.EventStartDateTime.Year.Equals(DateTime.Now.Year) && x.EventStartDateTime.Month.Equals(DateTime.Now.Month) && x.EventStartDateTime.Day.Equals(DateTime.UtcNow.Day)).Select(employeeEvent => new ScheduleEventModel { Event = employeeEvent, EventDateTime = employeeEvent.EventStartDateTime }));
            scheduleEventModels.AddRange(user.Tasks.Where(x => x.EventStartDateTime.Year.Equals(DateTime.Now.Year) && x.EventStartDateTime.Month.Equals(DateTime.Now.Month) && x.EventStartDateTime.Day.Equals(DateTime.UtcNow.Day)).Select(task => new ScheduleEventModel { Task = task, EventDateTime = task.EventStartDateTime }));
            List<ScheduleGetModel> scheduleGetModels = new List<ScheduleGetModel>();
            foreach (var scheduleEventModel in scheduleEventModels)
            {
                ScheduleGetModel scheduleGetModel = _mapper.Map<ScheduleGetModel>(scheduleEventModel);
                scheduleGetModels.Add(scheduleGetModel);
            }
            return scheduleGetModels;
        }
    }
}