
-- Setup Affiliate and their SubId here
DECLARE @Affiliate NVARCHAR(100) = 'WebGains'
DECLARE @SubId NVARCHAR(100) = 'clickref'

DECLARE @AffiliateId INT 
DECLARE @MappingRuleId INT

BEGIN TRAN
  INSERT Exclusive.Affiliate  (Name) Values (@Affiliate)
  SELECT @AffiliateId = SCOPE_IDENTITY()


  INSERT Exclusive.AffiliateMappingRule (Description, AffiliateId, IsActive) Values ('Affiliate Membership card alias', @AffiliateId, 1)
  SELECT @MappingRuleId = SCOPE_IDENTITY()

  INSERT Exclusive.AffiliateMapping (AffiliateMappingRuleId, AffilateValue) VALUES (@MappingRuleId , @SubId )


  COMMIT TRAN




  SELECT TOP (1000) [Id]
      ,[Name]
  FROM [Exclusive].[Affiliate]

  Select * from Exclusive.AffiliateMappingRule

  Select * from Exclusive.AffiliateMapping where AffiliateMappingRuleId > 4
