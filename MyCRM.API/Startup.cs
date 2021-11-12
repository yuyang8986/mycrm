using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCRM.Persistence;
using MyCRM.Persistence.Data;
using MyCRM.Services.Repository.ActivityRepository;
using MyCRM.Services.Repository.AppointmentRepository;
using MyCRM.Services.Repository.CompanyRepository;
using MyCRM.Services.Repository.EmployeeRepository;
using MyCRM.Services.Repository.EventRepository;
using MyCRM.Services.Repository.OrganizationRepository;
using MyCRM.Services.Repository.PeopleRepository;
using MyCRM.Services.Repository.PipelineRepository;
using MyCRM.Services.Repository.StageRepository;
using MyCRM.Services.Repository.TargetTemplateRepository;
using MyCRM.Services.Repository.TaskRepository;
using MyCRM.Services.Services;
using MyCRM.Services.Services.AccountUserService;
using MyCRM.Services.Services.DashboardService;
using MyCRM.Services.Services.EmailSenderService;
using MyCRM.Services.Services.EmployeeService;
using MyCRM.Services.Services.ScheduleService;
using MyCRM.Services.Services.StageService;
using MyCRM.Shared.Communications.Requests.Employee;
using MyCRM.Shared.Exceptions;
using MyCRM.Shared.Models.User;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System;
using Stripe;

namespace MyCRM.API
{
    public class Startup
    {
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, IHostingEnvironment environment, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Environment = environment;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.IsDevelopment())
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LocalConnection")));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            }
            _logger.LogInformation("Added TodoRepository to services");
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IAccountUserService, AccountUserService>();
            services.AddScoped<IStageRepository, StageRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IPeopleRepository, PeopleRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IPipelineRepository, PipelineRepository>();
            services.AddScoped<IStageRepository, StageRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IStageService, StageService>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<ITargetTemplateRepository, TargetTemplateRepository>();
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();

            services.Configure<AuthMessageSenderOptions>(Configuration);

            StripeConfiguration.ApiKey = "sk_live_qrJZNaDlZ4AdlQVDB8yarMNB003JmZgyBu";
            // requires
            // using Microsoft.AspNetCore.Identity.UI.Services;
            // using WebPWrecover.Services;
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<IAccountManager, AccountManager>();
            services.AddAutoMapper(typeof(Startup));
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "MyCRM API v1", Version = "v1" });
                var xmlPath = AppDomain.CurrentDomain.BaseDirectory + @"MyCRM.API.xml";
                //c.IncludeXmlComments(xmlPath);
            });

            services.AddIdentityServer(
                options =>
                {
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (Environment.IsDevelopment())
                        options.IssuerUri = Configuration["LocalIdentityServer"];
                    //options.IssuerUri = Configuration["ProdServer"];
                    else
                        options.IssuerUri = Configuration["ProdServer"];
                });
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer("Bearer", options =>
                {
                    if (Environment.IsDevelopment())
                        options.Authority = Configuration["LocalIdentityServer"];
                    //options.Authority = Configuration["ProdServer"];
                    else if (Environment.IsProduction()) options.Authority = Configuration["ProdServer"];

                    options.RequireHttpsMetadata = false;

                    options.Audience = "v1";
                });

            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("manager", policy => policy.RequireRole("manager"));
                    options.AddPolicy("admin", policy => policy.RequireRole("admin"));
                }
                );

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Lockout.AllowedForNewUsers = false;
            });

            services.AddMvcCore(
                    opt =>
                    {
                        opt.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                            _ => "The field is required.");
                        opt.Filters.Add(typeof(CustomExceptionFilterAttribute));
                    }).AddApiExplorer()
                .AddAuthorization()
                .AddJsonFormatters().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }).AddFluentValidation(fv =>
                    {
                        fv.RegisterValidatorsFromAssemblyContaining<EmployeeAddRequestValidator>();
                        fv.ImplicitlyValidateChildProperties = true;
                    }
                );

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            //services.AddTransient<IProfileService, IdentityClaimsProfileService>();
            services.AddHttpContextAccessor();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _logger.LogInformation("In Development environment");
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => { builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
            app.UseAuthentication();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyCRM API v1"); });
        }
    }
}