/****** Add site owner, category, leagues, charity's and clubs  ******/
-- This will add the white label, membership plans and the registration codes if they don't already exist

BEGIN TRAN



--###### site owner table ######
DECLARE @OwnerSettings table(
	[Id]	INT NOT NULL, 
	[Description]  NVARCHAR (20)	NOT NULL,
	[ClanDescription] NVARCHAR (MAX),
	[BeneficiaryConfirmation] NVARCHAR (MAX),
	[StarndardRewardConfirmation] NVARCHAR (MAX),
	[ClanHeading] NVARCHAR (MAX),
	[BeneficiaryHeading]  NVARCHAR (MAX),
	[StarndardHeading] NVARCHAR (MAX)
	)


INSERT @OwnerSettings
  ([Id], [Description],[ClanDescription],[BeneficiaryConfirmation],[StarndardRewardConfirmation],[ClanHeading],[BeneficiaryHeading],[StarndardHeading])
  VALUES
  (2, 'Bullseye','<p>Your Bullseye Exclusive Rewards account enables you to support YOUR chosen Darts organisation in these difficult times by donating 30% of your free cashback directly to your selected organisation.</p>

<p>You can of course select the standard Bullseye Exclusive Rewards account and no donation is necessary. You will simply retain 100% of your FREE cashback yourself to withdraw to your own bank account.</p>

<p>Just so you know…..if you choose to donate…every penny of your donation will be sent to your chosen organisation and our aim is to raise as much money as possible to support Darts across the UK.</p>

<p>If you choose to donate, your organisation will be displayed on your rewards platform and you will be able to see your donated balance on your account page. Together…we can make a difference.</p>','<p>Thank you for choosing to open a Bullseye Exclusive Rewards account. You have selected to support {{Description}} and 30% of your free cashback earned from making purchases via your new rewards account will go directly to support your chosen organisation. Thank you for your generosity. Your account page will show very clearly your donated balance and your remaining cashback, which will become available for you to withdraw to your bank account.</p>

<p>As a valued member, we hope you enjoy all the benefits and savings that your new Bullseye Exclusive Rewards account offers.</p>

<p>Now let’s get you started… </p>','<p>Thank you for choosing our standard Bullseye Exclusive Rewards account. All FREE cashback earned by you, from making purchases via your new rewards account will show on your account page and 100% of your cashback will become available for you to withdraw to your bank account.</p>

<p>As a valued member we really hope you enjoy all the benefits and savings that your new Bullseye Exclusive Rewards account offers.</p>

<p>Now let’s get you started…</p>','Select Your Organisation','Supporting Your Organisation','Standard Bullseye Exclusive Rewards Account')


SET IDENTITY_INSERT [CMS].[SiteOwner] ON

MERGE [CMS].[SiteOwner] AS target
USING @OwnerSettings AS source 
ON (target.[Id] = source.[Id])
WHEN MATCHED THEN
  UPDATE SET target.[Description] = source.[Description],
  target.[ClanDescription]=source.[ClanDescription],
  target.[BeneficiaryConfirmation]=source.[BeneficiaryConfirmation],
  target.[StarndardRewardConfirmation]=source.[StarndardRewardConfirmation],
  target.[ClanHeading]=source.[ClanHeading],
  target.[BeneficiaryHeading]=source.[BeneficiaryHeading],
  target.[StarndardHeading]=source.[StarndardHeading]

WHEN NOT MATCHED THEN
	INSERT ([Id], [Description],[ClanDescription],[BeneficiaryConfirmation],[StarndardRewardConfirmation],[ClanHeading],[BeneficiaryHeading],[StarndardHeading])
	VALUES (source.[Id], source.[Description],source.[ClanDescription],source.[BeneficiaryConfirmation],source.[StarndardRewardConfirmation],source.[ClanHeading],source.[BeneficiaryHeading],source.[StarndardHeading]) 
OUTPUT $action, Inserted.[Id], Inserted.[Description], Inserted.[ClanDescription],Inserted.[BeneficiaryConfirmation],Inserted.[StarndardRewardConfirmation],Inserted.[ClanHeading],Inserted.[BeneficiaryHeading],Inserted.[StarndardHeading];                 

SET IDENTITY_INSERT [CMS].[SiteOwner] OFF
--###### site owner table ######

--###### site category table ######
SELECT TOP (1000) [Id]
      ,[Description]
  FROM [CMS].[SiteCategory]
DECLARE @CategorySettings table(
	[Id]	INT NOT NULL, 
	[Description]  NVARCHAR (20)	NOT NULL)

INSERT @CategorySettings
  ([Id], [Description])
  VALUES
  (9, 'Darts')

SET IDENTITY_INSERT [CMS].[SiteCategory] ON

MERGE [CMS].[SiteCategory] AS target
USING @CategorySettings AS source 
ON (target.[Id] = source.[Id])
WHEN MATCHED THEN
  UPDATE SET target.[Description] = source.[Description]
WHEN NOT MATCHED THEN
	INSERT ([Id], [Description])
	VALUES (source.[Id], source.[Description]) 
OUTPUT $action, Inserted.[Id], Inserted.[Description];                 

SET IDENTITY_INSERT [CMS].[SiteCategory] OFF
--###### site category table ######

--###### League table ######
--NOTE original league table description length was too small...
  --ALTER TABLE [CMS].[League]
  --ALTER COLUMN  [Description] NVARCHAR (50) NULL

DECLARE @LeagueSettings table(
	[Id]	INT NOT NULL,
	[Description]	NVARCHAR (50) NULL,
	[ImagePath]	NVARCHAR (512) NULL,
	[SiteCategoryId] INT
	)

INSERT @LeagueSettings
  ([Id], [Description],[ImagePath],[SiteCategoryId])
  VALUES	
	(13,'Darts',NULL,9)

SET IDENTITY_INSERT [CMS].[League] ON

MERGE [CMS].[League] AS target
USING @LeagueSettings AS source 
ON (target.[Id] = source.[Id])
WHEN MATCHED THEN
  UPDATE SET target.[Description] = source.[Description],
  target.[ImagePath] = source.[ImagePath],
  target.[SiteCategoryId] = source.[SiteCategoryId] 

WHEN NOT MATCHED THEN
	INSERT ([Id], [Description],[ImagePath],[SiteCategoryId])
	VALUES (source.[Id], source.[Description],source.[ImagePath],source.[SiteCategoryId]) 
OUTPUT $action, Inserted.[Id], Inserted.[Description],Inserted.[ImagePath],Inserted.[SiteCategoryId];                 

SET IDENTITY_INSERT [CMS].[League] OFF
--###### League table ######

--###### Charity table ######
DECLARE @CharitySettings table(
	[Id]	INT NOT NULL,
	[CharityName]	NVARCHAR (255) NULL,
	[CharityURL]	NVARCHAR (512) NULL
	)

INSERT @CharitySettings
  ([Id], [CharityName],[CharityURL])
  VALUES
--Darts
(203,'UK Darts Association','https://ukdartsassociation.com'),
(204,'Darts Academies UK','https://ukdartsassociation.com'),
(205,'World Disability Darts Association','https://world-disability-darts.org')

SET IDENTITY_INSERT [CMS].[Charity] ON

MERGE [CMS].[Charity] AS target
USING @CharitySettings AS source 
ON (target.[Id] = source.[Id])
WHEN MATCHED THEN
  UPDATE SET target.[CharityName] = source.[CharityName], target.[CharityURL] = source.[CharityURL]
WHEN NOT MATCHED THEN
	INSERT ([Id], [CharityName],[CharityURL])
	VALUES (source.[Id], source.[CharityName], source.[CharityURL]) 
OUTPUT $action, Inserted.[Id], Inserted.[CharityName];                 

SET IDENTITY_INSERT [CMS].[Charity] OFF

--###### Charity table ######

-- #### Check white label exists ####
DECLARE @DisplayName nvarchar(max)
SELECT @DisplayName ='Bullseye'
DECLARE @Bullseye int

IF NOT Exists(SELECT Id FROM [CMS].[WhiteLabelSettings] WHERE [DisplayName] = @DisplayName)
BEGIN
	--### White label missing need to add white label ##
	DECLARE @Name nvarchar(max)
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
	DECLARE @Twitter int
	DECLARE @FaceBook int
	DECLARE @Instagram int
	DECLARE @Pinterest int
	DECLARE @LinkedIn int
	DECLARE @YouTube int
	DECLARE @SiteOwner int = 2

	SELECT @Name ='Bullseye Rewards'
	SELECT @URL ='https://bullseye.exclusiverewards.co.uk' --<<< LIVE URL
	SELECT @Slug ='bullseye'
	SELECT @CompanyNumber ='11616720'
	SELECT @CSSFile ='bullseye.css'
	SELECT @Logo ='logo.svg'
	SELECT @NewsletterLogo ='logo_newsletter.png'
	SELECT @ClaimsEmail ='claims@exclusiverewards.co.uk'
	SELECT @HelpEmail ='help-me@exclusiverewards.co.uk'
	SELECT @MainEmail ='enquiries@exclusiverewards.co.uk'
	SELECT @Address ='15 Hoghton Street, Southport, United Kingdom, PR9 0NS'
	SELECT @CardName ='Exclusive Rewards'

	SELECT @Twitter = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Twitter'
	SELECT @FaceBook = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Facebook'
	SELECT @Instagram = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Instagram'
	SELECT @Pinterest = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'Pinterest'
	SELECT @LinkedIn = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'LinkedIn'
	SELECT @YouTube = [Id] from [Exclusive].[SocialMediaCompany] WHERE [Name] = 'YouTube'

	INSERT INTO [CMS].[WhiteLabelSettings]
	([Name] ,[DisplayName] ,[URL] ,[Slug]
	,[CompanyNumber],[CSSFile],[Logo],[ClaimsEmail]
	,[HelpEmail],[MainEmail],[Address],[CardName]
	,[NewsletterLogo],[PrivacyPolicy],[Terms],[SiteType],[SiteOwnerId])
	VALUES
	(@Name, @DisplayName,@URL,@Slug,@CompanyNumber,@CSSFile,@Logo,@ClaimsEmail,@HelpEmail,@MainEmail,@Address,@CardName
	 ,@NewsletterLogo,'/Account/PrivacyPolicy?country=GB','/Account/Terms?country=GB',0,@SiteOwner)

	SELECT @Bullseye = SCOPE_IDENTITY()

	--IF NOT EXISTS
	--(SELECT [Id] FROM [CMS].[WebsiteSocialMediaLink] WHERE [WhiteLabelSettingsId] = @Bullseye)
	--BEGIN
	--	INSERT [CMS].[WebsiteSocialMediaLink]
	--	([CountryCode],[SocialMediaCompanyId],[SocialMediaURI],[WhiteLabelSettingsId])
	--	VALUES
	--	 ('GB', @Instagram,'https://www.instagram.com/Bullseye/', @Bullseye),
	--	 ('GB', @FaceBook,'https://www.facebook.com/Bullseye/', @Bullseye),
	--	 ('GB', @Twitter,'https://twitter.com/Bullseye', @Bullseye),
	--	 ('GB', @YouTube,'https://www.youtube.com/Bullseye', @Bullseye)
	--END

--##Add to Newsletter

DECLARE @NewsletterId INT
SELECT @NewsletterId = Id FROM [Marketing].[Newsletter]

-- Add Campaign Data
DECLARE @CampaignId int

  IF NOT EXISTS(Select * FROM [Marketing].[Campaigns] WHERE [WhiteLabelId] = @Bullseye)
  BEGIN
    INSERT INTO [Marketing].[Campaigns]
    (
        [WhiteLabelId], [ContactListId], [ContactListName], [CampaignId], [CampaignName], [SenderId], [Enabled]
    )
    VALUES
    (
        @Bullseye, NULL, REPLACE(@Name, ' ', '') + '_ContactList',
        NULL, REPLACE(@Name, ' ', '') + '_Campaign', 
        1068141, 1
    )
    SELECT @CampaignId = SCOPE_IDENTITY()
  END
  ELSE
  BEGIN
        SELECT @CampaignId = Id FROM [Marketing].[Campaigns] WHERE [WhiteLabelId] = @Bullseye
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
            @NewsletterId, @CampaignId, 1
        )
  END
--##

END
ELSE
BEGIN
	SELECT @Bullseye = Id FROM [CMS].[WhiteLabelSettings] WHERE [DisplayName] = @DisplayName
	UPDATE [CMS].[WhiteLabelSettings] SET [CharityName]='Exclusive Sport Rewards',[SiteType]=1,
	[CharityUrl]='http://bullseye.exclusive.co.uk' WHERE [DisplayName] = @DisplayName
      
END
-- #### Check white label exists ####

-- #### Check membership plan exists
DECLARE @BeneficiaryPlanId int
IF NOT Exists(SELECT Id FROM [Exclusive].[MembershipPlan] WHERE [Description] = 'Bullseye Beneficiary Rewards Plan')
BEGIN

	/*
	TalkSPORT registration code creation - both Standard & Beneficiary 
	*/

	-- Editing
	Declare @WhiteLabelName nvarchar(max) = 'Bullseye Rewards' --<< [CMS].[WhiteLabelSettings].[Name]
	Declare @cardProvider Varchar(max) = 'Bullseye'
	--Edit these values for Standard plans
	Declare @StandardRegistrationCode Varchar(max) = 'BUS2020'
	Declare @StandardCustomerCashbackPercentage INT = 81
	Declare @StandardDeductionPercentage INT = 19

	--Edit these values for Beneficiary plans
	Declare @BeneficiaryRegistrationCode Varchar(max) = 'UKDA21'
	Declare @BeneficiaryCustomerCashbackPercentage INT = 56
	Declare @BeneficiaryDeductionPercentage INT = 19
	Declare @BeneficiaryPercentage INT = 25

	-- End editing
	--Note Diamond upgrades have different %

	-- VALIDATION
	IF @WhiteLabelName IS NULL OR @WhiteLabelName = '' OR NOT EXISTS( SELECT Id FROM [CMS].[WhiteLabelSettings] WHERE [Name] = @WhiteLabelName)
	BEGIN
		;THROW 51000, 'White label not found, please provide a valid whitelabel name', 1;
	END

	IF @cardProvider IS NULL OR @cardProvider = ''
	BEGIN
		;THROW 51000, 'Please update the @cardProvider variable, no name was provided', 1;
	END
	-- End Validation

	Declare @WhiteLabelId INT 
	SELECT TOP(1) @WhiteLabelId = Id FROM [CMS].[WhiteLabelSettings] WHERE [Name] = @WhiteLabelName
	SELECT 'White label selected:', * FROM [CMS].[WhiteLabelSettings] WHERE id = @WhiteLabelId

	-- Add the records
	
		--Create Partner if needed
		DECLARE @CardProviderId INT = null
		IF NOT EXISTS( SELECT Id FROM [Exclusive].[Partner] WHERE [Name] = @cardProvider)
		BEGIN

		  INSERT Exclusive.Partner ([name], [IsDeleted], [Type] ) Values (@cardProvider, 0, 1)
		  SELECT @CardProviderId = SCOPE_IDENTITY()
		  SELECT 'New partner added'
		END
		ELSE
		BEGIN
			SELECT @CardProviderId = Id FROM [Exclusive].[Partner] WHERE [Name] = @cardProvider
			SELECT 'Existing partner used'
		END

	-- Add Standard Plan if needed
	IF NOT EXISTS( SELECT Id FROM [Exclusive].[MembershipPlan] WHERE [Description] = @CardProvider + ' Standard Rewards Plan')
	BEGIN

	  DECLARE @PlanID INT = null
	  INSERT [Exclusive].[MembershipPlan] ([PartnerId], [MembershipPlanTypeId], [Duration] ,[NumberOfCards]  ,[ValidFrom]  ,[ValidTo]   ,[CustomerCardPrice]  ,[PartnerCardPrice]
		  ,[CustomerCashbackPercentage]  ,[DeductionPercentage]  ,[IsDeleted]  ,[CurrencyCode]   ,[Description]   ,[IsActive]  ,[MembershipLevelId]   ,[PaidByEmployer]   ,[MinimumValue]
		  ,[PaymentFee]   ,[CardProviderId],[WhitelabelId] )
		VALUES (1, 4, 36500, 100000000, GetDate(), '31 Dec 2120', 0, 0, @StandardCustomerCashbackPercentage, @StandardDeductionPercentage, 0, 'GBP', @CardProvider + ' Standard Rewards Plan', 1, 1, 0, 10, 1, @CardProviderId, @WhiteLabelId)
	  SELECT @planId = SCOPE_IDENTITY()

	  INSERT Exclusive.MembershipRegistrationCode (MembershipPlanId,[RegistartionCode] , ValidFrom, ValidTo, NumberOfCards, IsActive, IsDeleted)
		VALUES (@PlanID, @StandardRegistrationCode, GetDate(), '31 Dec 2120', 100000000, 1, 0)

	  INSERT Exclusive.MembershipPlanPaymentProvider (MembershipPlanId, PaymentProviderId, OneOffPaymentRef)
		VALUES (@PlanId, 2, 'ZRFM9WT7LQE9A')
		SELECT 'New standard plan added'
	END
	

	-- Add Beneficiary Plan if needed
	IF NOT EXISTS( SELECT Id FROM [Exclusive].[MembershipPlan] WHERE [Description] = @CardProvider + ' Beneficiary Rewards Plan')
	BEGIN

	  --DECLARE @BeneficiaryPlanID INT = null
	  INSERT [Exclusive].[MembershipPlan] ([PartnerId], [MembershipPlanTypeId], [Duration] ,[NumberOfCards]  ,[ValidFrom]  ,[ValidTo]   ,[CustomerCardPrice]  ,[PartnerCardPrice]
		  ,[CustomerCashbackPercentage]  ,[DeductionPercentage]  ,[IsDeleted]  ,[CurrencyCode]   ,[Description]   ,[IsActive]  ,[MembershipLevelId]   ,[PaidByEmployer]   ,[MinimumValue]
		  ,[PaymentFee]   ,[CardProviderId], [BenefactorPercentage],[WhitelabelId] )

	  VALUES (1, 3, 36500, 100000000, GetDate(), '01 Jan 2120', 0, 0, @BeneficiaryCustomerCashbackPercentage, @BeneficiaryDeductionPercentage, 0, 'GBP', @CardProvider + ' Beneficiary Rewards Plan', 1, 1, 0, 10, 1, @CardProviderId, @BeneficiaryPercentage, @WhiteLabelId)
	  SELECT @BeneficiaryPlanID = SCOPE_IDENTITY()

	  INSERT Exclusive.MembershipRegistrationCode (MembershipPlanId,[RegistartionCode] , ValidFrom, ValidTo, NumberOfCards, IsActive, IsDeleted)
	  VALUES (@BeneficiaryPlanID, @BeneficiaryRegistrationCode, GetDate(), '01 Jan 2120', 100000000, 1, 0)

	  INSERT Exclusive.MembershipPlanPaymentProvider (MEmbershipPlanId, PaymentProviderId, OneOffPaymentRef)
	  VALUES (@BeneficiaryPlanID, 2, 'ZRFM9WT7LQE9A')
	  SELECT 'New beneficiary plan added'

	END
	-- Add Beneficiary Diamond Plan if needed
	

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

	SElect P.Description,   PP.* 
	from [Exclusive].[MembershipPlanPaymentProvider] PP
	inner join Exclusive.MembershipPlan P ON PP.MembershipPlanId = P.Id
	where p.IsActive = 1
	order by p.id desc

END
ELSE
BEGIN
	SELECT @BeneficiaryPlanId = Id FROM [Exclusive].[MembershipPlan] WHERE [Description] = @CardProvider + ' Beneficiary Rewards Plan'
END
-- #### Check membership plan exists
 
--###### Clan table ######
DECLARE @ClanSettings table(
	[Id]	INT NOT NULL, 
	[LeagueId]	INT	NULL, 
	[Description]        NVARCHAR (50)    NOT NULL, 
	[ImagePath]          NVARCHAR (512)   NULL, 
	[PrimaryColour]      NVARCHAR (10)    NULL, 
	[SecondaryColour]    NVARCHAR (10)    NULL, 
	[CharityId]            INT            NULL, 
	[SiteOwnerId]          INT            NOT NULL, 
	[SiteCategoryId]       INT            NOT NULL, 
	[WhiteLabelId]         INT            NULL,
	[MembershipPlanId]     INT            NOT NULL
	)

INSERT @ClanSettings
  ([Id], [LeagueId],[Description]
   ,[PrimaryColour],[SecondaryColour]
   ,[CharityId],[SiteOwnerId],[SiteCategoryId]
      ,[WhiteLabelId], [MembershipPlanId])
  VALUES
--English Clubs

(202,13,'UK Darts Association','#0066FF','#FFFFFF',203,2,9,@Bullseye,@BeneficiaryPlanId),
(203,13,'World Disability Darts Association','#CE0000','#FFFFFF',205,2,9,@Bullseye,@BeneficiaryPlanId),
(204,13,'Darts Academies UK','#000000','#FFFFFF',204,2,9,@Bullseye,@BeneficiaryPlanId)

SET IDENTITY_INSERT [CMS].[SiteClan] ON

MERGE [CMS].[SiteClan] AS target
USING @ClanSettings AS source 
ON (target.[Id] = source.[Id])
WHEN MATCHED THEN
  UPDATE SET target.[LeagueId] = source.[LeagueId], target.[Description] = source.[Description],
			target.[PrimaryColour] = source.[PrimaryColour], target.[SecondaryColour] = source.[SecondaryColour],
			target.[CharityId] = source.[CharityId], target.[SiteOwnerId] = source.[SiteOwnerId],
			target.[SiteCategoryId] = source.[SiteCategoryId], target.[WhiteLabelId] = source.[WhiteLabelId],
			target.[MembershipPlanId] = source.[MembershipPlanId]
WHEN NOT MATCHED THEN
	INSERT ([Id], [LeagueId],[Description]
			,[PrimaryColour],[SecondaryColour],[CharityId]
			,[SiteOwnerId],[SiteCategoryId],[WhiteLabelId], [MembershipPlanId])
	VALUES (source.[Id], source.[LeagueId], source.[Description],
			source.[PrimaryColour], source.[SecondaryColour], source.[CharityId],
			source.[SiteOwnerId], source.[SiteCategoryId], source.[WhiteLabelId], source.[MembershipPlanId]
			) 
OUTPUT $action, Inserted.[Id], Inserted.[Description];                 

SET IDENTITY_INSERT [CMS].[SiteClan] OFF
--###### Clan table ######


SELECT * FROM [CMS].[SiteCategory]
SELECT * FROM [CMS].[SiteOwner]
SELECT * FROM [CMS].[League]
SELECT * FROM [CMS].[Charity]
SELECT * FROM [CMS].[WhiteLabelSettings] ORDER BY ID DESC
SELECT * FROM [CMS].[WebsiteSocialMediaLink]  ORDER BY ID DESC
SELECT * FROM [CMS].[SiteClan]

Select * FROM [Marketing].[NewsletterCampaignLink] ORDER BY ID DESC
SELECT * FROM [Marketing].[Campaigns] ORDER BY ID DESC
SELECT * FROM [CMS].[WhiteLabelSettings] ORDER BY ID DESC
SELECT * FROM [CMS].[WebsiteSocialMediaLink] ORDER BY ID DESC

--COMMIT TRAN
--ROLLBACK TRAN
