CREATE TABLE [Exclusive].[CategoryFeatureDetail] (
    [CategoryId]        INT            NOT NULL,
    [CountryCode]       NVARCHAR (3)   NOT NULL,
    [FeatureMerchantId] INT            NOT NULL,
    [SelectedImage]     NVARCHAR (512) NULL,
    [UnselectedImage]   NVARCHAR (512) NULL,
    CONSTRAINT [PK_CategoryFeatureDetail] PRIMARY KEY CLUSTERED ([CategoryId] ASC, [CountryCode] ASC),
    CONSTRAINT [FK_CategoryFeatureDetail_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Exclusive].[Category] ([Id]),
    CONSTRAINT [FK_CategoryFeatureDetail_Merchant_FeatureMerchantId] FOREIGN KEY ([FeatureMerchantId]) REFERENCES [Exclusive].[Merchant] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_CategoryFeatureDetail_FeatureMerchantId]
    ON [Exclusive].[CategoryFeatureDetail]([FeatureMerchantId] ASC);

