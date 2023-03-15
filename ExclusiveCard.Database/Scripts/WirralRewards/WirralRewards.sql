BEGIN TRAN

UPDATE [CMS].[WhiteLabelSettings]
SET 
	[URL] = 'https://wirral.exclusiverewards.co.uk',
	[Slug] = 'wirral-rewards',
	[Logo] = 'logo.svg',
	[NewsletterLogo] = 'wirral.png'
WHERE [Name] = 'Wirral Rewards'


SELECT *
  FROM [CMS].[WhiteLabelSettings]
  WHERE [Name] = 'Wirral Rewards'


--COMMIT TRAN
--ROLLBACK TRAN
