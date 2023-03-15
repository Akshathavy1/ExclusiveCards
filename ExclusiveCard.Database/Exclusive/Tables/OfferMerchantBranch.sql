CREATE TABLE [Exclusive].[OfferMerchantBranch] (
    [OfferId]          INT NOT NULL,
    [MerchantBranchId] INT NOT NULL,
    CONSTRAINT [PK_OfferMerchantBranch] PRIMARY KEY CLUSTERED ([OfferId] ASC, [MerchantBranchId] ASC),
    CONSTRAINT [FK_OfferMerchantBranch_MerchantBranch_MerchantBranchId] FOREIGN KEY ([MerchantBranchId]) REFERENCES [Exclusive].[MerchantBranch] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OfferMerchantBranch_Offer_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Exclusive].[Offer] ([Id]),
    CONSTRAINT [AK_OfferMerchantBranch_MerchantBranchId_OfferId] UNIQUE NONCLUSTERED ([MerchantBranchId] ASC, [OfferId] ASC)
);

