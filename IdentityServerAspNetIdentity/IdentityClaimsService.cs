using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCRM.Persistence.Data;
using MyCRM.Shared.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerAspNetIdentity
{
    public class IdentityClaimsProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _context;

        public IdentityClaimsProfileService(UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory
            )
        {
            _userManager = userManager;
            _context = context;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == sub);
            var roles = await _userManager.GetRolesAsync(user);
            var principal = await _claimsFactory.CreateAsync(user);
            try
            {
                var claims = principal.Claims.ToList();
                claims.AddRange(new
                    List<Claim>
                    {
                        //new Claim("role", role.First()),
                        new Claim("username", user.UserName),
                        new Claim("userId", user.Id),
                        new Claim("organizationId", user.OrganizationId.ToString()),
                       // new Claim("roles", String.Join(",", roles.ToArray()))
                    });

                //claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

                // note: to dynamically add roles (ie. for users other than consumers - simply look them up by sub id
                // need this for role-based authorization - https://stackoverflow.com/questions/40844310/role-based-authorization-with-identityserver4

                context.IssuedClaims = claims;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}