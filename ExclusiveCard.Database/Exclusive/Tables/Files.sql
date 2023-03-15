CREATE TABLE [Exclusive].[Files] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (100)  NULL,
    [PartnerId]       INT             NULL,
    [Type]            NVARCHAR (15)   NULL,
    [StatusId]        INT             NOT NULL,
    [PaymentStatusId] INT             NULL,
    [TotalAmount]     DECIMAL (18, 2) NULL,
    [CreatedDate]     DATETIME2 (7)   NOT NULL,
    [ChangedDate]     DATETIME2 (7)   NULL,
    [PaidDate]        DATETIME2 (7)   NULL,
    [UpdatedBy]       NVARCHAR (450)  NULL,
    [ConfirmedAmount] DECIMAL (18, 2) NULL,
    [Location]        NVARCHAR (255)  NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Files_Partner_PartnerId] FOREIGN KEY ([PartnerId]) REFERENCES [Exclusive].[Partner] ([Id]),
    CONSTRAINT [FK_Files_Status_PaymentStatusId] FOREIGN KEY ([PaymentStatusId]) REFERENCES [Exclusive].[Status] ([Id]),
    CONSTRAINT [FK_Files_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [Exclusive].[Status] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Files_PartnerId]
    ON [Exclusive].[Files]([PartnerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Files_PaymentStatusId]
    ON [Exclusive].[Files]([PaymentStatusId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Files_StatusId]
    ON [Exclusive].[Files]([StatusId] ASC);

