using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.ViewModels.Contact.EmployeeViewModel;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Services.Repository.OrganizationRepository
{
    public interface IOrganizationRepository : IRepository<Organization, int>
    {
        Task<ResponseBaseModel<EmployeeCountViewModel>> GetEmployeeCount(CancellationToken cancellationToken);
    }
}