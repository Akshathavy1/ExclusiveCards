GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190409094914_AdvertSlot', N'2.1.8-servicing-32085')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190415091342_PanelMaxSlot', N'2.1.8-servicing-32085')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190415093308_WebsiteSocialMediaLink', N'2.1.8-servicing-32085')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190415102007_AlterColumn', N'2.1.8-servicing-32085')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190422141613_SP_ExpireMembershipCards', N'2.1.8-servicing-32085')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190423050419_SP_ClearStagingCashbackTransactions', N'2.1.8-servicing-32085')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190423050509_SP_RunDailyMaintenanceTasks', N'2.1.8-servicing-32085')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190424053551_SP_StagingOfferToExclusiveOffer1', N'2.1.8-servicing-32085')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190424053649_SP_MoveOfferFromStagingToExclusive1', N'2.1.8-servicing-32085')



ALTER TABLE CMS.Adverts
ADD AdvertTypeId SMALLINT NOT NULL DEFAULT 1
GO

ALTER TABLE CMS.Adverts
ADD NumberOfSlots SMALLINT NOT NULL DEFAULT 0

GO
sp_rename 'CMS.Adverts.Description', 'Name', 'COLUMN';

GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CMS].[AdvertSlots](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdvertId] [int] NOT NULL,
	[SlotId] [int] NOT NULL,
	[ImagePath] [nvarchar](512) NOT NULL,
	[URL] [nvarchar](1024) NULL,
	[Sequence] [int] NOT NULL,
 CONSTRAINT [PK_AdvertSlots] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [CMS].[AdvertSlots]  WITH CHECK ADD  CONSTRAINT [FK_AdvertSlots_Adverts_AdvertId] FOREIGN KEY([AdvertId])
REFERENCES [CMS].[Adverts] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [CMS].[AdvertSlots] CHECK CONSTRAINT [FK_AdvertSlots_Adverts_AdvertId]
GO


ALTER TABLE CMS.Panels
ADD MaxSlotsCount SMALLINT NOT NULL
GO

update CMS.Panels SET MaxSlotsCount = 5 WHERE Name = 'Home Page Banner'
GO
update CMS.Panels SET MaxSlotsCount = 6 WHERE Name = 'Offer Details Banner'
GO
update CMS.Panels SET MaxSlotsCount = 6 WHERE Name = 'Search Page Banner'

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CMS].[WebsiteSocialMediaLink](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CountryCode] [nvarchar](3) NULL,
	[SocialMediaCompanyId] [int] NOT NULL,
	[SocialMediaURI] [nvarchar](512) NULL,
 CONSTRAINT [PK_WebsiteSocialMediaLink] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [CMS].[WebsiteSocialMediaLink]  WITH CHECK ADD  CONSTRAINT [FK_WebsiteSocialMediaLink_SocialMediaCompany_SocialMediaCompanyId] FOREIGN KEY([SocialMediaCompanyId])
REFERENCES [Exclusive].[SocialMediaCompany] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [CMS].[WebsiteSocialMediaLink] CHECK CONSTRAINT [FK_WebsiteSocialMediaLink_SocialMediaCompany_SocialMediaCompanyId]
GO


INSERT INTO [CMS].[WebsiteSocialMediaLink]
           ([CountryCode]
           ,[SocialMediaCompanyId]
           ,[SocialMediaURI])
     VALUES
           ('GB'
           ,1
           ,'https://www.facebook.com/exclusiveprivilegecard/')
        GO


INSERT INTO [CMS].[WebsiteSocialMediaLink]
           ([CountryCode]
           ,[SocialMediaCompanyId]
           ,[SocialMediaURI])
     VALUES
           ('GB'
           ,2
           ,'https://twitter.com/ExclusiveCard')
        GO
INSERT INTO [CMS].[WebsiteSocialMediaLink]
           ([CountryCode]
           ,[SocialMediaCompanyId]
           ,[SocialMediaURI])
     VALUES
           ('GB'
           ,3
           ,'https://www.instagram.com/exclusiveprivilegecard/')
        GO

INSERT INTO [CMS].[WebsiteSocialMediaLink]
           ([CountryCode]
           ,[SocialMediaCompanyId]
           ,[SocialMediaURI])
     VALUES
           ('SC'
           ,1
           ,'https://www.facebook.com/ExclusiveCardSY/')
        GO

INSERT INTO [CMS].[WebsiteSocialMediaLink]
           ([CountryCode]
           ,[SocialMediaCompanyId]
           ,[SocialMediaURI])
     VALUES
           ('SC'
           ,2
           ,'https://twitter.com/ExclusiveCardSY')
        GO

INSERT INTO [CMS].[WebsiteSocialMediaLink]
           ([CountryCode]
           ,[SocialMediaCompanyId]
           ,[SocialMediaURI])
     VALUES
           ('SC'
           ,3
           ,'https://www.instagram.com/exclusivecardsy/')
    GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [Exclusive].[SP_ExpireMembershipCards]	                       
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [Exclusive].[MembershipCard]
	SET StatusId = (SELECT Id From Exclusive.Status s where s.Type = 'MembershipCard' AND s.Name = 'Expired' AND s.IsActive = 1)
	WHERE ValidTo < GETUTCDATE()
END
GO


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [Exclusive].[SP_ClearStagingCashbackTransactions]	                       
AS
BEGIN
	SET NOCOUNT ON;

	TRUNCATE TABLE [Staging].[CashbackTransaction]
	
END

GO


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [Exclusive].[SP_RunDailyMaintenanceTasks]
	                       
AS
BEGIN
	SET NOCOUNT ON;

	EXEC [Exclusive].[SP_ClearStagingCashbackTransactions]
	EXEC [Exclusive].[SP_ExpireMembershipCards]
END
GO



GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER Procedure [Exclusive].[SP_StagingOfferToExclusiveOffer]
	@OFFERIMPORTID INT, -- TO BE PASSED IN SP CALL
	@AFFILIATEID INT, -- TO BE PASSED IN SP CALL
	@RECORDTOPROCESS INT
AS
BEGIN
	BEGIN TRY
	--GET TOP 100 OFFERS INTO TEMPORARY TABLE
	SELECT TOP(@RECORDTOPROCESS) IDENTITY(int, 1, 1) AS RowID, CAST(Id as int) as Id,
	MerchantId, AffiliateId, OfferTypeId, StatusId, ValidFrom, ValidTo, Validindefinately, ShortDescription, LongDescription,
	Instructions, Terms, Exclusions, LinkUrl, OfferCode, Reoccuring, SearchRanking, Datecreated, Headline, affiliateReference
	INTO #STAGINGTEMP FROM STAGING.OFFER WHERE AffiliateId = @AFFILIATEID

	DECLARE @COUNT INT = 0
	DECLARE @INDEX INT = 1
	DECLARE @OFFEREXISTSCOUNT INT = 0
	DECLARE @SUCCESSRECORDS INT = 0
	DECLARE @UPDATEDRECORDS INT = 0

	--take count of the records in stagingTemp
	SELECT @COUNT = COUNT(*) FROM #STAGINGTEMP

	WHILE(@INDEX <= @COUNT)
		BEGIN
	                    
			DECLARE @AFFILIATEREFERENCE NVARCHAR(255) = NULL
			DECLARE @STMERCHANTID INT = NULL
			DECLARE @STAGINGOFFERID INT = NULL
			DECLARE @INSERTED INT = NULL

			-- Get the affiliateReference, MERCHANT ID AND OFFER ID FROM STAGING TEMP FOR THIS RECORD from first record
			SELECT @AFFILIATEREFERENCE = AffiliateReference, @STMERCHANTID = MerchantId, @STAGINGOFFERID = ID 
			FROM #STAGINGTEMP WHERE RowID = @INDEX AND AffiliateId = @AFFILIATEID

			--CHECK IF AFFILIATEID AND AFFILIATE REFERENCE ARE NOT NULL
			IF(@affiliateId IS NOT NULL AND @AFFILIATEREFERENCE IS NOT NULL AND @AFFILIATEREFERENCE <> '')
				BEGIN
			                    
					DECLARE @EXCLUSIVEOFFERID INT = NULL
					DECLARE @EXCLUSIVEMERCHANTID INT = NULL
					DECLARE @EXCLUSIVEOFFERSTATUS INT = NULL
					--CHECK IF RECORD EXISTS IN EXCLUSIVE.OFFER FOR THE AFFILIATE AND REFERENCE
					SELECT @EXCLUSIVEOFFERID = ID, @EXCLUSIVEMERCHANTID = MerchantId, @EXCLUSIVEOFFERSTATUS = StatusId FROM Exclusive.Offer WHERE AffiliateId = @AFFILIATEID AND AffiliateReference = @AFFILIATEREFERENCE

					IF(@EXCLUSIVEOFFERID IS NOT NULL)
						BEGIN					
							IF(@EXCLUSIVEOFFERSTATUS = (SELECT Id FROM Exclusive.Status WHERE Type = 'Offer' AND Name = 'Active'))
							BEGIN
							IF(@STMERCHANTID IS NOT NULL AND @EXCLUSIVEMERCHANTID IS NOT NULL AND @STMERCHANTID = @EXCLUSIVEMERCHANTID)
								BEGIN
														
									DECLARE @UPDATED INT = NULL
									--CAll SP TO CHECK IF CATEGORY AND COUNTRY MATCHES THE EXISTING AND UPDATE IF MATCHES ELSE MOVE TO DUPILCATES
									EXEC [Exclusive].[SP_CheckIfStagingOfferExistsInExclusive] @OFFERIMPORTID, @STAGINGOFFERID, @EXCLUSIVEOFFERID, @UPDATED OUTPUT

									IF(@UPDATED IS NOT NULL AND @UPDATED = 1)
										BEGIN
											-- UPDATE THE RECORD
											UPDATE EX SET
											EX.ShortDescription = ST.ShortDescription,
											EX.LongDescription = ST.LongDescription,
											EX.Exclusions = ST.Exclusions,
											EX.LinkUrl = ST.LinkUrl, 
											EX.ValidFrom = ST.ValidFrom, 
											EX.ValidTo = ST.ValidTo, 
											EX.Validindefinately = ST.Validindefinately,
											EX.Headline = ST.Headline, 
											EX.OfferCode = ST.OfferCode, 
											EX.Reoccuring = ST.Reoccuring, 
											EX.StatusId = ST.StatusId,
											Ex.DateUpdated = GETUTCDATE()
											FROM Exclusive.OFFER EX, Staging.Offer ST WHERE EX.Id = @EXCLUSIVEOFFERID AND ST.Id = @STAGINGOFFERID

											SET @UPDATEDRECORDS = @UPDATEDRECORDS + @UPDATED
										END
									ELSE IF(@UPDATED IS NOT NULL AND @UPDATED = 2)
										BEGIN
											INSERT INTO Staging.OfferImportAwinDuplicate(OfferImportFileId, PromotionId, [Type], Code, [Description], Starts, Ends, Terms, DeeplinkTracking, DateAdded, Title)
											(SELECT @OFFERIMPORTID, AFFILIATEREFERENCE, O.[Description], OfferCode, LongDescription, ValidFrom, ValidTo, Terms, LinkUrl, Datecreated, ShortDescription
												FROM Staging.Offer T INNER JOIN Exclusive.OfferType O ON (O.Id = T.OfferTypeId) where T.ID = @STAGINGOFFERID)
										END
								END
							ELSE
								BEGIN
									--INSERT INTO DUPLICATES IF Merchant not matches
									INSERT INTO Staging.OfferImportAwinDuplicate(OfferImportFileId, PromotionId, [Type], Code, [Description], Starts, Ends, Terms, DeeplinkTracking, DateAdded, Title)
									(SELECT @OFFERIMPORTID, AFFILIATEREFERENCE, O.[Description], OfferCode, LongDescription, ValidFrom, ValidTo, Terms, LinkUrl, Datecreated, ShortDescription
										FROM #STAGINGTEMP T INNER JOIN Exclusive.OfferType O ON (O.Id = T.OfferTypeId) where RowID = @INDEX)
								END
							END
							ELSE
							BEGIN

								UPDATE EX SET
											Ex.AffiliateReference = Ex.AffiliateReference + '-' + (SELECT CONVERT(VARCHAR, GETUTCDATE(), 112) + REPLACE(CONVERT(VARCHAR, GETUTCDATE(), 108), ':', ''))
											FROM Exclusive.OFFER EX WHERE EX.Id = @EXCLUSIVEOFFERID

																                     
							EXEC [Exclusive].[SP_MoveOfferFromStagingToExclusive] @STAGINGOFFERID, @INSERTED OUTPUT
					                    
							IF(@INSERTED IS NOT NULL AND @INSERTED > 0)
								BEGIN
									-- IMPORT SUCCESS SO, INCREMENT THE COUNT
									SET @SUCCESSRECORDS = @SUCCESSRECORDS + 1
								END
							END

						END
					ELSE
						BEGIN
	     
							EXEC [Exclusive].[SP_MoveOfferFromStagingToExclusive] @STAGINGOFFERID, @INSERTED OUTPUT
					                    
							IF(@INSERTED IS NOT NULL AND @INSERTED > 0)
								BEGIN
									-- IMPORT SUCCESS SO, INCREMENT THE COUNT
									SET @SUCCESSRECORDS = @SUCCESSRECORDS + 1
								END
						END
				END
			ELSE
				BEGIN
					--INSERT INTO DUPLICATES IF NULL. THIS SCENARIO SHOULDN'T OCCUR IDEALLY
					INSERT INTO Staging.OfferImportAwinDuplicate(OfferImportFileId, PromotionId, [Type], Code, [Description], Starts, Ends, Terms, DeeplinkTracking, DateAdded, Title)
					(SELECT @OFFERIMPORTID, AFFILIATEREFERENCE, O.[Description], OfferCode, LongDescription, ValidFrom, ValidTo, Terms, LinkUrl, Datecreated, ShortDescription
						FROM #STAGINGTEMP T INNER JOIN Exclusive.OfferType O ON (O.Id = T.OfferTypeId) where RowID = @INDEX)
				END

			--INCREMENT INDEX to take next record
			SET @INDEX = @INDEX + 1
		END
		END TRY
		BEGIN CATCH
		INSERT INTO dbo.Error(ErrorNumber, ErrorLine, ErrorMessage, ErrorSeverity, ErrorState)
		(SELECT ERROR_NUMBER(), ERROR_LINE(), ERROR_MESSAGE(), ERROR_SEVERITY(), ERROR_STATE())
		END CATCH
	DELETE FROM Staging.Offer WHERE ID IN (SELECT ID FROM #STAGINGTEMP)
	DROP TABLE #STAGINGTEMP

	-- Update Error record and success record into staging.offerimportAwin and status to Migrated state
	Update Staging.OfferImportFile SET Imported = Imported + @SUCCESSRECORDS,
	Duplicates = (Select Count(*) from Staging.OfferImportAwinDuplicate where OfferImportFileId = @OFFERIMPORTID),
	Updates = Updates + @UPDATEDRECORDS
	Where Id = @OFFERIMPORTID

	--check if staging table is empty and then update the status as migrated
	IF((select COUNT(*) from Staging.Offer) = 0)
		BEGIN
			Update Staging.OfferImportFile SET
			ImportStatus = (select Id from Exclusive.[Status] where [type] = 'Import' and Name='Migrated' and IsActive = 1)
			Where Id = @OFFERIMPORTID
		END
END




GO
/****** Object:  StoredProcedure [Exclusive].[SP_MoveOfferFromStagingToExclusive]    Script Date: 26-Apr-19 1:50:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Exclusive].[SP_MoveOfferFromStagingToExclusive]
	-- Add the parameters for the stored procedure here
	@STAGINGOFFERID INT,
	@INSERTED INT OUTPUT
AS
BEGIN
BEGIN TRY
	DECLARE @NEWEXCLUSIVEOFFERID INT = NULL

	-- INSERT INTO EXCLUSIVE.OFFER TABLE SINCE WE DID NOT FIND THE OFFER
	INSERT INTO Exclusive.Offer
		(MerchantId, AffiliateId, OfferTypeId, StatusId, ValidFrom, ValidTo,Validindefinately,ShortDescription,LongDescription,Instructions,Terms,Exclusions,LinkUrl,OfferCode,Reoccuring,SearchRanking,Datecreated,Headline,affiliateReference) 
	SELECT MerchantId,AffiliateId,OfferTypeId,StatusId,ValidFrom,ValidTo,Validindefinately,ShortDescription,LongDescription,Instructions,Terms,Exclusions,LinkUrl,OfferCode,Reoccuring,SearchRanking,Datecreated,Headline,affiliateReference
	FROM Staging.Offer Where ID = @STAGINGOFFERID

	SET @INSERTED = @@ROWCOUNT

	--take new Exclusive.Offer Id
	SELECT @NEWEXCLUSIVEOFFERID = SCOPE_IDENTITY()

	-- DROP THE TEMP TABLE IF EXISTS
	IF OBJECT_ID('tempdb..#STAGINGCATEGORYTEMPNEW') IS NOT NULL
		DROP TABLE #STAGINGCategoryNewTEMP
					                    
	DECLARE @CATINDEX INT = 1
	DECLARE @CATCOUNT INT = 0
					                    
	SELECT IDENTITY(int, 1, 1) AS RowID, *
	INTO #STAGINGCATEGORYTEMPNEW FROM STAGING.OfferCategory WHERE OfferId = @STAGINGOFFERID
					                    
	SELECT @CATCOUNT = COUNT(*) From #STAGINGCATEGORYTEMPNEW

	WHILE (@CATINDEX <= @CATCOUNT)
	BEGIN
		--INSERT INTO OFFER CATEGORY
		INSERT INTO Exclusive.OfferCategory(OfferId,CategoryId)
		(SELECT @NEWEXCLUSIVEOFFERID, CategoryId From #STAGINGCATEGORYTEMPNEW 
		where RowID = @CATINDEX)

		SET @CATINDEX = @CATINDEX + 1
	END


	DECLARE @CONINDEX INT = 1
	DECLARE @CONCOUNT INT = 0

	-- CHECK IF TEMP TABLE EXISTS AND DROP
	IF OBJECT_ID('tempdb..#STAGINGCOUNTRYTEMPNEW') IS NOT NULL
		DROP TABLE #STAGINGCOUNTRYTEMPNEW
					                    
	-- SELECT COUNTRIES FOR THE SELECTED OFFER FROM STAGING
	SELECT IDENTITY(int,1,1) as RowID, *
	INTO #STAGINGCOUNTRYTEMPNEW FROM Staging.OfferCountry WHERE OfferId = @STAGINGOFFERID
					                    
	SELECT @CONCOUNT = COUNT(*) From #STAGINGCOUNTRYTEMPNEW

	WHILE(@CONINDEX <= @CONCOUNT)
	BEGIN

		-- INSERT INTO OFFER COUNTRY
		INSERT INTO Exclusive.OfferCountry(OfferId,CountryCode,IsActive) 
		(SELECT @NEWEXCLUSIVEOFFERID, CountryCode, IsActive From #STAGINGCOUNTRYTEMPNEW 
		where RowID = @CONINDEX)
						                    
		SET @CONINDEX = @CONINDEX + 1
	END

	SELECT @INSERTED
	END TRY
		BEGIN CATCH
		INSERT INTO dbo.Error(ErrorNumber, ErrorLine, ErrorMessage, ErrorSeverity, ErrorState)
		(SELECT ERROR_NUMBER(), ERROR_LINE(), ERROR_MESSAGE(), ERROR_SEVERITY(), ERROR_STATE())
		END CATCH
END
