using AutoMapper;
using Castle.Core.Logging;
using ExclusiveCard.Providers;
using ExclusiveCard.Providers.Email;
using ExclusiveCard.Providers.Marketing;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ICategoryService = ExclusiveCard.Services.Interfaces.Admin.ICategoryService;
using ITagService = ExclusiveCard.Services.Interfaces.Admin.ITagService;

namespace ExclusiveCard.IntegrationTests
{
    public sealed class ServiceHelper
    {
        private static ServiceHelper _instance;

        public IAffiliateFieldMappingService AffiliateFieldMappingService;
        public IAffiliateFileMappingService AffiliateFileMappingService;
        public IAffiliateFileService AffiliateFileService;
        public IAffiliateMappingRuleService AffiliateMappingRuleService;
        public IAffiliateMappingService AffiliateMappingService;
        public IAffiliateService AffiliateService;
        public IContactDetailService ContactDetailService;
        public IMembershipCardAffiliateReferenceService MembershipCardAffiliateReferenceService;
        public IOLD_MembershipPlanService MembershipPlanService;
        public IOLD_MembershipRegistrationCodeService MembershipRegistrationCodeService;
        public IMerchantBranchService MerchantBranchService;
        public IMerchantImageService MerchantImageService;
        public Services.Interfaces.Admin.IMerchantService MerchantService;
        public IMerchantSocialMediaLinkService MerchantSocialMediaLinkService;
        public IOfferCategoryService OfferCategoryService;
        public IOfferCountryService OfferCountryService;
        public IOfferImportAwinService OfferImportAwinService;
        public IOfferImportFileService OfferImportFileService;
        public IOfferService OfferService;
        public IOfferTagService OfferTagService;
        public IOfferTypeService OfferTypeService;
        public Services.Interfaces.Admin.IPartnerService PartnerService;
        public Services.Interfaces.Public.IPartnerService NewPartnerService;
        public ISecurityQuestionService SecurityQuestionService;
        public ISocialMediaCompanyService SocialMediaCompanyService;
        public IMembershipService MembershipService;

        //public IStagingOfferCategoryService StagingOfferCategoryService;
        //public IStagingOfferCountryService StagingOfferCountryService;
        //public IStagingOfferService StagingOfferService;

        public IStatusServices StatusService;
        public ITagService TagService;
        public ICashbackService CashbackService;

        public IBankDetailService BankDetailService;
        public ICashbackPayoutService CashbackPayoutService;
        public ICategoryService CategoryService;
        public IClickTrackingService ClickTrackingService;
        public ICustomerBankDetailService CustomerBankDetailService;
        public ICustomerPaymentService CustomerPaymentService;
        public IPaymentNotificationService PaymentNotificationService;
        public Services.Interfaces.Public.IMembershipCardService PMembershipCardService;
        public Services.Interfaces.Admin.IMembershipCardService MembershipCardService;
        public IPaymentProviderService PaymentProviderService;
        public IPayPalSubscriptionService PayPalSubscriptionService;
        public Services.Interfaces.Public.ICustomerService CustomerService;
        public Services.Interfaces.Admin.ICustomerService AdminCustomerService;
        public IOffersService OffersService;

        public Managers.IEmailManager EmailManager;
        public IUserService UserService;

        public IStagingCustomerRegistrationService StagingCustomerRegistrationService { get; set; }

        public IPartnerTransactionService PartnerTransactionService { get; set; }

        public IPartnerRewardService PartnerRewardService { get; set; }

        public IPartnerRewardWithdrawalService PartnerRewardWithdrawalService { get; set; }

        public ICustomerAccountService NewUserAccountService { get; set; }

        public ISFTPProvider SFTPProvider { get; set; }

        public IAzureStorageProvider AzureStorageProvider { get; set; }

        public IOptions<TypedAppSettings> Settings { get; set; }

        public IWhiteLabelService WhiteLabelService { get; set; }

        public IMemoryCache Cache;
        public IMapper Mapper;
        public ILogger Logger;
        public IMarketingProvider SendGrid;

        public void Initialize(ServiceProvider provider)
        {
            AffiliateFieldMappingService = provider.GetService<IAffiliateFieldMappingService>();
            AffiliateFileMappingService = provider.GetService<IAffiliateFileMappingService>();
            AffiliateFileService = provider.GetService<IAffiliateFileService>();
            AffiliateMappingRuleService = provider.GetService<IAffiliateMappingRuleService>();
            AffiliateMappingService = provider.GetService<IAffiliateMappingService>();
            AffiliateService = provider.GetService<IAffiliateService>();
            ContactDetailService = provider.GetService<IContactDetailService>();
            MembershipCardAffiliateReferenceService = provider.GetService<IMembershipCardAffiliateReferenceService>();
            MembershipPlanService = provider.GetService<IOLD_MembershipPlanService>();
            MembershipRegistrationCodeService = provider.GetService<IOLD_MembershipRegistrationCodeService>();
            MerchantBranchService = provider.GetService<IMerchantBranchService>();
            MerchantImageService = provider.GetService<IMerchantImageService>();
            MerchantService = provider.GetService<Services.Interfaces.Admin.IMerchantService>();
            MerchantSocialMediaLinkService = provider.GetService<IMerchantSocialMediaLinkService>();
            OfferCategoryService = provider.GetService<IOfferCategoryService>();
            OfferCountryService = provider.GetService<IOfferCountryService>();
            OfferImportAwinService = provider.GetService<IOfferImportAwinService>();
            OfferImportFileService = provider.GetService<IOfferImportFileService>();
            OfferService = provider.GetService<IOfferService>();
            OfferTagService = provider.GetService<IOfferTagService>();
            OfferTypeService = provider.GetService<IOfferTypeService>();
            PartnerService = provider.GetService<Services.Interfaces.Admin.IPartnerService>();
            NewPartnerService = provider.GetService<Services.Interfaces.Public.IPartnerService>();
            SecurityQuestionService = provider.GetService<ISecurityQuestionService>();
            SocialMediaCompanyService = provider.GetService<ISocialMediaCompanyService>();
            MembershipService = provider.GetService<IMembershipService>();

            //StagingOfferCategoryService = provider.GetService<IStagingOfferCategoryService>();
            //StagingOfferCountryService = provider.GetService<IStagingOfferCountryService>();
            //StagingOfferService = provider.GetService<IStagingOfferService>();

            StatusService = provider.GetService<IStatusServices>();
            TagService = provider.GetService<ITagService>();
            CashbackService = provider.GetService<ICashbackService>();

            BankDetailService = provider.GetService<IBankDetailService>();
            CashbackPayoutService = provider.GetService<ICashbackPayoutService>();
            CategoryService = provider.GetService<ICategoryService>();
            ClickTrackingService = provider.GetService<IClickTrackingService>();
            CustomerBankDetailService = provider.GetService<ICustomerBankDetailService>();
            PaymentNotificationService = provider.GetService<IPaymentNotificationService>();
            PMembershipCardService = provider.GetService<Services.Interfaces.Public.IMembershipCardService>();
            MembershipCardService = provider.GetService<Services.Interfaces.Admin.IMembershipCardService>();
            PaymentProviderService = provider.GetService<IPaymentProviderService>();
            PayPalSubscriptionService = provider.GetService<IPayPalSubscriptionService>();
            CustomerService = provider.GetService<Services.Interfaces.Public.ICustomerService>();
            AdminCustomerService = provider.GetService<Services.Interfaces.Admin.ICustomerService>();

            EmailManager = provider.GetService<Managers.IEmailManager>();
            UserService = provider.GetService<IUserService>();
            StagingCustomerRegistrationService = provider.GetService<IStagingCustomerRegistrationService>();
            PartnerTransactionService = provider.GetService<IPartnerTransactionService>();
            PartnerRewardService = provider.GetService<IPartnerRewardService>();
            PartnerRewardWithdrawalService = provider.GetService<IPartnerRewardWithdrawalService>();
            OffersService = provider.GetService<IOffersService>();
            NewUserAccountService = provider.GetService<ICustomerAccountService>();

            SFTPProvider = provider.GetService<ISFTPProvider>();
            AzureStorageProvider = provider.GetService<IAzureStorageProvider>();
            Settings = provider.GetService<IOptions<TypedAppSettings>>();
            WhiteLabelService = provider.GetService<IWhiteLabelService>();

            Cache = provider.GetService<IMemoryCache>();
            Mapper = provider.GetService<IMapper>();
            Logger = provider.GetService<ILogger>();
            SendGrid = provider.GetService<IMarketingProvider>();
        }

        public static ServiceHelper Instance
        {
            get
            {
                if (_instance != null) return _instance;
                else if (_instance == null) _instance = new ServiceHelper();

                return _instance;
            }
        }
    }
}