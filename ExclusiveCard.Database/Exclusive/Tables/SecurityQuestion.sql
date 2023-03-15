CREATE TABLE [Exclusive].[SecurityQuestion] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Question] NVARCHAR (500) NULL,
    [IsActive] BIT            NOT NULL,
    CONSTRAINT [PK_SecurityQuestion] PRIMARY KEY CLUSTERED ([Id] ASC)
);

