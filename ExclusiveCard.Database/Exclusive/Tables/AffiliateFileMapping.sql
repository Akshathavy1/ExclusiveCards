CREATE TABLE [Exclusive].[AffiliateFileMapping] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [AffiliateId] INT            NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AffiliateFileMapping] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AffiliateFileMapping_Affiliate_AffiliateId] FOREIGN KEY ([AffiliateId]) REFERENCES [Exclusive].[Affiliate] ([Id])
);

