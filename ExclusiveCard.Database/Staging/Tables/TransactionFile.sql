CREATE TABLE [Staging].[TransactionFile] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [FileName] NVARCHAR (100) NULL,
    [DateFrom] DATETIME2 (7)  NOT NULL,
    [DateTo]   DATETIME2 (7)  NOT NULL,
    [StatusId] INT            NOT NULL,
    CONSTRAINT [PK_TransactionFile] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TransactionFile_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [Exclusive].[Status] ([Id])
);

