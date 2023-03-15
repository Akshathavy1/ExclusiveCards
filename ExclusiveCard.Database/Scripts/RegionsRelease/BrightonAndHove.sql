DECLARE @Name nvarchar(max)
DECLARE @DisplayName nvarchar(max)
DECLARE @URL nvarchar(max)
DECLARE @Slug nvarchar(max)
DECLARE @CompanyNumber nvarchar(max)
DECLARE @CSSFile nvarchar(max)
DECLARE @Logo nvarchar(max)
DECLARE @NewsletterLogo nvarchar(max)
DECLARE @ClaimsEmail nvarchar(max)
DECLARE @HelpEmail nvarchar(max)
DECLARE @MainEmail nvarchar(max)
DECLARE @Address nvarchar(max)
DECLARE @CardName nvarchar(max)
DECLARE @CharityName nvarchar(max)
DECLARE @CharityUrl nvarchar(max)
DECLARE @Privicy nvarchar(512)
DECLARE @Terms nvarchar(512)
DECLARE @NewWhiteLabelId int
DECLARE @Twitter int
DECLARE @FaceBook int
DECLARE @Instagram int
DECLARE @Pinterest int
DECLARE @LinkedIn int
DECLARE @YouTube int

DECLARE @IsBeneficiary int = 0 --<<Set this to 1 if the white label has beneficiary accounts
DECLARE @PlanType int = 4 --<< 4 = Standard Partner Rewards, 3 = Beneficiary Rewards
DECLARE @PlanText nvarchar(max) = ' Standard Rewards Plan' --<< 4 = ' Standard Rewards Plan', 3 = ' Beneficiary Rewards Plan'

-- Edit these values for beneficiary plans
Declare @customerCashbackPercentage INT = 81 --<< 56 = standard beneficiary reward, 81 = standard partner reward
Declare @deductionPercentage INT = 19
Declare @beneficiaryPercentage INT = 0 --<< 25 = standard beneficiary reward, 0 = standard partner reward
-- End editing

DECLARE @DiamondNeeded bit = 1 --<< 1 = add diamond plan, 0 = don't add diamond plan
DECLARE @DiamondPlanText nvarchar(max) = ' Standard Rewards Diamond Upgrade Plan' --<< 4 = ' Standard Rewards Diamond Upgrade Plan', 3 = ' Beneficiary Rewards Diamond Upgrade Plan'


--## This is a Regions site !!! ##
DECLARE @IsRegional int = 1 --<<Set this to 1 if this is a regions site with local offers

SELECT @Name ='Brighton & Hove Rewards'
SELECT @DisplayName = 'Brighton & Hove Rewards'
SELECT @URL = 'https://brightonandhove.exclusiverewards.co.uk' --<< LIVE
SELECT @Slug = 'brightonandhove'
SELECT @CompanyNumber ='11616720'
SELECT @CSSFile = 'brightonandhove.css'
SELECT @Logo = 'logo.svg' 
SELECT @NewsletterLogo = 'logo_newsletter.png'
SELECT @ClaimsEmail ='claims@exclusiverewards.co.uk'
SELECT @HelpEmail ='help-me@exclusiverewards.co.uk'
SELECT @MainEmail ='enquiries@exclusiverewards.co.uk'
SELECT @Address = '15 Hoghton Street, Southport, United Kingdom, PR9 0NS'
SELECT @CardName ='Exclusive Rewards'
SELECT @Privicy = '/Account/PrivacyPolicy?country=GB'
SELECT @Terms = '/Account/Terms?country=GB'

Declare @cardProvider Varchar(max) = @Name --'Card Provider'
Declare @registrationCode Varchar(max) = 'BHSRC01' --'RegCode'
Declare @DiamondRegistrationCode Varchar(max) = 'BHDRC01' --'RegCode'


--Diamond level values
Declare @DiamondCustomerCashbackPercentage INT = 90
Declare @DiamondDeductionPercentage INT = 10
Declare @DiamondBeneficiaryPercentage INT = 0

SELECT @Twitter = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Twitter'
SELECT @FaceBook = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Facebook'
SELECT @Instagram = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Instagram'
SELECT @Pinterest = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Pinterest'
SELECT @LinkedIn = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'LinkedIn'
SELECT @YouTube = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'YouTube'

BEGIN TRAN

--## ##

  INSERT INTO [CMS].[WhiteLabelSettings]
  ([Name] ,[DisplayName] ,[URL] ,[Slug]
  ,[CompanyNumber],[CSSFile],[Logo],[ClaimsEmail]
  ,[HelpEmail],[MainEmail],[Address],[CardName]
  ,[PrivacyPolicy],[Terms],[NewsletterLogo]
  ,[CharityName],[CharityUrl], [SiteType], [IsRegional])
  VALUES
  (@Name, @DisplayName,@URL,@Slug
  ,@CompanyNumber,@CSSFile,@Logo,@ClaimsEmail
  ,@HelpEmail,@MainEmail,@Address,@CardName
  ,@Privicy,@Terms,@NewsletterLogo
  ,@CharityName,@CharityUrl, @IsBeneficiary, @IsRegional)
  
  SELECT @NewWhiteLabelId = SCOPE_IDENTITY()

IF NOT EXISTS
(SELECT [Id] FROM [CMS].[WebsiteSocialMediaLink] WHERE [WhiteLabelSettingsId] = @NewWhiteLabelId)
BEGIN
	INSERT [CMS].[WebsiteSocialMediaLink]
	([CountryCode],[SocialMediaCompanyId],[SocialMediaURI],[WhiteLabelSettingsId])
	VALUES
	 ('GB', @Instagram,'https://www.instagram.com/exclusiveprivilegecard/', @NewWhiteLabelId),
	 ('GB', @FaceBook,'https://www.facebook.com/exclusiveprivilegecard/', @NewWhiteLabelId),
	 ('GB', @Twitter,'https://twitter.com/exclusivecard_', @NewWhiteLabelId)
END

--## Regions local offer lists ##
IF @IsRegional = 1 AND NOT EXISTS(Select Id FROM [CMS].[OfferList] WHERE [WhitelabelId] = @NewWhiteLabelId)
BEGIN
  INSERT [CMS].[OfferList]
  ([ListName],[Description],[MaxSize],[IsActive],[IncludeShowAllLink],[ShowAllLinkCaption],[PermissionLevel],[WhitelabelId],[DisplayType])
  VALUES
  (@Name + ' featured locals', 'Featured local offer list',0,1,1,'Show All',0,@NewWhiteLabelId,2),
  (@Name + ' main locals', 'Main body local offer list',0,1,1,'Show All',0,@NewWhiteLabelId,1)
END

--##Add to Newsletter

DECLARE @NewsletterId INT
SELECT @NewsletterId = Id FROM [Marketing].[Newsletter]

-- Add Campaign Data
DECLARE @CampaignId int

  IF NOT EXISTS(Select Id FROM [Marketing].[Campaigns] WHERE [WhiteLabelId] = @NewWhiteLabelId)
  BEGIN
    INSERT INTO [Marketing].[Campaigns]
    (
        [NewsletterId], [WhiteLabelId], [CampaignReference], [CampaignName], [SenderId], [Enabled]
    )
    VALUES
    (
       @NewsletterId, @NewWhiteLabelId, NULL, REPLACE(@Name, ' ', '') + '_Campaign', 1068141, 0
    )
    SELECT @CampaignId = SCOPE_IDENTITY()
  END
  ELSE
  BEGIN
        SELECT @CampaignId = Id FROM [Marketing].[Campaigns] WHERE [WhiteLabelId] = @NewWhiteLabelId
  END

--##

-- Add the records

  --Create Partner
  DECLARE @CardProviderId INT = NULL
  SELECT @CardProviderId = [Id] FROM Exclusive.Partner WHERE [Name] = @cardProvider AND [IsDeleted] = 0 AND [Type] = 1

  IF @CardProviderId IS NULL
  BEGIN
	INSERT Exclusive.Partner (name, IsDeleted, Type ) Values (@cardProvider, 0, 1)
	SELECT @CardProviderId = SCOPE_IDENTITY()
  END

	-- Add Standard Plan if needed
	IF NOT EXISTS( SELECT Id FROM [Exclusive].[MembershipPlan] WHERE [Description] = @CardProvider + @PlanText)
	BEGIN

	  DECLARE @PlanID INT = null
	  INSERT [Exclusive].[MembershipPlan] ([PartnerId], [MembershipPlanTypeId], [Duration] ,[NumberOfCards]  ,[ValidFrom]  ,[ValidTo]   ,[CustomerCardPrice]  ,[PartnerCardPrice]
		  ,[CustomerCashbackPercentage]  ,[DeductionPercentage]  ,[IsDeleted]  ,[CurrencyCode]   ,[Description]   ,[IsActive]  ,[MembershipLevelId]   ,[PaidByEmployer]   ,[MinimumValue]
		  ,[PaymentFee]   ,[CardProviderId], [BenefactorPercentage],[WhitelabelId] )

	  VALUES (1, @PlanType, 36500, 100000000, GetDate(), GetDate() + 36500, 0, 0, @customerCashbackPercentage, @deductionPercentage, 0, 'GBP', @CardProvider + @PlanText, 1, 1, 0, 10, 1, @CardProviderId, @beneficiaryPercentage, @NewWhiteLabelId)
	  SELECT @planId = SCOPE_IDENTITY()

	  INSERT Exclusive.MembershipRegistrationCode (MembershipPlanId,[RegistartionCode] , ValidFrom, ValidTo, NumberOfCards, IsActive, IsDeleted)
	  VALUES (@PlanID, @registrationCode, GetDate(),  GetDate() + 36500, 100000000, 1, 0)

	  INSERT Exclusive.MembershipPlanPaymentProvider (MEmbershipPlanId, PaymentProviderId, OneOffPaymentRef)
	  VALUES (@PlanId, 2, 'ZRFM9WT7LQE9A')

  END

	-- Add Diamond Plan if needed
	IF @DiamondNeeded =1 AND NOT EXISTS( SELECT Id FROM [Exclusive].[MembershipPlan] WHERE [Description] = @CardProvider + @DiamondPlanText)
	BEGIN

		DECLARE @DiamondPlanID INT = null
		INSERT [Exclusive].[MembershipPlan] ([PartnerId], [MembershipPlanTypeId], [Duration] ,[NumberOfCards]  ,[ValidFrom]  ,[ValidTo]   ,[CustomerCardPrice]  ,[PartnerCardPrice]
			,[CustomerCashbackPercentage]  ,[DeductionPercentage]  ,[IsDeleted]  ,[CurrencyCode]   ,[Description]   ,[IsActive]  ,[MembershipLevelId]   ,[PaidByEmployer]   ,[MinimumValue]
			,[PaymentFee]   ,[CardProviderId], [BenefactorPercentage],[WhitelabelId] )
		VALUES (1, 4, 365, 100000000, GetDate(), GetDate() + 36500, 0, 0, @DiamondCustomerCashbackPercentage, @DiamondDeductionPercentage, 0, 'GBP', @CardProvider + @DiamondPlanText, 1, 2, 0, 10, 1, @CardProviderId, @DiamondBeneficiaryPercentage, @NewWhiteLabelId)
		SELECT @DiamondPlanID = SCOPE_IDENTITY()

		INSERT Exclusive.MembershipRegistrationCode (MembershipPlanId,[RegistartionCode] , ValidFrom, ValidTo, NumberOfCards, IsActive, IsDeleted)
		VALUES (@DiamondPlanID, @DiamondRegistrationCode, GetDate(), '31 Dec 2120', 100000000, 1, 0)

		INSERT Exclusive.MembershipPlanPaymentProvider (MembershipPlanId, PaymentProviderId, OneOffPaymentRef)
		VALUES (@DiamondPlanID, 2, 'ZRFM9WT7LQE9A')
		SELECT 'New diamond standard plan added'
	END

-- END of Add

-- Check the results
SELECT * 
FROM Exclusive.Partner 
order by id desc

Select * 
from [Exclusive].[MembershipPlan] MP
where isACtive = 1
order by id desc

Select  P.Description, C.* 
from [Exclusive].[MembershipRegistrationCode] C
inner join Exclusive.MembershipPlan P ON C.MembershipPlanId = P.Id
where p.IsActive = 1
order by p.id desc

Select P.Description,   PP.* 
from [Exclusive].[MembershipPlanPaymentProvider] PP
inner join Exclusive.MembershipPlan P ON PP.MembershipPlanId = P.Id
where p.IsActive = 1
order by p.id desc

SELECT * FROM [Marketing].[Campaigns] ORDER BY ID DESC

SELECT * FROM [CMS].[WhiteLabelSettings] ORDER BY ID DESC
SELECT * FROM [CMS].[WebsiteSocialMediaLink] ORDER BY ID DESC

SELECT * FROM [CMS].[OfferList] ORDER BY ID DESC

--COMMIT TRAN

--ROLLBACK TRAN
