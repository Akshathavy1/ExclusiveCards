using ExclusiveCard.Services.Models;
using ExclusiveCard.Services.Models.DTOs;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ExclusiveCard.Managers
{
    public interface ISSOManager
    {
        #region Reads

        /// <summary>
        /// Get all SSO 3rd party sites
        /// </summary>
        /// <returns>list of 3rd party sites</returns>
        Task<List<SSOConfiguration>> GetAllSSOConfigurations();

        /// <summary>
        /// Get SSO configuration details
        /// </summary>
        /// <param name="ssoConfigId"></param>
        /// <returns>configuration values</returns>
        Task<SSOConfiguration> GetSSOConfiguration(int ssoConfigId);

        #endregion Reads

        /// <summary>
        /// Process the SSO request
        /// </summary>
        /// <param name="issuer">the issuer URL</param>
        /// <param name="metadata">metadata string</param>
        /// <param name="acsUrl">Assertion consumer service url</param>
        /// <param name="idpCert">certificate</param>
        /// <param name="signResponse"></param>
        /// <param name="signAssertion"></param>
        /// <param name="base64Encode"></param>
        /// <param name="attributes">attributes required to be passed on</param>
        /// <returns></returns>
        //Task<string> ProcessSSO(string issuer, string metadata, string acsUrl, X509Certificate2 idpCert, bool signResponse, bool signAssertion, bool base64Encode, IDictionary<string, string> attributes = null);

        Task<string> ProcessSSO(int ssoConfigId, Customer customer, string email, string productCode = null);

        /// <summary>
        /// Search the keystore and key vault for a match
        /// </summary>
        /// <param name="nameOrThumbprint">for keyvault pass name, for keystore pass thumbprint</param>
        /// <param name="validOnly">ignore expired keys</param>
        /// <returns></returns>
        Task<X509Certificate2> GetCertificate(string nameOrThumbprint, bool validOnly = true);

        Task<string> GetIDPMetadata(int ssoConfigId);
    }
}