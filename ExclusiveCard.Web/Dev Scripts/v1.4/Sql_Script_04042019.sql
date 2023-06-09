INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190327123431_OfferTypeFieldChanges', N'2.1.4-rtm-31024')
GO

ALTER TABLE Exclusive.OfferType
ADD SearchRanking int NOT NULL DEFAULT(0)
GO


update Exclusive.OfferType set SearchRanking = 1 where Id = (select Id from Exclusive.OfferType where Description = 'Cashback' and IsActive = 1 and SearchRanking = 0)
update Exclusive.OfferType set SearchRanking = 2 where Id = (select Id from Exclusive.OfferType where Description = 'Standard Cashback' and IsActive = 1 and SearchRanking = 0)
update Exclusive.OfferType set SearchRanking = 3 where Id = (select Id from Exclusive.OfferType where Description = 'Voucher Code' and IsActive = 1 and SearchRanking = 0)
update Exclusive.OfferType set SearchRanking = 4 where Id = (select Id from Exclusive.OfferType where Description = 'Sales' and IsActive = 1 and SearchRanking = 0)
update Exclusive.OfferType set SearchRanking = 5 where Id = (select Id from Exclusive.OfferType where Description = 'High Street' and IsActive = 1 and SearchRanking = 0)
update Exclusive.OfferType set SearchRanking = 6 where Id = (select Id from Exclusive.OfferType where Description = 'Restaurant' and IsActive = 1 and SearchRanking = 0)


INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190401052242_customOfferAndImportFileFields', N'2.1.4-rtm-31024')
GO

ALTER TABLE [Staging].[OfferImportFile] 
ADD Updates int Not Null DEFAULT(0)
GO

ALTER TABLE [Exclusive].[Offer]  WITH CHECK ADD  CONSTRAINT [FK_Offer_Affiliate_AffiliateId] FOREIGN KEY([AffiliateId])
REFERENCES [Exclusive].[Affiliate] ([Id])
GO

ALTER TABLE [Exclusive].[Offer] CHECK CONSTRAINT [FK_Offer_Affiliate_AffiliateId]
GO


GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190402114225_DateUpdatedForOffer', N'2.1.8-servicing-32085')

ALTER TABLE Exclusive.Offer
ADD DateUpdated DATETIME NULL
GO

INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190403105556_SP_CheckIfStagingOfferExistsInExclusive', N'2.1.4-rtm-31024')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190403110045_SP_MoveOfferFromStagingToExclusive', N'2.1.4-rtm-31024')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190403110321_SP_StagingOfferToExclusiveOffer', N'2.1.4-rtm-31024')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190403110756_SP_TriggerStagingToExclusive', N'2.1.4-rtm-31024')
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190403111006_SP_CustomerSearch', N'2.1.4-rtm-31024')
GO

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Error](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ErrorNumber] [int] NULL,
	[ErrorLine] [int] NULL,
	[ErrorMessage] [nvarchar](4000) NULL,
	[ErrorSeverity] [int] NULL,
	[ErrorState] [int] NULL,
 CONSTRAINT [PK_Error] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Exclusive].[SP_CheckIfStagingOfferExistsInExclusive]
	@OFFERIMPORTID INT,
	@STAGINGOFFERID INT,
	@EXCLUSIVEOFFERID INT,
	@UPDATED INT OUTPUT
AS
BEGIN
	-- GET OFFER CATEGORY AND OFFER COUNTRY FROM STAGING TABLE
	IF OBJECT_ID('tempdb..#STAGINGCATEGORYTEMP') IS NOT NULL
		DROP TABLE #STAGINGCATEGORYTEMP

	IF OBJECT_ID('tempdb..#EXCLUSIVECATORYTEMP') IS NOT NULL
		DROP TABLE #EXCLUSIVECATORYTEMP

	IF OBJECT_ID('tempdb..#STAGINGCOUNTRYTEMP') IS NOT NULL
		DROP TABLE #STAGINGCOUNTRYTEMP

	IF OBJECT_ID('tempdb..#EXCLUSIVECOUNTRYTEMP') IS NOT NULL
		DROP TABLE #EXCLUSIVECOUNTRYTEMP

	-- CATEGORY
	SELECT * INTO #STAGINGCATEGORYTEMP FROM Staging.OfferCategory WHERE OfferId = @STAGINGOFFERID

	SELECT * INTO #EXCLUSIVECATORYTEMP FROM Exclusive.OfferCategory WHERE OfferId = @EXCLUSIVEOFFERID

	--COUNTRY
	SELECT * INTO #STAGINGCOUNTRYTEMP FROM STAGING.OfferCountry WHERE OfferId = @STAGINGOFFERID					
					
	SELECT * INTO #EXCLUSIVECOUNTRYTEMP FROM Exclusive.OfferCountry WHERE OfferId = @EXCLUSIVEOFFERID

	IF((SELECT COUNT(*) FROM #STAGINGCOUNTRYTEMP) = (SELECT COUNT(*) FROM #EXCLUSIVECOUNTRYTEMP))
		BEGIN
			--CHECK IF COUNTRY IN STAGING ARE THERE IN EXCLUSIVE
			DECLARE @CHECK1COUNT INT = 0
			DECLARE @CHECK2COUNT INT = 0

			DECLARE @CHECK3COUNT INT = 0
			DECLARE @CHECK4COUNT INT = 0

			SELECT @CHECK1COUNT = COUNT(*) FROM #STAGINGCOUNTRYTEMP WHERE CountryCode NOT IN (SELECT CountryCode FROM #EXCLUSIVECOUNTRYTEMP)
			SELECT @CHECK2COUNT = COUNT(*) FROM #EXCLUSIVECOUNTRYTEMP WHERE CountryCode NOT IN (SELECT CountryCode FROM #STAGINGCOUNTRYTEMP)

			SELECT @CHECK3COUNT = COUNT(*) FROM #STAGINGCATEGORYTEMP WHERE CategoryId NOT IN (SELECT CategoryId FROM #EXCLUSIVECATORYTEMP)
			SELECT @CHECK4COUNT = COUNT(*) FROM #EXCLUSIVECATORYTEMP WHERE CategoryId NOT IN (SELECT CategoryId FROM #STAGINGCATEGORYTEMP)

			-- CHECK @CHECK1COUNT AND @CHECK2COUNT ARE 0 AND @MERCHANT ID MATCHES MERCHANT ID IN EXCLUSIVE OFFER
			IF(@CHECK1COUNT = 0 AND @CHECK2COUNT = 0 AND @CHECK3COUNT = 0 AND @CHECK4COUNT = 0)
				BEGIN
					-- SET TO 1 TO UPDATE EXCLSUIVE OFFER
					SET @UPDATED = 1
				END
			ELSE
				BEGIN
					-- SET TO 2 TO INSERT TO DUPLICATES
					SET @UPDATED = 2
				END
		END
	ELSE
		BEGIN
			--DUPLICATE RECORD WHEN COUNT OF COUNTRIES DON'T MATCH
			-- SET TO 2 TO INSERT TO DUPLICATES
			SET @UPDATED = 2
		END

	--DROP TEMP TABLES
	IF OBJECT_ID('tempdb..#STAGINGCATEGORYTEMP') IS NOT NULL
		DROP TABLE #STAGINGCATEGORYTEMP

	IF OBJECT_ID('tempdb..#EXCLUSIVECATORYTEMP') IS NOT NULL
		DROP TABLE #EXCLUSIVECATORYTEMP

	IF OBJECT_ID('tempdb..#STAGINGCOUNTRYTEMP') IS NOT NULL
		DROP TABLE #STAGINGCOUNTRYTEMP

	IF OBJECT_ID('tempdb..#EXCLUSIVECOUNTRYTEMP') IS NOT NULL
		DROP TABLE #EXCLUSIVECOUNTRYTEMP
END

GO



GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Exclusive].[SP_MoveOfferFromStagingToExclusive]
	-- Add the parameters for the stored procedure here
	@STAGINGOFFERID INT,
	@EXCLUSIVEOFFERID INT,
	@INSERTED INT OUTPUT
AS
BEGIN
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
END

GO





GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		GSS - WINSTON
-- Create date: 15 Mar 2019
-- Description:	Get files to be processed with status in Processing and move offers from staging to exclusive
-- =============================================
ALTER PROCEDURE [Exclusive].[SP_TriggerStagingToExclusive]
	@RECORDCOUNT INT
AS
BEGIN
	
	--CREATE TEMP VARIABLE TABLE
	DECLARE @RecordToProcess AS TABLE 
	(
	ROWID INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
	FileId INT NOT NULL, 
	AffiliateId INT NOT NULL
	)

	declare @Status INT
	SELECT @Status = ID FROM Exclusive.[Status] WHERE NAME = 'Processing' AND [TYPE] ='Import'

	--GET RECORDS INTO TEMP VARIABLE TABLE TO PROCESS THE FILES
	INSERT INTO @RecordToProcess (FileId, AffiliateId)
    (SELECT I.Id, AF.AffiliateId
	FROM Staging.OfferImportFile I WITH(NOLOCK) 
	INNER JOIN Exclusive.AffiliateFile AF ON (I.AffiliateFileId = AF.Id)
	WHERE I.ImportStatus = @Status)


	-- INITIALISE VARIABLES
	DECLARE @INDEX INT = 1
	DECLARE @COUNT INT = 0

	SELECT @COUNT = COUNT(*) FROM @RecordToProcess

	--LOOP THROUGH AND CALL SP_StagingOfferToExclusiveOffer
	WHILE(@INDEX <= @COUNT)
	BEGIN
		DECLARE @FILEID INT 
		DECLARE @AFFILIATE INT

		SELECT @FILEID = T.FileId, @AFFILIATE = T.AffiliateId FROM @RecordToProcess T WHERE T.ROWID = @INDEX

		Exec Exclusive.SP_StagingOfferToExclusiveOffer @FILEID, @AFFILIATE, @RECORDCOUNT

		SET @INDEX = @INDEX + 1
	END

END




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
			BEGIN TRY
				BEGIN TRANSACTION IMPORT
					DECLARE @AFFILIATEREFERENCE INT = NULL
					DECLARE @STMERCHANTID INT = NULL
					DECLARE @STAGINGOFFERID INT = NULL

					-- Get the affiliateReference, MERCHANT ID AND OFFER ID FROM STAGING TEMP FOR THIS RECORD from first record
					SELECT @AFFILIATEREFERENCE = AffiliateReference, @STMERCHANTID = MerchantId, @STAGINGOFFERID = ID 
					FROM #STAGINGTEMP WHERE RowID = @INDEX AND AffiliateId = @AFFILIATEID

					--CHECK IF AFFILIATEID AND AFFILIATE REFERENCE ARE NOT NULL
					IF(@affiliateId IS NOT NULL AND @AFFILIATEREFERENCE IS NOT NULL AND @AFFILIATEREFERENCE <> '')
						BEGIN
			
							DECLARE @EXCLUSIVEOFFERID INT = NULL
							DECLARE @EXCLUSIVEMERCHANTID INT = NULL
							--CHECK IF RECORD EXISTS IN EXCLUSIVE.OFFER FOR THE AFFILIATE AND REFERENCE
							SELECT @EXCLUSIVEOFFERID = ID, @EXCLUSIVEMERCHANTID = MerchantId FROM Exclusive.Offer WHERE AffiliateId = @AFFILIATEID AND AffiliateReference = @AFFILIATEREFERENCE

							IF(@EXCLUSIVEOFFERID IS NOT NULL)
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
													EX.ValidFrom = ST.ValidFrom, 
													EX.ValidTo = ST.ValidTo,													
													EX.Headline = ST.Headline,													
													Ex.DateUpdated = GETUTCDATE()
													FROM Exclusive.OFFER EX, Staging.Offer ST WHERE EX.Id = @EXCLUSIVEOFFERID AND ST.Id = @STAGINGOFFERID
													--EX.Exclusions = ST.Exclusions, EX.LinkUrl = ST.LinkUrl, EX.OfferCode = ST.OfferCode, EX.Reoccuring = ST.Reoccuring, EX.StatusId = ST.StatusId, EX.Validindefinately = ST.Validindefinately,

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
							 
									 DECLARE @INSERTED INT = NULL
							 
									 EXEC [Exclusive].[SP_MoveOfferFromStagingToExclusive] @STAGINGOFFERID, @EXCLUSIVEOFFERID, @INSERTED OUTPUT
					
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
				COMMIT TRANSACTION IMPORT
			END TRY
			BEGIN CATCH
				IF(@@TRANCOUNT > 0)
				BEGIN
					--INSERT INTO DUPLICATES AS I COULDN'T MOVE IT TO EXCLUSIVE OFFER
					INSERT INTO Staging.OfferImportAwinDuplicate(OfferImportFileId, PromotionId, [Type], Code, [Description], Starts, Ends, Terms, DeeplinkTracking, DateAdded, Title)
					(SELECT @OFFERIMPORTID, AFFILIATEREFERENCE, O.[Description], OfferCode, LongDescription, ValidFrom, ValidTo, Terms, LinkUrl, Datecreated, ShortDescription
						FROM #STAGINGTEMP T INNER JOIN Exclusive.OfferType O ON (O.Id = T.OfferTypeId) where RowID = @INDEX)
					
					--INSERT ERROR INTO ERROR
					INSERT INTO dbo.Error(ErrorNumber, ErrorLine, ErrorMessage, ErrorSeverity, ErrorState)
					(SELECT ERROR_NUMBER(), ERROR_LINE(), ERROR_MESSAGE(), ERROR_SEVERITY(), ERROR_STATE())
					ROLLBACK TRANSACTION IMPORT
				END
			END CATCH
		END
	
	DELETE FROM Staging.Offer WHERE ID IN (SELECT ID FROM #STAGINGTEMP)	
	DROP TABLE #STAGINGTEMP

	-- Update Error record and success record into staging.offerimportAwin and status to Migrated state
	Update Staging.OfferImportFile SET Imported = Imported + @SUCCESSRECORDS, Updates = Updates + @UPDATEDRECORDS,
	Duplicates = (Select Count(*) from Staging.OfferImportAwinDuplicate where OfferImportFileId = @OFFERIMPORTID)
	Where Id = @OFFERIMPORTID

	--check if staging table is empty and then update the status as migrated
	IF((select COUNT(*) from Staging.Offer) = 0)
		BEGIN
			Update Staging.OfferImportFile SET
			ImportStatus = (select Id from Exclusive.[Status] where [type] = 'Import' and Name='Migrated' and IsActive = 1)
			Where Id = @OFFERIMPORTID
		END
END
