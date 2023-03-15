INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190219063412_countryCodeAddToStagingImportFile', N'2.1.4-rtm-31024')
GO	

ALTER TABLE [Staging].[OfferImportFile] Add [CountryCode] [nvarchar](3) NULL
GO

