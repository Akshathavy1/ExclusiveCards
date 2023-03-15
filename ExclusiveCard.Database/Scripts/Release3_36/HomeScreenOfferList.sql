/****** Script to add new home page offer list (unpopulated) ******/

IF NOT EXISTS (SELECT Id FROM [CMS].[OfferList] WHERE [ListName] = 'Homepage Offers')
BEGIN

	INSERT [CMS].[OfferList]
			   ([ListName]
			   ,[Description]
			   ,[MaxSize]
			   ,[IsActive]
			   ,[IncludeShowAllLink]
			   ,[ShowAllLinkCaption]
			   ,[PermissionLevel])
		 VALUES
			   ('Homepage Offers','Homepage Offers', 0,1,1,'Show All',0)

	PRINT 'Offer list added'

END
ELSE
	PRINT 'Offer list already exists'