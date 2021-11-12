using ETLib.Models.QueryResponse;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence;
using MyCRM.Persistence.Data;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Shared.Logging;
using MyCRM.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ETLib.Interfaces.Repository;
using MyCRM.Shared.ViewModels.ApplicationUser;
using MyCRM.Services.Services.EmailSenderService;

namespace MyCRM.Services.Repository.EmployeeRepository
{
    public class ApplicationUserRepository : RepositoryBase, IApplicationUserRepository
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountManager _accountManager;
        private readonly IAccountUserService _accountUserService;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;

        private char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z' };

        public ApplicationUserRepository(ApplicationDbContext context,
            IMapper mapper,
            UserManager<ApplicationUser> userManager, IAccountManager accountManager, ILogger<ApplicationUserRepository> logger, IEmailSender emailSender,
            IAccountUserService accountUserService) : base(context)
        {
            _mapper = mapper;
            _userManager = userManager;
            _accountManager = accountManager;
            _emailSender = emailSender;
            _accountUserService = accountUserService;
            _logger = logger;
        }

        public async Task<ResponseBaseModel<ApplicationUser>> GetById(string id, CancellationToken cancellationToken)
        {
            var employee = await Context.Users.Include(x => x.Organization)
                .Include(s => s.PipeLineFlows).Where(s => s.Id == id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (employee == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Employee{id} NOT FOUND", id);
                return ResponseBaseModel<ApplicationUser>.GetNotFoundResponse();
            }
            return ResponseBaseModel<ApplicationUser>.GetSuccessResponse(employee);
        }

        Task<ResponseBaseModel<IEnumerable<ApplicationUser>>> IRepository<ApplicationUser, string>.GetAll(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //public async Task<ResponseBaseModel<IEnumerable<ApplicationUser>>> GetAll()
        //{
        //    var user = await _accountUserService.GetCurrentApplicationUserWithPipelines();

        //    var allUsers = user.Organization.ApplicationUsers;

        //    foreach (var applicationUser in allUsers)
        //    {
        //        var roles = await _accountManager.GetUserRolesAsync(applicationUser);
        //        applicationUser.RoleStrings = roles;
        //    }

        //    return ResponseBaseModel<IEnumerable<ApplicationUser>>.GetSuccessResponse(allUsers);
        //}

        public async Task<ResponseBaseModel<IEnumerable<ApplicationUserGetAllViewModel>>> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var user = await _accountUserService.GetCurrentApplicationUserWithOutData();

                var allUsers = await Context.ApplicationUsers.Where(x => x.OrganizationId == user.OrganizationId)
                    .Include(x => x.PipeLineFlows).ThenInclude(x => x.Stage)
                    .Include(x => x.Companies)
                    .ThenInclude(x => x.Peoples).ToListAsync(cancellationToken);

                List<ApplicationUserGetAllViewModel> applicationUserGetAllViewModels = new List<ApplicationUserGetAllViewModel>();
                foreach (var applicationUser in allUsers)
                {
                    var roles = await _accountManager.GetUserRolesAsync(applicationUser);
                    applicationUser.RoleStrings = roles;

                    ApplicationUserGetAllViewModel applicationUserGetAllViewModel =
                        _mapper.Map<ApplicationUserGetAllViewModel>(applicationUser);

                    applicationUserGetAllViewModel.RoleStrings = roles;
                    applicationUserGetAllViewModels.Add(applicationUserGetAllViewModel);
                }
                return ResponseBaseModel<IEnumerable<ApplicationUserGetAllViewModel>>.GetSuccessResponse(applicationUserGetAllViewModels.OrderBy(x => x.Name));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<ResponseBaseModel<ApplicationUser>> Add(ApplicationUser applicationUser, bool isManager)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
            var activeAccount = user.Organization.ApplicationUsers.Count(s => s.IsActive);

            if (activeAccount > user.Organization.SubscriptionQuantity)
                throw new Exception("Not Enough Subscription on Accounts");
            applicationUser.UserName = applicationUser.Email;
            applicationUser.OrganizationId = user.OrganizationId;
            applicationUser.CreatedBy = user.Name;
            applicationUser.CreatedDate = DateTime.Now;
            var random = new Random();
            string password = "";
            for (int i = 0; i < 10; i++)
            {
                int rnd = random.Next(0, Pattern.Length);
                password += Pattern[rnd];
            }
            var (succeeded, errors) = await _accountManager.CreateUserAsync(applicationUser,
               isManager ? new[] { "manager", "employee" } : new[] { "employee" }, password);
            if (!succeeded)
            {
                var e = new Exception(errors.First().ToString());
                _logger.LogWarning(LoggingEvents.InsertItemFailed, e, "Create Employee({Id}) Failed", applicationUser.Id);
                throw e;
            }
            var userCreated = await _accountManager.GetUserByUserNameAsync(applicationUser.Email);

            if (userCreated == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Employee{id} NOT FOUND", applicationUser.Id);
                return ResponseBaseModel<ApplicationUser>.GetDbSaveFailedResponse();
            }
            await _emailSender.SendEmailAsync(applicationUser.Email, "Temporary Password",
                $" <img border=\"0\" style=\"max-width:15%!important; width:15%; height:auto \" src=\"http://cdn.mcauto-images-production.sendgrid.net/d0589116d89f7f13/4442293c-f53c-41d9-8554-9a764803b29b/3000x782.png \">" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">Hi {applicationUser.Name},</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">Your temp Password to access Dealo is '{password}'</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">Thank you</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><span style=\"font - family: verdana, geneva, sans - serif\">Dealo Team</span></div>" +
                $"<div style=\"font-family: inherit; text-align: inherit\"><br></div>");
            return ResponseBaseModel<ApplicationUser>.GetSuccessResponse(userCreated);
        }

        public async Task<ResponseBaseModel<ApplicationUser>> Update(string id, ApplicationUser request, bool isManager)
        {
            var user = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
            var activeAccount = user.Organization.ApplicationUsers.Count(s => s.IsActive);
            var currentUser = await _accountUserService.GetUserWithEmployeeOrganizationData(true);
            if (activeAccount > user.Organization.SubscriptionQuantity)
                throw new Exception("Not Enough Subscription on Accounts");
            var userToUpdate = await _userManager.FindByIdAsync(id);
            userToUpdate.FirstName = request.FirstName;
            userToUpdate.LastName = request.LastName;
            userToUpdate.IsActive = request.IsActive;
            userToUpdate.Email = request.Email;
            userToUpdate.PhoneNumber = request.PhoneNumber;
            userToUpdate.UpdatedBy = currentUser.Name;
            userToUpdate.UpdatedDate = DateTime.Now;

            var roles = await _accountManager.GetUserRolesAsync(userToUpdate);
            if (isManager)
            {
                if (!roles.Contains("manager"))
                {
                    var result = await _userManager.AddToRoleAsync(userToUpdate, "manager");

                    if (!result.Succeeded)
                    {
                        _logger.LogWarning(LoggingEvents.InsertItemFailed, "Update Employee({Id}) Failed", id);
                        return ResponseBaseModel<ApplicationUser>.GetDbSaveFailedResponse();
                    }
                }
            }
            else
            {
                if (roles.Contains("manager"))
                {
                    var result = await _userManager.RemoveFromRoleAsync(userToUpdate, "manager");

                    if (!result.Succeeded)
                    {
                        _logger.LogWarning(LoggingEvents.InsertItemFailed, "Update Employee({Id}) Failed", id);
                        return ResponseBaseModel<ApplicationUser>.GetDbSaveFailedResponse();
                    }
                }
            }

            Context.Update(userToUpdate);

            return await SaveDbAndReturnReponse<ApplicationUser>(userToUpdate);
        }

        public async Task<ResponseBaseModel<ApplicationUser>> UpdateEmployeeTemplate(string id, Guid templateId)

        {
            //var currentUser = await _accountUserService.GetUserWithEmployeeOrganizationData();

            var user = await Context.ApplicationUsers.FindAsync(id);

            if (user == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Employee{id} NOT FOUND", id);
                return ResponseBaseModel<ApplicationUser>.GetNotFoundResponse();
            }

            user.TargetTemplateId = templateId;
            Context.Update(user);

            return await SaveDbAndReturnReponse<ApplicationUser>(user);
        }

        public async Task<ResponseBaseModel<ApplicationUser>> RemoveEmployeeFromTemplate(string id)

        {
            //var currentUser = await _accountUserService.GetUserWithEmployeeOrganizationData(false);

            var user = await Context.ApplicationUsers.FindAsync(id);

            if (user == null)
            {
                _logger.LogWarning(LoggingEvents.GetItemNotFound, "Employee{id} NOT FOUND", id);
                return ResponseBaseModel<ApplicationUser>.GetNotFoundResponse();
            }

            user.TargetTemplateId = null;
            Context.Update(user);

            return await SaveDbAndReturnReponse<ApplicationUser>(user);
        }

        public Task<ResponseBaseModel<ApplicationUser>> Update(string id, ApplicationUser request)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseBaseModel<ApplicationUser>> Delete(string id)
        {
            //var currentUser = await _accountUserService.GetUserWithEmployeeOrganizationData(true);

            var user = await _userManager.FindByIdAsync(id);
            user.IsActive = false;
            Context.Update(user);
            return await SaveDbAndReturnReponse<ApplicationUser>(user);
        }

        public Task<ResponseBaseModel<IEnumerable<ApplicationUser>>> GetAll(CancellationToken cancellationToken, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseBaseModel<ApplicationUser>> Add(ApplicationUser request)
        {
            throw new NotImplementedException();
        }
    }
}