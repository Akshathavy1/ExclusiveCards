CREATE TABLE [Exclusive].[CashbackSummary] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [AffiliateId]      INT             NULL,
    [MembershipCardId] INT             NOT NULL,
    [PartnerId]        INT             NULL,
    [AccountType]      NVARCHAR (1)    NOT NULL,
    [CurrencyCode]     NVARCHAR (3)    NULL,
    [PendingAmount]    DECIMAL (18, 2) NOT NULL,
    [ConfirmedAmount]  DECIMAL (18, 2) NOT NULL,
    [ReceivedAmount]   DECIMAL (18, 2) NOT NULL,
    [FeeDue]           DECIMAL (18, 2) DEFAULT ((0.0)) NOT NULL,
    [PaidAmount]       DECIMAL (18, 2) DEFAULT ((0.0)) NOT NULL,
    CONSTRAINT [PK_CashbackSummary] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CashbackSummary_Affiliate_AffiliateId] FOREIGN KEY ([AffiliateId]) REFERENCES [Exclusive].[Affiliate] ([Id]),
    CONSTRAINT [FK_CashbackSummary_MembershipCard_MembershipCardId] FOREIGN KEY ([MembershipCardId]) REFERENCES [Exclusive].[MembershipCard] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CashbackSummary_Partner_PartnerId] FOREIGN KEY ([PartnerId]) REFERENCES [Exclusive].[Partner] ([Id])
);

