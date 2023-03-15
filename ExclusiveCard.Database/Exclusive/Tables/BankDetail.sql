CREATE TABLE [Exclusive].[BankDetail] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [BankName]        NVARCHAR (100) NULL,
    [ContactDetailId] INT            NULL,
    [SortCode]        NVARCHAR (16)  NULL,
    [AccountNumber]   NVARCHAR (16)  NULL,
    [AccountName]     NVARCHAR (100) NULL,
    [IsDeleted]       BIT            NOT NULL,
    CONSTRAINT [PK_BankDetail] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BankDetail_ContactDetail_ContactDetailId] FOREIGN KEY ([ContactDetailId]) REFERENCES [Exclusive].[ContactDetail] ([Id])
);

