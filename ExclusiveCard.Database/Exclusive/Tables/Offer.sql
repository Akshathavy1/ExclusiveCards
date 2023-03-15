CREATE TABLE [Exclusive].[Offer] (
    [Id]                      INT            IDENTITY (1, 1) NOT NULL,
    [MerchantId]              INT            NOT NULL,
    [AffiliateId]             INT            NULL,
    [OfferTypeId]             INT            NOT NULL,
    [StatusId]                INT            NOT NULL,
    [ValidFrom]               DATETIME2 (7)  NULL,
    [ValidTo]                 DATETIME2 (7)  NULL,
    [Validindefinately]       BIT            NOT NULL,
    [ShortDescription]        NVARCHAR (MAX) NULL,
    [LongDescription]         NVARCHAR (MAX) NULL,
    [Instructions]            NVARCHAR (MAX) NULL,
    [Terms]                   NVARCHAR (MAX) NULL,
    [Exclusions]              NVARCHAR (MAX) NULL,
    [LinkUrl]                 NVARCHAR (MAX) NULL,
    [OfferCode]               NVARCHAR (128) NULL,
    [Reoccuring]              BIT            NOT NULL,
    [SearchRanking]           SMALLINT       NOT NULL,
    [Datecreated]             DATETIME2 (7)  NOT NULL,
    [Headline]                NVARCHAR (100) NULL,
    [AffiliateReference]      NVARCHAR (255) NULL,
    [DateUpdated]             DATETIME       NULL,
    [RedemptionAccountNumber] NVARCHAR (32)  NULL,
    [RedemptionProductCode]   NVARCHAR (32)  NULL,
    [SSOConfigId] INT NULL, 
    [ProductCode] NVARCHAR(MAX) NULL
    CONSTRAINT [PK_Offer] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Offer_Affiliate_AffiliateId] FOREIGN KEY ([AffiliateId]) REFERENCES [Exclusive].[Affiliate] ([Id]),
    CONSTRAINT [FK_Offer_Merchant_MerchantId] FOREIGN KEY ([MerchantId]) REFERENCES [Exclusive].[Merchant] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Offer_OfferType_OfferTypeId] FOREIGN KEY ([OfferTypeId]) REFERENCES [Exclusive].[OfferType] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Offer_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [Exclusive].[Status] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SSO_ConfigId] FOREIGN KEY ([SSOConfigId]) REFERENCES [Exclusive].[SSOConfiguration] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_Offer_7C0D0BA2E6127A698929B618F37A973F]
    ON [Exclusive].[Offer]([StatusId] ASC, [OfferTypeId] ASC)
    INCLUDE([MerchantId], [SearchRanking], [ValidFrom], [Validindefinately], [ValidTo]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_Offer_0D1482A12DA66331A7DC0DC4CEF3041B]
    ON [Exclusive].[Offer]([OfferTypeId] ASC, [MerchantId] ASC)
    INCLUDE([AffiliateId], [Datecreated], [Exclusions], [Headline], [Instructions], [LinkUrl], [LongDescription], [OfferCode], [Reoccuring], [SearchRanking], [ShortDescription], [StatusId], [Terms], [ValidFrom], [Validindefinately], [ValidTo]);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Offer_AffiliateId_AffiliateReference]
    ON [Exclusive].[Offer]([AffiliateId] ASC, [AffiliateReference] ASC) WHERE ([AffiliateId] IS NOT NULL AND [AffiliateReference] IS NOT NULL);

