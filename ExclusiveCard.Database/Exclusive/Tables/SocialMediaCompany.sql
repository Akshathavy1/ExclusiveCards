CREATE TABLE [Exclusive].[SocialMediaCompany] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (128) NULL,
    [IsEnabled] BIT            NOT NULL,
    CONSTRAINT [PK_SocialMediaCompany] PRIMARY KEY CLUSTERED ([Id] ASC)
);

