--Db Seeding AWINCycle

Declare @affiliateId int;
Declare @affiliateFileMappingId int;


SELECT @affiliateId = Id FROM [Exclusive].[Affiliate] WHERE Name = 'Awin'

SELECT @affiliateFileMappingId = Id FROM Exclusive.AffiliateFileMapping WHERE [Description] = 'FileMapping' AND AffiliateId = @affiliateId

Select * from Exclusive.AffiliateMappingRule

	insert into [Exclusive].[AffiliateMappingRule](Description,AffiliateId,IsActive) values('AWIN Merchants',1,1);
	insert into [Exclusive].[AffiliateMappingRule](Description,AffiliateId,IsActive) values('AWIN Countries',1,1);
	insert into [Exclusive].[AffiliateMappingRule](Description,AffiliateId,IsActive) values('AWIN OfferTypes',1,1);
	insert into [Exclusive].[AffiliateMappingRule](Description,AffiliateId,IsActive) values('AWIN Categories',1,1);

Select * from Exclusive.AffiliateFieldMapping

	insert into [Exclusive].[AffiliateFieldMapping](AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,Delimiter,AffiliateMappingRuleId,AffiliateTransformId,AffiliateMatchTypeId) 
	values(@affiliateFileMappingId,'Advertiser','Staging.Offer','MerchantId',0,'',(Select Id from Exclusive.AffiliateMappingRule where Description = 'AWIN Merchants'),2,1);

	insert into [Exclusive].[AffiliateFieldMapping](AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,Delimiter,AffiliateMappingRuleId,AffiliateTransformId,AffiliateMatchTypeId) 
	values(@affiliateFileMappingId,'Regions','Staging.OfferCountry','OfferCountries',1,',',(Select Id from Exclusive.AffiliateMappingRule where Description = 'AWIN Countries'),3,1);

	insert into [Exclusive].[AffiliateFieldMapping](AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,Delimiter,AffiliateMappingRuleId,AffiliateTransformId,AffiliateMatchTypeId)
	values(@affiliateFileMappingId,'Type','Staging.Offer','OfferTypeId',0,'',(Select Id from Exclusive.AffiliateMappingRule where Description = 'AWIN OfferTypes'),2,1);

	insert into [Exclusive].[AffiliateFieldMapping](AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,Delimiter,AffiliateMappingRuleId,AffiliateTransformId,AffiliateMatchTypeId) 
	values(@affiliateFileMappingId,'Categories','Staging.OfferCategory','OfferCategories',1,',',(Select Id from Exclusive.AffiliateMappingRule where Description = 'AWIN Categories'),3,1);

	insert into [Exclusive].[AffiliateFieldMapping](AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,AffiliateTransformId,AffiliateMatchTypeId)  
	values(@affiliateFileMappingId,'Code','Staging.Offer','OfferCode',0,1,1);

	insert into [Exclusive].[AffiliateFieldMapping](AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,AffiliateTransformId,AffiliateMatchTypeId)  
	values(@affiliateFileMappingId,'Description','Staging.Offer','LongDescription',0,1,1);

	insert into [Exclusive].[AffiliateFieldMapping]
	(AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,AffiliateTransformId,AffiliateMatchTypeId)  
	values(@affiliateFileMappingId,'Starts','Staging.Offer','ValidFrom',0,1,1);

	insert into [Exclusive].[AffiliateFieldMapping]
	(AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,AffiliateTransformId,AffiliateMatchTypeId)  
	values(@affiliateFileMappingId,'Ends','Staging.Offer','ValidTo',0,1,1);

	insert into [Exclusive].[AffiliateFieldMapping]
	(AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,AffiliateTransformId,AffiliateMatchTypeId)  
	values(@affiliateFileMappingId,'Terms','Staging.Offer','Terms',0,1,1);

	insert into [Exclusive].[AffiliateFieldMapping]
	(AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,AffiliateTransformId,AffiliateMatchTypeId)  
	values(@affiliateFileMappingId,'DeeplinkTracking','Staging.Offer','LinkUrl',0,1,1);

	insert into [Exclusive].[AffiliateFieldMapping]
	(AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,AffiliateTransformId,AffiliateMatchTypeId)  
	values(@affiliateFileMappingId,'DateAdded','Staging.Offer','Datecreated',0,1,1);

	insert into [Exclusive].[AffiliateFieldMapping]
	(AffiliateFileMappingId,AffiliateFieldName,ExclusiveTable,ExclusiveFieldName,IsList,AffiliateTransformId,AffiliateMatchTypeId)  
	values(@affiliateFileMappingId,'Title','Staging.Offer','ShortDescription',0,1,1);


