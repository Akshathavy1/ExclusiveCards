  BEGIN TRAN
  
  UPDATE [CMS].[WhiteLabelSettings]
  SET [URL] = 'https://manchester.exclusiverewards.co.uk/'
	 ,[Slug] = 'manchester'
	 ,[NewsletterLogo] = 'logo_newsletter.png'
  WHERE [Name] = 'Manchester Rewards'

  --COMMIT TRAN
