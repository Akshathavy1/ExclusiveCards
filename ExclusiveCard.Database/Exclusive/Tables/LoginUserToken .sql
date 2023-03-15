CREATE TABLE [Exclusive].[LoginUserToken ] (
    [AspNetUserId] NVARCHAR (450)   NULL,
    [Token]        NVARCHAR (MAX)   NULL,
    [TokenValue]   UNIQUEIDENTIFIER NOT NULL,
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_LoginUserToken ] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LoginUserToken _AspNetUsers_AspNetUserId] FOREIGN KEY ([AspNetUserId]) REFERENCES [Exclusive].[AspNetUsers] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_LoginUserToken _AspNetUserId]
    ON [Exclusive].[LoginUserToken ]([AspNetUserId] ASC);

