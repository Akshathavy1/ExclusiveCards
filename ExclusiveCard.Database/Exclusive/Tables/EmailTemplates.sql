CREATE TABLE [Exclusive].[EmailTemplates] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [EmailName]      NVARCHAR (100) NULL,
    [Subject]        NVARCHAR (512) NULL,
    [BodyText]       NVARCHAR (MAX) NULL,
    [BodyHtml]       NVARCHAR (MAX) NULL,
    [HeaderText]     NVARCHAR (MAX) NULL,
    [HeaderHtml]     NVARCHAR (MAX) NULL,
    [FooterText]     NVARCHAR (MAX) NULL,
    [FooterHtml]     NVARCHAR (MAX) NULL,
    [IsDeleted]      BIT            NOT NULL,
    [TemplateTypeId] INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_EmailTemplates] PRIMARY KEY CLUSTERED ([Id] ASC)
);

