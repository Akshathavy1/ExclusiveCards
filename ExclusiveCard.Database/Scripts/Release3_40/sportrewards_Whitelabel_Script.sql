/****** for updating TalkSPORT to Sport Rewards if exist  ******/
BEGIN TRAN

DECLARE @NewName nvarchar(20) = 'Exclusive Sport'
DECLARE @OldName nvarchar(20) = 'talkSPORT'
DECLARE @NewURL nvarchar(100) = 'https://exclusivesportrewards.com' --<<< LIVE URL
--DECLARE @NewURL nvarchar(100) = 'http://uat.exclusivesportrewards.com'

DECLARE @WhiteLabelId int

IF  Exists(SELECT Id FROM [CMS].[WhiteLabelSettings] WHERE [DisplayName] = @OldName)
BEGIN 

	SELECT @WhiteLabelId = Id FROM [CMS].[WhiteLabelSettings] WHERE [DisplayName] = @OldName

	UPDATE [CMS].[WhiteLabelSettings] SET
		[DisplayName] = @NewName,
		[Name] ='Exclusive Sport Rewards',
		--[URL] ='http://uat.exclusivesportrewards.com',
		[URL] ='https://exclusivesportrewards.com', --<<< LIVE URL
		[Slug] ='sport-rewards',
		[CompanyNumber] ='11616720',
		[CSSFile] ='sport-rewards.css',
		[Logo] ='logo.png',
		[NewsletterLogo] = 'logo_newsletter.png',
		[ClaimsEmail] ='claims@exclusiverewards.co.uk',
		[HelpEmail] ='help-me@exclusiverewards.co.uk',
		[MainEmail] ='enquiries@exclusiverewards.co.uk',
		[Address] ='15 Hoghton Street, Southport, United Kingdom, PR9 0NS',
		--[CardName]='Exclusive Sport Rewards', --<< should always be 'Exclusive Rewards'
		[CharityName]='Exclusive Sport Rewards',
		[RegistrationUrl] = @NewURL + SUBSTRING([RegistrationUrl], CHARINDEX('/Account/Register?', [RegistrationUrl]), LEN([RegistrationUrl]))
	WHERE [Id] = @WhiteLabelId
   
	-- Update SiteOwner Description
	UPDATE [CMS].[SiteOwner] SET [Description]= @NewName WHERE [Description] = @OldName

	--Update Parner
	UPDATE [Exclusive].[Partner] SET [name]= @NewName  WHERE [Name] = @OldName		  

	--Update MemberShipPlan
	Update [Exclusive].[MembershipPlan]
	SET [Description] = @NewName + SUBSTRING([Description], LEN(@OldName) + 1,LEN([Description]))
	WHERE SUBSTRING([Description], 1, LEN(@OldName)) = @OldName
	AND [WhitelabelId] = @WhiteLabelId
	--[Marketing].[Campaigns]
	IF EXISTS(SELECT Id FROM [Marketing].[ContactLists] WHERE [WhiteLabelId] = @WhiteLabelId)
	OR EXISTS(SELECT [CampaignId] FROM [Marketing].[Campaigns] WHERE [WhiteLabelId] = @WhiteLabelId AND [CampaignId] IS NOT NULL)
	BEGIN
		-- ??? Do we care about this ???
		SELECT '## Contact list exists on SendGrid site so unable to update [Marketing].[Campaigns] ##'
	END
	ELSE
	BEGIN
		UPDATE [Marketing].[Campaigns]
		SET 
			--[ContactListName] = REPLACE(@NewName,' ','') + 'Rewards_ContactList_TEST',
			[ContactListName] = REPLACE(@NewName,' ','') + 'Rewards_ContactList', --<< LIVE
			--[CampaignName]= REPLACE(@NewName,' ','') + 'Rewards_Campaign_TEST'
			[CampaignName]= REPLACE(@NewName,' ','') + 'Rewards_Campaign' --<< LIVE
		WHERE [WhiteLabelId] = @WhiteLabelId
	END

	SELECT * FROM [CMS].[WhiteLabelSettings] 
	SELECT * FROM [CMS].[SiteOwner]
	SELECT * FROM [Exclusive].[Partner]
	SELECT * FROM [Exclusive].[MembershipPlan]
	SELECT * FROM [Marketing].[Campaigns]

END
ELSE
BEGIN
	SELECT 'Did not find ' + @OldName ' in [CMS].[WhiteLabelSettings]'
END

--COMMIT TRAN
--ROLLBACK TRAN

