GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190205041202_ErrorLog', N'2.1.4-rtm-31024')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190205044841_ErrorLogExtended', N'2.1.4-rtm-31024')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190207054636_customerPaymentcustom', N'2.1.4-rtm-31024')
GO


GO

/****** Object:  Table [Exclusive].[ErrorLog]    Script Date: 07/02/2019 14:49:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Exclusive].[ErrorLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MethodName] [nvarchar](100) NULL,
	[StackTrace] [nvarchar](max) NULL,
	[ApplicationId] [int] NOT NULL,
	[AssemblyName] [nvarchar](100) NULL,
	[ClassName] [nvarchar](100) NULL,
	[EnvironmentId] [int] NOT NULL,
	[Exception] [nvarchar](max) NULL,
	[InnerMessage] [nvarchar](max) NULL,
	[Level] [nvarchar](5) NULL,
	[Logger] [nvarchar](80) NULL,
	[MachineName] [nvarchar](50) NULL,
	[Message] [nvarchar](4000) NULL,
	[ProcessId] [int] NOT NULL,
	[ThreadId] [int] NOT NULL,
	[TimeStamp] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [Exclusive].[ErrorLog] ADD  DEFAULT ((0)) FOR [ApplicationId]
GO

ALTER TABLE [Exclusive].[ErrorLog] ADD  DEFAULT ((0)) FOR [EnvironmentId]
GO

ALTER TABLE [Exclusive].[ErrorLog] ADD  DEFAULT ((0)) FOR [ProcessId]
GO

ALTER TABLE [Exclusive].[ErrorLog] ADD  DEFAULT ((0)) FOR [ThreadId]
GO

ALTER TABLE [Exclusive].[ErrorLog] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [TimeStamp]
GO


alter table Exclusive.CustomerPayment
add PaymentProviderRef nvarchar(100);