CREATE TABLE [Exclusive].[Logs]
(
	[LogId] [int] IDENTITY(1,1) not null,
	[Level] [varchar](max) null,
	[CallSite] [varchar](max) null,
	[Type] [varchar](max) null,
	[Message] [varchar](max) not null,
	[StackTrace] [varchar](max) null,
	[InnerException] [varchar](max) null,
	[AdditionalInfo] [varchar](max) null,
	[LoggedOnDate] [datetime] not null constraint [df_logs_loggedondate]  default (getutcdate()),

	constraint [pk_logs] primary key clustered 
	(
		[LogId]
	)
)
