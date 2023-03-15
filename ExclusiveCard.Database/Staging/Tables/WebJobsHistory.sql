CREATE TABLE [Staging].[WebJobsHistory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Timestamp] DATETIME2 NULL DEFAULT GetUTCDate(), 
    [JobName] VARCHAR(100) NOT NULL,
    [Status] VARCHAR(30) NULL,
    [Detail] VARCHAR(4096) NULL,
    [Errors] VARCHAR(4096) NULL
)
