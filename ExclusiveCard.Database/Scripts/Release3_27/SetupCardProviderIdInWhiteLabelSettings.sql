-- THIS IS OBSOLETE,  CAN BE BINNED

--Live Scripts to Update CardProviderId in WhiteLabel Settings for following Matched Partner

DECLARE @PartnerName nvarchar(100);
DECLARE @PartnerId int;
DECLARE @WhiteLabelName nvarchar(255);
DECLARE @WhiteLabelId int;
DECLARE @DefaultPartnerId int
DECLARE @Count int;
DECLARE @i int = 1;


DECLARE @TempWhiteLabelPartner table
(
	Id int not null,
	WhiteLabelName nvarchar(255),
	PartnerName nvarchar(100)
)
-- Live Value Names of WhiteLabel & Partner
INSERT INTO @TempWhiteLabelPartner([Id], [WhiteLabelName], [PartnerName])
	VALUES(1, 'Exclusive Rewards', 'Exclusive Media');
INSERT INTO @TempWhiteLabelPartner([Id], [WhiteLabelName], [PartnerName])
	VALUES(2, 'Exclusive Consumer Rights', 'ConsumerRightsLive');
--	VALUES(2, 'Exclusive Consumer Rights', 'ConsumerRights'); --<< uat
INSERT INTO @TempWhiteLabelPartner([Id], [WhiteLabelName], [PartnerName])
	VALUES(3, 'CAW Digital Rewards', 'CAW Digital Rewards');
INSERT INTO @TempWhiteLabelPartner([Id], [WhiteLabelName], [PartnerName])
	VALUES(4, 'Marine FC Rewards', 'Marine FC Rewards');
INSERT INTO @TempWhiteLabelPartner([Id], [WhiteLabelName], [PartnerName])
	VALUES(5, 'Lloyd & Co Employee Rewards', 'Lloyd Employment Benefits');
INSERT INTO @TempWhiteLabelPartner([Id], [WhiteLabelName], [PartnerName])
	VALUES(6, 'Community Iron Rewards', 'Braintree Irons Community');
INSERT INTO @TempWhiteLabelPartner([Id], [WhiteLabelName], [PartnerName])
	VALUES(7, 'Orient Rewards', 'Leyton Orient FC');
INSERT INTO @TempWhiteLabelPartner([Id], [WhiteLabelName], [PartnerName])
	VALUES(8, 'Friday Rewards', 'Shireen Friday Standard');
INSERT INTO @TempWhiteLabelPartner([Id], [WhiteLabelName], [PartnerName])
	VALUES(9, 'United By Rewards', 'United By Football');

SELECT TOP(1) @DefaultPartnerId = Id FROM [Exclusive].[Partner] Where [Name] = 'Exclusive Media'
--Default everything to Exclusive...
UPDATE [CMS].[WhiteLabelSettings] SET CardProviderId = @DefaultPartnerId

--SELECT * FROM @TempWhiteLabelPartner
SELECT @Count = COUNT(*) FROM @TempWhiteLabelPartner

-- While Condition to loop through Temp Table
WHILE(@i <= @Count)
BEGIN
	-- Get Partner Name & WhiteLabelName from the Temp Table.
	SELECT @PartnerName = [PartnerName], 
		   @WhiteLabelName = [WhiteLabelName] 
	  FROM @TempWhiteLabelPartner 
	 WHERE Id = @i;
	
	-- Check the Exiting Table for WhiteSettings for the Name
	IF EXISTS(Select * FROM [CMS].[WhiteLabelSettings] WHERE [Name] = @WhiteLabelName AND [URL] NOT LIKE '%localhost%')
	BEGIN 
		-- Check & Get value of White Label
		SELECT TOP 1 @WhiteLabelId = Id FROM [CMS].[WhiteLabelSettings] 
		WHERE [Name] = @WhiteLabelName AND [URL] NOT LIKE '%localhost%';

		SELECT @PartnerId = 0

		IF(@WhiteLabelId IS NOT NULL AND @WhiteLabelId != 0)
		BEGIN
			-- Get PartnerId from the Partner Name
			SELECT @PartnerId = Id FROM [Exclusive].[Partner] Where [Name] = @PartnerName

			IF(@PartnerId IS NULL OR @PartnerId = 0 )
			BEGIN 
				--set to default
				SELECT @PartnerId = @DefaultPartnerId
			END
			--UPDATE CardProviderId for WhiteLabelSettings Table
			UPDATE [CMS].[WhiteLabelSettings] SET CardProviderId = @PartnerId 
			WHERE Id = @WhiteLabelId;
		END
	END
	
	-- Increment i by i + 1 to fetch next record
	SET @i = @i + 1;
	PRINT @i;
END

select * From [CMS].[WhiteLabelSettings]
