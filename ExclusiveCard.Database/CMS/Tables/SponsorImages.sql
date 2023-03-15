CREATE TABLE [CMS].[SponsorImages]
(
	[Id]             INT         IDENTITY (1, 1) NOT NULL,    
    [File]           NVARCHAR   (255) NULL,
    [WhiteLabelId]   INT         NOT NULL, 
    CONSTRAINT [PK_SponsorImages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SponsorImages_WhiteLabelSettings_WhiteLabelSettingsId] FOREIGN KEY ([WhiteLabelId]) REFERENCES [CMS].[WhitelabelSettings] ([Id]) ON DELETE CASCADE
);
