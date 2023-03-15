using ExclusiveCard.Managers;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Models.DTOs;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ExclusiveCard.Services
{
    public class SSOService : ISSOService
    {
        #region Private Members

        private readonly IConfiguration _configuration;
        private readonly ISSOManager _ssoManager;
        private readonly ILogger _logger;

        #endregion Private Members

        #region Constructor

        public SSOService(ISSOManager ssoManager, IConfiguration configuration)
        {
            _ssoManager = ssoManager;

            _configuration = configuration;
            _logger = LogManager.GetLogger("databaseLogger");
        }

        #endregion Constructor

        #region Public Methods

        ///<see cref="ISSOService.GetAllSSOConfigurations()"/>
        public async Task<List<SSOConfiguration>> GetAllSSOConfigurations()
        {
            return await _ssoManager.GetAllSSOConfigurations();
        }

        ///<see cref="ISSOService.GetSSOConfiguration(int)"/>
        public async Task<SSOConfiguration> GetSSOConfiguration(int ssoConfigId)
        {
            return await _ssoManager.GetSSOConfiguration(ssoConfigId);
        }


        public async Task<string> ProcessSSO(int ssoConfigId, Customer customer, string email, string productCode = null)
        {
            return await _ssoManager.ProcessSSO(ssoConfigId, customer, email, productCode);
        }

        public async Task<string> GetIDPMetadata(int ssoConfigId)
        {
            return await _ssoManager.GetIDPMetadata(ssoConfigId);
        }

        #endregion Public Methods

    }
}
