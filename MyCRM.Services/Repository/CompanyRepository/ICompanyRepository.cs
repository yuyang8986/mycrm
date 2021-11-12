using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using MyCRM.Shared.Communications.Requests.Company;
using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.ViewModels.Contact.CompanyViewModel;

namespace MyCRM.Services.Repository.CompanyRepository
{
    public interface ICompanyRepository : IRepository<Company, int>
    {
        Task<ResponseBaseModel<Company>> Add(Company company, Guid? pipelineId, int? peopleId);

        Task<ResponseBaseModel<Company>> Update(int id, CompanyPutRequest request);

        Task<ResponseBaseModel<IEnumerable<CompanyGetModel>>> GetAllForCurrentEmployee(CancellationToken cancellationToken, bool includeDeleted = false);

        Task<ResponseBaseModel<IEnumerable<CompanyGetModel>>> GetAll(CancellationToken cancellationToken, bool includeDelete = false);
    }
}