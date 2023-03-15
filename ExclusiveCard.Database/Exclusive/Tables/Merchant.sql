CREATE TABLE [Exclusive].[Merchant] (
    [Id]                    INT            IDENTITY (1, 1) NOT NULL,
    [Name]                  NVARCHAR (128) NULL,
    [ContactDetailsId]      INT            NULL,
    [ContactName]           NVARCHAR (128) NULL,
    [ShortDescription]      NVARCHAR (128) NULL,
    [LongDescription]       NVARCHAR (MAX) NULL,
    [WebsiteUrl]            NVARCHAR (512) NULL,
    [IsDeleted]             BIT            NOT NULL,
    [Terms]                 NVARCHAR (MAX) NULL,
    [BrandColour]           NVARCHAR (7)   NULL,
    [FeatureImageOfferText] BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Merchant] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Merchant_ContactDetail_ContactDetailsId] FOREIGN KEY ([ContactDetailsId]) REFERENCES [Exclusive].[ContactDetail] ([Id]) ON DELETE CASCADE
);

