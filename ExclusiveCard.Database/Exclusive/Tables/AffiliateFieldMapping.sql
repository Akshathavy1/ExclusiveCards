CREATE TABLE [Exclusive].[AffiliateFieldMapping] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [AffiliateFileMappingId] INT            NOT NULL,
    [AffiliateFieldName]     NVARCHAR (100) NULL,
    [ExclusiveTable]         NVARCHAR (100) NULL,
    [ExclusiveFieldName]     NVARCHAR (100) NULL,
    [IsList]                 BIT            NOT NULL,
    [Delimiter]              NVARCHAR (8)   NULL,
    [AffiliateMappingRuleId] INT            NULL,
    [AffiliateTransformId]   INT            NOT NULL,
    [AffiliateMatchTypeId]   INT            NOT NULL,
    CONSTRAINT [PK_AffiliateFieldMapping] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AffiliateFieldMapping_AffiliateFileMapping_AffiliateFileMappingId] FOREIGN KEY ([AffiliateFileMappingId]) REFERENCES [Exclusive].[AffiliateFileMapping] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AffiliateFieldMapping_AffiliateMappingRule_AffiliateMappingRuleId] FOREIGN KEY ([AffiliateMappingRuleId]) REFERENCES [Exclusive].[AffiliateMappingRule] ([Id])
);

