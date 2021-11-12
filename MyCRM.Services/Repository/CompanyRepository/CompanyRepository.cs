using AutoMapper;
using ETLib.Interfaces.Repository;
using ETLib.Models.QueryResponse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Communications.Requests.Company;
using MyCRM.Shared.Constants;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.ViewModels.Contact.CompanyViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyCRM.Services.Repository.CompanyRepository
{
    public class CompanyRepository : RepositoryBase, ICompanyRepository
    {
        private readonly IAccountManager _accountManager;
        private readonly IAccountUserService _accountUserService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CompanyRepository(ApplicationDbContext context, IAccountManager accountManager, IAccountUserService accountUserService, ILogger<CompanyRepository> logger, IMapper mapper) : base(context)
        {
            _accountManager = accountManager;
            _accountUserService = accountUserService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ResponseBaseModel<Company>> GetById(int id, CancellationToken cancellationToken)
        {
            var company = await Context.Companies.FindAsync(id);
            if (company == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Company{id} NOT FOUND", id);
                return ResponseBaseModel<Company>.GetNotFoundResponse();
            }
            return ResponseBaseModel<Company>.GetSuccessResponse(company);
        }

        private async Task<Func<Company, bool>> FindCompaniesFunc()
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
            var roles = await _accountManager.GetUserRolesAsync(user);
            Func<Company, bool> findCompaniesFunc;
            if (roles.Contains("manager"))
            {
                findCompaniesFunc = (p) => p.ApplicationUser?.OrganizationId == user.OrganizationId && !p.Name.Equals(CompanyNamePredefined.ImportedFromPhone);
            }
            else
            {
                findCompaniesFunc = (p) => p.ApplicationUser.Id == user.Id && !p.Name.Equals(CompanyNamePredefined.ImportedFromPhone);
            }

            return findCompaniesFunc;
        }

        public async Task<ResponseBaseModel<IEnumerable<CompanyGetModel>>> GetAll(CancellationToken cancellationToken, bool includeDelete = false)
        {
            try
            {
                var findCompaniesFunc = await FindCompaniesFunc();

                var companies = Context.Companies.
                    Include(s => s.Pipelines)
                    .ThenInclude(s => s.People).
                    Include(s => s.Peoples).ThenInclude(s => s.Pipelines).ThenInclude(s => s.People).
                    Include(x => x.ApplicationUser)
                    .ThenInclude(x => x.PipeLineFlows).ThenInclude(s => s.Stage)
                    .Include(x => x.ApplicationUser)
                    .ThenInclude(x => x.Companies).ThenInclude(s => s.Peoples).ThenInclude(s => s.Pipelines).
                    ThenInclude(s => s.People).
                    Where(findCompaniesFunc).ToList();

                List<CompanyGetModel> companyGetModels = new List<CompanyGetModel>();
                foreach (var company in companies)
                {
                    CompanyGetModel companyGetModel = _mapper.Map<CompanyGetModel>(company);
                    companyGetModel.Peoples = companyGetModel.Peoples.OrderBy(x => x.Name).ToList();
                    companyGetModels.Add(companyGetModel);
                }

                if (!includeDelete)
                {
                    companyGetModels = companyGetModels.Where(s => !s.IsDeleted).ToList();
                }

                return ResponseBaseModel<IEnumerable<CompanyGetModel>>.GetSuccessResponse(companyGetModels.OrderBy(x => x.Name));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<ResponseBaseModel<Company>> Add(Company company)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResponseBaseModel<Company>> Update(int id, Company request)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ResponseBaseModel<Company>> Update(int id, CompanyPutRequest request)
        {
            var company = await Context.Companies
                .Include(s => s.Peoples)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (company == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Company{id} NOT FOUND", id);
                return ResponseBaseModel<Company>.GetNotFoundResponse();
            }
            company.Name = request.Name;
            company.Email = request.Email;
            company.Location = request.Location;
            company.Phone = request.Phone;
            company.SecondaryPhone = request.SecondaryPhone;
            company.SecondaryEmail = request.SecondaryEmail;

            Context.Update(company);
            return await SaveDbAndReturnReponse(company);
        }

        public async Task<ResponseBaseModel<IEnumerable<CompanyGetModel>>> GetAllForCurrentEmployee(CancellationToken cancellationToken, bool includeDeleted = false)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();

            var companies = await Context.Companies.
                Include(x => x.ApplicationUser)
                .ThenInclude(x => x.PipeLineFlows).ThenInclude(s => s.Stage)
                .Include(x => x.ApplicationUser)
                .ThenInclude(x => x.Companies).ThenInclude(s => s.Peoples).ThenInclude(s => s.Pipelines).Where(s => s.ApplicationUserId == user.Id && s.Name != CompanyNamePredefined.ImportedFromPhone).ToListAsync(cancellationToken);
            //return ResponseBaseModel<IEnumerable<Company>>.GetSuccessResponse(companies);

            if (!includeDeleted)

                companies = companies.Where(s => !s.IsDeleted).ToList();
            List<CompanyGetModel> companyGetModels = new List<CompanyGetModel>();
            foreach (var company in companies)
            {
                CompanyGetModel companyGetModel = _mapper.Map<CompanyGetModel>(company);
                companyGetModels.Add(companyGetModel);
            }

            return ResponseBaseModel<IEnumerable<CompanyGetModel>>.GetSuccessResponse(companyGetModels.OrderBy(x => x.Name));
        }

        public async Task<ResponseBaseModel<Company>> Delete(int id)
        {
            var company = await Context.Companies.FindAsync(id);
            if (company == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Company{id} NOT FOUND", id);
                return ResponseBaseModel<Company>.GetNotFoundResponse();
            }

            company.IsDeleted = true;
            Context.Companies.Update(company);
            return await SaveDbAndReturnReponse(company);
        }

        public async Task<ResponseBaseModel<Company>> Add(Company company, Guid? pipelineId, int? peopleId)
        {
            //if (pipelineId.HasValue)
            //{
            //    var pipeline = await Context.Pipelines.FindAsync(pipelineId);

            //    if (pipeline != null)
            //    {
            //        if (company.Pipelines == null)
            //        {
            //            company.Pipelines = new List<Pipeline> { pipeline };
            //        }
            //        else
            //        {
            //            company.Pipelines.Add(pipeline);
            //        }
            //    }
            //    else
            //    {
            //        throw new NotFoundException(nameof(Pipeline), pipelineId);
            //    }
            //}

            //if (peopleId.HasValue)
            //{
            //    var people = await Context.Peoples.FindAsync(peopleId);
            //    if (people != null)
            //    {
            //        if (company.Peoples == null)
            //        {
            //            company.Peoples = new List<People> { people };
            //        }
            //        else
            //        {
            //            company.Peoples.Add(people);
            //        }
            //    }
            //    else
            //    {
            //        throw new NotFoundException(nameof(Pipeline), pipelineId);
            //    }
            //}

            var user = await _accountUserService.GetUserWithEmployeeOrganizationData();
            company.ApplicationUserId = user.Id;

            Context.Companies.Add(company);
            return await SaveDbAndReturnReponse(company);
        }

        public Task<ResponseBaseModel<IEnumerable<Company>>> GetAll(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ResponseBaseModel<IEnumerable<Company>>> IRepository<Company, int>.GetAll(CancellationToken cancellationToken, bool includeDeleted)
        {
            throw new NotImplementedException();
        }
    }
}