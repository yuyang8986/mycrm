using ETLib.Models.QueryResponse;
using MyCRM.Shared.Communications.Responses.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Services.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<ResponseBaseModel<DashboardResponseModel>> GetDashboard(CancellationToken cancellationToken);

        Task<ResponseBaseModel<DashboardResponseModel>> GetDashboardById(string id, CancellationToken cancellationToken);
    }
}