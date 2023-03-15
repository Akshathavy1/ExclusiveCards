CREATE TABLE [Exclusive].[CustomerSecurityQuestion] (
    [CustomerId]         INT            NOT NULL,
    [SecurityQuestionId] INT            NOT NULL,
    [Answer]             NVARCHAR (500) NULL,
    CONSTRAINT [PK_CustomerSecurityQuestion] PRIMARY KEY CLUSTERED ([CustomerId] ASC, [SecurityQuestionId] ASC),
    CONSTRAINT [FK_CustomerSecurityQuestion_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Exclusive].[Customer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CustomerSecurityQuestion_SecurityQuestion_SecurityQuestionId] FOREIGN KEY ([SecurityQuestionId]) REFERENCES [Exclusive].[SecurityQuestion] ([Id]) ON DELETE CASCADE
);

