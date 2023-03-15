DECLARE @SouthportId int 
DECLARE @IpswichFCId int 
DECLARE @IpswichRegionId int

SELECT @SouthportId = Id FROM [CMS].[WhiteLabelSettings] WHERE [Name] = 'Southport Rewards'
SELECT @IpswichFCId = Id FROM [CMS].[WhiteLabelSettings] WHERE [Name] = 'Ipswich Town Rewards'
SELECT @IpswichRegionId = Id FROM [CMS].[WhiteLabelSettings] WHERE [Name] = 'Ipswich Rewards'

BEGIN TRAN 

--All records with IsRegional = 0 and are not Ipswich Town FC, 
--set the DefaultRegion = The Id of the Southport regional site (@SouthportId)
UPDATE [CMS].[WhiteLabelSettings]
SET [DefaultRegion] = @SouthportId 
WHERE 
[IsRegional] = 0
AND Id != @IpswichFCId
AND [DefaultRegion] != @SouthportId

--All records with IsRegional = 1, 
--set DefaultRegion = their own Id (these should reference themselves)
UPDATE [CMS].[WhiteLabelSettings]
SET [DefaultRegion] = [Id]
WHERE 
[IsRegional] = 1
AND [DefaultRegion] != [Id]

--Only if the Ipswich regional site exists
IF @IpswichRegionId IS NOT NULL
BEGIN
	--Ipswich Town FC, 
	--set the DefaultRegion = the Id of the new Ipswich regional site
	UPDATE [CMS].[WhiteLabelSettings]
	SET [DefaultRegion] = @IpswichRegionId 
	WHERE 
	[Id] = @IpswichFCId
	AND [DefaultRegion] != @IpswichRegionId
END
ELSE
BEGIN
	PRINT 'Ipswich Regional White label Not found'

	--Ipswich Town FC, 
	--Should not get to here, but set it to Southport for now...
	UPDATE [CMS].[WhiteLabelSettings]
	SET [DefaultRegion] =  @SouthportId
	WHERE 
	[Id] = @IpswichFCId
	AND [DefaultRegion] != @SouthportId
END


--Checks >>
--SELECT * FROM [CMS].[WhiteLabelSettings]

--COMMIT TRAN
--ROLLBACK TRAN
