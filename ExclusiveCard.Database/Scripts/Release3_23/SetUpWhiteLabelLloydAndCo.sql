--## Script to add Lloyd & Co white label site ##
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


SELECT @Name ='Lloyd & Co Employee Rewards'
SELECT @DisplayName ='Lloyd & Co Financial planning Ltd'
SELECT @URL ='https://lloydfp.exclusiverewards.co.uk'
SELECT @Slug ='lloyd-eb'
SELECT @CompanyNumber ='11616720'
SELECT @CSSFile ='lloyd-eb.css'
SELECT @Logo ='logo.svg'
SELECT @ClaimsEmail ='claims@exclusiverewards.co.uk'
SELECT @HelpEmail ='help-me@exclusiverewards.co.uk'
SELECT @MainEmail ='enquiries@exclusiverewards.co.uk'
SELECT @Address ='Chartered House, 15 Hoghton Street, Southport, Merseyside, PR9 0NS'
SELECT @CardName ='Exclusive Rewards'
SELECT @Privicy = '/Account/PrivacyPolicy?country=GB'
SELECT @Terms = '/Account/Terms?country=GB'

SELECT @Twitter = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Twitter'
SELECT @FaceBook = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Facebook'
SELECT @Instagram = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Instagram'
SELECT @Pinterest = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Pinterest'

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
	 ('GB', @FaceBook,'https://www.facebook.com/LloydCoFinancialPlanningLtd', @NewWhiteLabelId)
    --https://www.facebook.com/LloydCoFinancialPlanningLtd
END

SELECT * FROM [CMS].[WhiteLabelSettings] ORDER BY ID DESC
SELECT * FROM [CMS].[WebsiteSocialMediaLink] ORDER BY ID DESC

--COMMIT TRAN

--ROLLBACK TRAN


