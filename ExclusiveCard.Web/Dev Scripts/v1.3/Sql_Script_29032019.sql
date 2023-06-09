
DECLARE @affiliateId int
DECLARE @ruleId int
IF NOT Exists(SELECT 1 FROM Exclusive.Affiliate where Name ='Ebay')
BEGIN
	INSERT INTO [Exclusive].[Affiliate] VALUES ('Ebay')
	select @affiliateId = scope_identity()

	IF(@affiliateId > 0)
	BEGIN
		IF NOT EXISTS(SELECT 1 FROM Exclusive.AffiliateMappingRule WHERE [Description] = 'Affiliate Membership card alias' AND AffiliateId = @affiliateId)
		BEGIN
			INSERT INTO Exclusive.AffiliateMappingRule VALUES('Affiliate Membership card alias', @affiliateId, 1)
			SELECT @ruleId = SCOPE_IDENTITY()
			IF NOT EXISTS(SELECT 1 FROM Exclusive.AffiliateMapping WHERE AffilateValue = 'customID' AND AffiliateMappingRuleId = @ruleId)
			BEGIN
				INSERT INTO Exclusive.AffiliateMapping VALUES(@ruleId, 'customID', null)
			END
		END
	END
END
GO


DECLARE @affiliateId int
DECLARE @ruleId int
IF NOT EXISTS(SELECT 1 FROM Exclusive.Affiliate where Name ='Affili.net')
BEGIN
	INSERT INTO [Exclusive].[Affiliate] VALUES ('Affili.net')
	select @affiliateId = scope_identity()

	IF(@affiliateId > 0)
	BEGIN
		IF NOT EXISTS(SELECT 1 FROM Exclusive.AffiliateMappingRule WHERE [Description] = 'Affiliate Membership card alias' AND AffiliateId = @affiliateId)
		BEGIN
			INSERT INTO Exclusive.AffiliateMappingRule VALUES('Affiliate Membership card alias', @affiliateId, 1)
			SELECT @ruleId = SCOPE_IDENTITY()
			IF NOT EXISTS(SELECT 1 FROM Exclusive.AffiliateMapping WHERE AffilateValue = 'subid' AND AffiliateMappingRuleId = @ruleId)
			BEGIN
				INSERT INTO Exclusive.AffiliateMapping VALUES(@ruleId, 'subid', null)
			END
		END
	END
END
GO


DECLARE @affiliateId int
DECLARE @ruleId int
IF NOT EXISTS(SELECT 1 FROM Exclusive.Affiliate where Name ='Rakuten')
BEGIN
	INSERT INTO [Exclusive].[Affiliate] VALUES ('Rakuten')
	select @affiliateId = scope_identity()

	IF(@affiliateId > 0)
	BEGIN
		IF NOT EXISTS(SELECT 1 FROM Exclusive.AffiliateMappingRule WHERE [Description] = 'Affiliate Membership card alias' AND AffiliateId = @affiliateId)
		BEGIN
			INSERT INTO Exclusive.AffiliateMappingRule VALUES('Affiliate Membership card alias', @affiliateId, 1)
			SELECT @ruleId = SCOPE_IDENTITY()
			IF NOT EXISTS(SELECT 1 FROM Exclusive.AffiliateMapping WHERE AffilateValue = 'u1' AND AffiliateMappingRuleId = @ruleId)
			BEGIN
				INSERT INTO Exclusive.AffiliateMapping VALUES(@ruleId, 'u1', null)
			END
		END
	END
END
GO

DECLARE @affiliateId int
DECLARE @ruleId int
IF NOT EXISTS(SELECT 1 FROM Exclusive.Affiliate where Name ='CJ Affiliate')
BEGIN
	INSERT INTO [Exclusive].[Affiliate] VALUES ('CJ Affiliate')
	select @affiliateId = scope_identity()

	IF(@affiliateId > 0)
	BEGIN
		IF NOT EXISTS(SELECT 1 FROM Exclusive.AffiliateMappingRule WHERE [Description] = 'Affiliate Membership card alias' AND AffiliateId = @affiliateId)
		BEGIN
			INSERT INTO Exclusive.AffiliateMappingRule VALUES('Affiliate Membership card alias', @affiliateId, 1)
			SELECT @ruleId = SCOPE_IDENTITY()
			IF NOT EXISTS(SELECT 1 FROM Exclusive.AffiliateMapping WHERE AffilateValue = 'sid' AND AffiliateMappingRuleId = @ruleId)
			BEGIN
				INSERT INTO Exclusive.AffiliateMapping VALUES(@ruleId, 'sid', null)
			END
		END
	END
END
GO