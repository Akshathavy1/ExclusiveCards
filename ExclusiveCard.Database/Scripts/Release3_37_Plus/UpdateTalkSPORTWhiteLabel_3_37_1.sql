DECLARE @WhiteLabelName nvarchar(max) ='talkSPORT Rewards'
DECLARE @WhiteLabelId int
DECLARE @Logo nvarchar(max) = 'logo.svg'
DECLARE @URL nvarchar(max) ='https://ExclusiveTalkSportRewards.com' --<<< LIVE URL

SELECT @WhiteLabelId = Id FROM [CMS].[WhiteLabelSettings]
WHERE [Name] = @WhiteLabelName

BEGIN TRAN

UPDATE [CMS].[WhiteLabelSettings]
SET [Logo] = @Logo,
	[URL] = @URL
WHERE Id = @WhiteLabelId


SELECT * FROM [CMS].[WhiteLabelSettings]


--COMMIT TRAN
--ROLLBACK TRAN


