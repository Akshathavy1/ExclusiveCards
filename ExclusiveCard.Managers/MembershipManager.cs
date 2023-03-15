using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using db = ExclusiveCard.Data.Models;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using dto = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    /// <summary>
    /// The Membership Manager looks after membership cards, plans and registration codes.
    /// It is responsible for creating new membership cards, finding the correct card to use when shopping
    /// and validating users have an active membership
    /// </summary>
    public class MembershipManager : IMembershipManager
    {
        #region Private Fields and Constructor

        private const int STANDARD_LEVEL = 0;
        private const int DIAMOND_LEVEL = 10;
        private const string EXCLUSIVE_CARD_PROVIDER = "Exclusive Media";
        private const int CARD_PROVIDER_TYPE = 1;

        private readonly ExclusiveContext _exclusiveContext;
        private readonly IRepository<db.MembershipPlan> _membershipPlanRepo;
        private readonly IRepository<db.MembershipRegistrationCode> _registrationCodeRepo;
        private readonly IRepository<db.MembershipPendingToken> _pendingTokenRepo;
        private readonly IRepository<db.SequenceNumbers> _sequenceRepo;
        private readonly IRepository<db.CustomerPayment> _customerPaymentRepo;
        // Use the custom Membership Card Repo rather than the generic version
        private readonly IMembershipCardRepository _membershipCardRepo;
        private readonly IRepository<db.Partner> _partnerRepo;
        private IMapper _mapper;

        public MembershipManager(IMembershipCardRepository membershipCardRepo, IRepository<db.MembershipPlan> membershipPlanRepo,
                                IRepository<db.MembershipPendingToken> pendingTokenRepo, IRepository<db.MembershipRegistrationCode> registrationCodeRepo,
                                IRepository<db.SequenceNumbers> sequenceRepo, IRepository<db.CustomerPayment> customerPaymentRepo, 
                                IRepository<db.Partner> partnerRepo, IMapper mapper,  ExclusiveContext exclusiveContext)
        {
            _membershipCardRepo = membershipCardRepo;
            _membershipPlanRepo = membershipPlanRepo;
            _pendingTokenRepo = pendingTokenRepo;
            _registrationCodeRepo = registrationCodeRepo;
            _sequenceRepo = sequenceRepo;
            _customerPaymentRepo = customerPaymentRepo;
            _partnerRepo = partnerRepo;
            _exclusiveContext = exclusiveContext;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        public dto.MembershipCard GetOriginalMembershipCard(int customerId)
        {

            var dbCard = _membershipCardRepo.IncludeAndThenInclude(x => x.CustomerId == customerId,
                                                                        x => x.Include(y => y.MembershipPlan).Include(y => y.PartnerReward))
                            .OrderBy(x => x.ValidFrom).OrderBy(x => x.MembershipPlan.MembershipLevel.Level).FirstOrDefault();

            var dtoCard = _mapper.Map<dto.MembershipCard>(dbCard);
            return dtoCard;
        }

        public dto.MembershipCard GetActiveMembershipCard(string aspNetUserId)
        {

            var dbCard = _membershipCardRepo.IncludeAndThenInclude(x => x.Customer.AspNetUserId == aspNetUserId 
                                                                             &&  x.IsActive 
                                                                             && x.MembershipStatus.Name == Data.Constants.Status.Active 
                                                                             //&& x.MembershipStatus.Type == Data.Constants.StatusType.MembershipCard 
                                                                             && x.ValidTo >= DateTime.UtcNow,
                                                                        x => x.Include(y => y.MembershipPlan).Include(y => y.PartnerReward))
                            .OrderByDescending(x => x.MembershipPlan.MembershipLevel.Level).FirstOrDefault();


            var dtoCard = _mapper.Map<dto.MembershipCard>(dbCard);
            return dtoCard;
        }

        public dto.MembershipCard GetActiveMembershipCard(int customerId)
        {

            var dbCard = _membershipCardRepo.IncludeAndThenInclude(x => x.CustomerId == customerId
                                                                             && x.IsActive
                                                                             && x.MembershipStatus.Name == Data.Constants.Status.Active
                                                                             //&& x.MembershipStatus.Type == Data.Constants.StatusType.MembershipCard 
                                                                             && x.ValidTo >= DateTime.UtcNow,
                                                                        x => x.Include(y => y.MembershipPlan).Include(y => y.PartnerReward))
                            .OrderByDescending(x => x.MembershipPlan.MembershipLevel.Level).FirstOrDefault();

            var dtoCard = _mapper.Map<dto.MembershipCard>(dbCard);
            return dtoCard;
        }

        public dto.MembershipCard GetDiamondMembershipCard(string aspNetUserId)
        {
            //throw new Exception("The GetDiamondMemberhsipCard method makes no sense. Stop here and figure out what the hell is going on.");
            var dbCard = _membershipCardRepo.IncludeAndThenInclude(x => x.Customer.AspNetUserId == aspNetUserId &&
                                                                           x.IsActive &&
                                                                           x.MembershipStatus.Name ==
                                                                           Data.Constants.Status.Pending &&
                                                                           x.MembershipStatus.Type ==
                                                                           Data.Constants.StatusType.MembershipCard &&
                                                                           x.ValidTo >= DateTime.UtcNow,
                                                                           x => x.Include(y => y.MembershipStatus))
                          .OrderByDescending(x => x.MembershipPlan.MembershipLevel.Level).FirstOrDefault();

            var dtoCard = _mapper.Map<dto.MembershipCard>(dbCard);
            return dtoCard;
        }

        public dto.MembershipCard GetDiamondMembershipCard(int customerId)
        {
            //throw new Exception("The GetDiamondMemberhsipCard method makes no sense. Stop here and figure out what the hell is going on.");
            var dbCard = _membershipCardRepo.IncludeAndThenInclude(x => x.CustomerId == customerId &&
                                                                           x.IsActive &&
                                                                           x.MembershipStatus.Name ==
                                                                           Data.Constants.Status.Pending
                                                                           &&
                                                                           x.MembershipStatus.Type ==
                                                                           Data.Constants.StatusType.MembershipCard &&
                                                                           x.ValidTo >= DateTime.UtcNow,
                                                                           x => x.Include(y => y.MembershipStatus))
                          .OrderByDescending(x => x.MembershipPlan.MembershipLevel.Level).FirstOrDefault();

            var dtoCard = _mapper.Map<dto.MembershipCard>(dbCard);
            return dtoCard;
        }

        /// <summary>
        /// Gets a membership plan based on the plan Id
        /// </summary>
        /// <param name="planId">Id of the membership plan (Exclusive.MembershipPlan.Id)</param>
        /// <returns>The Membership Plan DTO</returns>
        public dto.MembershipPlan GetMembershipPlan(int planId)
        {
            var dbPlan = _membershipPlanRepo.Include(x => x.MembershipPlanPaymentProvider, x => x.MembershipLevel).FirstOrDefault(p => p.Id == planId);
            var dtoPlan = _mapper.Map<dto.MembershipPlan>(dbPlan);

            return dtoPlan;
        }

        /// <summary>
        /// Finds the diamond membership plan from the plan Id of the Standard plan given
        /// This method looks for all membership plans issued by the same card provider as who issued the standard plan 
        /// If no standard plan is provided, or the standard plan issuing provider does not have it's own matching diamond plan, 
        /// then the default exclusive provider and it's diamond plans are used
        /// Filter: First active plan that has the membership level of diamond and the same plan type as the standard plan id
        /// A PartnerReward standard plan will return a PartnerReward diamond plan
        /// A BenefitRewards standard plan will return BenefitRewards diamond plan
        /// There should only be one (or zero) diamond plans for each card provider with the same plan type
        /// The default exclusive provider is expected to have diamond plans set up for each plan type in use
        /// </summary>
        /// <param name="standardPlanId">
        /// Plan Id belonging to a standard plan.
        /// Passing in 0 will result in the first Exclusive Reward diamond plan found being returned (plan type is ignored in this case)
        /// </param>
        /// <returns>Best match diamond plan for the standard plan id provided or null if a diamond plan could not be found</returns>
        public dto.MembershipPlan GetDiamondMembershipPlan(int standardPlanId)
        {
            dto.MembershipPlan diamondPlan = null;
            db.MembershipPlan dbDiamondPlan = null;
            db.MembershipPlan dbStandardPlan = null;

            if (standardPlanId != 0)
            {
                dbStandardPlan = _membershipPlanRepo.GetById(standardPlanId);
                if (dbStandardPlan != null)
                    dbDiamondPlan = _membershipPlanRepo.Include(x => x.MembershipLevel).FirstOrDefault(x => x.CardProviderId == dbStandardPlan.CardProviderId && x.MembershipLevel.Level == DIAMOND_LEVEL && x.MembershipPlanTypeId == dbStandardPlan.MembershipPlanTypeId && x.IsActive == true && x.IsDeleted == false);
            }

            // If no standard plan Id provided, return the Exclusive Rewards diamond plan
            if (standardPlanId == 0 || dbDiamondPlan == null)
            {
                var partner = _partnerRepo.Get(x => x.Name == EXCLUSIVE_CARD_PROVIDER && x.Type == CARD_PROVIDER_TYPE);
                if (partner != null)
                {
                    if (dbStandardPlan != null)
                        dbDiamondPlan = _membershipPlanRepo.Get(x => x.CardProviderId == partner.Id && x.MembershipLevel.Level == DIAMOND_LEVEL && x.MembershipPlanTypeId == dbStandardPlan.MembershipPlanTypeId && x.IsActive == true && x.IsDeleted == false);
                    else
                        dbDiamondPlan = _membershipPlanRepo.Get(x => x.CardProviderId == partner.Id && x.MembershipLevel.Level == DIAMOND_LEVEL && x.IsActive == true && x.IsDeleted == false);
                }
            }
            if(dbDiamondPlan != null)
                diamondPlan = _mapper.Map<dto.MembershipPlan>(dbDiamondPlan);
            return diamondPlan;
        }

        public dto.MembershipPlan GetDiamondMembershipPlan(string CardProviderName, int CardPrividerType, int PlanTypeId)
        {
            dto.MembershipPlan result = null;
            db.MembershipPlan dbDiamondPlan = null;

            var partner = _partnerRepo.Get(x => x.Name == CardProviderName && x.Type == CardPrividerType);
            if (partner != null)
            {
                dbDiamondPlan = _membershipPlanRepo.Get(x => x.CardProviderId == partner.Id && x.MembershipLevel.Level == DIAMOND_LEVEL && x.MembershipPlanTypeId == PlanTypeId && x.IsActive == true && x.IsDeleted == false);
                if (dbDiamondPlan != null)
                    result = _mapper.Map<dto.MembershipPlan>(dbDiamondPlan);
            }

            return result;
        }

        /// <summary>
        /// Creates one or more membership cards.
        /// This method is designed where a new customer is getting their membership cards setup.
        /// There are separate methods for upgrading or renewing existing cards.
        /// Support for classic Cashback cards has been removed as part of this refactor. It is only dealing with Exclusive Reward Cards now  (PlanType = 4)
        /// If card provider is paying for diamond, 
        ///  -> customer will get a diamond and standard card created, both linked to same partner 
        /// Otherwise
        /// -> customer will only receive a standard card
        /// Pre-requisites for this method are that customer is created and partner reward record already added. Plan & registration code are valid.
        /// </summary>
        /// <param name="customerId">Id of customer to assign cards to (Exclusive.Customer.Id)</param>
        /// <param name="planId">Id of Membership plan cards will be created for (Exclusive.MembershipPlan.Id)</param>
        /// <param name="registrationCodeId">Id of the registration code that was provided during customer sign up</param>
        /// <param name="termsConditionsId">Id of the Terms & Conditions record for version of these that the customer has accepted</param>
        /// <param name="partnerRewardId">Id of the partner reward belonging to this customer -  already created in a previous step of creating an account</param>
        /// <returns>List of the membership cards created</returns>
        public List<dto.MembershipCard> CreateMembershipCards(dto.MembershipPlan plan, int customerId, int registrationCodeId, int? termsConditionsId, int? partnerRewardId, string countryCode)
        {
            var newCards = new List<dto.MembershipCard>();
            var dbPlan = GetMembershipPlan(plan.Id);
            var standardPlan = plan;
            db.MembershipCard diamondCard = null;
            db.MembershipCard standardCard = null;

            // Use explicit transaction to ensure unique membership card
            using (var transaction = _exclusiveContext.Database.BeginTransaction())
            {

                try
                {
                    // Generate Card Number - this is shared by all levels of the card (i.e diamond and standard cards have same number)
                    string cardNumber = GenerateMembershipCardNumber(plan.CardProvider?.MembershipCardPrefix, countryCode);

                    // Check Plan Type to determine whether to create a diamond card
                    if (plan.PaidByEmployer || dbPlan.MembershipLevelId == (int)Enums.MembershipLevel.Diamond)
                    {
                        // Create diamond card
                        diamondCard = CreateMembershipCard(plan, customerId, registrationCodeId, termsConditionsId, partnerRewardId, cardNumber, MembershipCardStatus.Active);
                        

                        // Find the standard plan
                        var dbStandardPlan = _membershipPlanRepo.Filter(x => x.CardProviderId == plan.CardProviderId && x.MembershipLevel.Level == STANDARD_LEVEL).FirstOrDefault();
                        standardPlan = _mapper.Map<dto.MembershipPlan>(dbStandardPlan);
                    }

                    // Create standard card
                    standardCard = CreateMembershipCard(standardPlan, customerId, registrationCodeId, termsConditionsId, partnerRewardId, cardNumber, MembershipCardStatus.Active);
                    
                    // Update the Db
                    _membershipCardRepo.SaveChanges();
                    transaction.Commit();

                    // Map the cards to DTOs
                    if (diamondCard != null)
                    {
                        var dtoDiamondCard = _mapper.Map<dto.MembershipCard>(diamondCard);
                        newCards.Add(dtoDiamondCard);
                    }

                    var dtoStandardCard = _mapper.Map<dto.MembershipCard>(standardCard);
                    newCards.Add(dtoStandardCard);

                    
                }
                catch(Exception ex)
                {

                    transaction.Rollback();
                    throw new Exception("Creation of membership cards failed", ex);
                }


            }


            return newCards;
        }

        /// <summary>
        /// Create a PendingMembership token against the plan and registration code provided. 
        /// This is a first step to creating a new membership. It validates the code to ensure that the max  
        /// number of codes / memberships have not already been issued. Assuming all ok, it create a pending token (just a GUID)
        /// in the db to reserve this code for the rest of the process.
        /// The plan is assumed to have already been validated by one of the GetPlan methods, so validation is not repeated again here. 
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="registrationCode"></param>
        /// <returns></returns>
        public dto.MembershipPendingToken CreatePendingToken(dto.MembershipPlan plan, string registrationCode)
        {
            dto.MembershipPendingToken token = null;

            if (plan == null)
                throw new Exception("Unable to create pending token - membership plan not found");

            var code = plan.MembershipRegistrationCodes.FirstOrDefault(x => x.RegistartionCode == registrationCode);
            if (code == null)
                throw new Exception("Unable to create pending token - registration code not found");

            bool valid = ValidateNumberOfPlansAndCodes(plan, code);

            if (valid)
            {
                var pendingToken = new db.MembershipPendingToken()
                {
                    DateCreated = DateTime.UtcNow,
                    MembershipRegistrationCodeId = code.Id, 
                    Token = Guid.NewGuid()
                };
                _pendingTokenRepo.Create(pendingToken);
                _pendingTokenRepo.SaveChanges();

                token = _mapper.Map<dto.MembershipPendingToken>(pendingToken);
            }
                           

            return token;
        }

        /// <summary>
        /// Finds the membership plan associated with a pending membership token.
        /// The pending membership token is created when a user first enters the registration code on the website for sign-up.
        /// The token is valid for up to 1 hour. 
        /// This method will only return plans if the pending token is still valid.
        /// Additionally a plan will only be returned if on  the membership plan, 
        /// the records are marked as active, not deleted and between the valid From and Valid To dates 
        /// </summary>
        /// <param name="pendingToken"></param>
        /// <returns>
        /// A Membership Plan Entity if the token is valid
        /// Null if the token is invalid or cannot be found
        /// </returns>
        public dto.MembershipPlan GetMembershipPlanFromPendingToken(Guid? pendingToken, out int? registrationCodeId)
        {
            dto.MembershipPlan dtoPlan = null;
            var rightNow = DateTime.UtcNow;

            var token = _pendingTokenRepo.IncludeAndThenInclude(t => t.Token == pendingToken && (DateTime.UtcNow - t.DateCreated).Hours <= 1,
                                                               c => c.Include(p => p.MembershipRegistrationCode.MembershipPlan).ThenInclude(x => x.CardProvider))
                                                               .FirstOrDefault(
                                                                                 x => x.MembershipRegistrationCode.MembershipPlan.IsActive == true
                                                                                 && x.MembershipRegistrationCode.MembershipPlan.IsDeleted == false
                                                                                 && x.MembershipRegistrationCode.MembershipPlan.ValidFrom < rightNow
                                                                                 && x.MembershipRegistrationCode.MembershipPlan.ValidTo > rightNow
                                                                                );


            if (token != null && token.MembershipRegistrationCode != null)
                dtoPlan = _mapper.Map<dto.MembershipPlan>(token.MembershipRegistrationCode.MembershipPlan);

            registrationCodeId = token?.MembershipRegistrationCodeId;

            return dtoPlan;
        }

        /// <summary>
        /// Find the membership plan associated with a registration code
        /// A plan will only be returned if on both the registrationcode and the membershipplan, 
        /// the records are marked as active, not deleted and between the valid From and Valid To dates 
        /// </summary>
        /// <param name="registrationCode"></param>
        /// <returns></returns>
        public dto.MembershipPlan GetMembershipPlanFromRegistrationCode(string registrationCode)
        {
            dto.MembershipPlan dtoPlan = null;
            var rightNow = DateTime.UtcNow;

            var code = _registrationCodeRepo.IncludeAndThenInclude(x => x.RegistartionCode == registrationCode && x.IsActive == true && x.IsDeleted == false
                                                                     && x.ValidFrom < rightNow && x.ValidTo > rightNow,
                                                                   p => p.Include(q => q.MembershipPlan).ThenInclude(x => x.CardProvider))
                                                                   .FirstOrDefault( y => y.IsActive == true && y.IsDeleted == false
                                                                                      && y.ValidFrom < rightNow && y.ValidTo > rightNow);

            if (code != null)
                dtoPlan = _mapper.Map<dto.MembershipPlan>(code.MembershipPlan);

            return dtoPlan;
        }

        /// <summary>
        /// Upgrades a standard membership into a diamond one.
        /// Will validate to ensure that a customer payment has been made for a diamond membership, from this customer 
        /// against this standard membership card and within the last 7 days.
        /// If no payment found, the diamond membership card is still setup but is configured with a status of Pending rather than Active.
        /// 
        /// To skip payment validation, leave paymentDetails null
        /// </summary>
        /// <param name="standardCard">The existing membership card to be upgraded</param>
        /// <param name="paymentDetails">The payment description for the product sold by the payment provider (i.e PayPal) Required so no mixing up payments from account boosts for example.</param>
        /// <returns></returns>
        public dto.MembershipCard UpgradeToDiamond(dto.MembershipCard standardCard, string paymentDetails, decimal paymentAmount)
        {
            MembershipCardStatus status = MembershipCardStatus.Active;
            
            if (paymentDetails != null)
                status = ValidatePayment(standardCard, paymentDetails);

            //## Find the Diamond plan
            var diamondPlan = GetDiamondMembershipPlan(standardCard.MembershipPlanId);

            // Find the diamond plan
            //var dbDiamondPlan = _membershipPlanRepo.Filter(x => x.CardProviderId == standardCard.MembershipPlan.CardProviderId && x.MembershipLevel.Level == DIAMOND_LEVEL).FirstOrDefault();

            //// If no standard plan Id provided, return the Exclusive Rewards diamond plan
            //if (dbDiamondPlan == null)
            //{
            //    var partner = _partnerRepo.Get(x => x.Name == EXCLUSIVE_CARD_PROVIDER && x.Type == CARD_PROVIDER_TYPE);
            //    if (partner != null)
            //    {
            //        dbDiamondPlan = _membershipPlanRepo.Get(x => x.CardProviderId == partner.Id && x.MembershipLevel.Level == DIAMOND_LEVEL);
            //    }
            //}
            //diamondPlan = _mapper.Map<dto.MembershipPlan>(dbDiamondPlan);

            // create the new diamond card (in addition to the existing card)
            var dbDiamondCard = CreateMembershipCard(diamondPlan, (int)standardCard.CustomerId, (int)standardCard.RegistrationCode, standardCard.TermsConditionsId, standardCard.PartnerRewardId, standardCard.CardNumber, status);
            _membershipCardRepo.SaveChanges();
            var diamondCard = _mapper.Map<dto.MembershipCard>(dbDiamondCard);

            
            //
            if ((MembershipPlanTypeEnum)diamondPlan.MembershipPlanTypeId == MembershipPlanTypeEnum.BenefitRewards)
            {
                CashbackTransaction cashBackTransaction = new CashbackTransaction();
               // cashBackTransaction.PurchaseAmount = diamondPlan.CustomerCardPrice;
                cashBackTransaction.PurchaseAmount = paymentAmount;
                cashBackTransaction.CashbackAmount = Convert.ToDecimal("0.00");
                cashBackTransaction.MerchantId = null;
                cashBackTransaction.MembershipCardId = dbDiamondCard.Id;
                cashBackTransaction.TransactionDate = DateTime.UtcNow;
                cashBackTransaction.DateReceived = DateTime.UtcNow; ;
                cashBackTransaction.StatusId = (int)Cashback.UserPaid;
                cashBackTransaction.Summary = "Diamond Upgrade";
                cashBackTransaction.AccountType = 'R';
                cashBackTransaction.CurrencyCode = diamondPlan.CurrencyCode;
                var req = _mapper.Map<db.CashbackTransaction>(cashBackTransaction);
                // AccountType R
                _membershipCardRepo.CreateCashBackTransactions(req);

                // AccountType B
                cashBackTransaction.AccountType = 'B';
                if (paymentAmount != 0)
                {
                    //cashBackTransaction.CashbackAmount = Convert.ToDecimal(diamondPlan.CustomerCardPrice * 0.3m);
                    cashBackTransaction.CashbackAmount = Convert.ToDecimal(paymentAmount * 0.3m);
                }

                cashBackTransaction.StatusId = (int)Cashback.Received;
                req = _mapper.Map<db.CashbackTransaction>(cashBackTransaction);
                _membershipCardRepo.CreateCashBackTransactions(req);
            }
            //
            

            // Return the new membership card
            return diamondCard;
        }


        /// <summary>
        /// Renews an expired membership card.
        /// Unlike Exclusive.Cards system, the same card will be used, a new card will not be issued
        /// A renewal will edit the existing record and update the status and expiry date.
        /// </summary>
        /// <param name="expiredCard">The membership card to be renewed</param>
        /// <param name="paymentDetails">Customer payment Details field, used to confirm a payment has been made. Leave  null to skip validation.</param>
        /// <param name="duration">Number of days to renew the card for</param>
        /// <returns></returns>
        public dto.MembershipCard Renew(dto.MembershipCard expiredCard, string paymentDetails, int duration = 0)
        {

            var cardEntity = _membershipCardRepo.GetById(expiredCard.Id);
            if (cardEntity == null)
                throw new Exception("Renewal of Membership failed - card not found. Card Id = " + expiredCard?.Id.ToString());


            // Validate the customer payment. If validation fails, card will be renewed but status set to pending.
            MembershipCardStatus status = MembershipCardStatus.Active;
            if (paymentDetails != null)
                status = ValidatePayment(expiredCard, paymentDetails);
            cardEntity.StatusId = (int)status;

            // If no duration supplied, default to that on the plan
            if (duration == 0)
                duration = expiredCard.MembershipPlan.Duration;
            
            // Set Valid to.  Leave Valid From as original date.
            cardEntity.ValidTo = DateTime.UtcNow.Date.AddDays(duration);

            _membershipCardRepo.Update(cardEntity);
            _membershipCardRepo.SaveChanges();

            var renewedCard = _mapper.Map<dto.MembershipCard>(cardEntity);
            return renewedCard;
        }

        public dto.PartnerDto GetCardProvider(int cardId)
        {
            dto.PartnerDto dtoPartner = null;

            var dbCard = _membershipCardRepo.Include(x => x.MembershipPlan, y => y.MembershipPlan.CardProvider).FirstOrDefault( y => y.Id == cardId);
                
            if (dbCard?.MembershipPlan?.CardProvider != null)
                dtoPartner = _mapper.Map<dto.PartnerDto>(dbCard.MembershipPlan.CardProvider);

            return dtoPartner;
        }

        public dto.MembershipPlan  GetTalkSportRegistrationCode(int whiteLabelId, int membershipPlanTypeId)
        {

            try
            {
                dto.MembershipPlan dtoPlan = null;
                var rightNow = DateTime.UtcNow;

                var code = _registrationCodeRepo.IncludeAndThenInclude(x =>  x.IsActive == true && x.IsDeleted == false
                                                                            && x.ValidFrom < rightNow && x.ValidTo > rightNow, 
                        p => p.Include(q => q.MembershipPlan))
                    .FirstOrDefault(y => y.IsActive == true && y.IsDeleted == false
                                                            && y.ValidFrom < rightNow && y.ValidTo > rightNow
                                                            && y.MembershipPlan.WhitelabelId== whiteLabelId
                                                            && y.MembershipPlan.MembershipPlanTypeId == membershipPlanTypeId);

                if (code != null)
                    dtoPlan = _mapper.Map<dto.MembershipPlan>(code.MembershipPlan);

                return dtoPlan;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Validate that there is a customer payment against this customer Id, membership card Id and matching payment details that has beeen 
        /// made in the last 7 days (time limit to check recent payment but allow for some slack in case of system issues, delays etc).
        /// </summary>
        /// <param name="card">Card the payment was made against</param>
        /// <param name="paymentDetails">The Details field from the customer payment record</param>
        /// <returns></returns>
        private MembershipCardStatus ValidatePayment(MembershipCard card, string paymentDetails)
        {

            var paymentDate = DateTime.UtcNow.AddDays(-7);
            var payment = _customerPaymentRepo.Get(x => x.CustomerId == card.CustomerId && x.MembershipCardId == card.Id && x.Details == paymentDetails && x.PaymentDate >= paymentDate);

            // If no payment found, a diamond membership card will still be issued but with a status of pending
            var status = MembershipCardStatus.Pending;
            if (payment != null)
                status = MembershipCardStatus.Active;
            return status;
        }

        private db.MembershipCard CreateMembershipCard(dto.MembershipPlan plan, int customerId, int registrationCodeId, int? termsConditionsId, 
                                                        int? partnerRewardId, string cardNumber, MembershipCardStatus status)
        {
            DateTime currentDate = DateTime.Today;
            
            // Create Card
            var dbCard = new db.MembershipCard()
            {
                CustomerId = customerId,
                MembershipPlanId = plan.Id,
                CardNumber = cardNumber,
                ValidFrom = currentDate,
                ValidTo = currentDate.AddDays(plan.Duration),
                DateIssued = DateTime.UtcNow,
                StatusId = (int)status,
                IsActive = true,
                IsDeleted = false,
                PhysicalCardRequested = false,  // No longer issuing any physical cards
                RegistrationCode = registrationCodeId, 
                PartnerRewardId = partnerRewardId,
                // ,TermsConditionsId = termsConditionsId  //TODO: fix insertion of Ts and Cs if we think it relevent

                   
            };

            // Use the bespoke createMembershipCard method, which will create the card and the Cashback summary
            _membershipCardRepo.CreateMembershipCard(dbCard, (MembershipPlanTypeEnum)plan.MembershipPlanTypeId);

            return dbCard;
        }

        private string GenerateMembershipCardNumber(string prefix, string countryCode)
        {
            string cardNumber = string.Empty;
            long nextNumber = 0;

            var seqNumber = _sequenceRepo.Filter(x => x.Description == "MembershipCardNumber").FirstOrDefault();
            if (seqNumber != null)
            {
                nextNumber = seqNumber.Value + 1;
                seqNumber.Value = nextNumber;
                cardNumber = string.Format("{0}{1:D7}{2}", prefix??"EX", nextNumber, countryCode);
                _sequenceRepo.Update(seqNumber);
                _sequenceRepo.SaveChanges();
            }
            else
                throw new Exception("Unable to find next membership card number");
            
            return cardNumber;
        }

        private bool ValidateNumberOfPlansAndCodes(dto.MembershipPlan plan, dto.MembershipRegistrationCode code)
        {
            bool result = false;

            // Get max number of codes or memberships allowed. If value in db is Zero, allow unlimited numbers.
            int codeLimit = code.NumberOfCards == 0 ? int.MaxValue : code.NumberOfCards;
            int planLimit = plan.NumberOfCards == 0 ? int.MaxValue : plan.NumberOfCards;

            // Count how many pending tokens there are for current code
            DateTime validDate = DateTime.UtcNow.AddHours(-1);
            var pendingTokens = _pendingTokenRepo.FilterNoTrackAsync(x => x.MembershipRegistrationCode.MembershipPlan.Id == plan.Id && x.DateCreated > validDate).Result;
            int planPending = pendingTokens.Count();
            int codePending = pendingTokens.Where(x => x.MembershipRegistrationCodeId == code.Id).Count();

            // Find total memberships based on code and plan
            var planMemberships = _membershipCardRepo.FilterNoTrackAsync(x => x.MembershipPlanId == plan.Id).Result;
            int planTotal = planMemberships.Count();
            int codeTotal = planMemberships.Where(x => x.MembershipRegistrationCode != null && x.MembershipRegistrationCode.RegistartionCode == code.RegistartionCode).Count();

            if (planTotal + planPending < planLimit && codeTotal + codePending < codeLimit)
                result = true;

            return result;
        }



        #endregion
    }
}
