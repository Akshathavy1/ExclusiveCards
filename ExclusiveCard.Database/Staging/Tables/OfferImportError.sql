CREATE TABLE [Staging].[OfferImportError]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	[ErrorMessage] NVARCHAR(MAX)  NOT NULL,	
	[AffiliateId] INT Default (0),
	[AffiliateMappingRuleId] INT default(0),
	[AffiliateValue] NVARCHAR(1024),
	[OfferImportFileId] INT DEFAULT(0),
	[OfferImportRecordId] INT DEFAULT(0),
	[ErrorDateTime] DATETIME2 DEFAULT(GetDATE())

)

GO

CREATE INDEX [IX_OfferImportError_OfferImportFileId] ON [Staging].[OfferImportError] ([OfferImportFileId])
