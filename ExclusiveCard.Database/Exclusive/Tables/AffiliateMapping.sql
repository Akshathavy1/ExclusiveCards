CREATE TABLE [Exclusive].[AffiliateMapping] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [AffiliateMappingRuleId] INT            NOT NULL,
    [AffilateValue]          NVARCHAR (MAX) NULL,
    [ExclusiveValue]         NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AffiliateMapping] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AffiliateMapping_AffiliateMappingRule_AffiliateMappingRuleId] FOREIGN KEY ([AffiliateMappingRuleId]) REFERENCES [Exclusive].[AffiliateMappingRule] ([Id])
);

