CREATE TABLE [CMS].[OfferDiamondClubCategories]
(
	[Id] INT NOT NULL PRIMARY KEY,
	    [OfferId] INT NOT NULL, 
    [DiamondClubCategoryId] INT NOT NULL, 
    CONSTRAINT [FK_OfferDiamondClubCategories_Offer_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Exclusive].[Offer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OfferDiamondClubCategories_DiamondClubCategories_DiamondClubCategoryId] FOREIGN KEY ([DiamondClubCategoryId]) REFERENCES [CMS].[DiamondClubCategories] ([Id]) ON DELETE CASCADE

)
