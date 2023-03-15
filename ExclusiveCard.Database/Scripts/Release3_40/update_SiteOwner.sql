/****** for updating SiteOwmer  to Sport Rewards if exist  ******/
BEGIN TRAN

DECLARE @Description nvarchar(max)
SELECT @Description ='Bullseye'
DECLARE @SiteOwnerId int

IF  Exists(SELECT Id FROM [CMS].[SiteOwner] WHERE [Description] = @Description)
BEGIN 

	SELECT @SiteOwnerId = Id FROM [CMS].[SiteOwner] WHERE [Description] = @Description 

	UPDATE [CMS].[SiteOwner] SET
	[ClanDescription] = '<p>Your Bullseye Exclusive Rewards account enables you to support YOUR chosen Darts organisation in these difficult times by donating 30% of your free cashback directly to your selected organisation.</p>

		<p>You can of course select the standard Bullseye Exclusive Rewards account and no donation is necessary. You will simply retain 100% of your FREE cashback yourself to withdraw to your own bank account.</p>

		<p>Just so you know…..if you choose to donate…every penny of your donation will be sent to your chosen organisation and our aim is to raise as much money as possible to support Darts across the UK.</p>

		<p>If you choose to donate, your organisation will be displayed on your rewards platform and you will be able to see your donated balance on your account page. Together…we can make a difference.</p>'
	WHERE [Id] = @SiteOwnerId   
	
END
ELSE
BEGIN
	SELECT 'Did not find ' + @Description ' in [CMS].[SiteOwner]'
END

--COMMIT TRAN
--ROLLBACK TRAN

