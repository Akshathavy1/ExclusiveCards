using ExclusiveCard.Services.Interfaces.Public;
using dto = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Managers;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ExclusiveUser = ExclusiveCard.Data.Models.ExclusiveUser;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.CRUDS;
using NLog;
using ICustomerManager = ExclusiveCard.Managers.ICustomerManager;
using IBankDetailManager = ExclusiveCard.Managers.IBankDetailManager;

namespace ExclusiveCard.Services.Public
{

    /// <summary>
    /// The CustomerAccountService is used by Customers on the website. 
    /// Customer is defined as a user who may or may not have purchased a membership card, who is viewing offers 
    /// on the website or via an app.
    /// This service is responsible for login/logout/password resets, validating registration codes and 
    /// creating new customer accounts
    /// </summary>
    public class CustomerAccountService : ICustomerAccountService
    {
        #region Private fields and constructor

        private readonly IUserManager _userManager;
        private readonly IMembershipManager _membershipManager;
        private readonly ICustomerManager _customerManager;
        private readonly IRewardManager _rewardManager;
        private readonly ICashbackManager _cashbackManager;
        private readonly IEmailManager _emailManager;
        private readonly ICashbackPayoutManager _payoutManager;
        private readonly IBankDetailManager _bankDetailManager;
        private readonly ILogger _logger;


        public CustomerAccountService(IUserManager userManager, IMembershipManager membershipManager, ICustomerManager customerManager,
                                      IRewardManager rewardManager, ICashbackManager cashbackManager, IEmailManager emailManager, 
                                      ICashbackPayoutManager payoutManager, IBankDetailManager bankDetailManager)
        {
            _userManager = userManager;
            _membershipManager = membershipManager;
            _customerManager = customerManager;
            _rewardManager = rewardManager;
            _cashbackManager = cashbackManager;
            _emailManager = emailManager;
            _payoutManager = payoutManager;
            _bankDetailManager = bankDetailManager;

            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region public methods

        #region Registraiton 

        /// <summary>
        /// Creates a customer account, from a pending membersip Token.
        /// Tokens are created as the first step in  Sign In process on the website
        /// </summary>
        /// <param name="customerAccount"></param>
        /// <param name="pendingMembershipToken"></param>
        /// <returns></returns>
        public dto.UserToken CreateAccountFromPendingToken(dto.CustomerAccountDto customerAccount, string confirmUrl, bool login, dto.MembershipPlan plan = null)
        {
            int? registrationCodeId;
            // Step 1 - Find the Membership plan from the pending membership token provided
            if (plan == null)
            {
                //TODO:  Write some validation of CustomerAccount

                plan = _membershipManager.GetMembershipPlanFromPendingToken(customerAccount.PendingMembershipToken.Token, out registrationCodeId);
                if (plan == null)
                    throw new Exception("Could not create account - pending token was invalid");
            }
            else
                registrationCodeId = customerAccount.PendingMembershipToken?.MembershipRegistrationCodeId;


            // Step 2 - Check if user Login already exists for this email address
            var exists = _userManager.CheckExistsAsync(customerAccount.Username).Result;
            if (exists)
                throw new Exception("Could not create account - username already exists");

            // Step 3 - create a user Login
            var user = new dto.ExclusiveUser()
            {
                Email = customerAccount.Customer?.ContactDetail?.EmailAddress,
                UserName = customerAccount.Username
            };

            var aspNetUserId = _userManager.CreateAsync(user, customerAccount.Password).Result;
            customerAccount.Customer.AspNetUserId = aspNetUserId ?? throw new Exception("Could not create account - create user failed");

            // Step 4  - Create a new customer record
            var customer = _customerManager.Create(customerAccount.Customer);

            // Step 5 - Create PartnerReward , if applicable
            int? rewardId = null;
            if (plan.MembershipPlanTypeId == (int)Enums.MembershipPlanTypeEnum.PartnerReward)
            {
                rewardId = _rewardManager.CreatePartnerReward((int)plan.CardProviderId, customer);
            }

            // Step 6 - Create the Membership Card(s)
            var cards = _membershipManager.CreateMembershipCards(plan, customer.Id, (int)registrationCodeId, customerAccount.TermsConditionsId,
                                                                 rewardId, customerAccount.CountryCode);
            if (cards == null || cards.Count < 1)
                throw new Exception("No membership cards have been created for customer id " + customer.Id.ToString());

            if (login)
            {
                var loginresult = _userManager.CustomerLoginAsync(customerAccount.Username, customerAccount.Password).Result;
            }

            // Step 7  -  Send out the confirmation email
            // Big thanks to the GSS test team for finding that this step was missing before the users did
            var emailToken = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;

            if (plan.CardProvider == null || !plan.CardProvider.Name.Contains("ConsumerRights")) //Only send email if not 41 = ConsumerRightsLive 
            {
                _emailManager.SendEmailAsync(user.Email, (int)EmailTemplateType.AccountConfirmationEmail,
                                         new
                                         {
                                             Name = $"{customer.Forename} {customer.Surname}",
                                             url = $"{confirmUrl}?userData={customer.AspNetUserId}&eToken={emailToken}"
                                         });
            }
            else
            {
                //Mark customer email address as confirmed for consumer rights
                var result = _userManager.ConfirmEmailTokenAsync(user, emailToken).Result;
                if (!result.Succeeded)
                {
                    //Log failure?
                    _logger.Error($"Unable to confirm email, consumer rights customer {customer.Id}");
                }
            }
            // Keep this the same as the controller was expecting for now
            var userToken = new dto.UserToken()
            {
                Id = customer.Id,
                Name = $"{customer.Forename} {customer.Surname}",
                //Token = ,
                Role = "User"
            };

            return userToken;

        }


        /// <summary>
        /// Creates a customer account from a registration code
        /// The registation code is provided when an account is created by an API call rather than the web
        /// Other than validation of the code, the rest of the process is the same as via pending membership token
        /// </summary>
        /// <param name="customerAccount"></param>
        /// <param name="registrationCode"></param>
        /// <returns></returns>
        public dto.UserToken CreateAccountFromRegistrationCode(dto.CustomerAccountDto customerAccount, string registrationCode, string confirmUrl)
        {
            dto.UserToken userToken = null;

            // Step 1 - Validate the registration code
            var planAndToken = ValidateCode(customerAccount, registrationCode);
            MembershipPlan plan = planAndToken.Item1;

            // Step 2 - Call the Create Account from Pending Token to continue. Should all be the same from here.
            userToken = CreateAccountFromPendingToken(customerAccount, confirmUrl, false, plan);

            return userToken;
        }


        #endregion

        #region Get Data

        /// <summary>
        /// Returns the currently logged in user
        /// </summary>
        /// <param name="principal">The security prinicipal for the current user (part of Microsoft Identity)</param>
        /// <returns>
        /// If a logged in user is found, returns a populated dto.ExculsiveUser object.
        /// If not logged in, returns null
        /// </returns>
        public async Task<dto.ExclusiveUser> GetUserAsync(System.Security.Claims.ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }

        public async Task<dto.ExclusiveUser> GetUserAsync(string userName)
        {
            var user = await _userManager.GetUserAsync(userName);
            return user;
        }

        /// <summary>
        /// Get the cashback and partner reward balances for the specified customer
        /// </summary>
        /// <param name="customerId">Id of customer</param>
        /// <param name="partnerRewardId">Id of the partner reward record (as on each membership card)</param>
        /// <returns>An object containing the cashback balances and partner rewards balances and last updated date</returns>
        public dto.CustomerBalances GetBalances(int customerId, int partnerRewardId)
        {
            // Get Cashback balances from cashback manager
            var balances = _cashbackManager.GetCustomerBalances(customerId);

            if (partnerRewardId > 0)
            {
                // Get reward figures from reward manager
                balances = _rewardManager.GetRewardSummary(partnerRewardId, balances);
            }
            else
            {
                balances.Withdrawn = _payoutManager.GetWithdrawnAmount(customerId);
                balances.CurrentValue = decimal.Subtract(balances.ReceivedAmount, balances.Withdrawn);
                //balances.CurrentValue = balances.ReceivedAmount;
                // balances.ReceivedAmount = decimal.Subtract(balances.ReceivedAmount, balances.Withdrawn);
            }

            return balances;
        }

        public dto.CustomerAccountSummary GetAccountSummary(int customerId)
        {
            var activeCard = _membershipManager.GetActiveMembershipCard(customerId);
            var customer = _customerManager.Get(customerId);

            return GetAccountSummaryDetails(activeCard, customer);
        }


        public dto.CustomerAccountSummary GetAccountSummary(string userId)
        {
            var activeCard = _membershipManager.GetActiveMembershipCard(userId);
            var customer = _customerManager.Get(userId);

            return GetAccountSummaryDetails(activeCard, customer);
        }

        private CustomerAccountSummary GetAccountSummaryDetails(MembershipCard activeCard, Customer customer)
        {
            dto.CustomerAccountSummary summary = new dto.CustomerAccountSummary();
            if (activeCard != null)
            {
                summary.CardExpiryDate = activeCard.ValidTo;
                summary.CardNumber = activeCard.CardNumber;
                summary.CardProviderId = activeCard.MembershipPlan?.CardProviderId ?? 0;
                summary.RewardPartnerId = activeCard.MembershipPlan?.PartnerId ?? 0;
                summary.CustomerId = customer.Id;
                summary.CustomerName = customer?.FullName;
                summary.NiNumber = customer.NINumber;
                summary.EmailConfirmed = customer.IdentityUser.EmailConfirmed;
                summary.SiteClanId = customer.SiteClanId;
                summary.MembershipCardId = activeCard.Id;
                summary.CardStatus = ((Enums.MembershipCardStatus)activeCard.StatusId).ToString();


                if (activeCard.MembershipPlan?.MembershipLevelId == (int)Enums.MembershipLevel.Diamond)
                    summary.IsDiamondCustomer = true;

                var planType = (Enums.MembershipPlanTypeEnum)activeCard.MembershipPlan.MembershipPlanTypeId;
                summary.PlanType = planType.ToString();
                summary.PlanName = activeCard.MembershipPlan.Description;
                summary.MembershipPlanId = activeCard.MembershipPlanId;

                var partnerRewards = _rewardManager.GetPartnerRewards(activeCard.PartnerRewardId ?? 0);
                if (partnerRewards != null)
                {
                    summary.RewardKey = partnerRewards.RewardKey;
                    summary.RewardPassword = partnerRewards.Password;
                }

                summary.Balances = GetBalances(customer.Id, activeCard.PartnerRewardId ?? 0);
                //get Benefactor deposits
                if (planType == Enums.MembershipPlanTypeEnum.BenefitRewards)
                {
                    CustomerBalances benefitBalance = GetBenefactorDeposits(customer.Id);
                    summary.Balances.DonatedAmount =
                        decimal.Add(benefitBalance.ConfirmedAmount, benefitBalance.ReceivedAmount);
                }
            }

            return summary;
        }

        /// <summary>
        /// Returns the default diamond plan that a customer can upgrade too
        /// Not sure if it really belongs on a customer service, but there doesn't seem a 
        /// better place for it at the moment.
        /// </summary>
        /// <returns>
        /// The Diamond membership plan object, that will be used for upgrades
        /// This will be the Exclusive Rewards diamond plan. 
        /// Other card providers are not granted the option of lettings standard users upgrade via the site, 
        /// third party diamond plans must be purchased by the card provider 
        /// </returns>
        public dto.MembershipPlan GetDefaultDiamondPlan()
        {
            var plan = _membershipManager.GetDiamondMembershipPlan(0);
            return plan;
        }

        public dto.MembershipPlan GetMembershipPlan(int planId)
        {
            var plan = _membershipManager.GetMembershipPlan(planId);
            return plan;
        }

        public dto.Customer GetCustomer(string userId)
        {
            var customer = _customerManager.Get(userId);
            return customer;
        }

        #endregion

        public dto.BankDetail GetBankDetail(int bankDetailId)
        {
            var bankDetail = _bankDetailManager.Get(bankDetailId);
            return bankDetail;
        }
        public dto.BankDetail CreateBankDetail(dto.BankDetail bankDetail)
        {
            bankDetail = _bankDetailManager.Create(bankDetail);
            return bankDetail;
        }
        public dto.BankDetail UpdateBankDetail(dto.BankDetail bankDetail)
        {
            bankDetail = _bankDetailManager.Update(bankDetail);
            return bankDetail;
        }

        public dto.CustomerBankDetail GetCustomerBankDetail(int customerId, int bankdetailId = 0)
        {
            var result = _bankDetailManager.GetCustomerBankDetail(customerId, bankdetailId);
            return result;
        }
        public dto.CustomerBankDetail CreateCustomerBankDetail(dto.CustomerBankDetail customerBankDetail)
        {
            var result = _bankDetailManager.CreateCustomerBankDetail(customerBankDetail);
            return result;
        }
        public dto.CustomerBankDetail UpdateCustomerBankDetail(dto.CustomerBankDetail customerBankDetail)
        {
            var result = _bankDetailManager.UpdateCustomerBankDetail(customerBankDetail);
            return result;
        }


        #region Login

        public async Task ForgotPassword(string userName)
        {
            await Task.CompletedTask;

            throw new NotImplementedException();
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(ExclusiveUser user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailTokenAsync(ExclusiveUser user, string token)
        {
            return await _userManager.ConfirmEmailTokenAsync(user, token);
        }

        //TODO:  Replace with Customer object
        public async Task<dto.UserAccountDetails> Login(string userName, string password)
        {
            var user = await _userManager.CustomerLoginAsync(userName, password);
            return user;

        }

        public async Task Logout(string returnUrl = null)
        {
            await _userManager.LogoutAsync();
        }

        public dto.UserToken ValidateRegistrationCode(string code)
        {
            dto.UserToken userToken = null;

            var planandtoken = ValidateCode(code);
            var plan = planandtoken.Item1;
            var token = planandtoken.Item2;
            if (plan != null)
            {
                userToken = new dto.UserToken()
                {
                    CardCost = plan.CustomerCardPrice,
                    MembershipPlanId = plan.Id,
                    Token = token.Token,
                    Id = token.Id,
                    WhitelabelId = plan.WhitelabelId
                };
            }

            return userToken;
        }

        public dto.CustomerBalances GetBenefactorDeposits(int customerId)
        {
            // Get benefactor deposit balances from cashback manager
            var balances = _cashbackManager.GetCustomerBalances(customerId, 'B');

            return balances;
        }

        public dto.MembershipPlan GetTalkSportRegistrationCode(int whiteLabelId, int membershipPlanTypeId)
        {
            var plan = _membershipManager.GetTalkSportRegistrationCode(whiteLabelId, membershipPlanTypeId);
            return plan;
        }


        #endregion

        #region Update Data

        public dto.Customer UpdateCustomerSettings(dto.Customer customer)
        {
            customer = _customerManager.UpdateCustomerSettings(customer);
            return customer;
        }

        #endregion

        #endregion

        #region Private methods

        private Tuple<MembershipPlan, MembershipPendingToken> ValidateCode(string code)
        {
            return ValidateCode(null, code);
        }

        private Tuple<MembershipPlan, MembershipPendingToken> ValidateCode(CustomerAccountDto customerAccount, string registrationCode)
        {

            // Step 1 - Find the Membership plan from registration Code provided
            var plan = _membershipManager.GetMembershipPlanFromRegistrationCode(registrationCode);
            if (plan == null)
                throw new Exception("Unable to create account - registration code is invalid or plan not available");

            // Step 2  - Create a pending token
            var token = _membershipManager.CreatePendingToken(plan, registrationCode);
            if (token == null)
                throw new Exception("Unable to create account -  insufficient memberships available for this code/plan");

            if (customerAccount != null)
                customerAccount.PendingMembershipToken = token;

            return new Tuple<MembershipPlan, MembershipPendingToken>(plan, token);
        }

        #endregion

    }
}