using DTOs = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;
using System;
using System.Collections.Generic;
using System.Text;
using ExclusiveCard.Managers;
using System.Threading.Tasks;
using NLog;

namespace ExclusiveCard.Services.Public
{
    public class PartnerService : IPartnerService
    {
        #region Private Fields and Constructor

        private readonly IUserManager _userManager;
        private readonly IMembershipManager _membershipManager;
        private readonly ICashbackManager _cashbackManager;
        private readonly IRewardManager _rewardManager;
        private readonly IPartnerManager _PartnerManager;
        public readonly ILogger Logger;

        public PartnerService(IUserManager userManager, IMembershipManager membershipManager, ICashbackManager cashbackManager, IRewardManager rewardManager, IPartnerManager partnerManager)
        {
            _userManager = userManager;
            _membershipManager = membershipManager;
            _cashbackManager = cashbackManager;
            _rewardManager = rewardManager;
            _PartnerManager = partnerManager;
            Logger = LogManager.GetLogger(GetType().FullName);
        }

        #endregion


        #region public methods

        /// <summary>
        /// Logs a partner into the API.
        /// If successful a login token is returned.
        /// On successful login, after the token is generated, the Partner is logged out of Microsoft Identity immediately
        /// This is required in order for single sign on to be implemented for customers
        /// The token remains valid even though partner is logged out
        /// </summary>
        /// <param name="userName">The partner's username</param>
        /// <param name="password">The partners's password</param>
        /// <param name="audience">The hostname from the web request. Used to ensure token only valid for requess from same host </param>
        /// <returns>A JWT token that will authenticate the partner for further Partner API calls</returns>
        public async Task<string> LoginAsync(string userName, string password, string audience)
        {
            var result = await _userManager.PartnerLoginAsync(userName, password, audience);
            return result;
        }

        /// <summary>
        /// Signs in a customer to the Exculsive Rewards website 
        /// Provided to enable single sign on between partner and Excusive Rewards
        /// Requires that the partner is logged in to the API before the method is called 
        /// and is in poccession of a valid token.
        /// If login of customer succeeds, a standard ASP.NET Core authentication cookie is added to the web repsonse. 
        /// If the cookie is returned to customer browser, they will be signed in on the Exculsive Rewards site. 
        /// </summary>
        /// <param name="customerName">Username of customer to login (not the customer's forename or surname fields!)</param>
        /// <param name="token">The login token that proves Partner is logged into API</param>
        /// <returns>A null string on success, an error message otherwise</returns>
        public async Task<string> CustomerSignInAsync(string customerName, string token)
        {
            var validated = await ValidateCustomerAndPartnerAsync(customerName, token);
            string errorMsg = validated.Item1;

            // If no errors, go ahead and sign in the customer
            if (errorMsg == null)
            {
                await _userManager.PartnerCustomerSignInAsync(customerName);
            }

            if (errorMsg != null)
                errorMsg = "Cannot sign in customer - " + errorMsg;

            return errorMsg;
        }


        public bool ValidateLoginToken(string token, string audience)
        {
            bool result = false;
            result = _userManager.ValidatePartnerToken(token, audience);

            return result;
        }

        public async Task LogoutAsync()
        {
            await _userManager.LogoutAsync();
        }

        /// <summary>
        /// Validate that the userName (customerName) is valid and the customer has an active membership card
        /// Also validate token and ensure that the membership card was issued by same partner as who is logged in 
        /// to the Partner API via this token or that the token was issued to the mobile App partner.
        /// </summary>
        /// <param name="customerName">Username of the customer to validate</param>
        /// <param name="token">The login token passed in the Bearer header of the web request</param>
        /// <returns></returns>
        public async Task<Tuple<string, DTOs.MembershipCard>> ValidateCustomerAndPartnerAsync(string customerName, string token)
        {
            string errorMsg = null;
            DTOs.MembershipCard card = null;

            // Get the ASPNETUser record from the customerUserName
            var user = await _userManager.GetUserAsync(customerName);
            if (user == null)
                errorMsg = "username not found";

            // Get the active membership card from this user
            if (errorMsg == null)
            {
                card = _membershipManager.GetActiveMembershipCard(user.Id);
                if (card == null)
                    errorMsg = "active membership not found";
            }
            // Get user name from token
            var tokenUserName = _userManager.GetClaim(token, "NameId");
            //Get the user who issued the token
            var tokenUser = await _userManager.GetUserAsync(tokenUserName);

            // Check the card provider Id matches the partner's Id
            if (errorMsg == null)
            {
                //Get the partner of the token user
                var tokenPartner = _PartnerManager.GetProvider(tokenUser?.Id);
                Logger.Debug("Claim nameId = " + tokenUserName);
                var partner = _membershipManager.GetCardProvider(card.Id);
                Logger.Debug("Card Id = " + card.Id.ToString());
                Logger.Debug("Partner Name = " + partner?.Name);
                if (string.Compare(tokenUserName, partner?.Name, ignoreCase: true) != 0 && tokenPartner?.Name != Data.Constants.Keys.MobileAppPartner)
                    errorMsg = "cannot match partner";
            }

            return new Tuple<string, DTOs.MembershipCard>(errorMsg, card);
        }

        #endregion

        #region private methods



        #endregion
    }
}
