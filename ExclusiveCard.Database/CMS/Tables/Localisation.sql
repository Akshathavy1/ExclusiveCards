CREATE TABLE [CMS].[Localisation] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [LocalisedText]    NVARCHAR (512) NULL,
    [Context]          NVARCHAR (512) NULL,
    [LocalisationCode] NVARCHAR (5)   NULL,
    CONSTRAINT [PK_Localisation] PRIMARY KEY CLUSTERED ([Id] ASC)
);

