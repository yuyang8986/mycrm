using AutoMapper;
using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.People;
using MyCRM.Shared.Constants;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.ViewModels.Contact.PersonViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Services.Repository.PeopleRepository
{
    public class PeopleRepository : RepositoryBase, IPeopleRepository
    {
        private readonly IAccountManager _accountManager;
        private readonly IAccountUserService _accountUserService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public PeopleRepository(ApplicationDbContext context, IAccountManager accountManager, IAccountUserService accountUserService, ILogger<PeopleRepository> logger, IMapper mapper) : base(context)
        {
            _accountManager = accountManager;
            _accountUserService = accountUserService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseBaseModel<People>> GetById(int id, CancellationToken cancellationToken)
        {
            var people = await Context.Peoples.FindAsync(id);

            return ResponseBaseModel<People>.GetSuccessResponse(people);
        }

        private async Task<Func<People, bool>> FindPeoplesFunc()
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
            var roles = await _accountManager.GetUserRolesAsync(user);
            Func<People, bool> findPeoplesFunc;
            if (roles.Contains("manager"))
            {
                findPeoplesFunc = (p) => p.ApplicationUser?.OrganizationId == user.OrganizationId;
            }
            else
            {
                findPeoplesFunc = (p) => p.ApplicationUser.Id == user.Id;
            }

            return findPeoplesFunc;
        }

        public async Task<ResponseBaseModel<IEnumerable<PersonGetModel>>> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var findPeoplesFunc = await FindPeoplesFunc();

                // ReSharper disable once InvertIf
                //if (user.Employee.IsSupervisor)
                //{
                var peoples = Context.Peoples
                    .Include(s => s.Pipelines).ThenInclude(s => s.Stage)
                    .Include(s => s.Company)
                    .ThenInclude(s => s.ApplicationUser)
                    .Where(findPeoplesFunc)
                    .ToList();
                //return ResponseBaseModel<IEnumerable<People>>.GetSuccessResponse(peoples);
                //}

                //else
                //{
                //var peoples = Context.Peoples.Include(s => s.Company).Include(x => x.Employee)
                //    .ThenInclude(x => x.PipeLineFlows).ThenInclude(s => s.Stage).Include(x => x.Employee)
                //    .ThenInclude(x => x.PipeLineFlows).ThenInclude(s => s.Appointment)
                //    .Include(x => x.Employee)
                //    .ThenInclude(x => x.Peoples)
                //    .Include(x => x.Employee)
                //    .ThenInclude(x => x.Companies).ThenInclude(s => s.Peoples).Include(s => s.Employee)
                //    .Where(s => s.Employee.Id == user.Employee.Id).ToList();
                List<PersonGetModel> personGetModels = new List<PersonGetModel>();
                foreach (var person in peoples)
                {
                    PersonGetModel personGetModel = _mapper.Map<PersonGetModel>(person);
                    personGetModels.Add(personGetModel);
                }
                return ResponseBaseModel<IEnumerable<PersonGetModel>>.GetSuccessResponse(personGetModels.OrderBy(x => x.Name));
                //}
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.ListItemsFailed, e, "List Peoples Failed");
                return ResponseBaseModel<IEnumerable<PersonGetModel>>.GetUnexpectedErrorResponse(e);
            }
        }

        public Task<ResponseBaseModel<People>> Add(People request)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ResponseBaseModel<People>> ImportMultiplePersons(ImportRequest requests)
        {
            var user = await _accountUserService.GetCurrentApplicationUserWithOutData();

            foreach (var request in requests.PeopleList)
            {
                var company = Context.Companies.Where(x => x.ApplicationUserId == user.Id && x.Name.Equals(CompanyNamePredefined.ImportedFromPhone)).FirstOrDefault();
                var newPerson = new People();
                newPerson.FirstName = request.FirstName;
                newPerson.LastName = request.LastName;
                newPerson.Phone = request.Phone;
                newPerson.Email = request.Email;
                if (string.IsNullOrEmpty(request.CompanyName))
                {
                    if (company == null)
                    {
                        var newCompany = new Company(CompanyNamePredefined.ImportedFromPhone);
                        newCompany.ApplicationUserId = user.Id;
                        Context.Companies.Add(newCompany);
                        await SaveDbAndReturnReponse(newCompany);
                        Context.SaveChanges();
                        newPerson.CompanyId = newCompany.Id;
                    }
                    else
                    {
                        newPerson.CompanyId = company.Id;
                    }
                }
                else
                {
                    var newCompany = new Company(request.CompanyName);
                    newCompany.ApplicationUserId = user.Id;
                    Context.Companies.Add(newCompany);
                    await SaveDbAndReturnReponse(newCompany);
                    Context.SaveChanges();
                    newPerson.CompanyId = newCompany.Id;
                }
                Context.Peoples.Add(newPerson);
                if (!await Save()) return ResponseBaseModel<People>.GetDbSaveFailedResponse();
            }
            return ResponseBaseModel<People>.GetSuccessResponse(null);
        }

        public async Task<ResponseBaseModel<People>> Update(int id, PeoplePutRequest request)
        {
            var people = await Context.Peoples.Include(s => s.Pipelines).Where(s => s.Id == id).FirstOrDefaultAsync();
            if (people == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "People{id} NOT FOUND", id);
                return ResponseBaseModel<People>.GetNotFoundResponse();
            }
            people.FirstName = request.FirstName;
            people.LastName = request.LastName;
            people.Email = request.Email;
            people.WorkEmail = request.WorkEmail;
            people.Phone = request.Phone;
            people.WorkPhone = request.WorkPhone;
            people.CompanyId = request.CompanyId;

            //if (!string.IsNullOrWhiteSpace(request.PipelineId.ToString()))
            //{
            //    var addedPipeline = await Context.Pipelines.FindAsync(request.PipelineId);
            //    if (addedPipeline == null) return ResponseBaseModel<People>.GetNotFoundResponse(typeof(Pipeline));

            //    people.Pipelines.Add(addedPipeline);
            //}
            Context.Peoples.Update(people);

            return await SaveDbAndReturnReponse(people);
        }

        public async Task<ResponseBaseModel<IEnumerable<PersonGetModel>>> GetPeoplesForCurrentEmployee(CancellationToken cancellationToken)
        {
            try
            {
                var user = await _accountUserService.GetUserWithEmployeeOrganizationData();

                var peoples = await Context.Peoples
                    //.Include(s => s.Pipelines).ThenInclude(s => s.Stage)
                    .Include(s => s.Company)
                    //.ThenInclude(s => s.ApplicationUser)
                    .Where(p => p.Company.ApplicationUserId == user.Id).ToListAsync(cancellationToken: cancellationToken);
                List<PersonGetModel> personGetModels = new List<PersonGetModel>();
                foreach (var person in peoples)
                {
                    PersonGetModel personGetModel = _mapper.Map<PersonGetModel>(person);
                    personGetModels.Add(personGetModel);
                }

                return ResponseBaseModel<IEnumerable<PersonGetModel>>.GetSuccessResponse(personGetModels.OrderBy(x => x.Name));
            }
            catch (Exception e)
            {
                _logger.LogError(LoggingEvents.ListItemsFailed, e, "Get Peoples For Current Employee Failed");
                return ResponseBaseModel<IEnumerable<PersonGetModel>>.GetUnexpectedErrorResponse(e);
            }
        }

        public async Task<ResponseBaseModel<People>> Delete(int id)
        {
            var people = await Context.Peoples.FindAsync(id);
            if (people == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "People{id} NOT FOUND", id);
                return ResponseBaseModel<People>.GetNotFoundResponse();
            }
            people.IsDeleted = true;
            Context.Peoples.Update(people);
            return await SaveDbAndReturnReponse(people);
            _logger.LogWarning(LoggingEvents.DeleteItemFailed, "Delete Peoples({id}) Failed", id);
            return ResponseBaseModel<People>.GetDbSaveFailedResponse();
        }

        public async Task<ResponseBaseModel<People>> Add(People people, int companyId, Guid? pipelineId)
        {
            //check data passed in is valid

            var company = await Context.Companies.FindAsync(companyId);
            if (company != null)
            {
                people.CompanyId = companyId;
            }
            else
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Company{id} NOT FOUND", companyId);
                return ResponseBaseModel<People>.GetNotFoundResponse(typeof(Company));
            }

            //if (pipelineId.HasValue)
            //{
            //    var pipeline = await Context.Pipelines.FindAsync(pipelineId);
            //    if (pipeline != null)
            //    {
            //        if (people.Pipelines == null)
            //        {
            //            people.Pipelines = new List<Pipeline> { pipeline };
            //        }
            //        else
            //        {
            //            people.Pipelines.Add(pipeline);
            //        }
            //    }
            //    else
            //    {
            //        return ResponseBaseModel<People>.GetNotFoundResponse(typeof(Pipeline));
            //    }
            //}

            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();

            //people.EmployeeId = user.Employee.Id;
            Context.Peoples.Add(people);

            return await SaveDbAndReturnReponse(people);
        }

        public async Task<ResponseBaseModel<People>> ScanPerson(CreatePersonWithCompanyRequest request)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
            var transction = Context.Database.BeginTransaction();
            People newPerson = new People();
            try
            {
                Company newCompany = new Company(request.Company)
                {
                    ApplicationUserId = user.Id,
                    Location = request.Address
                };
                Context.Companies.Add(newCompany);

                Context.SaveChanges();

                newPerson.FirstName = request.FirstName;
                newPerson.LastName = request.LastName;
                newPerson.Phone = request.Phone;
                newPerson.WorkPhone = request.WorkPhone;
                newPerson.Email = request.Email;
                newPerson.CompanyId = newCompany.Id;

                Context.Peoples.Add(newPerson);
                if (!await Save()) return ResponseBaseModel<People>.GetDbSaveFailedResponse();
                Context.SaveChanges();
                transction.Commit();
                return ResponseBaseModel<People>.GetSuccessResponse(newPerson);
            }
            catch (Exception)
            {
                transction.Rollback();
                return ResponseBaseModel<People>.GetDbSaveFailedResponse();
            }
        }

        public Task<ResponseBaseModel<IEnumerable<PersonGetModel>>> GetAll(bool includeDelete = false)
        {
            throw new NotImplementedException();
        }

        Task<ResponseBaseModel<IEnumerable<People>>> IRepository<People, int>.GetAll(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ResponseBaseModel<IEnumerable<People>>> IRepository<People, int>.GetAll(CancellationToken cancellationToken, bool includeDeleted)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBaseModel<People>> Update(int id, People request)
        {
            throw new NotImplementedException();
        }
    }
}