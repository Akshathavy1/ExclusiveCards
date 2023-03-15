using System;
using AutoMapper;
using Castle.Core.Logging;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ICategoryService = ExclusiveCard.Services.Interfaces.Admin.ICategoryService;
using IMerchantService = ExclusiveCard.Services.Interfaces.Admin.IMerchantService;
using ITagService = ExclusiveCard.Services.Interfaces.Admin.ITagService;

namespace ExclusiveCard.Website.App
{
    public class ServiceHelper
    {
        private static ServiceHelper _instance;


        public IMerchantBranchService MerchantBranchService => _provider.GetService<IMerchantBranchService>();
        public IMerchantService MerchantService => _provider.GetService<IMerchantService>();



        public IStatusServices StatusService => _provider.GetService<IStatusServices>();
        public ITagService TagService => _provider.GetService<ITagService>();

        public Services.Interfaces.Public.ICategoryService PCategoryService =>
            _provider.GetService<Services.Interfaces.Public.ICategoryService>();


        public Services.Interfaces.Public.ICustomerService CustomerService =>
            _provider.GetService<Services.Interfaces.Public.ICustomerService>();


        public Services.Interfaces.Public.IMembershipCardService MembershipCardService =>
            _provider.GetService<Services.Interfaces.Public.IMembershipCardService>();

        public IOffersService OffersService => _provider.GetService<IOffersService>();

       
        public IUserService UserService => _provider.GetService<IUserService>();


        public IMemoryCache Cache => _provider.GetService<IMemoryCache>();
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