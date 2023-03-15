using Microsoft.EntityFrameworkCore;
using ExclusiveCard.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ST = ExclusiveCard.Data.StagingModels;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ExclusiveCard.Data.Context
{
    public class ExclusiveContext : IdentityDbContext<ExclusiveUser>
    {
        public ExclusiveContext(DbContextOptions<ExclusiveContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationBuilder builder = new ConfigurationBuilder();
                builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

                var root = builder.Build();
                var connectionString = root.GetConnectionString("exclusive");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<Merchant> Merchant { get; set; }

        public DbSet<ContactDetail> ContactDetail { get; set; }

        public DbSet<MerchantBranch> MerchantBranch { get; set; }

        public DbSet<MerchantImage> MerchantImage { get; set; }

        public DbSet<MerchantSocialMediaLink> MerchantSocialMediaLink { get; set; }

        public DbSet<SocialMediaCompany> SocialMediaCompany { get; set; }

        public DbSet<Offer> Offer { get; set; }

        public DbSet<OfferCategory> OfferCategory { get; set; }

        public DbSet<OfferCountry> OfferCountry { get; set; }

        public DbSet<OfferMerchantBranch> OfferMerchantBranch { get; set; }

        public DbSet<OfferTag> OfferTag { get; set; }

        public DbSet<OfferType> OfferType { get; set; }

        public DbSet<Status> Status { get; set; }

        public DbSet<Tag> Tag { get; set; }

        public DbSet<OfferListItem> OfferListItem { get; set; }

        public DbSet<OfferList> OfferList { get; set; }

        public DbSet<Category> Category { get; set; }

        //public DbSet<User> User { get; set; }
        public DbSet<Localisation> Localisation { get; set; }

        public DbSet<BankDetail> BankDetail { get; set; }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<LoginUserToken> LoginUserToken { get; set; }

        public DbSet<CustomerBankDetail> CustomerBankDetail { get; set; }

        public DbSet<MembershipCard> MembershipCard { get; set; }

        public DbSet<MembershipPlan> MembershipPlan { get; set; }

        public DbSet<AgentCode> AgentCode { get; set; } 

        public DbSet<MembershipRegistrationCode> MembershipRegistrationCode { get; set; }
        public DbSet<RegistrationCodeSummary> RegistrationCodeSummary { get; set; }


        public DbSet<MembershipCardAffiliateReference> MembershipCardAffiliateReference { get; set; }

        public DbSet<MembershipPendingToken> MembershipPendingToken { get; set; }

        public DbSet<Partner> Partner { get; set; }

        public DbSet<SecurityQuestion> SecurityQuestion { get; set; }

        public DbSet<CustomerSecurityQuestion> CustomerSecurityQuestion { get; set; }

        public DbSet<Affiliate> Affiliate { get; set; }

        public DbSet<AffiliateFile> AffiliateFile { get; set; }

        public DbSet<AffiliateFileMapping> AffiliateFileMapping { get; set; }

        public DbSet<AffiliateMapping> AffiliateMapping { get; set; }

        public DbSet<AffiliateMappingRule> AffiliateMappingRule { get; set; }

        public DbSet<AffiliateFieldMapping> AffiliateFieldMapping { get; set; }

        public DbSet<MigrationMapping> MigrationMapping { get; set; }

        public DbSet<CashbackPayout> CashbackPayout { get; set; }

        public DbSet<CashbackSummary> CashbackSummary { get; set; }

        public DbSet<CashbackTransaction> CashbackTransaction { get; set; }

        public DbSet<ClickTracking> ClickTracking { get; set; }

        public DbSet<CustomerPayment> CustomerPayment { get; set; }

        public DbSet<PaymentNotification> PaymentNotification { get; set; }

        public DbSet<PaymentProvider> PaymentProvider { get; set; }

        public DbSet<MembershipPlanPaymentProvider> MembershipPlanPaymentProvider { get; set; }

        public DbSet<PayPalSubscription> PayPalSubscription { get; set; }

        public DbSet<MembershipPlanType> MembershipPlanType { get; set; }

        public DbSet<EmailTemplate> EmailTemplate { get; set; }

        public DbSet<EmailsSent> EmailsSent { get; set; }

        public DbSet<Files> Files { get; set; }

        public DbSet<PartnerRewards> PartnerRewards { get; set; }

        public DbSet<MembershipPlanBenefits> MembershipPlanBenefits { get; set; }

        public DbSet<TermsConditions> TermsConditions { get; set; }

        public DbSet<PartnerRewardWithdrawal> PartnerRewardWithdrawal { get; set; }

        public DbSet<SequenceNumbers> SequenceNumbers { get; set; }

        public DbSet<MembershipLevel> MembershipLevel { get; set; }

        public DbSet<OfferRedemption> OfferRedemption { get; set; }

        public DbSet<SSOConfiguration> SSOConfiguration { get; set; }

        // STAGING TABLES

        //public DbSet<ST.Offer> StagingOffer { get; set; }
        //public DbSet<ST.OfferCategory> StagingOfferCategory { get; set; }
        //public DbSet<ST.OfferCountry> StagingOfferCountry { get; set; }
        public DbSet<ST.OfferImportAwin> StagingOfferImportAwin { get; set; }

        public DbSet<ST.OfferImportFile> StagingOfferImportFile { get; set; }

        public DbSet<ST.OfferImportError> StagingOfferImportError { get; set; }

        public DbSet<ST.TransactionFile> StagingTransactionFile { get; set; }

        public DbSet<ST.CashbackTransaction> StagingCashbackTransaction { get; set; }

        public DbSet<ST.CashbackTransactionError> CashbackTransactionError { get; set; }

        public DbSet<ErrorLog> ErrorLogs { get; set; }

        public DbSet<ST.CustomerRegistration> StagingCustomerRegistration { get; set; }

        //CMS tables

        public DbSet<WebsiteSocialMediaLink> WebsiteSocialMediaLink { get; set; }

        public DbSet<WhiteLabelSettings> WhiteLabelSettings { get; set; }

        public DbSet<CategoryFeatureDetail> CategoryFeatureDetail { get; set; }

        public DbSet<SiteCategory> SiteCategory { get; set; }

        public DbSet<League> League { get; set; }

        public DbSet<SiteOwner> SiteOwner { get; set; }

        public DbSet<Charity> Charity { get; set; }

        public DbSet<SiteClan> SiteClan { get; set; }

        public DbSet<SponsorImages> SponsorImages { get; set; }

        //Marketing tables
        public DbSet<Newsletter> Newsletter { get; set; }

        public DbSet<MarketingCampaign> Campaigns { get; set; }

        //public DbSet<NewsletterCampaignLink> NewsletterCampaignLink { get; set; }
        public DbSet<MarketingContactList> SendGridContactList { get; set; }

        public DbSet<MarketingContact> SendGridContact { get; set; }

        public DbSet<EmailCustomField> EmailCustomField { get; set; }
        public DbSet<PlanAgentHistory> PlanAgentHistory { get; set; }

        //MigrationMapping

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OfferList>().ToTable("OfferList", schema: "CMS");
            modelBuilder.Entity<OfferListItem>().ToTable("OfferListItem", schema: "CMS");
            modelBuilder.Entity<Localisation>().ToTable("Localisation", schema: "CMS");

            modelBuilder.Entity<WebsiteSocialMediaLink>().ToTable("WebsiteSocialMediaLink", schema: "CMS");
            modelBuilder.Entity<WhiteLabelSettings>().ToTable("WhiteLabelSettings", "CMS");
            modelBuilder.Entity<SiteCategory>().ToTable("SiteCategory", "CMS");
            modelBuilder.Entity<League>().ToTable("League", "CMS");
            modelBuilder.Entity<SiteOwner>().ToTable("SiteOwner", "CMS");
            modelBuilder.Entity<Charity>().ToTable("Charity", "CMS");
            modelBuilder.Entity<SiteClan>().ToTable("SiteClan", "CMS");

            //modelBuilder.Entity<ST.Offer>().ToTable("Offer", schema: "Staging");
            //modelBuilder.Entity<ST.OfferCategory>().ToTable("OfferCategory", schema: "Staging");
            //modelBuilder.Entity<ST.OfferCountry>().ToTable("OfferCountry", schema: "Staging");
            modelBuilder.Entity<ST.OfferImportAwin>().ToTable("OfferImportAwin", schema: "Staging");
            modelBuilder.Entity<ST.OfferImportFile>().ToTable("OfferImportFile", schema: "Staging");
            modelBuilder.Entity<ST.TransactionFile>().ToTable("TransactionFile", schema: "Staging");
            modelBuilder.Entity<ST.CashbackTransaction>().ToTable("CashbackTransaction", schema: "Staging");
            modelBuilder.Entity<ST.CashbackTransactionError>().ToTable("CashbackTransactionErrors", schema: "Staging");

            modelBuilder.Entity<Newsletter>().ToTable("Newsletter", schema: "Marketing");
            modelBuilder.Entity<MarketingCampaign>().ToTable("Campaigns", schema: "Marketing");
            modelBuilder.Entity<MarketingContact>().ToTable("Contacts", "Marketing");
            modelBuilder.Entity<MarketingContactList>().ToTable("ContactLists", "Marketing");
            modelBuilder.Entity<EmailCustomField>().ToTable("EmailCustomFields", "Marketing");

            modelBuilder.HasDefaultSchema("Exclusive");

            modelBuilder.Entity<CashbackTransaction>(entity =>
            {
                entity.ToTable("CashbackTransaction", "Exclusive");

                entity.HasIndex(e => e.FileId);

                entity.HasIndex(e => e.PaymentStatusId);

                entity.Property(e => e.AccountType)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.AffiliateTransactionReference).HasMaxLength(50);

                entity.Property(e => e.CurrencyCode).HasMaxLength(3);

                entity.Property(e => e.Detail).HasMaxLength(70);

                entity.Property(e => e.Summary).HasMaxLength(30);
            });

            modelBuilder.Entity<Merchant>()
                .HasOne(m => m.ContactDetail)
                .WithMany(t => t.Merchants)
                .HasForeignKey(m => m.ContactDetailsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MerchantBranch>()
                .HasOne(m => m.ContactDetail)
                .WithMany(t => t.MerchantBranches)
                .HasForeignKey(m => m.ContactDetailsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OfferCategory>()
                .HasKey(m => new { m.OfferId, m.CategoryId });

            modelBuilder.Entity<OfferCategory>()
                .HasOne(m => m.Offer)
                .WithMany(t => t.OfferCategories)
                .HasForeignKey(m => m.OfferId);

            modelBuilder.Entity<OfferCategory>()
                .HasOne(m => m.Category)
                .WithMany(t => t.OfferCategories)
                .HasForeignKey(m => m.CategoryId);

            modelBuilder.Entity<OfferMerchantBranch>()
                .HasKey(m => new { m.OfferId, m.MerchantBranchId });

            modelBuilder.Entity<OfferTag>()
                .HasKey(m => new { m.OfferId, m.TagId });

            modelBuilder.Entity<OfferCountry>()
            .HasKey(m => new { m.OfferId, m.CountryCode });

            modelBuilder.Entity<OfferCountry>()
                .HasOne(m => m.Offer)
                .WithMany(t => t.OfferCountries)
                .HasForeignKey(m => m.OfferId);

            modelBuilder.Entity<OfferListItem>()
                .HasKey(m => new { m.OfferId, m.OfferListId, m.CountryCode });

            modelBuilder.Entity<CustomerBankDetail>()
                .HasKey(m => new { m.CustomerId, m.BankDetailsId });

            modelBuilder.Entity<MembershipCardAffiliateReference>()
                .HasKey(m => new { m.AffiliateId, m.MembershipCardId });

            modelBuilder.Entity<CustomerSecurityQuestion>()
                .HasKey(m => new { m.CustomerId, m.SecurityQuestionId });

            //modelBuilder.Entity<ST.OfferCountry>()
            //   .HasKey(m => new { m.OfferId, m.CountryCode });

            modelBuilder.Entity<MembershipCard>()
                .HasIndex(m => new
                {
                    m.CardNumber,
                    m.ValidFrom,
                    m.ValidTo
                })
                .IsUnique();

            modelBuilder.Entity<MembershipPlanPaymentProvider>()
               .HasKey(m => new { m.MembershipPlanId, m.PaymentProviderId });

            //modelBuilder.Entity<ST.OfferCategory>()
            //   .HasKey(m => new { m.OfferId, m.CategoryId });

            modelBuilder.Entity<Category>()
                .HasIndex(u => u.UrlSlug)
                .IsUnique();

            modelBuilder.Entity<Offer>()
                .HasIndex(p => new { p.AffiliateId, p.AffiliateReference }).IsUnique();

            modelBuilder.Entity<Customer>()
                .HasOne(m => m.IdentityUser)
                .WithOne(t => t.Customer)
                .HasForeignKey<Customer>(m => m.AspNetUserId);

            modelBuilder.Entity<Partner>()
                .HasOne(m => m.IdentityUser)
                .WithOne(t => t.Partner)
                .HasForeignKey<Partner>(m => m.AspNetUserId);

            modelBuilder.Entity<CategoryFeatureDetail>()
                .HasKey(m => new { m.CategoryId, m.CountryCode });

            modelBuilder.Entity<OfferRedemption>()
                .HasKey(k => new { k.MembershipCardId, k.OfferId });

            modelBuilder.Entity<MembershipPlan>()
                .HasOne(m => m.CardProvider)
                .WithMany(t => t.MembershipPlans)
                .HasForeignKey(m => m.CardProviderId)
                .HasForeignKey(a => a.AgentCodeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MarketingCampaign>()
                .HasOne(m => m.WhiteLabelSettings)
                .WithMany(t => t.Campaigns)
                .HasForeignKey(m => m.WhiteLabelId);

            modelBuilder.Entity<MarketingCampaign>()
                .HasOne(m => m.Newsletter);

            modelBuilder.Query<RedemptionDataModel>();

            modelBuilder.Entity<MembershipRegistrationCode>()
                .HasOne(m => m.RegistrationCodeSummary)
                .WithMany(t=>t.MembershipRegistrationCodes)
                .HasForeignKey(m => m.RegistrationCodeSummaryId);

        }
    }
}