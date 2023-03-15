﻿DECLARE @Name nvarchar(max)
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
DECLARE @DefaultRegion int

DECLARE @IsBeneficiary int = 0 --<<Set this to 1 if the white label has beneficiary accounts

DECLARE @PlanType int = 4 --<< 4 = Standard Partner Rewards, 3 = Beneficiary Rewards

-- Plan Cashback %
Declare @customerCashbackPercentage INT
Declare @deductionPercentage INT
Declare @beneficiaryPercentage INT
DECLARE @PlanText nvarchar(max)
Declare @DiamondCustomerCashbackPercentage INT
Declare @DiamondDeductionPercentage INT
Declare @DiamondBeneficiaryPercentage INT
DECLARE @DiamondPlanText nvarchar(max)
-- Add your PayPal Button Reference Code here:
-- When updating a standard plan, if the diamond plan didin't have a value here, then the upgrade button will be disabled/hidden
-- Exclusive rewards Live PayPal Button Reference = 'YJHLQGJU2QH3Y' (You probably need to create a new button for a white label though)
DECLARE @DiamondPayPalReferenceCode nvarchar(max) = 'YJHLQGJU2QH3Y' -- NULL 

DECLARE @DiamondNeeded bit = 1 --<< 1 = add diamond plan, 0 = don't add diamond plan
DECLARE @DiamondCardPrice decimal(18,2) = 0.00 --19.99 --0.00
DECLARE @PayedByEmployer bit = CASE WHEN @DiamondCardPrice = 0.00 THEN 1 ELSE 0 END
DECLARE @EmployerCardPrice decimal(18,2) = CASE WHEN @DiamondCardPrice = 0.00 THEN 19.99 ELSE 0.00 END
IF @PlanType = 3
BEGIN
	SELECT @customerCashbackPercentage = 55 --<< 55 = standard beneficiary reward
	SELECT @deductionPercentage = 20 -- << Always 20
	SELECT @beneficiaryPercentage = 25 --<< 25 = standard beneficiary reward
	SELECT @DiamondCustomerCashbackPercentage = 55 --<< 55 = standard beneficiary reward
	SELECT @DiamondDeductionPercentage = 20 -- << Always 20
	SELECT @DiamondBeneficiaryPercentage = 25 --<< 25 = standard beneficiary reward
	SELECT @PlanText = ' Beneficiary Rewards Plan' --<< 3 = ' Beneficiary Rewards Plan'
	SELECT @DiamondPlanText = ' Beneficiary Rewards Diamond Upgrade Plan' --<< 3 = ' Beneficiary Rewards Diamond Upgrade Plan'
	SELECT @CharityName = ''
	SELECT @CharityUrl = ''
END
ELSE
BEGIN
	SELECT @customerCashbackPercentage = 80 --<< 55 = standard beneficiary reward, 80 = standard partner reward
	SELECT @deductionPercentage = 20 -- << Always 20
	SELECT @beneficiaryPercentage = 0 --<< 25 = standard beneficiary reward, 0 = standard partner reward
	SELECT @DiamondCustomerCashbackPercentage = 80 --<< 55 = standard beneficiary reward, 80 = standard partner reward
	SELECT @DiamondDeductionPercentage = 20 -- << Always 20
	SELECT @DiamondBeneficiaryPercentage = 0 --<< 25 = standard beneficiary reward, 0 = standard partner reward
	SELECT @PlanText = ' Standard Rewards Plan' --<< 4 = ' Standard Rewards Plan'
	SELECT @DiamondPlanText = ' Standard Rewards Diamond Upgrade Plan' --<< 4 = ' Standard Rewards Diamond Upgrade Plan'
END

--## This is NOT a Regions site !!! ##
DECLARE @IsRegional int = 0 --<< Set this to 1 if this is a regions site with local offers

SELECT @Name ='Action Coach Rewards'
SELECT @DisplayName = 'Action Coach Rewards'
SELECT @URL = 'https://actioncoach.exclusiverewards.co.uk' --<< LIVE
SELECT @Slug = 'action-coach'
SELECT @CompanyNumber ='11616720'
SELECT @CSSFile = 'action-coach.css'
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
--Declare @registrationCode Varchar(max) = 'ACOSPR01' --'RegCode'
Declare @DiamondRegistrationCode Varchar(max) = 'ACSDPR01' --'RegCode'

SELECT @Twitter = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Twitter'
SELECT @FaceBook = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Facebook'
SELECT @Instagram = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Instagram'
SELECT @Pinterest = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Pinterest'
SELECT @LinkedIn = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'LinkedIn'
SELECT @YouTube = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'YouTube'

BEGIN TRAN

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

IF @IsRegional = 1
BEGIN
	-- Set the default region to the new record id for regional sites
	SELECT @DefaultRegion = @NewWhiteLabelId
END
ELSE
BEGIN
	-- Set the default region to southport for non-region sites
	SELECT @DefaultRegion = Id FROM [CMS].[WhiteLabelSettings] WHERE [Name] = 'Southport Rewards'
END
UPDATE [CMS].[WhiteLabelSettings] SET [DefaultRegion] = @DefaultRegion WHERE Id = @NewWhiteLabelId

--## UPDATE THIS TO CHANGE SOCIAL MEDIA URL LINKS ##
IF NOT EXISTS
(SELECT [Id] FROM [CMS].[WebsiteSocialMediaLink] WHERE [WhiteLabelSettingsId] = @NewWhiteLabelId)
BEGIN
	INSERT [CMS].[WebsiteSocialMediaLink]
	([CountryCode],[SocialMediaCompanyId],[SocialMediaURI],[WhiteLabelSettingsId])
	VALUES
	 ('GB', @YouTube,'https://www.youtube.com/user/actioncoachtv', @NewWhiteLabelId),
	 ('GB', @FaceBook,'https://www.facebook.com/ActionCOACHBusinessCoaching', @NewWhiteLabelId),
	 ('GB', @Twitter,'https://twitter.com/actioncoach?lang=en', @NewWhiteLabelId),
	 ('GB', @LinkedIn,'https://www.linkedin.com/company/actioncoach/', @NewWhiteLabelId)
	 
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
	  DECLARE @StandardRegCodeSummaryID INT = null

	  INSERT [Exclusive].[MembershipPlan] ([PartnerId], [MembershipPlanTypeId], [Duration] ,[NumberOfCards]  ,[ValidFrom]  ,[ValidTo]   ,[CustomerCardPrice]  ,[PartnerCardPrice]
		  ,[CustomerCashbackPercentage]  ,[DeductionPercentage]  ,[IsDeleted]  ,[CurrencyCode]   ,[Description]   ,[IsActive]  ,[MembershipLevelId]   ,[PaidByEmployer]   ,[MinimumValue]
		  ,[PaymentFee]   ,[CardProviderId], [BenefactorPercentage],[WhitelabelId] )

	  VALUES (1, @PlanType, 36500, 100000000, GETUTCDATE(), GETUTCDATE() + 36500, 0, 0, @customerCashbackPercentage, @deductionPercentage, 0, 'GBP', @CardProvider + @PlanText, 1, 1, 0, 10, 1, @CardProviderId, @beneficiaryPercentage, @NewWhiteLabelId)
	  SELECT @planId = SCOPE_IDENTITY()

	  ---- ## Add Registration code summary record
	  --INSERT [Exclusive].[RegistrationCodeSummary]
	  --([MembershipPlanId], [ValidFrom], [ValidTo], [NumberOfCodes])
	  --VALUES
	  --(@PlanId, GETUTCDATE(),  GETUTCDATE() + 36500, 1)
	  --SELECT @StandardRegCodeSummaryID = SCOPE_IDENTITY()
	  ---- ## Add Registration code summary record
	  --INSERT Exclusive.MembershipRegistrationCode 
	  --(MembershipPlanId,[RegistartionCode] , ValidFrom, ValidTo, NumberOfCards, IsActive, IsDeleted, [RegistrationCodeSummaryId])
	  --VALUES (@PlanID, @registrationCode, GETUTCDATE(),  GETUTCDATE() + 36500, 100000000, 1, 0, @StandardRegCodeSummaryID)

	  INSERT Exclusive.MembershipPlanPaymentProvider (MembershipPlanId, PaymentProviderId, OneOffPaymentRef)
	  VALUES (@PlanId, 2, 'ZRFM9WT7LQE9A')

  END

	-- Add Diamond Plan if needed
	IF @DiamondNeeded = 1 AND NOT EXISTS( SELECT Id FROM [Exclusive].[MembershipPlan] WHERE [Description] = @CardProvider + @DiamondPlanText)
	BEGIN

		DECLARE @DiamondPlanID INT = null
		DECLARE @DiamondRegCodeSummaryID INT = null

		INSERT [Exclusive].[MembershipPlan] 
		([PartnerId], [MembershipPlanTypeId], [Duration] 
		,[NumberOfCards]  ,[ValidFrom]  ,[ValidTo]   
		,[CustomerCardPrice]  ,[PartnerCardPrice],[CustomerCashbackPercentage]  
		,[DeductionPercentage]  ,[IsDeleted]  ,[CurrencyCode]
		,[Description]   ,[IsActive]  ,[MembershipLevelId]
		,[PaidByEmployer],[MinimumValue],[PaymentFee]
		,[CardProviderId], [BenefactorPercentage],[WhitelabelId] )
		VALUES 
		(1, @PlanType, 365
		, 300, GETUTCDATE(), GETUTCDATE() + 365
		, @DiamondCardPrice, @EmployerCardPrice, @DiamondCustomerCashbackPercentage
		, @DiamondDeductionPercentage, 0, 'GBP'
		, @CardProvider + @DiamondPlanText, 1, 2
		, @PayedByEmployer, 10, 1
		, @CardProviderId, @DiamondBeneficiaryPercentage, @NewWhiteLabelId)
		SELECT @DiamondPlanID = SCOPE_IDENTITY()

	    ---- ## Add Registration code summary record
	    --INSERT [Exclusive].[RegistrationCodeSummary]
	    --([MembershipPlanId], [ValidFrom], [ValidTo], [NumberOfCodes])
	    --VALUES
	    --(@PlanId, GETUTCDATE(),  GETUTCDATE() + 365, 1)
	    --SELECT @DiamondRegCodeSummaryID = SCOPE_IDENTITY()
	    ---- ## Add Registration code summary record
		--INSERT Exclusive.MembershipRegistrationCode 
		--(MembershipPlanId,[RegistartionCode] , ValidFrom, ValidTo, NumberOfCards, IsActive, IsDeleted, [RegistrationCodeSummaryId])
		--VALUES (@DiamondPlanID, @DiamondRegistrationCode, GETUTCDATE(), GETUTCDATE() + 365, 100000000, 1, 0, @DiamondRegCodeSummaryID)

		--## OLD method, pre admin plan management ##
		INSERT Exclusive.MembershipRegistrationCode 
		(MembershipPlanId, [RegistartionCode] , ValidFrom, ValidTo, NumberOfCards, IsActive, IsDeleted)
		VALUES (@DiamondPlanID, @DiamondRegistrationCode, GETUTCDATE(), GETUTCDATE() + 365, 300, 1, 0)
		--## OLD method, pre admin plan management ##

		--Create MembershipPlanPaymentProvider only if this is not a pre-payed upgrade
		IF @PayedByEmployer = 0
		BEGIN
			INSERT Exclusive.MembershipPlanPaymentProvider (MembershipPlanId, PaymentProviderId, OneOffPaymentRef, SubscribeAppRef)
			VALUES (@DiamondPlanID, 2, 'ZRFM9WT7LQE9A', @DiamondPayPalReferenceCode)
		END

		SELECT 'New diamond plan added'
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

---- ## Registration code summary records
--SELECT P.Description, RCS.* 
--FROM [Exclusive].[RegistrationCodeSummary] RCS
--INNER JOIN [Exclusive].[MembershipPlan] P
--ON RCS.MembershipPlanId = P.Id
--order by P.id desc
---- ## Registration code summary records

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