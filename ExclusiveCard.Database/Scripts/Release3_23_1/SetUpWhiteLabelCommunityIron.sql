DECLARE @Name nvarchar(max)
DECLARE @DisplayName nvarchar(max)
DECLARE @URL nvarchar(max)
DECLARE @Slug nvarchar(max)
DECLARE @CompanyNumber nvarchar(max)
DECLARE @CSSFile nvarchar(max)
DECLARE @Logo nvarchar(max)
DECLARE @ClaimsEmail nvarchar(max)
DECLARE @HelpEmail nvarchar(max)
DECLARE @MainEmail nvarchar(max)
DECLARE @Address nvarchar(max)
DECLARE @CardName nvarchar(max)
DECLARE @Privicy nvarchar(512)
DECLARE @Terms nvarchar(512)
DECLARE @NewWhiteLabelId int
DECLARE @WhiteLabelId int --<<Used for removal of braintree
DECLARE @Twitter int
DECLARE @FaceBook int
DECLARE @Instagram int
DECLARE @Pinterest int
DECLARE @LinkedIn int
DECLARE @LinkedInLabel nvarchar(max) = 'LinkedIn'


SELECT @Name ='Community Iron Rewards'
SELECT @DisplayName = 'Community Iron'
SELECT @URL = 'https://bfccommunityiron.exclusiverewards.co.uk'
SELECT @Slug = 'community-iron'
SELECT @CompanyNumber ='11616720'
SELECT @CSSFile = 'community-iron.css'
SELECT @Logo = 'logo.png' 
SELECT @ClaimsEmail ='claims@exclusiverewards.co.uk'
SELECT @HelpEmail ='help-me@exclusiverewards.co.uk'
SELECT @MainEmail ='enquiries@exclusiverewards.co.uk'
SELECT @Address = 'Spa Road, Witham, Essex, CM8 1UN'
SELECT @CardName ='Exclusive Rewards'
SELECT @Privicy = '/Account/PrivacyPolicy?country=GB'
SELECT @Terms = '/Account/Terms?country=GB'

SELECT @Twitter = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Twitter'
SELECT @FaceBook = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Facebook'
SELECT @Instagram = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Instagram'
SELECT @Pinterest = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Pinterest'

BEGIN TRAN
--## Add Linked In if missing ##

IF NOT EXISTS (SELECT [Id] FROM [Exclusive].[SocialMediaCompany] WHERE [Name] = @LinkedInLabel)
BEGIN
  INSERT INTO [Exclusive].[SocialMediaCompany]
  ([Name],[IsEnabled])
  VALUES
  (@LinkedInLabel, 1)
END

SELECT @LinkedIn = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = @LinkedInLabel

--## Braintree FC has been re-named 'Community iron', this is to tidy up previous installs ##
IF EXISTS (SELECT [ID] FROM [CMS].[WhiteLabelSettings] WHERE [Name] = 'Braintree FC Rewards')
BEGIN
	SELECT @WhiteLabelId = [ID] FROM [CMS].[WhiteLabelSettings]
	WHERE [Name] = 'Braintree FC Rewards'

	DELETE FROM [CMS].[WebsiteSocialMediaLink]
	WHERE [WhiteLabelSettingsId] = @WhiteLabelId

	DELETE FROM [CMS].[WhiteLabelSettings]
	WHERE [Id] = @WhiteLabelId

END

  INSERT INTO [CMS].[WhiteLabelSettings]
  ([Name] ,[DisplayName] ,[URL] ,[Slug]
  ,[CompanyNumber],[CSSFile],[Logo],[ClaimsEmail]
  ,[HelpEmail],[MainEmail],[Address],[CardName]
  ,[PrivacyPolicy],[Terms])
  VALUES
  (@Name, @DisplayName,@URL,@Slug
  ,@CompanyNumber,@CSSFile,@Logo,@ClaimsEmail
  ,@HelpEmail,@MainEmail,@Address,@CardName
  ,@Privicy,@Terms)
  
  SELECT @NewWhiteLabelId = SCOPE_IDENTITY()

IF NOT EXISTS
(SELECT [Id] FROM [CMS].[WebsiteSocialMediaLink] WHERE [WhiteLabelSettingsId] = @NewWhiteLabelId)
BEGIN
	INSERT [CMS].[WebsiteSocialMediaLink]
	([CountryCode],[SocialMediaCompanyId],[SocialMediaURI],[WhiteLabelSettingsId])
	VALUES
	 ('GB', @FaceBook,'https://www.facebook.com/communityiron', @NewWhiteLabelId)
END

SELECT * FROM [CMS].[WhiteLabelSettings] ORDER BY ID DESC
SELECT * FROM [CMS].[WebsiteSocialMediaLink] ORDER BY ID DESC

--COMMIT TRAN

--ROLLBACK TRAN


