CREATE TABLE [Exclusive].[ContactDetail] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Address1]      NVARCHAR (128) NULL,
    [Address2]      NVARCHAR (128) NULL,
    [Address3]      NVARCHAR (128) NULL,
    [Town]          NVARCHAR (128) NULL,
    [District]      NVARCHAR (128) NULL,
    [PostCode]      NVARCHAR (16)  NULL,
    [CountryCode]   NVARCHAR (3)   NULL,
    [Latitude]      NVARCHAR (16)  NULL,
    [Longitude]     NVARCHAR (16)  NULL,
    [LandlinePhone] NVARCHAR (16)  NULL,
    [MobilePhone]   NVARCHAR (16)  NULL,
    [EmailAddress]  NVARCHAR (512) NULL,
    [IsDeleted]     BIT            NOT NULL,
    CONSTRAINT [PK_ContactDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);

