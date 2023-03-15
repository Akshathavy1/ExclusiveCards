CREATE TABLE [Exclusive].[EmailsSent] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [EmailTemplateId] INT            NOT NULL,
    [CustomerId]      INT            NOT NULL,
    [EmailTo]         NVARCHAR (512) NULL,
    [DateSent]        DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_EmailsSent] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmailsSent_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Exclusive].[Customer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmailsSent_EmailTemplates_EmailTemplateId] FOREIGN KEY ([EmailTemplateId]) REFERENCES [Exclusive].[EmailTemplates] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_EmailsSent_CustomerId]
    ON [Exclusive].[EmailsSent]([CustomerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EmailsSent_EmailTemplateId]
    ON [Exclusive].[EmailsSent]([EmailTemplateId] ASC);

