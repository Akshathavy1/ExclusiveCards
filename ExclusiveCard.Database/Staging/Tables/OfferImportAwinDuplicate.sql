CREATE TABLE [Staging].[OfferImportAwinDuplicate] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [OfferImportFileId] INT            NOT NULL,
    [PromotionId]       NVARCHAR (255) NULL,
    [Type]              NVARCHAR (MAX) NULL,
    [Code]              NVARCHAR (MAX) NULL,
    [Description]       NVARCHAR (MAX) NULL,
    [Starts]            DATETIME2 (7)  NOT NULL,
    [Ends]              DATETIME2 (7)  NOT NULL,
    [Categories]        NVARCHAR (MAX) NULL,
    [Regions]           NVARCHAR (MAX) NULL,
    [Terms]             NVARCHAR (MAX) NULL,
    [DeeplinkTracking]  NVARCHAR (MAX) NULL,
    [Deeplink]          NVARCHAR (MAX) NULL,
    [CommissionGroups]  NVARCHAR (MAX) NULL,
    [Commission]        NVARCHAR (MAX) NULL,
    [Exclusive]         NVARCHAR (MAX) NULL,
    [DateAdded]         DATETIME2 (7)  NOT NULL,
    [Title]             NVARCHAR (512) NULL,
    CONSTRAINT [PK_OfferImportAwinDuplicate] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OfferImportAwinDuplicate_OfferImportFile_OfferImportFileId] FOREIGN KEY ([OfferImportFileId]) REFERENCES [Staging].[OfferImportFile] ([Id]) ON DELETE CASCADE
);

