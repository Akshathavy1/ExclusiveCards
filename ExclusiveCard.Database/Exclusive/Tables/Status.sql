CREATE TABLE [Exclusive].[Status] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (50) NULL,
    [Type]     NVARCHAR (50) NULL,
    [IsActive] BIT           NOT NULL,
    CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([Id] ASC)
);

