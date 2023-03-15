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
--DECLARE @DiamondPayPalReferenceCode nvarchar(max) = 'YJHLQGJU2QH3Y' -- NULL 

--DECLARE @DiamondNeeded bit = 1 --<< 1 = add diamond plan, 0 = don't add diamond plan
--DECLARE @DiamondCardPrice decimal(18,2) = 19.99 --0.00
--DECLARE @PayedByEmployer bit = CASE WHEN @DiamondCardPrice = 0.00 THEN 1 ELSE 0 END

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

SELECT @Name ='Fitness Rewards'
SELECT @DisplayName = 'Fitness Rewards'
SELECT @URL = 'https://fitness.exclusiverewards.co.uk' --<< LIVE
SELECT @Slug = 'fitness-rewards'
SELECT @CompanyNumber ='11616720'
SELECT @CSSFile = 'fitness-rewards.css'
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
--Declare @registrationCode Varchar(max) = 'GLFSPR01' --'RegCode'
--Declare @DiamondRegistrationCode Varchar(max) = 'MTRDPR01' --'RegCode'

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
--IF NOT EXISTS
--(SELECT [Id] FROM [CMS].[WebsiteSocialMediaLink] WHERE [WhiteLabelSettingsId] = @NewWhiteLabelId)
--BEGIN
--	INSERT [CMS].[WebsiteSocialMediaLink]
--	([CountryCode],[SocialMediaCompanyId],[SocialMediaURI],[WhiteLabelSettingsId])
--	VALUES
--	 ('GB', @Instagram,'https://www.instagram.com/theexceedgroup/?hl=en', @NewWhiteLabelId),
--	 ('GB', @FaceBook,'https://www.facebook.com/theexceedgroup/', @NewWhiteLabelId),
--	 ('GB', @Twitter,'https://twitter.com/theexceedgroup', @NewWhiteLabelId),
--	 ('GB', @LinkedIn,'https://www.linkedin.com/company/the-exceed-group/', @NewWhiteLabelId)
--END

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


-- Check the results

SELECT * FROM [Marketing].[Campaigns] ORDER BY ID DESC

SELECT * FROM [CMS].[WhiteLabelSettings] ORDER BY ID DESC
SELECT * FROM [CMS].[WebsiteSocialMediaLink] ORDER BY ID DESC

SELECT * FROM [CMS].[OfferList] ORDER BY ID DESC

--COMMIT TRAN

--ROLLBACK TRAN
