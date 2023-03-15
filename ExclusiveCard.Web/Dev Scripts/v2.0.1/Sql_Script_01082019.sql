GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190801040207_AlterSummaryFieldInCashback', N'2.1.11-servicing-32099')
GO

ALTER TABLE Exclusive.CashbackTransaction
ALTER Column Summary NVARCHAR(128)
