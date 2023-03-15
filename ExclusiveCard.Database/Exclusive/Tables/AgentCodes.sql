CREATE TABLE [Exclusive].[AgentCode]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
	[ReportCode] NVARCHAR (16) NOT NULL,
	[Name] NVARCHAR (255) NULL,
	[Description] nVARchar(255) NULL,
	[CommissionPercent] decimal (18,2) NULL,
	[StartDate] datetime2 null,
	[EndDate] datetime2 null,
	[isDeleted] bit DEFAULT(0) NOT NULL
)
