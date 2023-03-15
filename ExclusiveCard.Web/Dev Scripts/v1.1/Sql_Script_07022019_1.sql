GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190207054636_customerPaymentcustom', N'2.1.4-rtm-31024')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190207155430_ExtendOfferUrlLength', N'2.1.4-rtm-31024')
GO

ALTER TABLE Exclusive.CustomerPayment
ADD PaymentProviderRef NVARCHAR(100)

ALTER TABLE Exclusive.Offer
ALTER COLUMN LinkUrl NVARCHAR(MAX)
GO
ALTER TABLE Staging.Offer
ALTER COLUMN LinkUrl NVARCHAR(MAX)
