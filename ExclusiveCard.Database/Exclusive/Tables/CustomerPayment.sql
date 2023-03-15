CREATE TABLE [Exclusive].[CustomerPayment] (
    [Id]                    INT             IDENTITY (1, 1) NOT NULL,
    [CustomerId]            INT             NULL,
    [MembershipCardId]      INT             NULL,
    [PaymentProviderId]     INT             NULL,
    [PaymentDate]           DATETIME2 (7)   NOT NULL,
    [Amount]                DECIMAL (18, 2) NOT NULL,
    [CurrencyCode]          NVARCHAR (3)    NULL,
    [Details]               NVARCHAR (MAX) NULL,
    [CashbackTransactionId] INT             NULL,
    [PaymentNotificationId] INT             NULL,
    [PaymentProviderRef]    NVARCHAR (100)  NULL,
    CONSTRAINT [PK_CustomerPayment] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UNIQ_PaymentProviderRef] UNIQUE([PaymentProviderRef]), 
    CONSTRAINT [FK_CustomerPayment_CashbackTransaction_CashbackTransactionId] FOREIGN KEY ([CashbackTransactionId]) REFERENCES [Exclusive].[CashbackTransaction] ([Id]),
    CONSTRAINT [FK_CustomerPayment_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Exclusive].[Customer] ([Id]),
    CONSTRAINT [FK_CustomerPayment_MembershipCard_MembershipCardId] FOREIGN KEY ([MembershipCardId]) REFERENCES [Exclusive].[MembershipCard] ([Id]),
    CONSTRAINT [FK_CustomerPayment_PaymentNotification_PaymentNotificationId] FOREIGN KEY ([PaymentNotificationId]) REFERENCES [Exclusive].[PaymentNotification] ([Id]),
    CONSTRAINT [FK_CustomerPayment_Paymentprovider_PaymentProviderId] FOREIGN KEY ([PaymentProviderId]) REFERENCES [Exclusive].[Paymentprovider] ([Id])
);

