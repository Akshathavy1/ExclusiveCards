CREATE TABLE [Exclusive].[Customer] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [ContactDetailId]     INT            NULL,
    [Title]               NVARCHAR (100) NULL,
    [Forename]            NVARCHAR (100) NULL,
    [Surname]             NVARCHAR (100) NULL,
    [IsActive]            BIT            NOT NULL,
    [IsDeleted]           BIT            NOT NULL,
    [AspNetUserId]        NVARCHAR (450) NULL,
    [DateOfBirth]         DATETIME2 (7)  NULL,
    [DateAdded]           DATETIME2 (7)  DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [MarketingNewsLetter] BIT            DEFAULT ((0)) NOT NULL,
    [MarketingThirdParty] BIT            DEFAULT ((0)) NOT NULL,
    [NINumber]            NVARCHAR (10)  NULL,
    [SiteClanId]          INT            NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Customer_AspNetUsers_AspNetUserId] FOREIGN KEY ([AspNetUserId]) REFERENCES [Exclusive].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_Customer_ContactDetail_ContactDetailId] FOREIGN KEY ([ContactDetailId]) REFERENCES [Exclusive].[ContactDetail] ([Id]),
    CONSTRAINT [FK_Customer_SiteClan_SiteClanId] FOREIGN KEY ([SiteClanId]) REFERENCES [CMS].[SiteClan] ([Id])

);


GO
CREATE NONCLUSTERED INDEX [nci_wi_Customer_18F2F7601DF57F0AA3A758F12909333A]
    ON [Exclusive].[Customer]([AspNetUserId] ASC, [IsDeleted] ASC)
    INCLUDE([ContactDetailId], [DateAdded], [DateOfBirth], [Forename], [IsActive], [MarketingNewsLetter], [MarketingThirdParty], [NINumber], [Surname], [Title]);

