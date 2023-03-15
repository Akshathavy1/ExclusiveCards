CREATE TABLE [Exclusive].[ErrorLog] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [MethodName]    NVARCHAR (100)  NULL,
    [StackTrace]    NVARCHAR (MAX)  NULL,
    [ApplicationId] INT             DEFAULT ((0)) NOT NULL,
    [AssemblyName]  NVARCHAR (100)  NULL,
    [ClassName]     NVARCHAR (100)  NULL,
    [EnvironmentId] INT             DEFAULT ((0)) NOT NULL,
    [Exception]     NVARCHAR (MAX)  NULL,
    [InnerMessage]  NVARCHAR (MAX)  NULL,
    [Level]         NVARCHAR (5)    NULL,
    [Logger]        NVARCHAR (80)   NULL,
    [MachineName]   NVARCHAR (50)   NULL,
    [Message]       NVARCHAR (4000) NULL,
    [ProcessId]     INT             DEFAULT ((0)) NOT NULL,
    [ThreadId]      INT             DEFAULT ((0)) NOT NULL,
    [TimeStamp]     DATETIME2 (7)   DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

