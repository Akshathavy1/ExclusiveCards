CREATE TABLE [Exclusive].[PaymentNotification] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [PaymentProviderId]         INT            NOT NULL,
    [TransactionType]           NVARCHAR (MAX) NULL,
    [DateReceived]              DATETIME2 (7)  NOT NULL,
    [FullMessage]               NVARCHAR (MAX) NULL,
    [CustomerPaymentProviderId] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_PaymentNotification] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PaymentNotification_Paymentprovider_PaymentProviderId] FOREIGN KEY ([PaymentProviderId]) REFERENCES [Exclusive].[Paymentprovider] ([Id]) ON DELETE CASCADE
);

