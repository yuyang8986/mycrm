using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using MyCRM.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyCRM.Shared.ViewModels.ApplicationUser;

namespace MyCRM.Services.Repository.EmployeeRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser, string>
    {
        Task<ResponseBaseModel<ApplicationUser>> UpdateEmployeeTemplate(string id, Guid templateId);

        Task<ResponseBaseModel<ApplicationUser>> RemoveEmployeeFromTemplate(string id);

        Task<ResponseBaseModel<ApplicationUser>> Add(ApplicationUser applicationUser, bool isManager = false);

        //Task<ResponseBaseModel<IEnumerable<ApplicationUser>>> GetAllEmployeesWithPipelines();

        Task<ResponseBaseModel<IEnumerable<ApplicationUserGetAllViewModel>>> GetAll(CancellationToken cancellationToken);

        Task<ResponseBaseModel<ApplicationUser>> Update(string id, ApplicationUser request, bool isManager);
    }
}