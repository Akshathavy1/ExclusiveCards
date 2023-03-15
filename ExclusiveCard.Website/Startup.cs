using AutoMapper;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Providers.Email;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Public;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using ILogger = NLog.ILogger;

namespace ExclusiveCard.Website
{
    public class Startup
    {
        private const string enGBCulture = "en-GB";

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
                    ServiceLifetime.Scoped
                );

            
            //services.AddDefaultIdentity<ExclusiveUser>(options => options.Stores.MaxLengthForKeys = 128)
            //    .AddRoles<IdentityRole>()
            services.AddIdentity<ExclusiveUser, IdentityRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ExclusiveContext>()
                .AddDefaultTokenProviders();

            //https://stackoverflow.com/questions/57684093/using-usemvc-to-configure-mvc-is-not-supported-while-using-endpoint-routing
            services.AddMvc(options => options.EnableEndpointRouting = false);

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
                options.Lockout.MaxFailedAccessAttempts = 5;
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
                options.LoginPath = "/Account/SignIn";
                //TODO: Determine where Applicationcookie Options.AccessDenied path is actually used and  think about actually creating  the method it refers to
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo(enGBCulture),
                    new CultureInfo("en-IN")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: enGBCulture, uiCulture: enGBCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                var provider = new RouteDataRequestCultureProvider()
                {
                    RouteDataStringKey = "lang",
                    UIRouteDataStringKey = "lang",
                    Options = options
                };
                options.RequestCultureProviders = new[] { provider };
            });

            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("lang", typeof(LanguageRouteConstraint));
            });

            services.AddAutoMapper();

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            services.AddTransient<Filters.GlobalSetDefaultViewDataValuesAttribute>();

            services.AddMvc(options =>
                {
                    options.Filters.AddService<Filters.GlobalSetDefaultViewDataValuesAttribute>();
                })
                //.AddViewLocalization()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization()
                //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                ;

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(Convert.ToDouble(Configuration.GetSection("ExtendedSessionTimeout").Value));
            });

            RegisterAllTypes(services, typeof(Data.CRUDS.OfferManager).Assembly, typeof(Data.CRUDS.IOfferManager).Assembly);
            RegisterAllTypes(services, typeof(Managers.UserManager).Assembly, typeof(Managers.IUserManager).Assembly);
            RegisterAllTypes(services, typeof(Providers.AzureStorageProvider).Assembly, typeof(Providers.IAzureStorageProvider).Assembly);
            RegisterAllTypes(services, typeof(Services.Admin.PartnerTransactionService).Assembly, typeof(Services.Interfaces.Admin.IPartnerTransactionService).Assembly);
            RegisterAllTypes(services, typeof(LocalisationService).Assembly, typeof(ILocalisationService).Assembly);

            // Providers
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<ISFTPProvider, Providers.FTPProvider>();
            services.AddTransient<IEmailProvider, Providers.SendGridProvider>();

            services.Configure<TypedAppSettings>(Configuration);

            //Initialize services
            var serviceProvider = services.BuildServiceProvider();
            App.ServiceHelper.Instance.Initialize(serviceProvider);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ILogger log = LogManager.GetCurrentClassLogger();
            using (StreamReader iisUrlRewriteStreamReader =
                    File.OpenText("IISUrlRewrite.xml"))
            {
                var options = new RewriteOptions()
                    .AddIISUrlRewrite(iisUrlRewriteStreamReader)
                    .Add(MethodRules.RedirectXmlFileRequests)
                    .Add(MethodRules.RewriteTextFileRequests);

                app.UseRewriter(options);
            }

            app.UseExceptionHandler(options =>
            {
                options.Run(
                async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/html";
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace }";
                        await context.Response.WriteAsync(err).ConfigureAwait(false);
                    }
                });
            });
            app.UseHsts();
            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<ExclusiveContext>()
                        .Database.Migrate();
                }
            }
            catch { }

            var supportedCultures = new[]
            {
                new CultureInfo(enGBCulture),
                new CultureInfo("en-IN")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(enGBCulture),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });
            app.UseStaticFiles();
            // To configure external authentication,
            // see: http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseSession();

            //MvcOptions.EnableEndpointRouting = false
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "LocalizedDefault",
                    template: "{lang:lang}/{controller=Home}/{action=Index}/{id?}",
                    defaults: "{lang}/{controller}/{action}/{id}",
                    constraints: new { lang = @"(\w{2})|(\w{2}-\w{2})" }   // en or en-US
                                                                           //constraints: new { lang = @"(\w{2})|(\w{2}-\w{2})" },   // en or en-US
                                                                           //template: "{lang:lang}/{controller=Home}/{action=Index}/{id?}"
                    );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
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