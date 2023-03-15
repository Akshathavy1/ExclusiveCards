CREATE TABLE [CMS].[WebsiteSocialMediaLink] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [CountryCode]          NVARCHAR (3)   NULL,
    [SocialMediaCompanyId] INT            NOT NULL,
    [SocialMediaURI]       NVARCHAR (512) NULL,
    [WhiteLabelSettingsId] INT NULL, 
    CONSTRAINT [PK_WebsiteSocialMediaLink] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WebsiteSocialMediaLink_SocialMediaCompany_SocialMediaCompanyId] FOREIGN KEY ([SocialMediaCompanyId]) REFERENCES [Exclusive].[SocialMediaCompany] ([Id]) ON DELETE CASCADE
);

