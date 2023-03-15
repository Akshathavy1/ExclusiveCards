CREATE TABLE [Exclusive].[MerchantSocialMediaLink] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [MerchantId]           INT            NOT NULL,
    [SocialMediaCompanyId] INT            NOT NULL,
    [SocialMediaURI]       NVARCHAR (512) NULL,
    CONSTRAINT [PK_MerchantSocialMediaLink] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MerchantSocialMediaLink_Merchant_MerchantId] FOREIGN KEY ([MerchantId]) REFERENCES [Exclusive].[Merchant] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MerchantSocialMediaLink_SocialMediaCompany_SocialMediaCompanyId] FOREIGN KEY ([SocialMediaCompanyId]) REFERENCES [Exclusive].[SocialMediaCompany] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_MerchantSocialMediaLink_1144F35E4CC1CF9FD0798A21A873AD9E]
    ON [Exclusive].[MerchantSocialMediaLink]([MerchantId] ASC)
    INCLUDE([SocialMediaCompanyId], [SocialMediaURI]);

