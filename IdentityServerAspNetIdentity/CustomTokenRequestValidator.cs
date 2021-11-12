using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCRM.Persistence.Data;
using MyCRM.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCRM.Persistence;

namespace IdentityServerAspNetIdentity
{
    public class CustomTokenRequestValidator : ICustomTokenRequestValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IAccountManager _accountManager;

        public CustomTokenRequestValidator(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IAccountManager accountManager)
        {
            _userManager = userManager;
            _context = context;
            _accountManager = accountManager;
        }

        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            //var sub = context.Subject.GetSubjectId();
            var user = await _context.Users
                .Include(s => s.Organization).ThenInclude(x => x.ApplicationUsers)
                .Where(s => s.UserName == context.Result.ValidatedRequest.UserName).FirstOrDefaultAsync();

            var roles = await _accountManager.GetUserRolesAsync(user);
            //var role = await _userManager.GetRolesAsync(user);
            //var principal = await _claimsFactory.CreateAsync(user);
            try
            {
                if (user.Organization.IsSubExpired)
                {
                    if (roles.Contains("admin"))
                    {
                        context.Result.IsError = true;
                        context.Result.Error = "Subscription Expired";
                        context.Result.ErrorDescription = user.Organization.StripeSubscriptionId;
                    }
                    else
                    {
                        context.Result.IsError = true;
                        context.Result.Error = "Subscription Expired, please contact your admin";
                    }

                    //user.Organization.IsFreeTrail = false;
                    //_context.Organizations.Update(user.Organization);
                    //_context.SaveChanges();
                    //var subIdResponse = new Dictionary<string, object>
                    //{
                    //    {"subId", user.Organization.StripeSubscriptionId}
                    //};
                    //context.Result.CustomResponse = subIdResponse;
                    return;
                }

                int activeAccounts = user.Organization.ApplicationUsers.Count(s => s.IsActive);

                if (activeAccounts > user.Organization.SubscriptionQuantity)
                {
                    if (roles.Contains("admin")) return;
                    context.Result.IsError = true;
                    context.Result.Error = "Not Enough Accounts";
                    context.Result.ErrorDescription = user.Organization.StripeSubscriptionId;
                    //var subIdResponse = new Dictionary<string, object>
                    //{
                    //    {"subId", user.Organization.StripeSubscriptionId}
                    //};
                    //context.Result.CustomResponse = subIdResponse;
                    return;
                }

                if (!user.IsActive)
                {
                    context.Result.IsError = true;
                    context.Result.Error = "Account Disabled. Please contact your admin to enable account";
                    context.Result.ErrorDescription = "Account Disabled. Please contact GreenPOS admin";
                    return;
                }

                //else
                //{
                //    var claims = new List<Claim>
                //    {
                //        //new Claim("role", role.First()),
                //        new Claim("name", user.UserName),
                //        new Claim("isManager", user.Employee?.IsSupervisor !=null? user.Employee.IsSupervisor.ToString():"False"),
                //        new Claim("isActive", user.Employee?.IsActive !=null? user.Employee.IsActive.ToString():"False"),
                //        new Claim("email", user.Email),
                //        new Claim("employeeId", user.Employee?.Id != null? user.Employee?.Id.ToString():""),
                //        //new Claim(ClaimTypes.HomePhone, user.PhoneNumber),
                //        new Claim("username", user.UserName),
                //        new Claim("userId", user.Id),
                //        new Claim("organizationId",user.OrganizationId.ToString()),
                //        new Claim("employee",user.IsEmployeeAccount.ToString()),
                //    };

                //    //context.Result.CustomResponse =
                //}

                //claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
                //claims.Add(new Claim(JwtClaimTypes.Name, user.Name));
                //claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));
                //claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString()));

                // note: to dynamically add roles (ie. for users other than consumers - simply look them up by sub id
                // need this for role-based authorization - https://stackoverflow.com/questions/40844310/role-based-authorization-with-identityserver4

                //context.IssuedClaims = claims;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}