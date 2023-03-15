CREATE TABLE [Exclusive].[Paymentprovider] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (100) NULL,
    [IsActive] BIT            NOT NULL,
    CONSTRAINT [PK_Paymentprovider] PRIMARY KEY CLUSTERED ([Id] ASC)
);

