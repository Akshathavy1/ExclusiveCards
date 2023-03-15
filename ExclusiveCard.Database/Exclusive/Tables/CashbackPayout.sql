CREATE TABLE [Exclusive].[CashbackPayout] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [CustomerId]    INT             NOT NULL,
    [StatusId]      INT             NOT NULL,
    [PayoutDate]    DATETIME2 (7)   NULL,
    [Amount]        DECIMAL (18, 2) NOT NULL,
    [CurrencyCode]  NVARCHAR (3)    NULL,
    [BankDetailId]  INT             NULL,
    [ChangedDate]   DATETIME2 (7)   DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [RequestedDate] DATETIME2 (7)   DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [UpdatedBy]     NVARCHAR (450)  NULL,
    CONSTRAINT [PK_CashbackPayout] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CashbackPayout_BankDetail_BankDetailId] FOREIGN KEY ([BankDetailId]) REFERENCES [Exclusive].[BankDetail] ([Id]),
    CONSTRAINT [FK_CashbackPayout_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Exclusive].[Customer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CashbackPayout_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [Exclusive].[Status] ([Id]) ON DELETE CASCADE
);

