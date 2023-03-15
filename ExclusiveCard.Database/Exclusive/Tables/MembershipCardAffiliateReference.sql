CREATE TABLE [Exclusive].[MembershipCardAffiliateReference] (
    [MembershipCardId] INT            NOT NULL,
    [AffiliateId]      INT            NOT NULL,
    [CardReference]    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_MembershipCardAffiliateReference] PRIMARY KEY CLUSTERED ([AffiliateId] ASC, [MembershipCardId] ASC),
    CONSTRAINT [FK_MembershipCardAffiliateReference_Affiliate_AffiliateId] FOREIGN KEY ([AffiliateId]) REFERENCES [Exclusive].[Affiliate] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MembershipCardAffiliateReference_MembershipCard_MembershipCardId] FOREIGN KEY ([MembershipCardId]) REFERENCES [Exclusive].[MembershipCard] ([Id]) ON DELETE CASCADE
);

