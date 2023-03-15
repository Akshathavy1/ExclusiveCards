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
DECLARE @Twitter int
DECLARE @FaceBook int
DECLARE @Instagram int
DECLARE @Pinterest int
DECLARE @LinkedIn int
DECLARE @YouTube int

SELECT @Name ='United By Rewards'
SELECT @DisplayName = 'United by Football Ltd'
SELECT @URL = 'https://unitedbyfootball.exclusiverewards.co.uk' --<< LIVE
SELECT @Slug = 'united-by-football'
SELECT @CompanyNumber ='11616720'
SELECT @CSSFile = 'united-by-football.css'
SELECT @Logo = 'logo.png' 
SELECT @ClaimsEmail ='claims@exclusiverewards.co.uk'
SELECT @HelpEmail ='help-me@exclusiverewards.co.uk'
SELECT @MainEmail ='enquiries@exclusiverewards.co.uk'
SELECT @Address = '78 Bronte Paths, Stevenage, United Kingdom, SG2 0PJ'
SELECT @CardName ='Exclusive Rewards'
SELECT @Privicy = '/Account/PrivacyPolicy?country=GB'
SELECT @Terms = '/Account/Terms?country=GB'

  --"unitedByFootball": {
  --  "slug": "united-by-football",
  --  "name": "United By Rewards",
  --  "cardName": "United by Rewards Card",
  --  "url": "https://www.unitedbyfootballuk.co.uk",
  --  "logoFormat": "png"
  --}

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
	 ('GB', @FaceBook,'https://www.facebook.com/Unitedbyfootballuk/', @NewWhiteLabelId),
	 ('GB', @Twitter,'https://twitter.com/unitedbyfootba3', @NewWhiteLabelId),
	 ('GB', @LinkedIn,'https://www.linkedin.com/in/united-by-football-5232411a0', @NewWhiteLabelId)
END

--https://twitter.com/unitedbyfootba3
--https://www.linkedin.com/in/united-by-football-5232411a0
--https://www.facebook.com/Unitedbyfootballuk/


SELECT * FROM [CMS].[WhiteLabelSettings] ORDER BY ID DESC
SELECT * FROM [CMS].[WebsiteSocialMediaLink] ORDER BY ID DESC

--COMMIT TRAN

--ROLLBACK TRAN
