CREATE TABLE [Staging].[OfferImportFile] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [AffiliateFileId] INT            NOT NULL,
    [DateImported]    DATETIME2 (7)  NOT NULL,
    [FilePath]        NVARCHAR (MAX) NULL,
    [ErrorFilePath]   NVARCHAR (MAX) NULL,
    [ImportStatus]    INT            NOT NULL,
    [Staged]          INT            DEFAULT ((0)) NOT NULL,
    [TotalRecords]    INT            DEFAULT ((0)) NOT NULL,
    [Failed]          INT            DEFAULT ((0)) NOT NULL,
    [Imported]        INT            DEFAULT ((0)) NOT NULL,
    [CountryCode]     NVARCHAR (3)   NULL,
    [Duplicates]      INT            DEFAULT ((0)) NOT NULL,
    [Updates]         INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_OfferImportFile] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OfferImportFile_AffiliateFile_AffiliateFileId] FOREIGN KEY ([AffiliateFileId]) REFERENCES [Exclusive].[AffiliateFile] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OfferImportFile_Status_ImportStatus] FOREIGN KEY ([ImportStatus]) REFERENCES [Exclusive].[Status] ([Id])
);

