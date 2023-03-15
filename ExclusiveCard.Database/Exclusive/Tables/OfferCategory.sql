CREATE TABLE [Exclusive].[OfferCategory] (
    [OfferId]    INT NOT NULL,
    [CategoryId] INT NOT NULL,
    CONSTRAINT [PK_OfferCategory] PRIMARY KEY CLUSTERED ([OfferId] ASC, [CategoryId] ASC),
    CONSTRAINT [FK_OfferCategory_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Exclusive].[Category] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OfferCategory_Offer_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Exclusive].[Offer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [AK_OfferCategory_CategoryId_OfferId] UNIQUE NONCLUSTERED ([CategoryId] ASC, [OfferId] ASC)
);

