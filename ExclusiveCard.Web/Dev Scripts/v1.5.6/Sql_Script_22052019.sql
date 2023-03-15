IF NOT EXISTS (SELECT * FROM ExclusiveCard.Exclusive.EmailTemplates WHERE [TemplateTypeId] = 1)
	BEGIN
	--do what needs to be done if not
	INSERT INTO ExclusiveCard.Exclusive.EmailTemplates
	VALUES ('Welcome Email','Welcome to Exclusive Card',null,'<!DOCTYPE html>
	<html>
	<head></head>
	<body>
	<p>Dear [Name]</p>
	<h1>Renewal</h1>
	<p>Welcome to Exclusive Card.</p>

	<p>Your account is now open and you can enjoy all the benefits of being an Exclusive member.</p>
	<p>Please, always remember to make sure you are logged in to your account each time you use the <br>
		Exclusive website to take advantage of all the features and offers available. You will also not be able <br>
	to receive cashback unless you are logged into your account.</p>
	<p><b>You should now download our free mobile application (Exclusive Card) which is available on the <br>
	App Store for iPhone and Google Play Store for Android devices, in order to access the electronic <br>
	version of your membership card.</b>This only needs to be shown whenever you use high street or restaurant deals.</p>
	<p>If you encounter any problems or would like to ask any further questions regarding your new Exclusive <br>
		account please email us on :<a href="mailto: contact@exclusivecard.co.uk">contact@exclusivecard.co.uk</a></p> 

	<p>The Exclusive Team</p>
	</body>
	</html>',0,1)
	END
ELSE
	BEGIN
		UPDATE ExclusiveCard.Exclusive.EmailTemplates SET
		EmailName = 'Exclusive card Renewal',
		[Subject] = 'Thanks for Renewing'
		BodyHtml = '<!DOCTYPE html>
	<html>
	<head></head>
	<body>
	<p>Dear [Name]</p>
	
	<p>Welcome to Exclusive Card.</p>

	<p>Your account is now renewed and you can enjoy all the benefits of being an Exclusive member.</p>
	<p>Please, always remember to make sure you are logged in to your account each time you use the <br>
		Exclusive website to take advantage of all the features and offers available. You will also not be able <br>
	to receive cashback unless you are logged into your account.</p>
	<p><b>You should now download our free mobile application (Exclusive Card) which is available on the <br>
	App Store for iPhone and Google Play Store for Android devices, in order to access the electronic <br>
	version of your membership card.</b>This only needs to be shown whenever you use high street or restaurant deals.</p>
	<p>If you encounter any problems or would like to ask any further questions regarding your new Exclusive <br>
		account please email us on :<a href="mailto: contact@exclusivecard.co.uk">contact@exclusivecard.co.uk</a></p> 

	<p>The Exclusive Team</p>
	</body>
	</html>' WHERE [TemplateTypeId] = 1
	END



UPDATE E Set E.[Description] = 'Merchants' FROM Exclusive.AffiliateMappingRule E where E.[Description] = 'AWIN Merchants'

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190520103542_ChangeMappingRuletoMerchants', N'2.1.8-servicing-32085');

GO

