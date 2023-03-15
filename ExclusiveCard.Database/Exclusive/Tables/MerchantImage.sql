CREATE TABLE [Exclusive].[MerchantImage] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [MerchantId]   INT            NOT NULL,
    [ImagePath]    NVARCHAR (512) NULL,
    [DisplayOrder] SMALLINT       NOT NULL,
    [TimeStamp]    DATETIME2 (7)  DEFAULT ('2019-02-09T00:00:00.0000000Z') NOT NULL,
    [ImageType]    INT            DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_MerchantImage] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MerchantImage_Merchant_MerchantId] FOREIGN KEY ([MerchantId]) REFERENCES [Exclusive].[Merchant] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_MerchantImage_DA2A38A489D233BEF3E05764244C168E]
    ON [Exclusive].[MerchantImage]([MerchantId] ASC)
    INCLUDE([DisplayOrder], [ImagePath]);

