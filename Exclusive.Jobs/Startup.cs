using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Managers;
using ExclusiveCard.Providers;
using ExclusiveCard.Providers.Email;
using ExclusiveCard.Providers.Marketing;
using ExclusiveCard.Services.Admin;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace Exclusive.Jobs
{
    public class Startup
    {
        private const string enGBCulture = "en-GB";
        public IConfiguration Configuration { get; }

        public Startup()
        {
            string appsettingPath = @"D:\home\site\wwwroot\appsettings.json";
            try
            {
                //The build directory for this is set to ..\ExclusiveCard.Web\App_Data\Exclusive.jobs\
                //You WILL need to build and zip the netcoreapp3.1 directory that gets created, then drop it into Azure
                //The corresponding server directory for this is D:\home\site\wwwroot\App_Data\Exclusive.jobs\
                //Make sure you do this build in RELEASE mode!!!

                //For UAT & LIVE this app should pick up the web admin site's appsettings file from D:\home\site\wwwroot\
                //if you're debugging this locally, you can use this path 
#if DEBUG
                //GetCurrentDirectory() returns D:\local\Temp\jobs\ on UAT & LIVE so don't use this in production!!!
                appsettingPath = @$"{System.IO.Directory.GetCurrentDirectory()}\..\..\..\appsettings.json";
#endif

                var builder = new ConfigurationBuilder()
                    .AddJsonFile(appsettingPath);

                Configuration = builder.Build();
            }
            catch(System.Exception ex)
            {
                var log = LogManager.GetCurrentClassLogger();
                log.Error(ex);
            }
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("exclusive");
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<ExclusiveContext>(options =>
                {
                    options
                        .UseSqlServer(connectionString,
                            sqlOptions =>
                                sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName()
                                    .Name)).UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                },
                    ServiceLifetime.Scoped
                );


            services.AddAutoMapper();

            RegisterAllTypes(services, typeof(MarketingService).Assembly, typeof(IMarketingService).Assembly);
            RegisterAllTypes(services, typeof(MarketingManager).Assembly, typeof(IMarketingManager).Assembly);
            RegisterAllTypes(services, typeof(SendGridProvider).Assembly, typeof(IMarketingProvider).Assembly);

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<IConfiguration>(Configuration);
            //Initialize services
            var serviceProvider = services.BuildServiceProvider();

            //
            RegisterAllTypes(services, typeof(ExclusiveCard.Data.CRUDS.OfferManager).Assembly, typeof(ExclusiveCard.Data.CRUDS.IOfferManager).Assembly);
            RegisterAllTypes(services, typeof(ExclusiveCard.Managers.UserManager).Assembly, typeof(ExclusiveCard.Managers.IUserManager).Assembly);
            RegisterAllTypes(services, typeof(ExclusiveCard.Providers.AzureStorageProvider).Assembly, typeof(ExclusiveCard.Providers.IAzureStorageProvider).Assembly);
            RegisterAllTypes(services, typeof(ExclusiveCard.Services.Admin.PartnerTransactionService).Assembly, typeof(ExclusiveCard.Services.Interfaces.Admin.IPartnerTransactionService).Assembly);
            RegisterAllTypes(services, typeof(ExclusiveCard.Services.Public.LocalisationService).Assembly, typeof(ILocalisationService).Assembly);

            // Providers
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<ISFTPProvider, ExclusiveCard.Providers.FTPProvider>();
            services.AddTransient<IMarketingProvider, ExclusiveCard.Providers.SendGridProvider>();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            ILogger log = LogManager.GetCurrentClassLogger();

            //try
            //{
            //    using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
            //        .CreateScope();
            //    //serviceScope.ServiceProvider.GetService<ExclusiveContext>()
            //    //    .Database.Migrate();
            //}
            //catch (Exception e)
            //{
            //    log.Error(e);
            //}


        }

        private void RegisterAllTypes(IServiceCollection services, Assembly implementationAssembly, Assembly interfacesAssembly
            , ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var interfaceTypes = interfacesAssembly.GetTypes().Where(t => t.IsInterface);
            foreach (Type iType in interfaceTypes)
            {
                var implementation = implementationAssembly.GetTypes().FirstOrDefault(x => iType.IsAssignableFrom(x) & !x.IsInterface);
                if (implementation != null)
                { services.AddTransient(iType, implementation); }
            }

        }
    }
}
