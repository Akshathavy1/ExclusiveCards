CREATE TABLE [Exclusive].[OfferTag] (
    [OfferId] INT NOT NULL,
    [TagId]   INT NOT NULL,
    CONSTRAINT [PK_OfferTag] PRIMARY KEY CLUSTERED ([OfferId] ASC, [TagId] ASC),
    CONSTRAINT [FK_OfferTag_Offer_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Exclusive].[Offer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OfferTag_Tag_TagId] FOREIGN KEY ([TagId]) REFERENCES [Exclusive].[Tag] ([Id]) ON DELETE CASCADE
);

