CREATE TABLE [CMS].[OfferListItem] (
    [OfferListId]       INT           NOT NULL,
    [OfferId]           INT           NOT NULL,
    [ExcludedCountries] NVARCHAR (50) NULL,
    [DisplayOrder]      SMALLINT      NOT NULL,
    [DisplayFrom]       DATETIME2 (7) NULL,
    [DisplayTo]         DATETIME2 (7) NULL,
    [CountryCode]       NVARCHAR (3)  DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_OfferListItem] PRIMARY KEY CLUSTERED ([OfferId] ASC, [OfferListId] ASC, [CountryCode] ASC),
    CONSTRAINT [FK_OfferListItem_Offer_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Exclusive].[Offer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OfferListItem_OfferList_OfferListId] FOREIGN KEY ([OfferListId]) REFERENCES [CMS].[OfferList] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [AK_OfferListItem_CountryCode_OfferId_OfferListId] UNIQUE NONCLUSTERED ([CountryCode] ASC, [OfferId] ASC, [OfferListId] ASC)
);

