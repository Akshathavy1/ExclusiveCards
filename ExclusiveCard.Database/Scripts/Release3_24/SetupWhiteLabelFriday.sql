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
DECLARE @YouTubeLabel nvarchar(max) = 'YouTube' --<< used to add YouTube as a social media type if missing
DECLARE @YouTubeID int = 6  --<< used to add YouTube as a social media type if missing

SELECT @Name ='Friday Rewards'
SELECT @DisplayName = 'Friday Rewards'
SELECT @URL = 'https://Friday.exclusiverewards.co.uk' --<< LIVE
SELECT @Slug = 'friday-rewards'
SELECT @CompanyNumber ='11616720'
SELECT @CSSFile = 'friday-rewards.css'
SELECT @Logo = 'logo.png' 
SELECT @ClaimsEmail ='claims@exclusiverewards.co.uk'
SELECT @HelpEmail ='help-me@exclusiverewards.co.uk'
SELECT @MainEmail ='enquiries@exclusiverewards.co.uk'
SELECT @Address = ''
SELECT @CardName ='Exclusive Rewards'
SELECT @Privicy = '/Account/PrivacyPolicy?country=GB'
SELECT @Terms = '/Account/Terms?country=GB'

SELECT @Twitter = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Twitter'
SELECT @FaceBook = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Facebook'
SELECT @Instagram = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Instagram'
SELECT @Pinterest = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Pinterest'
SELECT @LinkedIn = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'LinkedIn'

BEGIN TRAN

--## Add YouTube if missing ##
IF NOT EXISTS (SELECT [Id] FROM [Exclusive].[SocialMediaCompany] WHERE [Name] = @YouTubeLabel)
BEGIN

SET IDENTITY_INSERT [Exclusive].[SocialMediaCompany] ON

  INSERT INTO [Exclusive].[SocialMediaCompany]
  ([Id], [Name], [IsEnabled])
  VALUES
  (@YouTubeID, @YouTubeLabel, 1)

SET IDENTITY_INSERT [Exclusive].[SocialMediaCompany] OFF

END

SELECT @YouTube = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = @YouTubeLabel
--## ##

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
	 ('GB', @Instagram,'https://instagram.com/thefridayrhodesfamily?igshid=mons9bbxwvyp', @NewWhiteLabelId),
	 ('GB', @FaceBook,'https://www.facebook.com/londons.friday', @NewWhiteLabelId),
	 ('GB', @LinkedIn,'https://www.linkedin.com/in/shireen-friday-68101096', @NewWhiteLabelId),
	 ('GB', @YouTube,'https://youtu.be/tHWtwVHMX_A', @NewWhiteLabelId)
END

--social media links for the Friday Rewards white label:
--https://instagram.com/thefridayrhodesfamily?igshid=mons9bbxwvyp
--https://www.facebook.com/londons.friday
--https://www.linkedin.com/in/shireen-friday-68101096
--https://youtu.be/tHWtwVHMX_A

SELECT * FROM [CMS].[WhiteLabelSettings] ORDER BY ID DESC
SELECT * FROM [CMS].[WebsiteSocialMediaLink] ORDER BY ID DESC

--COMMIT TRAN

--ROLLBACK TRAN