GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190710044958_StagingCashbackTransactionPaidField', N'2.1.11-servicing-32099')
GO

ALTER TABLE Staging.CashbackTransaction
ADD Paid bit not null default 0
GO

ALTER TABLE Staging.CashbackTransactionErrors
ADD Paid bit not null default 0
GO
