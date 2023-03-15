GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-02J7PG5KFVH4'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(37, 'I-02J7PG5KFVH4', 41,'2018-04-20', 29.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-4C713HVVVESD'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(1278, 'I-4C713HVVVESD', 38, '2019-03-07', 19.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-4GNDUKX4MH96'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(2, 'I-4GNDUKX4MH96', 41, null, 0,'One Time Only')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-5LXRFPCEH7DD'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(751, 'I-5LXRFPCEH7DD', 41, '2018-02-22', 29.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-6GGEPLS5XKVS'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(1013, 'I-6GGEPLS5XKVS', 41, '2017-06-02', 3.00, 'Monthly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-7J3K0U7SY9DB'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(894, 'I-7J3K0U7SY9DB', 41, '2018-04-03', 29.95,'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-826ST8SKUYMW'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(366, 'I-826ST8SKUYMW', 38, '2019-03-06', 19.95, null)
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-A3A170B4SBEG'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(840, 'I-A3A170B4SBEG', 41, '2018-03-17', 29.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-AP12EG03W8TB'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(941, 'I-AP12EG03W8TB', 41, '2018-04-10', 29.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-B1XRASL6R1R9'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(704, 'I-B1XRASL6R1R9', 41, '2017-04-09', 29.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-G28YPCYG6BAM'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(852, 'I-G28YPCYG6BAM', 41, '2018-03-21', 29.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-GMR7LJPCN8F5'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(923, 'I-GMR7LJPCN8F5', 41, '2018-04-07', 29.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-HTSU1147P9SF'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(1273, 'I-HTSU1147P9SF', 38, '2019-03-05', 19.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-S33Y6N6HAFA2'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(70, 'I-S33Y6N6HAFA2', 41, '2017-11-19', 29.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-T6JL8K6LT30L'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(1282, 'I-T6JL8K6LT30L', 37, '2019-03-10', 19.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-TBC42MJCP932'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(974, 'I-TBC42MJCP932', 41, '2017-06-17', 29.95, 'Yearly')
END
GO
IF NOT EXISTS (SELECT * FROM Exclusive.PayPalSubscription WHERE (PayPalId = 'I-VGGTVVWV8G79'))
BEGIN
INSERT INTO Exclusive.PayPalSubscription VALUES(705, 'I-VGGTVVWV8G79', 41, '2017-04-09', 29.95, 'Yearly')
END
GO

INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190312112330_addStagingOfferAwinDuplicate', N'2.1.4-rtm-31024')
GO

Alter Table Staging.OfferImportAwin
alter column PromotionId NVARCHAR(255) NULL
GO

/****** Object:  Table [Staging].[OfferImportAwinDuplicate]    Script Date: 12/03/2019 17:06:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Staging].[OfferImportAwinDuplicate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OfferImportFileId] [int] NOT NULL,
	[PromotionId] [nvarchar](255) NULL,
	[Type] [nvarchar](max) NULL,
	[Code] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Starts] [datetime2](7) NOT NULL,
	[Ends] [datetime2](7) NOT NULL,
	[Categories] [nvarchar](max) NULL,
	[Regions] [nvarchar](max) NULL,
	[Terms] [nvarchar](max) NULL,
	[DeeplinkTracking] [nvarchar](max) NULL,
	[Deeplink] [nvarchar](max) NULL,
	[CommissionGroups] [nvarchar](max) NULL,
	[Commission] [nvarchar](max) NULL,
	[Exclusive] [nvarchar](max) NULL,
	[DateAdded] [datetime2](7) NOT NULL,
	[Title] [nvarchar](512) NULL,
 CONSTRAINT [PK_OfferImportAwinDuplicate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [Staging].[OfferImportAwinDuplicate]  WITH CHECK ADD  CONSTRAINT [FK_OfferImportAwinDuplicate_OfferImportFile_OfferImportFileId] FOREIGN KEY([OfferImportFileId])
REFERENCES [Staging].[OfferImportFile] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [Staging].[OfferImportAwinDuplicate] CHECK CONSTRAINT [FK_OfferImportAwinDuplicate_OfferImportFile_OfferImportFileId]
GO