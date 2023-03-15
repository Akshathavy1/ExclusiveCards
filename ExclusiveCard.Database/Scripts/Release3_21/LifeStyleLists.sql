BEGIN TRAN

DECLARE @MerchantId int
DECLARE @DiamondOfferTypeId int
DECLARE @ActiveStatusId int
DECLARE @ListName nvarchar(50)
DECLARE @Description nvarchar(500)
DECLARE @OfferListId int
DECLARE @OfferId int

-- Pick up statics
SELECT @MerchantId = [Id] FROM [Exclusive].[Merchant]
  WHERE [Name] = 'Exclusive Rewards'
SELECT @DiamondOfferTypeId = [Id] FROM [Exclusive].[OfferType]
	WHERE [Description] = 'Diamond Offer'
SELECT @ActiveStatusId = [Id] FROM [Exclusive].[Status]
	WHERE [Name] = 'Active' and [Type] = 'Offer'

-- Add new list
SELECT @ListName = 'LifeStyle Hub'
SELECT @Description = 'LifeStyle Hub'

INSERT INTO [CMS].[OfferList]
  ([ListName],[Description],[MaxSize],[IsActive],[IncludeShowAllLink],[ShowAllLinkCaption],[PermissionLevel])
  VALUES
  (@ListName,@Description,0,1,1,'Show All',0)
SELECT @OfferListId = SCOPE_IDENTITY() 

-- Add new list
SELECT @ListName = 'LifeStyle Hub List'
SELECT @Description = 'LifeStyle Hub List'

  INSERT INTO [CMS].[OfferList]
  ([ListName],[Description],[MaxSize],[IsActive],[IncludeShowAllLink],[ShowAllLinkCaption],[PermissionLevel])
  VALUES
  (@ListName,@Description,0,1,1,'Show All',0)

-- Add offer for new hub page
INSERT INTO [Exclusive].[Offer]
  ([MerchantId], [OfferTypeId], [StatusId], [Validindefinately]
  ,[ShortDescription] ,[LongDescription]
  ,[LinkUrl]
  ,[SearchRanking],[Datecreated], [Reoccuring])
  VALUES
  (@MerchantId, @DiamondOfferTypeId, @ActiveStatusId, 1
  ,'Life Style Hub Offers','<p>Gain access to higher cashback rates and even better exclusive deals than in our free standard membership with our Diamond Package</p>'
  ,'/LifeStyleHub?country=GB'
  ,1,GetDate(),0)
SELECT @OfferId = SCOPE_IDENTITY() 

--Add offer country link (admin will fall over without this)
INSERT INTO [Exclusive].[OfferCountry]
([OfferId],[CountryCode],[IsActive])
 VALUES
 (@OfferId,'GB', 1)

-- Add offer/list link so the new hub page displays on the diamond offer list
INSERT INTO [CMS].[OfferListItem]
([OfferListId],[OfferId],[DisplayOrder],[CountryCode])
VALUES
(@OfferListId, @OfferId,1,'GB')

SELECT * FROM [CMS].[OfferList]
  ORDER BY ID DESC
SELECT * FROM [CMS].[OfferListItem]
  ORDER BY OfferListId DESC
SELECT * FROM [Exclusive].[Offer]
  ORDER BY ID DESC

--COMMIT TRAN
--ROLLBACK TRAN
