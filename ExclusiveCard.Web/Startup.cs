using AutoMapper;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Providers.Email;
using ExclusiveCard.Services;
using ExclusiveCard.Services.Admin;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Public;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net;
using System.Reflection;

namespace ExclusiveCard.WebAdmin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
                    ServiceLifetime.Scoped // Note that Scoped is the default choice
                    // in AddDbContext. It is shown here only for
                    // pedagogic purposes.
                );

            //services.AddDefaultIdentity<ExclusiveUser>(options => options.Stores.MaxLengthForKeys = 128)
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<ExclusiveContext>()
            //    .AddDefaultTokenProviders();

            //services.AddIdentity <ExclusiveUser, IdentityRole>()
            services.AddIdentity<ExclusiveUser, IdentityRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ExclusiveContext>()
                .AddDefaultTokenProviders();

            //https://stackoverflow.com/questions/57684093/using-usemvc-to-configure-mvc-is-not-supported-while-using-endpoint-routing
            services.AddMvc(options => options.EnableEndpointRouting = false);

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+$!#%^&*?";
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.Name = "4abd372380d74c999cc6ccec9f50ffb8";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

                options.LoginPath = "/Account/";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.AddMemoryCache();

            services.AddAutoMapper();

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(Convert.ToDouble(Configuration.GetSection("ExtendedSessionTimeout").Value));
            });

            //Register Dependencies

            RegisterAllTypes(services, typeof(Data.CRUDS.OfferManager).Assembly, typeof(Data.CRUDS.IOfferManager).Assembly);
            RegisterAllTypes(services, typeof(SFTPProvider).Assembly, typeof(ISFTPProvider).Assembly);
            //RegisterAllTypes(services, typeof(Services.Public.PartnerRewardService).Assembly, typeof(Services.Interfaces.Public.IPartnerRewardService).Assembly);
            RegisterAllTypes(services, typeof(LocalisationService).Assembly, typeof(ILocalisationService).Assembly);
            RegisterAllTypes(services, typeof(PartnerTransactionService).Assembly, typeof(IPartnerTransactionService).Assembly);
            RegisterAllTypes(services, typeof(Managers.OfferRedemptionManager).Assembly, typeof(Managers.IOfferRedemptionManager).Assembly);
            RegisterAllTypes(services, typeof(Providers.AzureStorageProvider).Assembly, typeof(Providers.IAzureStorageProvider).Assembly);

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<ISFTPProvider, Providers.FTPProvider>();
            services.AddTransient<IEmailProvider, Providers.SendGridProvider>();

            services.AddSession();
            services.Configure<TypedAppSettings>(Configuration);

            var serviceProvider = services.BuildServiceProvider();
            App.ServiceHelper.Instance.Initialize(serviceProvider);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                NLog.ILogger log = NLog.LogManager.GetCurrentClassLogger();
                //app.UseExceptionHandler("/Error/500");
                app.UseExceptionHandler(
                    options =>
                    {
                        options.Run(
                            async context =>
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                context.Response.ContentType = "text/html";
                                var ex = context.Features.Get<IExceptionHandlerFeature>();
                                if (ex != null)
                                {
                                    log.Error(ex);
                                    var err = $"<h1>Exclusive Card Error Page</h1>An error occurred while processing your request.";
                                    await context.Response.WriteAsync(err).ConfigureAwait(false);
                                }
                            });
                    }
                );
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseHsts();
            }
            
            //DependencyConfig.Configure();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Index}/{id?}");
            });
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
