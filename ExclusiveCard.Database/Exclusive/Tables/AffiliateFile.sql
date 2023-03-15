CREATE TABLE [Exclusive].[AffiliateFile] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [AffiliateId]            INT            NOT NULL,
    [FileName]               NVARCHAR (250) NULL,
    [Description]            NVARCHAR (500) NULL,
    [StagingTable]           NVARCHAR (100) NULL,
    [AffiliateFileMappingId] INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_AffiliateFile] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AffiliateFile_Affiliate_AffiliateId] FOREIGN KEY ([AffiliateId]) REFERENCES [Exclusive].[Affiliate] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AffiliateFile_AffiliateFileMapping_AffiliateFileMappingId] FOREIGN KEY ([AffiliateFileMappingId]) REFERENCES [Exclusive].[AffiliateFileMapping] ([Id])
);

