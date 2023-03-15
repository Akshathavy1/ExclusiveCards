using System;
using AutoMapper;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Providers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ICategoryService = ExclusiveCard.Services.Interfaces.Admin.ICategoryService;
using ITagService = ExclusiveCard.Services.Interfaces.Admin.ITagService;

namespace ExclusiveCard.WebAdmin.App
{
    [Obsolete("Service helper class needs showing the door. No new code should be added here. Use DI/IOC as intended , not like this.")]
    public sealed class ServiceHelper
    {
        private static ServiceHelper _instance;

        public IAffiliateFieldMappingService AffiliateFieldMappingService =>
            _provider.GetService<IAffiliateFieldMappingService>();


        public IAffiliateFileService AffiliateFileService => _provider.GetService<IAffiliateFileService>();

        public IAffiliateMappingService AffiliateMappingService => _provider.GetService<IAffiliateMappingService>();

        public IMerchantSocialMediaLinkService MerchantSocialMediaLinkService =>
            _provider.GetService<IMerchantSocialMediaLinkService>();

        public IOfferImportAwinService OfferImportAwinService => _provider.GetService<IOfferImportAwinService>();
        public IOfferImportFileService OfferImportFileService => _provider.GetService<IOfferImportFileService>();
        public IOfferService OfferService => _provider.GetService<IOfferService>();

        public ISocialMediaCompanyService SocialMediaCompanyService =>
            _provider.GetService<ISocialMediaCompanyService>();


        //public IStagingOfferService StagingOfferService => _provider.GetService<IStagingOfferService>();


        public IStatusServices StatusService => _provider.GetService<IStatusServices>();
        public IMapper Mapper => _provider.GetService<IMapper>();
        public NLog.ILogger Logger => _provider.GetService<NLog.ILogger>();


        public IOptions<TypedAppSettings> Settings => _provider.GetService<IOptions<TypedAppSettings>>();
        private IServiceProvider _provider;

        public void Initialize(ServiceProvider serviceProvider)
        {
            _provider = serviceProvider;
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