-- AffilateMapping seeding for Regions

DECLARE @affiliateMappingRuleId INT = 0;
DECLARE @affiliateMappingId INT = 0;


SELECT @affiliateMappingRuleId = Id from Exclusive.AffiliateMappingRule 
	where Description='AWIN Countries' and IsActive = 1 and AffiliateId = (select Id from Exclusive.Affiliate where Name='AWIN') 

Select  @affiliateMappingId = Id from Exclusive.AffiliateMapping 
where AffiliateMappingRuleId = @affiliateMappingRuleId and AffilateValue like '%Czech Republic%'  and ExclusiveValue like '%CZ%'

IF (@affiliateMappingId = 0)
BEGIN
	insert into Exclusive.AffiliateMapping values(@affiliateMappingRuleId, 'Czech Republic', 'CZ')
END

SET @affiliateMappingId = 0

Select  @affiliateMappingId = Id from Exclusive.AffiliateMapping 
where AffiliateMappingRuleId = @affiliateMappingRuleId and AffilateValue like '%United Kingdom%'  and ExclusiveValue like '%GB%'

IF (@affiliateMappingId = 0)
BEGIN
	insert into Exclusive.AffiliateMapping values(@affiliateMappingRuleId, 'United Kingdom', 'GB')
END 

SET @affiliateMappingId = 0

Select  @affiliateMappingId = Id from Exclusive.AffiliateMapping 
where AffiliateMappingRuleId = @affiliateMappingRuleId and AffilateValue like '%Poland%'  and ExclusiveValue like '%PL%'

IF @affiliateMappingId = 0
BEGIN
	insert into Exclusive.AffiliateMapping values(@affiliateMappingRuleId, 'Poland', 'PL')
END 

SET @affiliateMappingId = 0

Select  @affiliateMappingId = Id from Exclusive.AffiliateMapping 
where AffiliateMappingRuleId = @affiliateMappingRuleId and AffilateValue like '%Seychelles%'  and ExclusiveValue like '%SC%'

IF @affiliateMappingId = 0
BEGIN
	insert into Exclusive.AffiliateMapping values(@affiliateMappingRuleId, 'Seychelles', 'SC')
END 

SET @affiliateMappingId = 0

Select  @affiliateMappingId = Id from Exclusive.AffiliateMapping 
where AffiliateMappingRuleId = @affiliateMappingRuleId and AffilateValue like '%Slovakia%'  and ExclusiveValue like '%SK%'

IF @affiliateMappingId = 0
BEGIN
	insert into Exclusive.AffiliateMapping values(@affiliateMappingRuleId, 'Slovakia', 'SK')
END 


-- AffiliateMapping For OfferTypes

SET @affiliateMappingRuleId = 0
SET @affiliateMappingId = 0

SELECT @affiliateMappingRuleId = Id from Exclusive.AffiliateMappingRule 
	where Description='AWIN OfferTypes' and IsActive = 1 and AffiliateId = (select Id from Exclusive.Affiliate where Name='AWIN') 

SELECT @affiliateMappingId = Id from Exclusive.AffiliateMapping 
where AffiliateMappingRuleId = @affiliateMappingRuleId and AffilateValue like '%Vouchers Only%'

IF @affiliateMappingId = 0
BEGIN
	insert into Exclusive.AffiliateMapping values(@affiliateMappingRuleId, 'Vouchers Only', (select Id from Exclusive.OfferType where Description='Voucher Code' and IsActive = 1))
END 

SET @affiliateMappingId = 0

SELECT @affiliateMappingId = Id from Exclusive.AffiliateMapping 
where AffiliateMappingRuleId = @affiliateMappingRuleId and AffilateValue like '%Promotions Only%'

IF @affiliateMappingId = 0
BEGIN
	insert into Exclusive.AffiliateMapping values(@affiliateMappingRuleId, 'Promotions Only', (select Id from Exclusive.OfferType where Description='Sales' and IsActive = 1))
END 






