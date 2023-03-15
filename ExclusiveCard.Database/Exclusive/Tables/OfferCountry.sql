CREATE TABLE [Exclusive].[OfferCountry] (
    [OfferId]     INT          NOT NULL,
    [CountryCode] NVARCHAR (3) NOT NULL,
    [IsActive]    BIT          NOT NULL,
    CONSTRAINT [PK_OfferCountry] PRIMARY KEY CLUSTERED ([OfferId] ASC, [CountryCode] ASC),
    CONSTRAINT [FK_OfferCountry_Offer_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Exclusive].[Offer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [AK_OfferCountry_CountryCode_OfferId] UNIQUE NONCLUSTERED ([CountryCode] ASC, [OfferId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_OfferCountry_BA25C91BDB946394911AFB3C19174914]
    ON [Exclusive].[OfferCountry]([CountryCode] ASC, [IsActive] ASC);

