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

SELECT @Name ='London Broncos Rewards'
SELECT @DisplayName = 'London Broncos Rugby League'
SELECT @URL = 'https://londonbroncos.exclusiverewards.co.uk' --<< LIVE
SELECT @Slug = 'london-broncos'
SELECT @CompanyNumber ='11616720'
SELECT @CSSFile = 'london-broncos.css'
SELECT @Logo = 'logo.png' 
SELECT @NewsletterLogo = 'logo_newsletter.png'
SELECT @ClaimsEmail ='claims@exclusiverewards.co.uk'
SELECT @HelpEmail ='help-me@exclusiverewards.co.uk'
SELECT @MainEmail ='enquiries@exclusiverewards.co.uk'
SELECT @Address = 'Trailfinders Sports Club, Vallis Way, West Ealing, London W13 0DD'
SELECT @CardName ='Exclusive Rewards'
SELECT @Privicy = '/Account/PrivacyPolicy?country=GB'
SELECT @Terms = '/Account/Terms?country=GB'
SELECT @CharityName = 'London Broncos Rugby League'
SELECT @CharityUrl = 'https://londonbroncosrl.com'
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
	 ('GB', @Instagram,'https://www.instagram.com/londonbroncos/?hl=en', @NewWhiteLabelId),
	 ('GB', @FaceBook,'https://www.facebook.com/LondonBroncosRL', @NewWhiteLabelId),
	 ('GB', @Twitter,'https://twitter.com/LondonBroncosRL', @NewWhiteLabelId)
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

Select * FROM [Marketing].[NewsletterCampaignLink] ORDER BY ID DESC
SELECT * FROM [Marketing].[Campaigns] ORDER BY ID DESC

SELECT * FROM [CMS].[WhiteLabelSettings] ORDER BY ID DESC
SELECT * FROM [CMS].[WebsiteSocialMediaLink] ORDER BY ID DESC

--COMMIT TRAN

--ROLLBACK TRAN
