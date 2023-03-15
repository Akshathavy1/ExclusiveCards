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

DECLARE @IsBeneficiary int = 1 --<<Set this to 1 if beneficiary (this needs re-work! After TalkSport is implemented, sites will have both account types)

SELECT @Name ='Stroke of Luck Rewards'
SELECT @DisplayName = 'Stroke of Luck'
SELECT @URL = 'https://strokeofluck.exclusiverewards.co.uk' --<< LIVE
SELECT @Slug = 'stroke-of-luck'
SELECT @CompanyNumber ='11616720'
SELECT @CSSFile = 'stroke-of-luck.css'
SELECT @Logo = 'logo.png' 
SELECT @NewsletterLogo = 'logo_newsletter.png'
SELECT @ClaimsEmail ='claims@exclusiverewards.co.uk'
SELECT @HelpEmail ='help-me@exclusiverewards.co.uk'
SELECT @MainEmail ='enquiries@exclusiverewards.co.uk'
SELECT @Address = 'A Stroke of Luck, 68 Argyle Street, Birkenhead, Merseyside CH41 6AF'
SELECT @CardName ='Exclusive Rewards'
SELECT @Privicy = '/Account/PrivacyPolicy?country=GB'
SELECT @Terms = '/Account/Terms?country=GB'
SELECT @CharityName = 'A Stroke of Luck'
SELECT @CharityUrl = 'https://www.astrokeofluck.co.uk/'
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
  ,[CharityName],[CharityUrl], [SiteType])
  VALUES
  (@Name, @DisplayName,@URL,@Slug
  ,@CompanyNumber,@CSSFile,@Logo,@ClaimsEmail
  ,@HelpEmail,@MainEmail,@Address,@CardName
  ,@Privicy,@Terms,@NewsletterLogo
  ,@CharityName,@CharityUrl, @IsBeneficiary)
  
  SELECT @NewWhiteLabelId = SCOPE_IDENTITY()

IF NOT EXISTS
(SELECT [Id] FROM [CMS].[WebsiteSocialMediaLink] WHERE [WhiteLabelSettingsId] = @NewWhiteLabelId)
BEGIN
	INSERT [CMS].[WebsiteSocialMediaLink]
	([CountryCode],[SocialMediaCompanyId],[SocialMediaURI],[WhiteLabelSettingsId])
	VALUES
	 ('GB', @Instagram,'https://www.instagram.com/astrokeofluckuk', @NewWhiteLabelId),
	 ('GB', @FaceBook,'https://www.facebook.com/AStrokeOfLuckUK', @NewWhiteLabelId),
	 ('GB', @Twitter,'https://twitter.com/astrokeofluckuk', @NewWhiteLabelId)
END

--##Add to Newsletter

DECLARE @NewsletterId INT
SELECT @NewsletterId = Id FROM [Marketing].[Newsletter]

-- Add Campaign Data
DECLARE @CampaignId int

  IF NOT EXISTS(Select * FROM [Marketing].[Campaigns] WHERE [WhiteLabelId] = @NewWhiteLabelId)
  BEGIN
    INSERT INTO [Marketing].[Campaigns]
    (
        [WhiteLabelId], [ContactListId], [ContactListName], [CampaignId], [CampaignName], [SenderId], [Enabled]
    )
    VALUES
    (
        @NewWhiteLabelId, NULL, REPLACE(@Name, ' ', '') + '_ContactList',
        NULL, REPLACE(@Name, ' ', '') + '_Campaign', 
        1068141, 1
    )
    SELECT @CampaignId = SCOPE_IDENTITY()
  END
  ELSE
  BEGIN
        SELECT @CampaignId = Id FROM [Marketing].[Campaigns] WHERE [WhiteLabelId] = @NewWhiteLabelId
  END


  -- ADD Data for NewsLetter / Campaign Link
  IF NOT EXISTS(Select * FROM [Marketing].[NewsletterCampaignLink] WHERE [CampaignId] = @CampaignId)
  BEGIN
        INSERT INTO [Marketing].[NewsletterCampaignLink]
        (
            [NewsletterId], [CampaignId], [Enabled]
        )
        VALUES
        (
            @NewsletterId, @CampaignId, 0
        )
  END
--##

--## Add card provider and codes
/*
RegCode   :   Card Provider      :   Standard code only  :    81/19 split   no limit
*/

-- Edit these values for beneficiary plans
Declare @cardProvider Varchar(max) = @Name --'Card Provider'
Declare @registrationCode Varchar(max) = 'SOL2021' --'RegCode'
Declare @customerCashbackPercentage INT = 56 -- 81
Declare @deductionPercentage INT = 19
Declare @beneficiaryPercentage INT = 25 --0
-- End editing

-- Add the records

  --Create Partner
  DECLARE @CardProviderId INT = NULL
  SELECT @CardProviderId = [Id] FROM Exclusive.Partner WHERE [Name] = @cardProvider AND [IsDeleted] = 0 AND [Type] = 1

  IF @CardProviderId IS NULL
  BEGIN
	INSERT Exclusive.Partner (name, IsDeleted, Type ) Values (@cardProvider, 0, 1)
	SELECT @CardProviderId = SCOPE_IDENTITY()
  END

  DECLARE @PlanID INT = null
  INSERT [Exclusive].[MembershipPlan] ([PartnerId], [MembershipPlanTypeId], [Duration] ,[NumberOfCards]  ,[ValidFrom]  ,[ValidTo]   ,[CustomerCardPrice]  ,[PartnerCardPrice]
      ,[CustomerCashbackPercentage]  ,[DeductionPercentage]  ,[IsDeleted]  ,[CurrencyCode]   ,[Description]   ,[IsActive]  ,[MembershipLevelId]   ,[PaidByEmployer]   ,[MinimumValue]
      ,[PaymentFee]   ,[CardProviderId], [BenefactorPercentage],[WhitelabelId] )

  VALUES (1, 3, 36500, 100000000, GetDate(), GetDate() + 36500, 0, 0, @customerCashbackPercentage, @deductionPercentage, 0, 'GBP', @CardProvider + ' Beneficiary Rewards Plan', 1, 1, 0, 10, 1, @CardProviderId, @beneficiaryPercentage, @NewWhiteLabelId)
  SELECT @planId = SCOPE_IDENTITY()

  INSERT Exclusive.MembershipRegistrationCode (MembershipPlanId,[RegistartionCode] , ValidFrom, ValidTo, NumberOfCards, IsActive, IsDeleted)
  VALUES (@PlanID, @registrationCode, GetDate(),  GetDate() + 36500, 100000000, 1, 0)

  INSERT Exclusive.MembershipPlanPaymentProvider (MEmbershipPlanId, PaymentProviderId, OneOffPaymentRef)
  VALUES (@PlanId, 2, 'ZRFM9WT7LQE9A')


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

Select * FROM [Marketing].[NewsletterCampaignLink] ORDER BY ID DESC
SELECT * FROM [Marketing].[Campaigns] ORDER BY ID DESC

SELECT * FROM [CMS].[WhiteLabelSettings] ORDER BY ID DESC
SELECT * FROM [CMS].[WebsiteSocialMediaLink] ORDER BY ID DESC

--COMMIT TRAN

--ROLLBACK TRAN
