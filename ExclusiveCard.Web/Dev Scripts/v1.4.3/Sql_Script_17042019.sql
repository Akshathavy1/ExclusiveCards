GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190416115559_ExtendDetailsField', N'2.1.8-servicing-32085')
GO

ALTER TABLE Exclusive.CustomerPayment
ALTER COLUMN Details NVARCHAR(1024) NULL
GO