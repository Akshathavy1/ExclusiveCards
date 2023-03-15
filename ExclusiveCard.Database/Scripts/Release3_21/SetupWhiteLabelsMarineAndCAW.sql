--## Script to add Marine FC and CAW Digital white label sites ##
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


SELECT @Name ='CAW Digital Rewards'
SELECT @DisplayName ='CAW Digital Rewards Ltd'
SELECT @URL ='https://cawdigital.exclusiverewards.co.uk'
SELECT @Slug ='caw-digital'
SELECT @CompanyNumber ='11616720'
SELECT @CSSFile ='caw-digital.css'
SELECT @Logo ='logo.png'
SELECT @ClaimsEmail ='claims@exclusiverewards.co.uk'
SELECT @HelpEmail ='help-me@exclusiverewards.co.uk'
SELECT @MainEmail ='enquiries@exclusiverewards.co.uk'
SELECT @Address ='Suite 207, City House 131 Friargate, Preston, Lancashire, United Kingdom, PR1 2EF'
SELECT @CardName ='Exclusive Rewards'

BEGIN TRAN

  INSERT INTO [CMS].[WhiteLabelSettings]
  ([Name] ,[DisplayName] ,[URL] ,[Slug]
  ,[CompanyNumber],[CSSFile],[Logo],[ClaimsEmail]
  ,[HelpEmail],[MainEmail],[Address],[CardName])
  VALUES
  (@Name, @DisplayName,@URL,@Slug,@CompanyNumber,@CSSFile,@Logo,@ClaimsEmail,@HelpEmail,@MainEmail,@Address,@CardName)

SELECT [Id]
      ,[Name]
      ,[DisplayName]
      ,[URL]
      ,[Slug]
      ,[CompanyNumber]
      ,[CSSFile]
      ,[Logo]
      ,[ClaimsEmail]
      ,[HelpEmail]
      ,[MainEmail]
      ,[Address]
      ,[CardName]
  FROM [CMS].[WhiteLabelSettings]

--COMMIT TRAN

--ROLLBACK TRAN

SELECT @Name ='Marine FC Rewards'
SELECT @DisplayName ='Marine FC Rewards Ltd'
SELECT @URL ='https://cawdigital.exclusiverewards.co.uk'
SELECT @Slug ='marine-fc'
SELECT @CompanyNumber ='11616720'
SELECT @CSSFile ='marine-fc.css'
SELECT @Logo ='logo.png'
SELECT @ClaimsEmail ='claims@exclusiverewards.co.uk'
SELECT @HelpEmail ='help-me@exclusiverewards.co.uk'
SELECT @MainEmail ='enquiries@exclusiverewards.co.uk'
SELECT @Address ='The Marine Travel Arena, College Road, Crosby, Liverpool, United Kingdom, L23 3AS'
SELECT @CardName ='Exclusive Rewards'

BEGIN TRAN

  INSERT INTO [CMS].[WhiteLabelSettings]
  ([Name] ,[DisplayName] ,[URL] ,[Slug]
  ,[CompanyNumber],[CSSFile],[Logo],[ClaimsEmail]
  ,[HelpEmail],[MainEmail],[Address],[CardName])
  VALUES
  (@Name, @DisplayName,@URL,@Slug,@CompanyNumber,@CSSFile,@Logo,@ClaimsEmail,@HelpEmail,@MainEmail,@Address,@CardName)

SELECT [Id]
      ,[Name]
      ,[DisplayName]
      ,[URL]
      ,[Slug]
      ,[CompanyNumber]
      ,[CSSFile]
      ,[Logo]
      ,[ClaimsEmail]
      ,[HelpEmail]
      ,[MainEmail]
      ,[Address]
      ,[CardName]
  FROM [CMS].[WhiteLabelSettings]

--COMMIT TRAN

--ROLLBACK TRAN

