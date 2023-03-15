using ExclusiveCard.Services.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces
{
    public interface ISSOService
    {
        #region Reads

        /// <summary>
        /// Get SSO Configuration values
        /// </summary>
        /// <param name="ssoConfigId"></param>
        /// <returns>Confiig values</returns>
        Task<SSOConfiguration> GetSSOConfiguration(int ssoConfigId);

        /// <summary>
        /// Get all SSO 3rd party sites
        /// </summary>
        /// <returns>list of 3rd party sites</returns>
        Task<List<SSOConfiguration>> GetAllSSOConfigurations();

        #endregion Reads

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ssoConfigId">The Id of the Sso record related to the SAML service provider</param>
        /// <param name="customer">The customer wishing to access the service provider's offers</param>
        /// <param name="email">The customer's email address</param>
        /// <param name="productCode">The service provider's offer identifier</param>
        /// <returns></returns>
        Task<string> ProcessSSO(int ssoConfigId, Customer customer, string email, string productCode = null);

        Task<string> GetIDPMetadata(int ssoConfigId);
    }
}