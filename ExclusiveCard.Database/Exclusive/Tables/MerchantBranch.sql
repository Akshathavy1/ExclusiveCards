CREATE TABLE [Exclusive].[MerchantBranch] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [ContactDetailsId] INT            NULL,
    [MerchantId]       INT            NOT NULL,
    [Name]             NVARCHAR (128) NULL,
    [ShortDescription] NVARCHAR (128) NULL,
    [LongDescription]  NVARCHAR (MAX) NULL,
    [Notes]            NVARCHAR (200) NULL,
    [Mainbranch]       BIT            NOT NULL,
    [DisplayOrder]     SMALLINT       NOT NULL,
    [IsDeleted]        BIT            NOT NULL,
    CONSTRAINT [PK_MerchantBranch] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MerchantBranch_ContactDetail_ContactDetailsId] FOREIGN KEY ([ContactDetailsId]) REFERENCES [Exclusive].[ContactDetail] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MerchantBranch_Merchant_MerchantId] FOREIGN KEY ([MerchantId]) REFERENCES [Exclusive].[Merchant] ([Id])
);

