/****** Add site owner, category, leagues, charity's and clubs  ******/
-- This will add the white label, membership plans and the registration codes if they don't already exist

BEGIN TRAN

--###### site owner table ######
DECLARE @OwnerSettings table(
	[Id]	INT NOT NULL, 
	[Description]  NVARCHAR (20)	NOT NULL)


INSERT @OwnerSettings
  ([Id], [Description])
  VALUES
  (1, 'TalkSPORT')
SET IDENTITY_INSERT [CMS].[SiteOwner] ON

MERGE [CMS].[SiteOwner] AS target
USING @OwnerSettings AS source 
ON (target.[Id] = source.[Id])
WHEN MATCHED THEN
  UPDATE SET target.[Description] = source.[Description]
WHEN NOT MATCHED THEN
	INSERT ([Id], [Description])
	VALUES (source.[Id], source.[Description]) 
OUTPUT $action, Inserted.[Id], Inserted.[Description];                 

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
  (1, 'Football')

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
	[ImagePath]	NVARCHAR (512) NULL
	)

INSERT @LeagueSettings
  ([Id], [Description])
  VALUES
	(1,'Premier League'),
    (2,'Championship'),
    (3,'EFL League One'),
    (4,'EFL League Two'),
	(5,'Vanarama National League'),
    (6,'Vanarama National League North'),
    (7,'Vanarama National League South'),
    (8,'Scottish Premier Division'),
	(9,'Scottish Championship'),
    (10,'Scottish League One'),
    (11,'Scottish League Two'),
	(12,'Other')

SET IDENTITY_INSERT [CMS].[League] ON

MERGE [CMS].[League] AS target
USING @LeagueSettings AS source 
ON (target.[Id] = source.[Id])
WHEN MATCHED THEN
  UPDATE SET target.[Description] = source.[Description]
WHEN NOT MATCHED THEN
	INSERT ([Id], [Description])
	VALUES (source.[Id], source.[Description]) 
OUTPUT $action, Inserted.[Id], Inserted.[Description];                 

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
--English clubs
(1,'Arsenal FC','https://www.arsenal.com/'),
(2,'Aston Villa FC','https://www.avfc.co.uk/'),
(3,'Brighton & Hove Albion FC','https://www.brightonandhovealbion.com/'),
(4,'Burnley FC','https://www.burnleyfootballclub.com/'),
(5,'Chelsea FC','https://www.chelseafc.com/'),
(6,'Crystal Palace FC','https://www.cpfc.co.uk/'),
(7,'Everton FC','https://www.evertonfc.com/'),
(8,'Fulham FC','https://www.fulhamfc.com/'),
(9,'Leeds United','https://www.leedsunited.com/'),
(10,'Leicester City FC','https://www.lcfc.com/'),
(11,'Liverpool FC','https://www.liverpoolfc.com/'),
(12,'Manchester City FC','https://www.mancity.com/'),
(13,'Manchester United FC','https://www.manutd.com/'),
(14,'Newcastle United FC','https://www.nufc.co.uk/'),
(15,'Sheffield United FC','https://www.sufc.co.uk/'),
(16,'Southampton FC','https://www.southamptonfc.com/'),
(17,'Tottenham Hotspur FC','https://www.tottenhamhotspur.com/'),
(18,'West Bromwich Albion FC','https://www.westbrom.co.uk/'),
(19,'West Ham United FC','https://www.whufc.com/'),
(20,'Wolverhampton Wanderers FC','https://www.wolves.co.uk/'),
(21,'AFC Bournemouth','https://www.afcb.co.uk/'),
(22,'Barnsley FC','https://www.barnsleyfc.co.uk/'),
(23,'Birmingham City FC','https://www.bcfc.com/'),
(24,'Blackburn Rovers FC','https://www.rovers.co.uk/'),
(25,'Brentford FC','https://www.brentfordfc.com/'),
(26,'Bristol City FC','https://www.bcfc.co.uk/'),
(27,'Cardiff City FC','https://www.cardiffcityfc.co.uk/'),
(28,'Coventry City FC','https://www.ccfc.co.uk/'),
(29,'Derby County  FC','https://www.dcfc.co.uk/'),
(30,'Huddersfield Town AFC','https://www.htafc.com/'),
(31,'Luton Town FC','https://www.lutontown.co.uk/'),
(32,'Middlesbrough FC','https://www.mfc.co.uk/'),
(33,'Millwall FC','https://www.millwallfc.co.uk/'),
(34,'Norwich City FC','https://www.canaries.co.uk/'),
(35,'Nottingham Forest FC','https://www.nottinghamforest.co.uk/'),
(36,'Preston North End FC','https://www.pnefc.net/'),
(37,'Queens Park Rangers FC','https://www.qpr.co.uk/'),
(38,'Reading FC','https://www.readingfc.co.uk/'),
(39,'Rotherham United FC','https://www.themillers.co.uk/'),
(40,'Sheffield Wednesday FC','https://www.swfc.co.uk/'),
(41,'Stoke City FC','https://www.stokecityfc.com/'),
(42,'Swansea City AFC','https://www.swanseacity.com/'),
(43,'Watford FC','https://www.watfordfc.com/'),
(44,'Wycombe Wanderers FC','https://www.wycombewanderers.co.uk/'),
(45,'Accrington Stanley FC','https://www.accringtonstanley.co.uk/'),
(46,'AFC Wimbledon','https://www.afcwimbledon.co.uk/'),
(47,'Blackpool FC','https://www.blackpoolfc.co.uk/'),
(48,'Bristol Rovers FC','https://www.bristolrovers.co.uk/'),
(49,'Burton Albion FC','https://www.burtonalbionfc.co.uk/'),
(50,'Charlton Athletic FC','https://www.cafc.co.uk/home'),
(51,'Crewe Alexandra FC','https://www.crewealex.net/'),
(52,'Doncaster Rovers FC','https://www.doncasterroversfc.co.uk/'),
(53,'Fleetwood Town FC','https://www.fleetwoodtownfc.com/'),
(54,'Gillingham FC','https://www.gillinghamfootballclub.com/'),
(55,'Hull City FC','https://www.hullcitytigers.com/'),
(56,'Ipswich Town FC','https://www.itfc.co.uk/'),
(57,'Lincoln City FC','https://www.weareimps.com/'),
(58,'Milton Keynes Dons FC','https://www.mkdons.com/'),
(59,'Northampton Town FC','https://www.ntfc.co.uk/'),
(60,'Oxford United FC','https://www.oufc.co.uk/'),
(61,'Peterborough United FC','https://www.theposh.com/'),
(62,'Plymouth Argyle FC','https://www.pafc.co.uk/'),
(63,'Portsmouth FC','https://www.portsmouthfc.co.uk/'),
(64,'Rochdale AFC','https://www.rochdaleafc.co.uk/'),
(65,'Shrewsbury Town FC','https://www.shrewsburytown.com/'),
(66,'Sunderland AFC','https://safc.com/'),
(67,'Swindon Town FC','https://www.swindontownfc.co.uk/'),
(68,'Wigan Athletic FC','https://wiganathletic.com/'),
(69,'Barrow AFC','https://www.barrowafc.com/'),
(70,'Bolton Wanderers FC','https://www.bwfc.co.uk/'),
(71,'Bradford City AFC','https://www.bradfordcityfc.co.uk/'),
(72,'Cambridge United FC','https://www.cambridge-united.co.uk/'),
(73,'Carlisle United FC','https://www.carlisleunited.co.uk/'),
(74,'Cheltenham Town FC','https://www.ctfc.com/'),
(75,'Colchester United FC','https://www.cu-fc.com/'),
(76,'Crawley Town FC','https://www.crawleytownfc.com/'),
(77,'Exeter City FC','https://www.exetercityfc.co.uk/'),
(78,'Forest Green Rovers FC','https://www.fgr.co.uk/'),
(79,'Grimsby Town FC','https://www.grimsby-townfc.co.uk/'),
(80,'Harrogate Town AFC','https://www.harrogatetownafc.com/'),
(81,'Leyton Orient FC','https://www.leytonorient.com/'),
(82,'Mansfield Town FC','https://www.mansfieldtown.net/'),
(83,'Morecambe FC','https://www.morecambefc.com/'),
(84,'Newport County AFC','https://www.newport-county.co.uk/'),
(85,'Oldham Athletic AFC','https://www.oldhamathletic.co.uk/'),
(86,'Port Vale FC','https://www.port-vale.co.uk/'),
(87,'Salford City FC','https://salfordcityfc.co.uk/'),
(88,'Scunthorpe United FC','https://www.scunthorpe-united.co.uk/'),
(89,'Southend United FC','https://www.southendunited.co.uk/'),
(90,'Stevenage FC','https://www.stevenagefc.com/'),
(91,'Tranmere Rovers FC','https://www.tranmererovers.co.uk/'),
(92,'Walsall FC','https://www.saddlers.co.uk/'),
(93,'Aldershot Town FC','https://www.theshots.co.uk/'),
(94,'Altrincham FC','https://www.altrinchamfc.com/'),
(95,'Barnet FC','https://www.barnetfc.com/'),
(96,'Boreham wood FC','https://www.borehamwoodfootballclub.co.uk/'),
(97,'Bromley FC','https://www.bromleyfc.tv/site/'),
(98,'Chesterfield FC','https://chesterfield-fc.co.uk/'),
(99,'Dagenham and Redbridge FC','https://www.daggers.co.uk/'),
(100,'Dover Athletic FC','https://www.doverathletic.com/'),
(101,'Eastleigh FC','https://www.eastleighfc.com/'),
(102,'FC Halifax Town','https://fchalifaxtown.com/'),
(103,'Hartlepool United FC','https://www.hartlepoolunited.co.uk/'),
(104,'Kings Lynn Town FC','https://www.kltown.co.uk/'),
(105,'Marine FC','https://marinefc.com/'),
(106,'Maidenhead United FC','https://www.pitchero.com/clubs/maidenheadunited/'),
(107,'Notts County FC','https://www.nottscountyfc.co.uk/'),
(108,'Solihull Moors FC','https://www.solihullmoorsfc.co.uk/'),
(109,'Stockport County FC','https://www.stockportcounty.com/'),
(110,'Sutton United FC','https://www.suttonunited.net/'),
(111,'Torquay United FC','https://torquayunited.com/'),
(112,'Wealdstone FC','https://www.wealdstone-fc.com/'),
(113,'Weymouth FC','https://uptheterras.co.uk/'),
(114,'Woking FC','https://www.wokingfc.co.uk/home/'),
(115,'Wrexham AFC','https://www.wrexhamafc.co.uk/'),
(116,'Yeovil Town FC','https://www.ytfc.net/'),
(117,'AFC Fylde','https://www.afcfylde.co.uk/'),
(118,'AFC Telford','https://www.telfordunited.com/'),
(119,'Alfreton Town FC','https://www.alfretontownfootballclub.com/'),
(120,'Blyth Spartans AFC','https://www.blythspartans.com/'),
(121,'Boston United FC','https://www.bostonunited.co.uk/'),
(122,'Brackley Town FC','https://www.brackleytownfc.com/'),
(123,'Bradford (Park Avenue) AFC','https://bpafc.com/'),
(124,'Chester FC','https://www.chesterfc.com/'),
(125,'Chorley FC','https://www.chorleyfc.com/'),
(126,'Curzon Ashton FC','https://www.curzon-ashton.co.uk/'),
(127,'Darlington FC','https://darlingtonfc.co.uk/'),
(128,'Farsley Celtic FC','https://www.farsleyceltic.com/'),
(129,'Gateshead FC','https://www.gateshead-fc.com/'),
(130,'Gloucester City AFC','https://www.gloucestercityafc.com/'),
(131,'Guiseley AFC','https://guiseleyafc.co.uk/'),
(132,'Hereford FC','https://www.herefordfc.co.uk/'),
(133,'Kettering Town FC','https://www.ketteringtownfc.com/'),
(134,'Kidderminster Harriers FC','https://harriers.co.uk/'),
(135,'Leamington FC','https://www.leamingtonfc.co.uk/'),
(136,'Southport FC','https://southportfc.net/'),
(137,'Spennymoor Town FC','https://spennymoortownfc.co.uk/'),
(138,'York City FC','https://www.yorkcityfootballclub.co.uk/'),
(139,'Bath City FC','https://www.bathcityfc.com/'),
(140,'Billericay Town FC','https://www.billericaytownfc.co.uk/'),
(141,'Braintree Town FC','https://www.braintreetownfc.org.uk/'),
(142,'Chelmsford City FC','https://www.chelmsfordcityfc.com/'),
(143,'Chippenham Town FC','https://www.pitchero.com/clubs/chippenhamtown/'),
(144,'Concord Rangers FC','https://www.concordrangers.co.uk/'),
(145,'Dartford FC','https://www.dartfordfc.com/'),
(146,'Dorking Wanderers FC','https://www.dorkingwanderers.com/'),
(147,'Dulwich Hamlet FC','https://www.pitchero.com/clubs/dulwichhamlet/'),
(148,'Eastbourne Borough FC','https://www.ebfc.co.uk/'),
(149,'Ebbsfleet United FC','https://www.ebbsfleetunited.co.uk/'),
(150,'Hampton & Richmond Borough FC','https://www.hamptonfc.net/'),
(151,'Havant & Waterlooville FC','https://www.havantandwaterloovillefc.co.uk/'),
(152,'Hemel Hempstead Town FC','https://www.hemelfc.com/'),
(153,'Hungerford Town FC','https://www.hungerfordtown.com/'),
(154,'Maidstone United FC','https://www.maidstoneunited.co.uk/'),
(155,'Oxford City FC','https://oxfordcityfc.co.uk/'),
(156,'Slough Town FC','https://www.sloughtownfc.net/'),
(157,'St Albans City FC','https://www.stalbanscityfc.com/'),
(158,'Tonbridge Angels FC','https://www.tonbridgeangels.co.uk/'),
(159,'Welling United FC','https://www.wellingunited.com/'),
--Scottish clubs
(160,'Aberdeen FC','https://www.afc.co.uk/'),
(161,'Celtic FC','https://www.celticfc.com/'),
(162,'Dundee United FC','https://www.dundeeunitedfc.co.uk/'),
(163,'Hamilton Academical FC','https://www.hamiltonacciesfc.co.uk/'),
(164,'Hibernian FC','https://www.hibernianfc.co.uk/'),
(165,'Kilmarnock FC','https://kilmarnockfc.co.uk/'),
(166,'Livingston FC','https://livingstonfc.co.uk/'),
(167,'Motherwell FC','https://www.motherwellfc.co.uk/'),
(168,'Rangers FC','https://www.rangers.co.uk/'),
(169,'Ross County FC','https://www.rosscountyfootballclub.co.uk/'),
(170,'St Johnstone FC','https://www.perthstjohnstonefc.co.uk/'),
(171,'St Mirren FC','https://www.stmirren.com/'),
(172,'Alloa Athletic FC','https://www.alloaathletic.co.uk/'),
(173,'Arbroath FC','https://www.arbroathfc.co.uk/'),
(174,'Ayr United FC','https://ayrunitedfc.co.uk/'),
(175,'Dundee FC','https://dundeefc.co.uk/'),
(176,'Dunfermline Athletic FC','https://www.dafc.co.uk/'),
(177,'Greencock Morton FC','https://www.gmfc.net/'),
(178,'Heart of Midlothian FC','https://www.heartsfc.co.uk/'),
(179,'Inverness Caledonian Thistle FC','https://ictfc.com/'),
(180,'Queen of the South FC','https://www.qosfc.com/'),
(181,'Raith Rovers FC','https://www.raithrovers.net/'),
(182,'Airdrieonians FC','https://www.airdriefc.com/'),
(183,'Clyde FC','https://www.clydefc.co.uk/'),
(184,'Cove Rangers FC','https://coverangersfc.com/'),
(185,'Dumbarton FC','https://www.dumbartonfootballclub.com/'),
(186,'East Fife FC','https://eastfifefc.info/'),
(187,'Falkirk FC','https://www.falkirkfc.co.uk/'),
(188,'Forfar Athletic FC ','https://www.forfarathletic.co.uk/'),
(189,'Montrose FC','https://www.montrosefc.co.uk/'),
(190,'Partick Thistle FC','https://ptfc.co.uk/'),
(191,'Peterhead FC','https://www.peterheadfc.org/'),
(192,'Albion Rovers FC','https://albionroversfc.com/'),
(193,'Annan Athletic FC','https://www.annanathleticfc.com/'),
(194,'Brechin City FC','https://www.brechincity.com/bcfc/p'),
(195,'Cowdenbeath FC','https://www.cowdenbeathfc.com/'),
(196,'Edinburgh City FC','https://www.edinburghcityfc.com/'),
(197,'Elgin City FC','https://www.elgincity.net'),
(198,'Stenhousemuir FC','https://www.stenhousemuirfc.com'),
(199,'Stirling Albion FC','https://www.stirlingalbionfc.co.uk'),
(200,'Stranraer FC','https://stranraerfc.org'),
(201,'Queen''s Park FC','https://queensparkfc.co.uk'),
(202,'Samaritans','https://www.samaritans.org')

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
SELECT @DisplayName ='talkSPORT'
DECLARE @talkSport int

IF NOT Exists(SELECT Id FROM [CMS].[WhiteLabelSettings] WHERE [DisplayName] = @DisplayName)
BEGIN
	--### White label missing need to add TalkSPORT white label ##
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
	DECLARE @SiteOwner int = 1

	SELECT @Name ='talkSPORT Rewards'
	SELECT @URL ='http://talksportrewards.com' --<<< LIVE URL
	SELECT @Slug ='talksport'
	SELECT @CompanyNumber ='11616720'
	SELECT @CSSFile ='talksport.css'
	SELECT @Logo ='logo.png'
	SELECT @NewsletterLogo = 'logo_newsletter.png'
	SELECT @ClaimsEmail ='claims@exclusiverewards.co.uk'
	SELECT @HelpEmail ='help-me@exclusiverewards.co.uk'
	SELECT @MainEmail ='enquiries@exclusiverewards.co.uk'
	SELECT @Address ='talkSPORT, 1 London Bridge Street, London, SE1 9GF, United Kingdom.'
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

	SELECT @talkSport = SCOPE_IDENTITY()

	IF NOT EXISTS
	(SELECT [Id] FROM [CMS].[WebsiteSocialMediaLink] WHERE [WhiteLabelSettingsId] = @talkSport)
	BEGIN
		INSERT [CMS].[WebsiteSocialMediaLink]
		([CountryCode],[SocialMediaCompanyId],[SocialMediaURI],[WhiteLabelSettingsId])
		VALUES
		 ('GB', @Instagram,'https://www.instagram.com/talksport/', @talkSport),
		 ('GB', @FaceBook,'https://www.facebook.com/talkSPORT/', @talkSport),
		 ('GB', @Twitter,'https://twitter.com/talkSPORT', @talkSport),
		 ('GB', @YouTube,'https://www.youtube.com/talkSPORT', @talkSport)
	END

--##Add to Newsletter

DECLARE @NewsletterId INT
SELECT @NewsletterId = Id FROM [Marketing].[Newsletter]

-- Add Campaign Data
DECLARE @CampaignId int

  IF NOT EXISTS(Select * FROM [Marketing].[Campaigns] WHERE [WhiteLabelId] = @talkSport)
  BEGIN
    INSERT INTO [Marketing].[Campaigns]
    (
        [WhiteLabelId], [ContactListId], [ContactListName], [CampaignId], [CampaignName], [SenderId], [Enabled]
    )
    VALUES
    (
        @talkSport, NULL, REPLACE(@Name, ' ', '') + '_ContactList',
        NULL, REPLACE(@Name, ' ', '') + '_Campaign', 
        1068141, 1
    )
    SELECT @CampaignId = SCOPE_IDENTITY()
  END
  ELSE
  BEGIN
        SELECT @CampaignId = Id FROM [Marketing].[Campaigns] WHERE [WhiteLabelId] = @talkSport
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
	SELECT @talkSport = Id FROM [CMS].[WhiteLabelSettings] WHERE [DisplayName] = @DisplayName
END
-- #### Check white label exists ####

-- #### Check membership plan exists
DECLARE @BeneficiaryPlanId int
IF NOT Exists(SELECT Id FROM [Exclusive].[MembershipPlan] WHERE [Description] = 'talkSPORT Beneficiary Rewards Plan')
BEGIN

	/*
	TalkSPORT registration code creation - both Standard & Beneficiary 
	*/

	-- Editing
	Declare @WhiteLabelName nvarchar(max) = 'talkSPORT Rewards' --<< [CMS].[WhiteLabelSettings].[Name]
	Declare @cardProvider Varchar(max) = 'talkSPORT'
	--Edit these values for Standard plans
	Declare @StandardRegistrationCode Varchar(max) = 'TSS2020'
	Declare @StandardCustomerCashbackPercentage INT = 81
	Declare @StandardDeductionPercentage INT = 19
	--Edit these values for Beneficiary plans
	Declare @BeneficiaryRegistrationCode Varchar(max) = 'TSB2020'
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

	  INSERT Exclusive.MembershipPlanPaymentProvider (MEmbershipPlanId, PaymentProviderId, OneOffPaymentRef)
		VALUES (@PlanId, 2, 'ZRFM9WT7LQE9A')
		SELECT 'New standard plan added'
	END

	-- Add Standard Diamond Plan if needed
	IF NOT EXISTS( SELECT Id FROM [Exclusive].[MembershipPlan] WHERE [Description] = @CardProvider + ' Standard Rewards Diamond Upgrade Plan')
	BEGIN

	  DECLARE @DiamondPlanID INT = null
	  INSERT [Exclusive].[MembershipPlan] ([PartnerId], [MembershipPlanTypeId], [Duration] ,[NumberOfCards]  ,[ValidFrom]  ,[ValidTo]   ,[CustomerCardPrice]  ,[PartnerCardPrice]
		  ,[CustomerCashbackPercentage]  ,[DeductionPercentage]  ,[IsDeleted]  ,[CurrencyCode]   ,[Description]   ,[IsActive]  ,[MembershipLevelId]   ,[PaidByEmployer]   ,[MinimumValue]
		  ,[PaymentFee]   ,[CardProviderId],[WhitelabelId] )
		VALUES (1, 4, 36500, 100000000, GetDate(), '31 Dec 2120', 0, 0, 90, 10, 0, 'GBP', @CardProvider + ' Standard Rewards Diamond Upgrade Plan', 1, 2, 0, 10, 1, @CardProviderId, @WhiteLabelId)
	  SELECT @DiamondPlanID = SCOPE_IDENTITY()

	 -- INSERT Exclusive.MembershipRegistrationCode (MembershipPlanId,[RegistartionCode] , ValidFrom, ValidTo, NumberOfCards, IsActive, IsDeleted)
		--VALUES (@DiamondPlanID, @StandardRegistrationCode, GetDate(), '31 Dec 2120', 100000000, 1, 0)

	  INSERT Exclusive.MembershipPlanPaymentProvider (MEmbershipPlanId, PaymentProviderId, OneOffPaymentRef)
		VALUES (@DiamondPlanID, 2, 'ZRFM9WT7LQE9A')
		SELECT 'New diamond standard plan added'
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
	IF NOT EXISTS( SELECT Id FROM [Exclusive].[MembershipPlan] WHERE [Description] = @CardProvider + ' Beneficiary Rewards Diamond Upgrade Plan')
	BEGIN

	  DECLARE @BeneficiaryDiamondPlanID INT = null
	  INSERT [Exclusive].[MembershipPlan] ([PartnerId], [MembershipPlanTypeId], [Duration] ,[NumberOfCards]  ,[ValidFrom]  ,[ValidTo]   ,[CustomerCardPrice]  ,[PartnerCardPrice]
		  ,[CustomerCashbackPercentage]  ,[DeductionPercentage]  ,[IsDeleted]  ,[CurrencyCode]   ,[Description]   ,[IsActive]  ,[MembershipLevelId]   ,[PaidByEmployer]   ,[MinimumValue]
		  ,[PaymentFee]   ,[CardProviderId], [BenefactorPercentage],[WhitelabelId] )

	  VALUES (1, 3, 36500, 100000000, GetDate(), '01 Jan 2120', 0, 0, 65, 10, 0, 'GBP', @CardProvider + ' Beneficiary Rewards Diamond Upgrade Plan', 1, 2, 0, 10, 1, @CardProviderId, @BeneficiaryPercentage, @WhiteLabelId)
	  SELECT @BeneficiaryDiamondPlanID = SCOPE_IDENTITY()

	  --INSERT Exclusive.MembershipRegistrationCode (MembershipPlanId,[RegistartionCode] , ValidFrom, ValidTo, NumberOfCards, IsActive, IsDeleted)
	  --VALUES (@BeneficiaryDiamondPlanID, @BeneficiaryRegistrationCode, GetDate(), '01 Jan 2120', 100000000, 1, 0)

	  INSERT Exclusive.MembershipPlanPaymentProvider (MEmbershipPlanId, PaymentProviderId, OneOffPaymentRef)
	  VALUES (@BeneficiaryDiamondPlanID, 2, 'ZRFM9WT7LQE9A')
	  SELECT 'New beneficiary plan added'
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

	SElect P.Description,   PP.* 
	from [Exclusive].[MembershipPlanPaymentProvider] PP
	inner join Exclusive.MembershipPlan P ON PP.MembershipPlanId = P.Id
	where p.IsActive = 1
	order by p.id desc

END
ELSE
BEGIN
	SELECT @BeneficiaryPlanId = Id FROM [Exclusive].[MembershipPlan] WHERE [Description] = 'talkSPORT Beneficiary Rewards Plan'
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
(1,1,'Arsenal FC','#F00000','#FFFFFF',202,1,1,@talkSport,@BeneficiaryPlanId),
(2,1,'Aston Villa FC','#670E36','#95BFE5',202,1,1,@talkSport,@BeneficiaryPlanId),
(3,1,'Brighton & Hove Albion FC','#0054A6','#FFFFFF',202,1,1,@talkSport,@BeneficiaryPlanId),
(4,1,'Burnley FC','#81204C','#8FD2F4',202,1,1,@talkSport,@BeneficiaryPlanId),
(5,1,'Chelsea FC','#001489','#FFFFFF',202,1,1,@talkSport,@BeneficiaryPlanId),
(6,1,'Crystal Palace FC','#0055A5','#EE3E24',202,1,1,@talkSport,@BeneficiaryPlanId),
(7,1,'Everton FC','#00369C','#FFFFFF',202,1,1,@talkSport,@BeneficiaryPlanId),
(8,1,'Fulham FC','#FFFFFF','#000000',202,1,1,@talkSport,@BeneficiaryPlanId),
(9,1,'Leeds United','#FFFFFF','#1D4189',202,1,1,@talkSport,@BeneficiaryPlanId),
(10,1,'Leicester City FC','#0000EE','#FFFFFF',202,1,1,@talkSport,@BeneficiaryPlanId),
(11,1,'Liverpool FC','#E31B23','#FFFFFF',202,1,1,@talkSport,@BeneficiaryPlanId),
(12,1,'Manchester City FC','#98C5E9','#FFFFFF',202,1,1,@talkSport,@BeneficiaryPlanId),
(13,1,'Manchester United FC','#C70101','#1A1A1A',202,1,1,@talkSport,@BeneficiaryPlanId),
(14,1,'Newcastle United FC','#FFFFFF','#000000',202,1,1,@talkSport,@BeneficiaryPlanId),
(15,1,'Sheffield United FC','#E30613','#FFFFFF',202,1,1,@talkSport,@BeneficiaryPlanId),
(16,1,'Southampton FC','#FF0028','#FFFFFF',202,1,1,@talkSport,@BeneficiaryPlanId),
(17,1,'Tottenham Hotspur FC','#0B0E1E','#FFFFFF',202,1,1,@talkSport,@BeneficiaryPlanId),
(18,1,'West Bromwich Albion FC','#002F68','#FFFFFF',202,1,1,@talkSport,@BeneficiaryPlanId),
(19,1,'West Ham United FC','#7D2B3A','#59B3E4',202,1,1,@talkSport,@BeneficiaryPlanId),
(20,1,'Wolverhampton Wanderers FC','#FDB913','#000000',202,1,1,@talkSport,@BeneficiaryPlanId),
(21,2,'AFC Bournemouth','#D71921','#000000',21,1,1,@talkSport,@BeneficiaryPlanId),
(22,2,'Barnsley FC','#EF3340','#FFFFFF',22,1,1,@talkSport,@BeneficiaryPlanId),
(23,2,'Birmingham City FC','#11437E','#FFFFFF',23,1,1,@talkSport,@BeneficiaryPlanId),
(24,2,'Blackburn Rovers FC','#003DA5','#FFFFFF',24,1,1,@talkSport,@BeneficiaryPlanId),
(25,2,'Brentford FC','#C61D23','#FFFFFF',25,1,1,@talkSport,@BeneficiaryPlanId),
(26,2,'Bristol City FC','#E21A23','#FFFFFF',26,1,1,@talkSport,@BeneficiaryPlanId),
(27,2,'Cardiff City FC','#0C2C79','#FFFFFF',27,1,1,@talkSport,@BeneficiaryPlanId),
(28,2,'Coventry City FC','#69B3E7','#1B365D',28,1,1,@talkSport,@BeneficiaryPlanId),
(29,2,'Derby County  FC','#FFFFFF','#000000',29,1,1,@talkSport,@BeneficiaryPlanId),
(30,2,'Huddersfield Town AFC','#0072CE','#FFFFFF',30,1,1,@talkSport,@BeneficiaryPlanId),
(31,2,'Luton Town FC','#FA4616','#1B365D',31,1,1,@talkSport,@BeneficiaryPlanId),
(32,2,'Middlesbrough FC','#D5002E','#FFFFFF',32,1,1,@talkSport,@BeneficiaryPlanId),
(33,2,'Millwall FC','#001234','#FFFFFF',33,1,1,@talkSport,@BeneficiaryPlanId),
(34,2,'Norwich City FC','#FFF200','#00A651',34,1,1,@talkSport,@BeneficiaryPlanId),
(35,2,'Nottingham Forest FC','#C8102E','#FFFFFF',35,1,1,@talkSport,@BeneficiaryPlanId),
(36,2,'Preston North End FC','#FFFFFF','#004B87',36,1,1,@talkSport,@BeneficiaryPlanId),
(37,2,'Queens Park Rangers FC','#0E4D8B','#FFFFFF',37,1,1,@talkSport,@BeneficiaryPlanId),
(38,2,'Reading FC','#0032B9','#FFFFFF',38,1,1,@talkSport,@BeneficiaryPlanId),
(39,2,'Rotherham United FC','#DA291C','#FFFFFF',39,1,1,@talkSport,@BeneficiaryPlanId),
(40,2,'Sheffield Wednesday FC','#0033A0','#FFFFFF',40,1,1,@talkSport,@BeneficiaryPlanId),
(41,2,'Stoke City FC','#D7172F','#FFFFFF',41,1,1,@talkSport,@BeneficiaryPlanId),
(42,2,'Swansea City AFC','#FFFFFF','#000000',42,1,1,@talkSport,@BeneficiaryPlanId),
(43,2,'Watford FC','#000000','#FCF000',43,1,1,@talkSport,@BeneficiaryPlanId),
(44,2,'Wycombe Wanderers FC','#62B5E5','#0C2340',44,1,1,@talkSport,@BeneficiaryPlanId),
(45,3,'Accrington Stanley FC','#BC0C04','#FFFFFF',45,1,1,@talkSport,@BeneficiaryPlanId),
(46,3,'AFC Wimbledon','#0050B5','#FCE300',46,1,1,@talkSport,@BeneficiaryPlanId),
(47,3,'Blackpool FC','#FE5000','#FFFFFF',47,1,1,@talkSport,@BeneficiaryPlanId),
(48,3,'Bristol Rovers FC','#0033A0','#FFFFFF',48,1,1,@talkSport,@BeneficiaryPlanId),
(49,3,'Burton Albion FC','#FFFA05','#333333',49,1,1,@talkSport,@BeneficiaryPlanId),
(50,3,'Charlton Athletic FC','#D0011B','#FFFFFF',50,1,1,@talkSport,@BeneficiaryPlanId),
(51,3,'Crewe Alexandra FC','#DA291C','#FFFFFF',51,1,1,@talkSport,@BeneficiaryPlanId),
(52,3,'Doncaster Rovers FC','#DA291C','#FFFFFF',52,1,1,@talkSport,@BeneficiaryPlanId),
(53,3,'Fleetwood Town FC','#DA291C','#FFFFFF',53,1,1,@talkSport,@BeneficiaryPlanId),
(54,3,'Gillingham FC','#0C2340','#FFFFFF',54,1,1,@talkSport,@BeneficiaryPlanId),
(55,3,'Hull City FC','#EF8903','#000000',55,1,1,@talkSport,@BeneficiaryPlanId),
(56,3,'Ipswich Town FC','#0033A0','#FFFFFF',56,1,1,@talkSport,@BeneficiaryPlanId),
(57,3,'Lincoln City FC','#DA0000','#FFFFFF',57,1,1,@talkSport,@BeneficiaryPlanId),
(58,3,'Milton Keynes Dons FC','#FFFFFF','#C5B08B',58,1,1,@talkSport,@BeneficiaryPlanId),
(59,3,'Northampton Town FC','#862633','#FFFFFF',59,1,1,@talkSport,@BeneficiaryPlanId),
(60,3,'Oxford United FC','#FFD700','#00205B',60,1,1,@talkSport,@BeneficiaryPlanId),
(61,3,'Peterborough United FC','#0056B8','#FFFFFF',61,1,1,@talkSport,@BeneficiaryPlanId),
(62,3,'Plymouth Argyle FC','#034638','#FFFFFF',62,1,1,@talkSport,@BeneficiaryPlanId),
(63,3,'Portsmouth FC','#0033A0','#FFFFFF',63,1,1,@talkSport,@BeneficiaryPlanId),
(64,3,'Rochdale AFC','#10069F','#000000',64,1,1,@talkSport,@BeneficiaryPlanId),
(65,3,'Shrewsbury Town FC','#0047BB','#F2A900',65,1,1,@talkSport,@BeneficiaryPlanId),
(66,3,'Sunderland AFC','#DC0814','#FFFFFF',66,1,1,@talkSport,@BeneficiaryPlanId),
(67,3,'Swindon Town FC','#D22630','#FFFFFF',67,1,1,@talkSport,@BeneficiaryPlanId),
(68,3,'Wigan Athletic FC','#005CA6','#FFFFFF',68,1,1,@talkSport,@BeneficiaryPlanId),
(69,4,'Barrow AFC','#255AA8','#FFFFFF',69,1,1,@talkSport,@BeneficiaryPlanId),
(70,4,'Bolton Wanderers FC','#FFFFFF','#00205B',70,1,1,@talkSport,@BeneficiaryPlanId),
(71,4,'Bradford City AFC','#75263B','#F7A30A',71,1,1,@talkSport,@BeneficiaryPlanId),
(72,4,'Cambridge United FC','#FFA300','#333333',72,1,1,@talkSport,@BeneficiaryPlanId),
(73,4,'Carlisle United FC','#0047BB','#C8102E',73,1,1,@talkSport,@BeneficiaryPlanId),
(74,4,'Cheltenham Town FC','#DA291C','#FFFFFF',74,1,1,@talkSport,@BeneficiaryPlanId),
(75,4,'Colchester United FC','#004B87','#FFFFFF',75,1,1,@talkSport,@BeneficiaryPlanId),
(76,4,'Crawley Town FC','#A6192E','#FFFFFF',76,1,1,@talkSport,@BeneficiaryPlanId),
(77,4,'Exeter City FC','#DA291C','#FFFFFF',77,1,1,@talkSport,@BeneficiaryPlanId),
(78,4,'Forest Green Rovers FC','#99CC33','#000000',78,1,1,@talkSport,@BeneficiaryPlanId),
(79,4,'Grimsby Town FC','#333333','#FF0000',79,1,1,@talkSport,@BeneficiaryPlanId),
(80,4,'Harrogate Town AFC','#FFDB22','#000000',80,1,1,@talkSport,@BeneficiaryPlanId),
(81,4,'Leyton Orient FC','#B00B00','#FFFFFF',81,1,1,@talkSport,@BeneficiaryPlanId),
(82,4,'Mansfield Town FC','#F2A900','#0047BB',82,1,1,@talkSport,@BeneficiaryPlanId),
(83,4,'Morecambe FC','#BB3441','#FFFFFF',83,1,1,@talkSport,@BeneficiaryPlanId),
(84,4,'Newport County AFC','#FF9E1B','#333333',84,1,1,@talkSport,@BeneficiaryPlanId),
(85,4,'Oldham Athletic AFC','#0054A6','#FFFFFF',85,1,1,@talkSport,@BeneficiaryPlanId),
(86,4,'Port Vale FC','#FFFFFF','#000000',86,1,1,@talkSport,@BeneficiaryPlanId),
(87,4,'Salford City FC','#B20016','#FFFFFF',87,1,1,@talkSport,@BeneficiaryPlanId),
(88,4,'Scunthorpe United FC','#862633','#69B3E7',88,1,1,@talkSport,@BeneficiaryPlanId),
(89,4,'Southend United FC','#002248','#FFFFFF',89,1,1,@talkSport,@BeneficiaryPlanId),
(90,4,'Stevenage FC','#AC172B','#FFFFFF',90,1,1,@talkSport,@BeneficiaryPlanId),
(91,4,'Tranmere Rovers FC','#FFFFFF','#001489',91,1,1,@talkSport,@BeneficiaryPlanId),
(92,4,'Walsall FC','#DA291C','#FFFFFF',92,1,1,@talkSport,@BeneficiaryPlanId),
(93,5,'Aldershot Town FC','#FD433A','#163F83',93,1,1,@talkSport,@BeneficiaryPlanId),
(94,5,'Altrincham FC','#C3141E','#FFFFFF',94,1,1,@talkSport,@BeneficiaryPlanId),
(95,5,'Barnet FC','#F07D04','#000000',95,1,1,@talkSport,@BeneficiaryPlanId),
(96,5,'Boreham wood FC','#000000','#FFFFFF',96,1,1,@talkSport,@BeneficiaryPlanId),
(97,5,'Bromley FC','#FFFFFF','#000000',97,1,1,@talkSport,@BeneficiaryPlanId),
(98,5,'Chesterfield FC','#004990','#FFFFFF',98,1,1,@talkSport,@BeneficiaryPlanId),
(99,5,'Dagenham and Redbridge FC','#ED251B','#111D6E',99,1,1,@talkSport,@BeneficiaryPlanId),
(100,5,'Dover Athletic FC','#FFFFFF','#000000',100,1,1,@talkSport,@BeneficiaryPlanId),
(101,5,'Eastleigh FC','#0D0D39','#FFFFFF',101,1,1,@talkSport,@BeneficiaryPlanId),
(102,5,'FC Halifax Town','#0361AA','#FFFFFF',102,1,1,@talkSport,@BeneficiaryPlanId),
(103,5,'Hartlepool United FC','#204185','#FFFFFF',103,1,1,@talkSport,@BeneficiaryPlanId),
(104,5,'Kings Lynn Town FC','#365492','#D5AC3A',104,1,1,@talkSport,@BeneficiaryPlanId),
(105,12,'Marine FC','#f5f5f5','#1a1a1a',105,1,1,@talkSport,@BeneficiaryPlanId), --######
(106,5,'Maidenhead United FC','#FFFFFF','#000000',106,1,1,@talkSport,@BeneficiaryPlanId),
(107,5,'Notts County FC','#84754E','#000000',107,1,1,@talkSport,@BeneficiaryPlanId),
(108,5,'Solihull Moors FC','#F8EA10','#00204D',108,1,1,@talkSport,@BeneficiaryPlanId),
(109,5,'Stockport County FC','#194F90','#FFFFFF',109,1,1,@talkSport,@BeneficiaryPlanId),
(110,5,'Sutton United FC','#FDC623','#363533',110,1,1,@talkSport,@BeneficiaryPlanId),
(111,5,'Torquay United FC','#FFD000','#1553B7',111,1,1,@talkSport,@BeneficiaryPlanId),
(112,5,'Wealdstone FC','#2B6CB4','#FFFFFF',112,1,1,@talkSport,@BeneficiaryPlanId),
(113,5,'Weymouth FC','#A8012E','#4CA8FF',113,1,1,@talkSport,@BeneficiaryPlanId),
(114,5,'Woking FC','#A41123','#FFFFFF',114,1,1,@talkSport,@BeneficiaryPlanId),
(115,5,'Wrexham AFC','#B11921','#FFFFFF',115,1,1,@talkSport,@BeneficiaryPlanId),
(116,5,'Yeovil Town FC','#009639','#FFFFFF',116,1,1,@talkSport,@BeneficiaryPlanId),
(117,6,'AFC Fylde','#EE3124','#004990',117,1,1,@talkSport,@BeneficiaryPlanId),
(118,6,'AFC Telford','#FFFFFF','#000000',118,1,1,@talkSport,@BeneficiaryPlanId),
(119,6,'Alfreton Town FC','#E31B1B','#FFFFFF',119,1,1,@talkSport,@BeneficiaryPlanId),
(120,6,'Blyth Spartans AFC','#0B8A4D','#404040',120,1,1,@talkSport,@BeneficiaryPlanId),
(121,6,'Boston United FC','#E390E14','#000000',121,1,1,@talkSport,@BeneficiaryPlanId),
(122,6,'Brackley Town FC','#C82731','#FFFFFF',122,1,1,@talkSport,@BeneficiaryPlanId),
(123,6,'Bradford (Park Avenue) AFC','#00AF3A','#FFFFFF',123,1,1,@talkSport,@BeneficiaryPlanId),
(124,6,'Chester FC','#061E49','#FFFFFF',124,1,1,@talkSport,@BeneficiaryPlanId),
(125,6,'Chorley FC','#FFFFFF','#000000',125,1,1,@talkSport,@BeneficiaryPlanId),
(126,6,'Curzon Ashton FC','#02077C','#FFFFFF',126,1,1,@talkSport,@BeneficiaryPlanId),
(127,6,'Darlington FC','#FFFFFF','#000000',127,1,1,@talkSport,@BeneficiaryPlanId),
(128,6,'Farsley Celtic FC','#2E9844','#FFFFFF',128,1,1,@talkSport,@BeneficiaryPlanId),
(129,6,'Gateshead FC','#000000','#FFFFFF',129,1,1,@talkSport,@BeneficiaryPlanId),
(130,6,'Gloucester City AFC','#AB2E33','#F8A41C',130,1,1,@talkSport,@BeneficiaryPlanId),
(131,6,'Guiseley AFC','#0A3D7D','#EDB51C',131,1,1,@talkSport,@BeneficiaryPlanId),
(132,6,'Hereford FC','#FFFFFF','#000000',132,1,1,@talkSport,@BeneficiaryPlanId),
(133,6,'Kettering Town FC','#FE0018','#333333',133,1,1,@talkSport,@BeneficiaryPlanId),
(134,6,'Kidderminster Harriers FC','#A80004','#FFFFFF',134,1,1,@talkSport,@BeneficiaryPlanId),
(135,6,'Leamington FC','#ECBC2E','#353535',135,1,1,@talkSport,@BeneficiaryPlanId),
(136,6,'Southport FC','#F8B236','#181C25',136,1,1,@talkSport,@BeneficiaryPlanId),
(137,6,'Spennymoor Town FC','#000000','#FFFFFF',137,1,1,@talkSport,@BeneficiaryPlanId),
(138,6,'York City FC','#E32232','#203A76',138,1,1,@talkSport,@BeneficiaryPlanId),
(139,7,'Bath City FC','#FFFFFF','#000000',139,1,1,@talkSport,@BeneficiaryPlanId),
(140,7,'Billericay Town FC','#1518C7','#FFFFFF',140,1,1,@talkSport,@BeneficiaryPlanId),
(141,7,'Braintree Town FC','#FE7600','#0000EE',141,1,1,@talkSport,@BeneficiaryPlanId),
(142,7,'Chelmsford City FC','#770020','#FFFFFF',142,1,1,@talkSport,@BeneficiaryPlanId),
(143,7,'Chippenham Town FC','#2D4AA1','#FFFFFF',143,1,1,@talkSport,@BeneficiaryPlanId),
(144,7,'Concord Rangers FC','#F5FE05','#0A51BD',144,1,1,@talkSport,@BeneficiaryPlanId),
(145,7,'Dartford FC','#FFFFFF','#000000',145,1,1,@talkSport,@BeneficiaryPlanId),
(146,7,'Dorking Wanderers FC','#F01111','#212165',146,1,1,@talkSport,@BeneficiaryPlanId),
(147,7,'Dulwich Hamlet FC','#0D0080','#FF69B4',147,1,1,@talkSport,@BeneficiaryPlanId),
(148,7,'Eastbourne Borough FC','#CB0817','#000000',148,1,1,@talkSport,@BeneficiaryPlanId),
(149,7,'Ebbsfleet United FC','#F61C0E','#000000',149,1,1,@talkSport,@BeneficiaryPlanId),
(150,7,'Hampton & Richmond Borough FC','#C8102E','#041E42',150,1,1,@talkSport,@BeneficiaryPlanId),
(151,7,'Havant & Waterlooville FC','#FEDA00','#123A7C',151,1,1,@talkSport,@BeneficiaryPlanId),
(152,7,'Hemel Hempstead Town FC','#FF0000','#FFFFFF',152,1,1,@talkSport,@BeneficiaryPlanId),
(153,7,'Hungerford Town FC','#FFFFFF','#000000',153,1,1,@talkSport,@BeneficiaryPlanId),
(154,7,'Maidstone United FC','#F1A000','#262626',154,1,1,@talkSport,@BeneficiaryPlanId),
(155,7,'Oxford City FC','#1126AC','#FFFFFF',155,1,1,@talkSport,@BeneficiaryPlanId),
(156,7,'Slough Town FC','#000C34','#EFAF12',156,1,1,@talkSport,@BeneficiaryPlanId),
(157,7,'St Albans City FC','#1E3B71','#FFD206',157,1,1,@talkSport,@BeneficiaryPlanId),
(158,7,'Tonbridge Angels FC','#005C9E','#FFFFFF',158,1,1,@talkSport,@BeneficiaryPlanId),
(159,7,'Welling United FC','#C40000','#FFFFFF',159,1,1,@talkSport,@BeneficiaryPlanId),
--Scottish Clubs
(160,8,'Aberdeen FC','#CC0000','#FFFFFF',160,1,1,@talkSport,@BeneficiaryPlanId),
(161,8,'Celtic FC','#009B48','#FFFFFF',161,1,1,@talkSport,@BeneficiaryPlanId),
(162,8,'Dundee United FC','#FF6600','#444444',162,1,1,@talkSport,@BeneficiaryPlanId),
(163,8,'Hamilton Academical FC','#C3333D','#FFFFFF',163,1,1,@talkSport,@BeneficiaryPlanId),
(164,8,'Hibernian FC','#00762A','#FFFFFF',164,1,1,@talkSport,@BeneficiaryPlanId),
(165,8,'Kilmarnock FC','#0C45C4','#FFFFFF',165,1,1,@talkSport,@BeneficiaryPlanId),
(166,8,'Livingston FC','#FFCC00','#000000',166,1,1,@talkSport,@BeneficiaryPlanId),
(167,8,'Motherwell FC','#65263B','#FEA52B',167,1,1,@talkSport,@BeneficiaryPlanId),
(168,8,'Rangers FC','#0033A0','#FFFFFF',168,1,1,@talkSport,@BeneficiaryPlanId),
(169,8,'Ross County FC','#272753','#FFFFFF',169,1,1,@talkSport,@BeneficiaryPlanId),
(170,8,'St Johnstone FC','#337AB7','#FFFFFF',170,1,1,@talkSport,@BeneficiaryPlanId),
(171,8,'St Mirren FC','#000000','#FFFFFF',171,1,1,@talkSport,@BeneficiaryPlanId),
(172,9,'Alloa Athletic FC','#FF991A','#0A0A0A',172,1,1,@talkSport,@BeneficiaryPlanId),
(173,9,'Arbroath FC','#9F305B','#FFFFFF',173,1,1,@talkSport,@BeneficiaryPlanId),
(174,9,'Ayr United FC','#000000','#FFFFFF',174,1,1,@talkSport,@BeneficiaryPlanId),
(175,9,'Dundee FC','#172643','#FFFFFF',175,1,1,@talkSport,@BeneficiaryPlanId),
(176,9,'Dunfermline Athletic FC','#252525','#FFFFFF',176,1,1,@talkSport,@BeneficiaryPlanId),
(177,9,'Greencock Morton FC','#FFFFFF','#007DB6',177,1,1,@talkSport,@BeneficiaryPlanId),
(178,9,'Heart of Midlothian FC','#9F1931','#FFFFFF',178,1,1,@talkSport,@BeneficiaryPlanId),
(179,9,'Inverness Caledonian Thistle FC','#002675','#C8102E',179,1,1,@talkSport,@BeneficiaryPlanId),
(180,9,'Queen of the South FC','#094582','#FFFFFF',180,1,1,@talkSport,@BeneficiaryPlanId),
(181,9,'Raith Rovers FC','#1B3458','#1A355A',181,1,1,@talkSport,@BeneficiaryPlanId),
(182,10,'Airdrieonians FC','#DA2128','#FFFFFF',182,1,1,@talkSport,@BeneficiaryPlanId),
(183,10,'Clyde FC','#D12A2E','#000000',183,1,1,@talkSport,@BeneficiaryPlanId),
(184,10,'Cove Rangers FC','#0A40B5','#FFFFFF',184,1,1,@talkSport,@BeneficiaryPlanId),
(185,10,'Dumbarton FC','#FFCC00','#000000',185,1,1,@talkSport,@BeneficiaryPlanId),
(186,10,'East Fife FC','#ECBC2E','#000000',186,1,1,@talkSport,@BeneficiaryPlanId),
(187,10,'Falkirk FC','#002D56','#FFFFFF',187,1,1,@talkSport,@BeneficiaryPlanId),
(188,10,'Forfar Athletic FC ','#0093D2','#FFFFFF',188,1,1,@talkSport,@BeneficiaryPlanId),
(189,10,'Montrose FC','#0066FF','#FFFFFF',189,1,1,@talkSport,@BeneficiaryPlanId),
(190,10,'Partick Thistle FC','#000000','#FFFFFF',190,1,1,@talkSport,@BeneficiaryPlanId),
(191,10,'Peterhead FC','#30499C','#FFFFFF',191,1,1,@talkSport,@BeneficiaryPlanId),
(192,11,'Albion Rovers FC','#000000','#EDD500',192,1,1,@talkSport,@BeneficiaryPlanId),
(193,11,'Annan Athletic FC','#FAD103','#000000',193,1,1,@talkSport,@BeneficiaryPlanId),
(194,11,'Brechin City FC','#E2001A','#FFFFFF',194,1,1,@talkSport,@BeneficiaryPlanId),
(195,11,'Cowdenbeath FC','#003C74','#FFFFFF',195,1,1,@talkSport,@BeneficiaryPlanId),
(196,11,'Edinburgh City FC','#FFFFFF','#000000',196,1,1,@talkSport,@BeneficiaryPlanId),
(197,11,'Elgin City FC','#000000','#FFFFFF',197,1,1,@talkSport,@BeneficiaryPlanId),
(198,11,'Stenhousemuir FC','#7C0101','#F79324',198,1,1,@talkSport,@BeneficiaryPlanId),
(199,11,'Stirling Albion FC','#E31E21','#FFFFFF',199,1,1,@talkSport,@BeneficiaryPlanId),
(200,11,'Stranraer FC','#CE0000','#FFFFFF',200,1,1,@talkSport,@BeneficiaryPlanId),
(201,11,'Queen''s Park FC','#000000','#FFFFFF',201,1,1,@talkSport,@BeneficiaryPlanId)

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
