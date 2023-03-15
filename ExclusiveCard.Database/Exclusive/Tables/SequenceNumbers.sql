CREATE TABLE [Exclusive].[SequenceNumbers]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Description] NVARCHAR(50) NOT NULL, 
    [Value] BIGINT NOT NULL
)
