using AutoMapper;
using System.Linq;
using db = ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MapUserAndCustomer();
            MapCashbackAndPayments();
            MapMerchantsAndOffers();
            MapSiteAndWhiteLabels();
            MapAfiliatesAndFiles();
            MapMembershipPlansAndCards();
            MapMarketing();
        }

        public void MapUserAndCustomer()
        {
            // Exclusive User
            CreateMap<dto.ExclusiveUser, db.ExclusiveUser>()
                .ForMember(d => d.NormalizedEmail, s => s.Ignore())
                .ForMember(d => d.NormalizedUserName, s => s.Ignore())
                .ForMember(d => d.EmailConfirmed, s => s.Ignore())
                .ForMember(d => d.PasswordHash, s => s.Ignore())
                .ForMember(d => d.SecurityStamp, s => s.Ignore())
                .ForMember(d => d.ConcurrencyStamp, s => s.Ignore())
                .ForMember(d => d.PhoneNumberConfirmed, s => s.Ignore())
                .ForMember(d => d.TwoFactorEnabled, s => s.Ignore())
                .ForMember(d => d.LockoutEnabled, s => s.Ignore())
                .ForMember(d => d.LockoutEnd, s => s.Ignore())
                .ForMember(d => d.AccessFailedCount, s => s.Ignore())
                .ForMember(d => d.Customer, s => s.Ignore())
                .ForMember(d => d.Partner, s => s.Ignore());
            CreateMap<db.ExclusiveUser, dto.ExclusiveUser>();

            CreateMap<dto.Customer, db.Customer>()
                .ForMember(d => d.SendGridContact, s => s.Ignore());

            CreateMap<db.Customer, dto.Customer>()
                .ForMember(x => x.ContactDetail, s => s.MapFrom(d => d.ContactDetail))
                .ForMember(x => x.CustomerBankDetails, s => s.MapFrom(d => d.CustomerBankDetails))
                .ForMember(x => x.MembershipCards, s => s.MapFrom(d => d.MembershipCards))
                .ForMember(x => x.CashbackPayouts, s => s.Ignore())
                .ForMember(x => x.CustomerPayment, s => s.Ignore())
                .ForMember(x => x.PayPalSubscriptions, s => s.Ignore());

            CreateMap<dto.ContactDetail, db.ContactDetail>()
                .ForMember(d => d.BankDetails, s => s.Ignore())
                .ForMember(d => d.Customers, s => s.Ignore())
                .ForMember(d => d.Id, s => s.Ignore())
                .ForMember(d => d.MerchantBranches, s => s.Ignore())
                .ForMember(d => d.Merchants, s => s.Ignore())
                .ForMember(d => d.Partners, s => s.Ignore());

            CreateMap<db.ContactDetail, dto.ContactDetail>();

            CreateMap<dto.CustomerBankDetail, db.CustomerBankDetail>();
            CreateMap<db.CustomerBankDetail, dto.CustomerBankDetail>();

            CreateMap<dto.CustomerSecurityQuestion, db.CustomerSecurityQuestion>();
            CreateMap<db.CustomerSecurityQuestion, dto.CustomerSecurityQuestion>();

            CreateMap<dto.BankDetail, db.BankDetail>()
                .ForMember(d => d.CustomerBankDetails, s => s.Ignore())
                .ForMember(d => d.Partners, s => s.Ignore())
                .ForMember(d => d.CashbackPayouts, s => s.Ignore())
                .ForMember(d => d.PartnerRewardWithdrawals, s => s.Ignore());

            CreateMap<db.BankDetail, dto.BankDetail>()
                .ForMember(d => d.CustomerBankDetail, s => s.Ignore());

            CreateMap<dto.SecurityQuestion, db.SecurityQuestion>()
                .ForMember(d => d.CustomerSecurityQuestions, s => s.Ignore());

            CreateMap<db.SecurityQuestion, dto.SecurityQuestion>();

            CreateMap<dto.LoginUserToken, db.LoginUserToken>();

            CreateMap<db.LoginUserToken, dto.LoginUserToken>();

            CreateMap<db.Customer, Providers.Marketing.Models.MarketingContact>()
                .ForMember(d => d.FirstName, s => s.MapFrom(m => m.Forename))
                .ForMember(d => d.LastName, s => s.MapFrom(m => m.Surname))
                .ForMember(d => d.Address1, s => s.MapFrom(m => m.ContactDetail.Address1))
                .ForMember(d => d.Address2, s => s.MapFrom(m => m.ContactDetail.Address2))
                .ForMember(d => d.PostCode, s => s.MapFrom(m => m.ContactDetail.PostCode))
                .ForMember(d => d.Email, s => s.MapFrom(m => m.ContactDetail.EmailAddress))
                .ForMember(d => d.City, s => s.MapFrom(m => m.ContactDetail.District));

            CreateMap<db.Customer, Providers.Marketing.Models.MarketingContactId>()
                .ForMember(d => d.Id, s => s.MapFrom(m => m.SendGridContact.ContactReference));
        }

        public void MapCashbackAndPayments()
        {
            CreateMap<dto.PaymentSubscription, db.PayPalSubscription>()
                .ForMember(d => d.PayPalId, s => s.MapFrom(o => o.SubscriptionId))
                .ForMember(d => d.PayPalStatusId, s => s.MapFrom(o => o.StatusId))
                .ForMember(d => d.Customer, s => s.Ignore())
                .ForMember(d => d.MembershipPlan, s => s.Ignore())
                .ForMember(d => d.PayPalStatus, s => s.Ignore());

            CreateMap<db.PayPalSubscription, dto.PaymentSubscription>()
                .ForMember(d => d.SubscriptionId, s => s.MapFrom(o => o.PayPalId))
                .ForMember(d => d.StatusId, s => s.MapFrom(o => o.PayPalStatusId));

            CreateMap<dto.PaymentNotification, db.PaymentNotification>();
            CreateMap<dto.PaymentProvider, db.PaymentProvider>();

            CreateMap<dto.PartnerRewards, db.PartnerRewards>();

            CreateMap<dto.PayPalSubscription, db.PayPalSubscription>();

            CreateMap<dto.PartnerRewardWithdrawal, db.PartnerRewardWithdrawal>()
                .ForMember(d => d.PartnerReward, s => s.Ignore())
                .ForMember(d => d.WithdrawalStatus, s => s.Ignore())
                .ForMember(d => d.File, s => s.Ignore())
                .ForMember(d => d.BankDetail, s => s.Ignore());

            CreateMap<db.PartnerRewardWithdrawal, dto.PartnerRewardWithdrawal>()
                .ForMember(d => d.PartnerReward, s => s.Ignore())
                .ForMember(d => d.WithdrawalStatus, s => s.Ignore())
                .ForMember(d => d.File, s => s.Ignore())
                .ForMember(d => d.BankDetail, s => s.Ignore())
                .ForMember(d => d.Customer, s => s.Ignore());

            CreateMap<dto.CashbackTransaction, db.CashbackTransaction>()
                .ForMember(d => d.MembershipCard, s => s.Ignore());

            CreateMap<dto.CashbackSummary, db.CashbackSummary>();
            CreateMap<dto.CashbackPayout, db.CashbackPayout>();

            CreateMap<db.PartnerRewards, dto.PartnerRewards>();

            CreateMap<db.CustomerPayment, dto.CustomerPayment>();
            CreateMap<db.PaymentProvider, dto.PaymentProvider>();
            CreateMap<db.PaymentNotification, dto.PaymentNotification>();
            CreateMap<db.MembershipPlanPaymentProvider, dto.MembershipPlanPaymentProvider>();

            CreateMap<db.CashbackPayout, dto.CashbackPayout>();
            CreateMap<db.CashbackSummary, dto.CashbackSummary>();
            CreateMap<db.CashbackTransaction, dto.CashbackTransaction>().PreserveReferences();
            CreateMap<db.PayPalSubscription, dto.PayPalSubscription>();
            CreateMap<db.SSOConfiguration, dto.SSOConfiguration>();
        }

        public void MapMerchantsAndOffers()
        {
            CreateMap<dto.Category, db.Category>()
                .ForMember(d => d.CategoryFeatureDetails, s => s.Ignore());
            CreateMap<db.Category, dto.Category>();

            CreateMap<dto.Merchant, db.Merchant>();
            CreateMap<dto.MerchantBranch, db.MerchantBranch>();
            CreateMap<dto.MerchantImage, db.MerchantImage>();
            CreateMap<dto.MerchantSocialMediaLink, db.MerchantSocialMediaLink>();

            CreateMap<db.Merchant, dto.Merchant>();
            CreateMap<db.MerchantBranch, dto.MerchantBranch>();
            CreateMap<db.MerchantImage, dto.MerchantImage>();
            CreateMap<db.MerchantSocialMediaLink, dto.MerchantSocialMediaLink>();

            CreateMap<dto.Offer, db.Offer>()
                .ForMember(d => d.OfferRedemptions, s => s.Ignore());
            CreateMap<dto.OfferCategory, db.OfferCategory>();
            CreateMap<dto.OfferCountry, db.OfferCountry>();
            CreateMap<dto.OfferMerchantBranch, db.OfferMerchantBranch>();
            CreateMap<dto.OfferTag, db.OfferTag>();

            CreateMap<dto.OfferType, db.OfferType>();

            CreateMap<db.OfferType, dto.OfferType>()
                .ForMember(d => d.ListName, s => s.Ignore());

            CreateMap<db.Offer, dto.Offer>();
            CreateMap<db.OfferCategory, dto.OfferCategory>();
            CreateMap<db.OfferCountry, dto.OfferCountry>();
            CreateMap<db.OfferMerchantBranch, dto.OfferMerchantBranch>();
            CreateMap<db.OfferTag, dto.OfferTag>();

            CreateMap<dto.Status, db.Status>();
            CreateMap<dto.Tag, db.Tag>();

            CreateMap<db.Status, dto.Status>();
            CreateMap<db.Tag, dto.Tag>();

            CreateMap<dto.OfferList, db.OfferList>();
            CreateMap<db.OfferList, dto.OfferList>();
            CreateMap<db.OfferListItem, dto.OfferListItem>();
        }

        public void MapSiteAndWhiteLabels()
        {
            CreateMap<dto.SocialMediaCompany, db.SocialMediaCompany>();
            CreateMap<dto.Localisation, db.Localisation>();
            CreateMap<dto.ClickTracking, db.ClickTracking>();

            CreateMap<db.SocialMediaCompany, dto.SocialMediaCompany>();
            CreateMap<db.Localisation, dto.Localisation>();
            CreateMap<db.ClickTracking, dto.ClickTracking>();

            CreateMap<db.TermsConditions, dto.TermsConditions>()
                .ForMember(d => d.MembershipCards, s => s.Ignore())
                .ForMember(d => d.MembershipPlanTypes, s => s.Ignore());

            CreateMap<db.WebsiteSocialMediaLink, dto.Public.WebsiteSocialMediaLink>();
        }

        public void MapAfiliatesAndFiles()
        {
            CreateMap<dto.Affiliate, db.Affiliate>()
                .ForMember(d => d.AffiliateFiles, s => s.Ignore());
            CreateMap<dto.AffiliateFieldMapping, db.AffiliateFieldMapping>();
            CreateMap<dto.AffiliateFile, db.AffiliateFile>()
                .ForMember(d => d.OfferImportFiles, s => s.Ignore());
            CreateMap<dto.AffiliateFileMapping, db.AffiliateFileMapping>()
                .ForMember(d => d.Affiliate, s => s.Ignore())
                .ForMember(d => d.AffiliateFieldMappings, s => s.Ignore())
                .ForMember(d => d.AffiliateFiles, s => s.Ignore());
            CreateMap<dto.AffiliateMapping, db.AffiliateMapping>();
            CreateMap<dto.AffiliateMappingRule, db.AffiliateMappingRule>()
                .ForMember(d => d.Affiliate, s => s.Ignore())
                .ForMember(d => d.AffiliateFieldMappings, s => s.Ignore());

            CreateMap<db.Affiliate, dto.Affiliate>();
            CreateMap<db.AffiliateFile, dto.AffiliateFile>();
            CreateMap<db.AffiliateFieldMapping, dto.AffiliateFieldMapping>();
            CreateMap<db.AffiliateFileMapping, dto.AffiliateFileMapping>();
            CreateMap<db.AffiliateMapping, dto.AffiliateMapping>();
            CreateMap<db.AffiliateMappingRule, dto.AffiliateMappingRule>();

            CreateMap<dto.StagingModels.OfferImportAwin, Data.StagingModels.OfferImportAwin>();

            CreateMap<dto.StagingModels.OfferImportFile, Data.StagingModels.OfferImportFile>()
                .ForMember(d => d.OfferImportAwinDuplicates, s => s.Ignore())
                .ForMember(d => d.OfferImportAwins, s => s.Ignore())
                .ForMember(d => d.AffiliateFile, s => s.Ignore());

            CreateMap<dto.StagingModels.OfferImportError, Data.StagingModels.OfferImportError>();

            CreateMap<dto.StagingModels.CashbackTransactionError, Data.StagingModels.CashbackTransactionError>();
            CreateMap<dto.StagingModels.CustomerRegistration, Data.StagingModels.CustomerRegistration>();
            CreateMap<dto.StagingModels.CashbackTransaction, Data.StagingModels.CashbackTransaction>();
            CreateMap<dto.StagingModels.TransactionFile, Data.StagingModels.TransactionFile>();

            CreateMap<Data.StagingModels.OfferImportAwin, dto.StagingModels.OfferImportAwin>();
            CreateMap<Data.StagingModels.OfferImportFile, dto.StagingModels.OfferImportFile>();
            CreateMap<Data.StagingModels.TransactionFile, dto.StagingModels.TransactionFile>();
            CreateMap<Data.StagingModels.CashbackTransaction, dto.StagingModels.CashbackTransaction>();
            CreateMap<Data.StagingModels.CashbackTransactionError, dto.StagingModels.CashbackTransactionError>();
            CreateMap<Data.StagingModels.CustomerRegistration, dto.StagingModels.CustomerRegistration>();

            CreateMap<dto.Files, db.Files>()
                .ForMember(d => d.OfferRedemptions, s => s.Ignore())
                .ForMember(d => d.Partner, s => s.Ignore())
                .ForMember(d => d.Status, s => s.Ignore())
                .ForMember(d => d.PaymentStatus, s => s.Ignore());

            CreateMap<db.Files, dto.Files>()
                .ForMember(d => d.CreatedFrom, s => s.Ignore())
                .ForMember(d => d.CreatedTo, s => s.Ignore());
        }

        public void MapMembershipPlansAndCards()
        {
            CreateMap<dto.MembershipCard, db.MembershipCard>();

            CreateMap<db.MembershipCard, dto.MembershipCard>()
                .ForMember(d => d.AgentCode, s => s.Ignore());

            CreateMap<db.MembershipPlan, dto.MembershipPlan>();

            CreateMap<dto.MembershipPlan, db.MembershipPlan>()
                .ForMember(d => d.WhiteLabel, s => s.Ignore())
                .ForMember(d => d.SiteClan, s => s.Ignore());

            CreateMap<dto.MembershipCardAffiliateReference, db.MembershipCardAffiliateReference>();

            CreateMap<db.MembershipPlanBenefits, dto.MembershipPlanBenefits>()
                .ForMember(d => d.MembershipPlan, s => s.Ignore());

            CreateMap<db.MembershipCardAffiliateReference, dto.MembershipCardAffiliateReference>();
            CreateMap<db.MembershipRegistrationCode, dto.MembershipRegistrationCode>();
            CreateMap<db.MembershipPendingToken, dto.MembershipPendingToken>();

            CreateMap<db.Partner, dto.PartnerDto>()
                .ForMember(d => d.MembershipCardProviders, s => s.Ignore());
            CreateMap<dto.PartnerDto, db.Partner>()
                .ForMember(d => d.IdentityUser, s => s.Ignore());
            CreateMap<db.AgentCode, dto.AgentCode>().ReverseMap();
            CreateMap<db.RegistrationCodeSummary, dto.RegistrationCodeSummary>()
                .ForMember(d => d.BlobConnectionString, s => s.Ignore())
                .ForMember(d => d.ContainerName, s => s.Ignore())
                .ForMember(d => d.NumberOfUses, s => s.Ignore());
            CreateMap<dto.RegistrationCodeSummary, db.RegistrationCodeSummary>();
        }

        public void MapMarketing()
        {
            CreateMap<dto.WhiteLabelSettings, db.WhiteLabelSettings>();
            CreateMap<db.WhiteLabelSettings, dto.WhiteLabelSettings>();

            CreateMap<dto.MarketingCampaign, db.MarketingCampaign>();
            CreateMap<db.MarketingCampaign, dto.MarketingCampaign>()
                .ForMember(m => m.WhiteLabel, s => s.MapFrom(f => f.WhiteLabelSettings));

            CreateMap<dto.MarketingContactList, db.MarketingContactList>();
            CreateMap<db.MarketingContactList, dto.MarketingContactList>();

            //CreateMap<dto.NewsletterCampaignLink, db.NewsletterCampaignLink>();
            //CreateMap<db.NewsletterCampaignLink, dto.NewsletterCampaignLink>()
            //    .ForMember(m => m.WhiteLabelName, s => s.MapFrom(f => f.MarketingCampaign.WhiteLabelSettings.Name));

            CreateMap<db.MarketingContact, Providers.Marketing.Models.MarketingContactId>()
                .ForMember(m => m.Id, s => s.MapFrom(f => f.ContactReference));

            CreateMap<db.Customer, Providers.Marketing.Models.MarketingContact>()
                .ForMember(m => m.Email, s => s.MapFrom(f => f.ContactDetail.EmailAddress))
                .ForMember(m => m.FirstName, s => s.MapFrom(f => f.Forename))
                .ForMember(m => m.LastName, s => s.MapFrom(f => f.Surname))
                .ForMember(m => m.Address1, s => s.Ignore())
                .ForMember(m => m.Address2, s => s.Ignore())
                .ForMember(m => m.City, s => s.Ignore())
                .ForMember(m => m.PostCode, s => s.Ignore());

            CreateMap<db.OfferListItem, dto.Admin.MarketingOfferSummary>()
                .ForMember(m => m.MerchantName, s => s.MapFrom(f => f.Offer.Merchant.Name))
                .ForMember(m => m.Heading, s => s.MapFrom(f => f.Offer.Headline))
                .ForMember(m => m.OfferShortDescription, s => s.MapFrom(f => f.Offer.ShortDescription))
                .ForMember(m => m.OfferLongDescription, s => s.MapFrom(f => f.Offer.LongDescription))
                .ForMember(m => m.OfferId, s => s.MapFrom(f => f.Offer.Id))
                .ForMember(m => m.ImagePath, s => s.MapFrom(f => f.Offer.Merchant.MerchantImages.FirstOrDefault().ImagePath))
                .ForMember(m => m.MerchantId, s => s.MapFrom(f => f.Offer.Merchant.MerchantImages.FirstOrDefault().MerchantId))
                .ForMember(m => m.CountryCode, s => s.MapFrom(f => f.CountryCode));

            CreateMap<Providers.Marketing.Models.MarketingEventEmailConfig, dto.Email>()
                .ForMember(m => m.BodyPlainText, s => s.MapFrom(f => f.PlainContent))
                .ForMember(m => m.BodyHtml, s => s.MapFrom(f => f.HtmlContent))
                .ForMember(m => m.Subject, s => s.MapFrom(f => f.Subject))
                .ForMember(m => m.EmailTo, s => s.Ignore())
                .ForMember(m => m.EmailFromName, s => s.Ignore())
                .ForMember(m => m.EmailFrom, s => s.Ignore())
                .ForMember(m => m.EmailCC, s => s.Ignore())
                .ForMember(m => m.EmailBCC, s => s.Ignore());

            CreateMap<Providers.Marketing.Models.SearchResult, Providers.Marketing.Models.MarketingContactId>();
        }
    }
}