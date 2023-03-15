CREATE TABLE [Exclusive].[ClickTracking] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [OfferId]          INT             NOT NULL,
    [MembershipCardId] INT             NOT NULL,
    [AffiliateId]      INT             NOT NULL,
    [DeeplinkURL]      NVARCHAR (1024) NULL,
    [DateTime]         DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_ClickTracking] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClickTracking_Affiliate_AffiliateId] FOREIGN KEY ([AffiliateId]) REFERENCES [Exclusive].[Affiliate] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ClickTracking_MembershipCard_MembershipCardId] FOREIGN KEY ([MembershipCardId]) REFERENCES [Exclusive].[MembershipCard] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ClickTracking_Offer_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Exclusive].[Offer] ([Id]) ON DELETE CASCADE
);

