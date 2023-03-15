CREATE TABLE [Exclusive].[CashbackTransaction] (
    [Id]                            INT             IDENTITY (1, 1) NOT NULL,
    [AffiliateId]                   INT             NULL,
    [MembershipCardId]              INT             NOT NULL,
    [PartnerId]                     INT             NULL,
    [AccountType]                   NVARCHAR (1)    NOT NULL,
    [AffiliateTransactionReference] NVARCHAR (50)   NULL,
    [TransactionDate]               DATETIME2 (7)   NOT NULL,
    [PurchaseAmount]                DECIMAL (18, 2) NOT NULL,
    [CurrencyCode]                  NVARCHAR (3)    NULL,
    [Summary]                       NVARCHAR (30)   NULL,
    [Detail]                        NVARCHAR (70)   NULL,
    [StatusId]                      INT             NULL,
    [ExpectedPaymentDate]           DATETIME2 (7)   NULL,
    [DateConfirmed]                 DATETIME2 (7)   NULL,
    [DateReceived]                  DATETIME2 (7)   NULL,
    [PartnerCashbackPayoutId]       INT             NULL,
    [CashbackAmount]                DECIMAL (18, 2) DEFAULT ((0.0)) NOT NULL,
    [MerchantId]                    INT             NULL,
    [FileId]                        INT             NULL,
    [PaymentStatusId]               INT             NULL,
    CONSTRAINT [PK_CashbackTransaction] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CashbackTransaction_Affiliate_AffiliateId] FOREIGN KEY ([AffiliateId]) REFERENCES [Exclusive].[Affiliate] ([Id]),
    CONSTRAINT [FK_CashbackTransaction_CashbackPayout_PartnerCashbackPayoutId] FOREIGN KEY ([PartnerCashbackPayoutId]) REFERENCES [Exclusive].[CashbackPayout] ([Id]),
    CONSTRAINT [FK_CashbackTransaction_Files_FileId] FOREIGN KEY ([FileId]) REFERENCES [Exclusive].[Files] ([Id]),
    CONSTRAINT [FK_CashbackTransaction_MembershipCard_MembershipCardId] FOREIGN KEY ([MembershipCardId]) REFERENCES [Exclusive].[MembershipCard] ([Id]),
    CONSTRAINT [FK_CashbackTransaction_Merchant_MerchantId] FOREIGN KEY ([MerchantId]) REFERENCES [Exclusive].[Merchant] ([Id]),
    CONSTRAINT [FK_CashbackTransaction_Partner_PartnerId] FOREIGN KEY ([PartnerId]) REFERENCES [Exclusive].[Partner] ([Id]),
    CONSTRAINT [FK_CashbackTransaction_Status_PaymentStatusId] FOREIGN KEY ([PaymentStatusId]) REFERENCES [Exclusive].[Status] ([Id]),
    CONSTRAINT [FK_CashbackTransaction_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [Exclusive].[Status] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_CashbackTransaction_PaymentStatusId]
    ON [Exclusive].[CashbackTransaction]([PaymentStatusId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CashbackTransaction_FileId]
    ON [Exclusive].[CashbackTransaction]([FileId] ASC);

