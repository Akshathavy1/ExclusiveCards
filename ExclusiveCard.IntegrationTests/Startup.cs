using ExclusiveCard.Data.Context;
using ExclusiveCard.Services;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Public;
using ExclusiveCard.Website.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExclusiveCard.IntegrationTests
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
                    options.UseLazyLoadingProxies().UseInMemoryDatabase(databaseName: "Add_writes_to_database");

                },
                    ServiceLifetime.Scoped // Note that Scoped is the default choice
                                           // in AddDbContext. It is shown here only for
                                           // pedagogic purposes.
                );

            services.AddIdentity<IdentityUser, IdentityRole>(options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<ExclusiveContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

        }

        private static void ConfigureServicesIoc(IServiceCollection services)
        {
            services.AddTransient<LocalisedViewModel>();
            services.AddTransient<IAzureStorageProvider, AzureStorageProvider>();
            services.AddTransient<ISecurityService, SecurityService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ILocalisationService, LocalisationService>();
            services.AddTransient<IOffersService, OffersService>();
            services.AddTransient<IMerchantService, MerchantService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IUserAccountService, UserAccountService>();
            services.AddTransient<IMembershipService, MembershipService>();
            services.AddTransient<ICustomerSecurityQuestionService, CustomerSecurityQuestionService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<ICashbackSummaryService, CashbackSummaryService>();
            services.AddTransient<ICashbackTransactionService, CashbackTransactionService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IMembershipCardService, MembershipCardService>();
            services.AddTransient<IClickTrackingService, ClickTrackingService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IPaymentProviderService, PaymentProviderService>();
            services.AddTransient<IPaymentNotificationService, PaymentNotificationService>();
            services.AddTransient<ICustomerPaymentService, CustomerPaymentService>();
            services.AddTransient<IAdvertService, AdvertService>();
            services.AddTransient<IPayPalSubscriptionService, PayPalSubscriptionService>();

            services.AddTransient<Services.Interfaces.Admin.IOfferTypeService, Services.Admin.OfferTypeService>();
            services.AddTransient<Services.Interfaces.Admin.IMembershipRegistrationCodeService, Services.Admin.MembershipRegistrationCodeService>();
            services.AddTransient<Services.Interfaces.Admin.ISecurityQuestionService, Services.Admin.SecurityQuestionService>();
            services.AddTransient<Services.Interfaces.Admin.IContactDetailService, Services.Admin.ContactDetailService>();
            services.AddTransient<Services.Interfaces.Admin.IMembershipPlanService, Services.Admin.MembershipPlanService>();
            services.AddTransient<Services.Interfaces.Admin.IMerchantImageService, Services.Admin.MerchantImageService>();
            services.AddTransient<Services.Interfaces.Admin.IAffiliateMappingService, Services.Admin.AffiliateMappingService>();
            services.AddTransient<Services.Interfaces.Admin.IAffiliateMappingRuleService, Services.Admin.AffiliateMappingRuleService>();
            services.AddTransient<Services.Interfaces.Admin.IMembershipCardAffiliateReferenceService, Services.Admin.MembershipCardAffiliateReferenceService>();
            services.AddTransient<Services.Interfaces.Admin.IStatusServices, Services.Admin.StatusService>();
            services.AddTransient<Services.Interfaces.Admin.ICategoryService, Services.Admin.CategoryService>();
        }
    }
}
