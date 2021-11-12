using ETLib.Models.QueryResponse;
using MyCRM.Shared.Models.Contacts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ETLib.Interfaces.Repository;
using MyCRM.Shared.Communications.Requests.People;
using MyCRM.Shared.ViewModels.Contact.PersonViewModel;

namespace MyCRM.Services.Repository.PeopleRepository
{
    public interface IPeopleRepository : IRepository<People, int>
    {
        Task<ResponseBaseModel<People>> Add(People people, int companyId, Guid? pipelineId);

        Task<ResponseBaseModel<People>> Update(int id, PeoplePutRequest request);

        Task<ResponseBaseModel<IEnumerable<PersonGetModel>>> GetPeoplesForCurrentEmployee(CancellationToken cancellationToken);

        Task<ResponseBaseModel<IEnumerable<PersonGetModel>>> GetAll(CancellationToken cancellationToken);
        Task<ResponseBaseModel<People>> ScanPerson(CreatePersonWithCompanyRequest request);
        Task<ResponseBaseModel<People>> ImportMultiplePersons(ImportRequest requests);
    }
}