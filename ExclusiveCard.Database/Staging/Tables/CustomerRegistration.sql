CREATE TABLE [Staging].[CustomerRegistration] (
    [CustomerPaymentId] UNIQUEIDENTIFIER NOT NULL,
    [Data]              NVARCHAR (MAX)   NULL,
    [StatusId]          INT              NOT NULL,
    [AspNetUserId]      NVARCHAR (450)   NULL,
    CONSTRAINT [PK_CustomerRegistration] PRIMARY KEY CLUSTERED ([CustomerPaymentId] ASC),
    CONSTRAINT [FK_CustomerRegistration_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [Exclusive].[Status] ([Id]) ON DELETE CASCADE
);

