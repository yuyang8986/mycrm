// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCRM.Persistence.Data;
using MyCRM.Shared.Constants;
using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.TargetTemplate;
using MyCRM.Shared.Models.User;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyCRM.Shared.Models.Activities;

namespace MyCRM.Persistence
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync();
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountManager _accountManager;
        //private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(ApplicationDbContext context, IAccountManager accountManager)
        {
            _context = context;
            _accountManager = accountManager;
            //_logger = logger;
        }

        public virtual async Task SeedAsync()
        {
            //var services = new ServiceCollection();
            //services.AddDbContext<ApplicationDbContext>(options =>
            //   options.UseSqlServer(connectionString));

            //services.AddIdentity<ApplicationUser, ApplicationRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            // using (var serviceProvider = services.BuildServiceProvider())
            //{
            //using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            // {
            //var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            //var _accountManager = scope.ServiceProvider.GetRequiredService<_accountManager>();

            //_context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            await _context.Database.MigrateAsync().ConfigureAwait(true);
            if (!_context.Organizations.IgnoreQueryFilters().Any())
            {
                var organization = new Organization("GreenPOS Company")
                {
                    SubscriptionStartDate = DateTime.Now,
                    SubscriptionPlan = SubscriptionPlan.Premium,
                    StripeSubscriptionId = "sub_GBGqxmIF2iRCmd",
                    StripeCustomerId = "cus_GBGoZCtD2LvqtE",
                    //IsFreeTrail = false,
                    SubscriptionQuantity = 100,
                    SubscriptionExpirationDate = DateTime.Now.AddYears(10)
                };

                _context.Organizations.Add(organization);
                await _context.SaveChangesAsync();
            }

            //var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var alice = _accountManager.GetUserByUserNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    FirstName = "Alice",
                    LastName = "Yu",
                    Organization = _context.Organizations.First(),
                    Email = "alice@crm.com",
                    PhoneNumber = "04121212112",
                    IsActive = true
                };

                try
                {
                    await EnsureRoleAsync(_accountManager, "employee", "normal account");

                    var (succeeded, errors) = await _accountManager.CreateUserAsync(alice, new string[] { "employee" }, "Pass123$");
                    //var result = await userMgr.CreateAsync(alice, "Pass123$");
                    if (!succeeded)
                    {
                        throw new Exception($"Seeding \"alice\" user failed. Errors: {string.Join(Environment.NewLine, errors)}");
                    }

                    Console.WriteLine("alice created");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            var manager = _accountManager.GetUserByUserNameAsync("manager").Result;
            if (manager == null)
            {
                manager = new ApplicationUser
                {
                    UserName = "manager",
                    FirstName = "manager",
                    LastName = "Yu",
                    Organization = _context.Organizations.First(),
                    Email = "manager@crm.com",
                    PhoneNumber = "04121212112",
                    IsActive = true
                };

                try
                {
                    await EnsureRoleAsync(_accountManager, "manager", "manager");

                    var result = await _accountManager.CreateUserAsync(manager, new string[] { "employee", "manager" }, "Pass123$");
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Seeding \"manager\" user failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
                    }

                    Console.WriteLine("manager created");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                Console.WriteLine("manager already exists");
            }

            var admin = _accountManager.GetUserByUserNameAsync("admin").Result;
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin",
                    FirstName = "admin",
                    LastName = "Wei",
                    Organization = _context.Organizations.First(),
                    Email = "admin@crm.com",
                    PhoneNumber = "04121212112",
                    IsActive = true
                };

                try
                {
                    await EnsureRoleAsync(_accountManager, "admin", "admin");

                    var result = await _accountManager.CreateUserAsync(admin, new string[] { "employee", "manager", "admin" }, "Pass123$");
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Seeding \"admin\" user failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
                    }

                    Console.WriteLine("admin created");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                Console.WriteLine("admin already exists");
            }

            if (!_context.Activities.Any())
            {
                try
                {
                    var activity = new Activity { Name = "Meeting", ActivityType = ActivityType.Appointment, OrganizationId = _context.Organizations.First().Id };
                    var activity2 = new Activity { Name = "Lunch", ActivityType = ActivityType.Appointment, OrganizationId = _context.Organizations.First().Id };
                    var activity3 = new Activity { Name = "Dinner", ActivityType = ActivityType.Appointment, OrganizationId = _context.Organizations.First().Id };

                    _context.Activities.Add(activity);
                    _context.Activities.Add(activity2);

                    _context.Activities.Add(activity3);

                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                }
            }
            if (!_context.Stages.IgnoreQueryFilters().Any())
            {
                foreach (var stage in Organization.DefaultStages)
                {
                    stage.OrganizationId = _context.Organizations.First().Id;
                    _context.Stages.Add(stage);
                }

                await _context.SaveChangesAsync();
            }
            if (!_context.Companies.IgnoreQueryFilters().Any())
            {
                var company = new Company("ABC Company")
                {
                    ApplicationUser = _context.Users.First(),
                    Location = "123, Pit St, Sydney"
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
            }

            if (!_context.Peoples.IgnoreQueryFilters().Any())
            {
                var people = new People()
                {
                    //EmployeeId = context.Employees.First().Id,
                    FirstName = "Tom",
                    LastName = "Wang",
                    CompanyId = _context.Companies.First().Id
                };

                _context.Peoples.Add(people);
                await _context.SaveChangesAsync();
            }

            //if (!_context.Pipelines.IgnoreQueryFilters().Any())
            //{
            //    var pipeline = new Pipeline
            //    {
            //        DealName = "IBM Services",
            //        DealAmount = 1000,
            //        StageId = _context.Stages.First().Id,
            //        ApplicationUserId = _context.ApplicationUsers.First().Id,
            //        //CompanyId = context.Companies.First().Id,
            //        PeopleId = _context.Peoples.First().Id,
            //    };

            //    await _context.Pipelines.AddAsync(pipeline);
            //    await _context.SaveChangesAsync();
            //}
            if (!_context.TargetTemplates.IgnoreQueryFilters().Any())
            {
                var targetTemplate = new TargetTemplate
                {
                    Name = "2019 Senior Sales",
                    Q1 = 1000,
                    Q2 = 1200,
                    Q3 = 1100,
                    Q4 = 1000,
                    Organization = _context.Organizations.First(),
                };
                _context.TargetTemplates.Add(targetTemplate);
                await _context.SaveChangesAsync();
            }
            //}
            // }
        }

        private static async Task EnsureRoleAsync(IAccountManager _accountManager, string roleName, string description)
        {
            if ((await _accountManager.GetRoleByNameAsync(roleName)) == null)
            {
                ApplicationRole applicationRole = new ApplicationRole(roleName, description);

                var resultCreatingRole = await _accountManager.CreateRoleAsync(applicationRole);

                if (!resultCreatingRole.Succeeded)
                    throw new Exception(
                        $"Seeding \"{roleName}\" role failed. Errors: {string.Join(Environment.NewLine, resultCreatingRole.Errors)}");
            }
        }
    }
}