// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using MyCRM.Persistence;
using MyCRM.Persistence.Data;
using System.Reflection;
using IdentityServer4.Validation;
using MyCRM.Shared.Models.User;

namespace IdentityServerAspNetIdentity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;

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
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<IAccountManager, AccountManager>();

            //X509Certificate2 cert = null;
            //using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            //{
            //    certStore.Open(OpenFlags.ReadOnly);
            //    X509Certificate2Collection certCollection = certStore.Certificates.Find(
            //        X509FindType.FindByThumbprint,
            //        // Replace below with your cert's thumbprint
            //        "CB781679561914B7539BE120EE9C4F6780579A86",
            //        false);
            //    // Get the first cert with the thumbprint
            //    if (certCollection.Count > 0)
            //    {
            //        cert = certCollection[0];
            //        Log.Logger.Information($"Successfully loaded cert from registry: {cert.Thumbprint}");
            //    }
            //}

            //// Fallback to local file for development
            //if (cert == null)
            //{
            //    cert = new X509Certificate2(Path.Combine(Environment.ContentRootPath, "example.pfx"), "exportpassword");
            //    Log.Logger.Information($"Falling back to cert from file. Successfully loaded: {cert.Thumbprint}");
            //}

            #region add is4

            var issueUri = "";
            if (Environment.IsDevelopment())
            {
                issueUri = "http://127.0.0.1:5000";
            }
            else
            {
                issueUri = "https://mycrmidentity.azurewebsites.net";
            }

            var builder = services.AddIdentityServer(options =>
                    {
                        options.IssuerUri = issueUri;
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseSuccessEvents = true;
                    }).AddAspNetIdentity<ApplicationUser>()
                    .AddProfileService<IdentityClaimsProfileService>()

                    .AddCustomTokenRequestValidator<CustomTokenRequestValidator>()
                    //.AddSigningCredential(cert);
                    .AddDeveloperSigningCredential()
                    //.AddConfigurationStore(options =>
                    //{
                    //    options.ConfigureDbContext = b =>
                    //        b.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    //            sql => sql.MigrationsAssembly(migrationsAssembly));
                    //})
                    //.AddOperationalStore(options =>
                    //{
                    //options.ConfigureDbContext = build =>
                    //build.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")
                    //, sql => sql.MigrationsAssembly(migrationsAssembly)
                    //);
                    //this enables automatic token cleanup. this is optional.
                    //options.EnableTokenCleanup = true;
                    //options.TokenCleanupInterval = 30; // interval in seconds
                    //})
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApis())
                    .AddInMemoryClients(Config.GetClients())

                //
                ;

            #endregion add is4

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer("Bearer", options =>
                {
                    if (Environment.IsDevelopment())
                        options.Authority = Configuration["LocalServer"];
                    //options.Authority = Configuration["ProdServer"];
                    else if (Environment.IsProduction())
                        options.Authority = Configuration["ProdServer"];

                    options.RequireHttpsMetadata = false;
                    options.Audience = "v1";
                });

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            //comment below region when ef migrations error

            //if(Environment.IsDevelopment())
            //    SeedData.EnsureSeedData(Configuration.GetConnectionString("LocalConnection"));
            //else
            //    SeedData.EnsureSeedData(Configuration.GetConnectionString("DefaultConnection"));
            services.AddTransient<IProfileService, IdentityClaimsProfileService>();
            //services.AddScoped<ICustomTokenRequestValidator, CustomTokenRequestValidator>();

            if (Environment.IsDevelopment())
            {
                //builder.AddDeveloperSigningCredential();
            }
            else
            {
                //throw new Exception("need to configure key material");
            }

            //services.AddAuthentication()
            //    .AddGoogle(options =>
            //    {
            //        // register your IdentityServer with Google at https://console.developers.google.com
            //        // enable the Google+ API
            //        // set the redirect URI to http://localhost:5000/signin-google
            //        options.ClientId = "copy client ID from Google here";
            //        options.ClientSecret = "copy client secret from Google here";
            //    });

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()));
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseCors(builder => { builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}