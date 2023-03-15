using AutoMapper;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Services.Admin;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Public;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExclusiveCard.IntegrationTests
{
    public class Configuration
    {
        public static IServiceCollection Services { get; set; }
        public static ServiceProvider ServiceProvider { get; set; }

        static Configuration()
        {
            Services = new ServiceCollection();
        }

        public static async Task ConfigureServices()
        {
            await Task.CompletedTask;
            var dbDirName = Path.Combine(Directory.GetCurrentDirectory().Replace("bin\\Debug\\netcoreapp2.1", ""), "Data\\");
            AppDomain.CurrentDomain.SetData("DataDirectory", dbDirName);

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var optionsBuilder = new DbContextOptionsBuilder<ExclusiveContext>();
            var connectionString = configuration.GetConnectionString("exclusive");
            optionsBuilder.UseSqlServer(connectionString);

            Services.AddEntityFrameworkSqlServer()
                .AddDbContext<ExclusiveContext>(options =>
                    {
                        options
                        .UseSqlServer(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                    },
                    ServiceLifetime.Scoped // Note that Scoped is the default choice in AddDbContext. It is shown here only for pedagogic purposes.
                );
            Services.AddIdentity<ExclusiveUser, IdentityRole>(options => options.Stores.MaxLengthForKeys = 128)
            .AddEntityFrameworkStores<ExclusiveContext>()
            .AddDefaultTokenProviders();
            Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            Services.AddMemoryCache();
            Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            //ConfigureServicesIoc();
            Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            Services.AddSingleton<IConfiguration>(configuration);

            Services.AddAutoMapper();
            //Services.Configure<TypedAppSettings>(ConfigurationSetting);

            Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            Services.RegisterAllTypes(typeof(Data.CRUDS.OfferManager).Assembly,
                typeof(Data.CRUDS.IOfferManager).Assembly);
            Services.RegisterAllTypes(typeof(Managers.UserManager).Assembly,
                typeof(Managers.IUserManager).Assembly);
            Services.RegisterAllTypes(typeof(Providers.AzureStorageProvider).Assembly, typeof(Providers.IAzureStorageProvider).Assembly);
            //Services.RegisterAllTypes(typeof(SFTPProvider).Assembly, typeof(ISFTPProvider).Assembly);
            Services.RegisterAllTypes(typeof(Services.Admin.PartnerTransactionService).Assembly, typeof(Services.Interfaces.Admin.IPartnerTransactionService).Assembly);
            Services.RegisterAllTypes(typeof(Services.Admin.PartnerService).Assembly, typeof(Services.Interfaces.Admin.IPartnerService).Assembly); 
            Services.RegisterAllTypes(typeof(LocalisationService).Assembly, typeof(ILocalisationService).Assembly);

            Services.AddTransient<ISFTPProvider, Providers.FTPProvider>();
            Services.AddTransient<Managers.IMembershipManager, Managers.MembershipManager>();
            

            ServiceProvider = Services.BuildServiceProvider();
            ServiceHelper.Instance.Initialize(ServiceProvider);
            
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static void RegisterAllTypes(this IServiceCollection services, Assembly implementationAssembly, Assembly interfacesAssembly
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
