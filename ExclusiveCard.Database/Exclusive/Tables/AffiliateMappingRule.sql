CREATE TABLE [Exclusive].[AffiliateMappingRule] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (500) NULL,
    [AffiliateId] INT            NOT NULL,
    [IsActive]    BIT            NOT NULL,
    CONSTRAINT [PK_AffiliateMappingRule] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AffiliateMappingRule_Affiliate_AffiliateId] FOREIGN KEY ([AffiliateId]) REFERENCES [Exclusive].[Affiliate] ([Id]) ON DELETE CASCADE
);

