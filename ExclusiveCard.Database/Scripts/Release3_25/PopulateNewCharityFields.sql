BEGIN TRAN

CREATE TABLE #tmp_Updates
(
[Name] NVARCHAR(255),
SiteType int,
CharityName NVARCHAR(255),
CharityUrl NVARCHAR(512)
)
INSERT INTO #tmp_Updates
([Name],SiteType,CharityName,CharityUrl)
VALUES
 ('Orient Rewards',1,'Leyton Orient FC', 'https://www.leytonorient.com/')
,('Marine FC Rewards',1, 'Marine FC', 'https://www.marinefc.com/')
,('Community Iron Rewards',1, 'Braintree Community Iron', 'https://www2.communityiron.co.uk/')
,('United By Rewards',1, 'The Grass Roots Football Fund', 'https://www.unitedbyfootballuk.co.uk/')

--Update all the beneficiary white labels
UPDATE [CMS].[WhiteLabelSettings] 
SET SiteType = #tmp_Updates.SiteType, 
	CharityName = #tmp_Updates.CharityName,
	CharityUrl = #tmp_Updates.CharityUrl
FROM #tmp_Updates
WHERE [CMS].[WhiteLabelSettings].[Name] = #tmp_Updates.[Name]

--Set the site type for the rest of the whitle labels to standard accounts
UPDATE [CMS].[WhiteLabelSettings] 
SET SiteType = 0 WHERE SiteType IS NULL

SELECT * FROM [CMS].[WhiteLabelSettings]

DROP TABLE #tmp_Updates

--COMMIT TRAN
--ROLLBACK TRAN