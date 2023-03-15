CREATE TABLE [Exclusive].[OfferType] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [Description]              NVARCHAR (128) NULL,
    [IsActive]                 BIT            NOT NULL,
    [ActionTextLocalisationId] INT            NULL,
    [TitleLocalisationId]      INT            NULL,
    [SearchRanking]            INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_OfferType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OfferType_Localisation_ActionTextLocalisationId] FOREIGN KEY ([ActionTextLocalisationId]) REFERENCES [CMS].[Localisation] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OfferType_Localisation_TitleLocalisationId] FOREIGN KEY ([TitleLocalisationId]) REFERENCES [CMS].[Localisation] ([Id])
);

