DECLARE @DisplayName nvarchar(max)
DECLARE @Slug nvarchar(max)
DECLARE @Logo nvarchar(max)
DECLARE @NewsletterLogo nvarchar(max)
DECLARE @NewWhiteLabelId int

SELECT @DisplayName = 'Birmingham Rewards'
SELECT @Slug = 'birmingham'
SELECT @Logo = 'logo.svg' 
SELECT @NewsletterLogo = 'birmingham.png'

BEGIN TRAN
SELECT @NewWhiteLabelId = ID FROM [CMS].[WhiteLabelSettings]
WHERE DISPLAYNAME = @DisplayName

UPDATE [CMS].[WhiteLabelSettings]
SET Slug = @Slug, logo = @Logo, NewsletterLogo = @NewsletterLogo
WHERE id = @NewWhiteLabelId


SELECT * FROM [CMS].[WhiteLabelSettings] WHERE DISPLAYNAME = @DisplayName


--COMMIT TRAN

--ROLLBACK TRAN
