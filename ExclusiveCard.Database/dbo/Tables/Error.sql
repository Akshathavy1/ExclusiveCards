CREATE TABLE [dbo].[Error] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [ErrorNumber]   INT             NULL,
    [ErrorLine]     INT             NULL,
    [ErrorMessage]  NVARCHAR (4000) NULL,
    [ErrorSeverity] INT             NULL,
    [ErrorState]    INT             NULL,
    CONSTRAINT [PK_Error] PRIMARY KEY CLUSTERED ([Id] ASC)
);

