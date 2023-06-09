
-- 1.  Add merchant name here
DECLARE @Merchant varchar(100) = 'boohoo%'
DECLARE @FileId INT

-- Find latest Cashback File
SELECT TOP (1) @FileID = Id  FROM [Staging].[TransactionFile] order by id desc
SELECT TOP (1000) * FROM [Staging].[CashbackTransactionErrors] where TransactionFileId = @FileId AND merchantname like @Merchant

-- Show affiliates
Select * from Exclusive.Affiliate
-- Show possible merchants  
Select * from Exclusive.Merchant where Name like @Merchant and IsDeleted =0 


-- 2.  Set the Affiliate here
DECLARE @Affiliate INT = 1 -- Awin

-- 3.  Find the mapping rule
DECLARE @MappingRuleId INT
Select @MappingRuleId = Id from Exclusive.AffiliateMappingRule where affiliateid = @Affiliate And Description = 'Merchants'
Select * from Exclusive.AffiliateMapping where AffiliateMappingRuleId = @MappingRuleId and AffilateValue like @Merchant

-- 4.  Set the affilate value from error.MerchantName, and ExclusiveValue from Merchant.Id  for the correct merchant
DECLARE @AffiliateValue varchar(13) = 'boohooMAN.com' 
DECLARE @MerchantId int  = 555

-- 5  Insert the record 
BEGIN TRAN
INSERT Exclusive.AffiliateMapping (AffiliateMappingRuleId, AffilateValue, ExclusiveValue)  VALUES (@MappingRuleId, @AffiliateValue, @MerchantId)
--COMMIT TRAN




