GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190304125143_RemoveAwinDuplicates', N'2.1.4-rtm-31024')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190306061420_UniqueIndex', N'2.1.4-rtm-31024')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190306062831_StagingOfferReference', N'2.1.4-rtm-31024')
GO

ALTER TABLE EXCLUSIVE.Offer
ADD AffiliateReference NVARCHAR(255) NULL

GO

/****** Object:  Index [IX_Offer_AffiliateId_AffiliateReference]    Script Date: 06-Mar-19 11:46:58 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Offer_AffiliateId_AffiliateReference] ON [Exclusive].[Offer]
(
	[AffiliateId] ASC,
	[AffiliateReference] ASC
)
WHERE ([AffiliateId] IS NOT NULL AND [AffiliateReference] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE Staging.OfferImportFile
ADD Duplicates INT NOT NULL
GO

ALTER TABLE Staging.Offer
ADD AffiliateReference NVARCHAR(255) NULL

GO
INSERT INTO [Exclusive].[AffiliateFieldMapping] VALUES(1, 'PromotionId', 'Staging.Offer', 'AffiliateReference', 0, NULL, NULL, 1, 1)

GO
DROP INDEX [IX_MembershipCard_CardNumber] ON [Exclusive].[MembershipCard]
GO


