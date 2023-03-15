CREATE TABLE [Exclusive].[CustomerBankDetail] (
    [CustomerId]          INT           NOT NULL,
    [BankDetailsId]       INT           NOT NULL,
    [MandateAccepted]     BIT           NOT NULL,
    [DateMandateAccepted] DATETIME2 (7) NULL,
    [IsActive]            BIT           NOT NULL,
    [IsDeleted]           BIT           NOT NULL,
    CONSTRAINT [PK_CustomerBankDetail] PRIMARY KEY CLUSTERED ([CustomerId] ASC, [BankDetailsId] ASC),
    CONSTRAINT [FK_CustomerBankDetail_BankDetail_BankDetailsId] FOREIGN KEY ([BankDetailsId]) REFERENCES [Exclusive].[BankDetail] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CustomerBankDetail_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Exclusive].[Customer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [AK_CustomerBankDetail_BankDetailsId_CustomerId] UNIQUE NONCLUSTERED ([BankDetailsId] ASC, [CustomerId] ASC)
);

