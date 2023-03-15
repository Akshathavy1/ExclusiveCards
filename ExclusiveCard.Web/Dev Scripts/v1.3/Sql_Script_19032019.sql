GO

/****** Object:  StoredProcedure [Exclusive].[SP_TriggerStagingToExclusive]    Script Date: 19-Mar-19 10:34:40 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		GSS - WINSTON
-- Create date: 15 Mar 2019
-- Description:	Get files to be processed with status in Processing and move offers from staging to exclusive
-- =============================================
CREATE PROCEDURE [Exclusive].[SP_TriggerStagingToExclusive]
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

	--GET RECORDS INTO TEMP VARIABLE TABLE TO PROCESS THE FILES
	INSERT INTO @RecordToProcess (FileId, AffiliateId)
    (SELECT I.Id, AF.AffiliateId
	FROM Staging.OfferImportFile I WITH(NOLOCK) 
	INNER JOIN Exclusive.AffiliateFile AF ON (I.AffiliateFileId = AF.Id)
	WHERE I.ImportStatus = 42)


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


GO
/****** Object:  StoredProcedure [Exclusive].[SP_StagingOfferToExclusiveOffer]    Script Date: 19-Mar-19 10:34:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [Exclusive].[SP_StagingOfferToExclusiveOffer]
	@OFFERIMPORTID INT, -- TO BE PASSED IN SP CALL
	@AFFILIATETYPE INT, -- TO BE PASSED IN SP CALL
	@RECORDCOUNT INT
AS
BEGIN
DECLARE @COUNT INT = 0
DECLARE @INDEX INT = 1
DECLARE @OFFEREXISTSCOUNT INT = 0
DECLARE @SuccessRecord INT = 0

--GET ALL OFFERS INTO TEMPORARY TABLE
SELECT TOP(@RECORDCOUNT) IDENTITY(int, 1, 1) AS RowID,CAST(Id as int) as Id,
MerchantId,AffiliateId,OfferTypeId,StatusId,ValidFrom,ValidTo,Validindefinately,ShortDescription,LongDescription,
Instructions,Terms,Exclusions,LinkUrl,OfferCode,Reoccuring,SearchRanking,Datecreated,Headline,affiliateReference
INTO #STAGINGTEMP FROM STAGING.OFFER
--GET COUNT OF RECORDS TO BE MOVED
SELECT @COUNT = COUNT(*) FROM #STAGINGTEMP

--CHECK ANY DUPLICATE OFFER EXISTS IN STAGING TABLE
SELECT ID, AFFILIATEREFERENCE INTO #OFFEREXISTS FROM (SELECT ID, AFFILIATEREFERENCE FROM EXCLUSIVE.OFFER 
WHERE AFFILIATEID = @AFFILIATETYPE AND AffiliateReference IN (SELECT AFFILIATEREFERENCE FROM STAGING.OFFER)) A

--GET OfferExists Count 
SELECT @OFFEREXISTSCOUNT = COUNT(*) FROM #OFFEREXISTS


WHILE(@INDEX <= @COUNT)
BEGIN
	
	DECLARE @EXCULSIVEOFFERID INT = NULL
	DECLARE @CHECKEXCULSIVEOFFERID INT = null
	DECLARE @NewExclusiveID INT = null
	DECLARE @STAGINGCategoryROWCount INT = NULL
	DECLARE @STAGINGCountryROWCount INT = null
	BEGIN TRY	
		BEGIN TRANSACTION OFFERS
			IF(@OFFEREXISTSCOUNT > 0)
			BEGIN
				-- CHECK for AFFILIATEREFERENCE Exists in OfferExists Temp Table
				SELECT @EXCULSIVEOFFERID = ID from #OFFEREXISTS where AFFILIATEREFERENCE = (SELECT AFFILIATEREFERENCE from #STAGINGTEMP where RowID = @INDEX)
				IF(@EXCULSIVEOFFERID is Not NULL)
				BEGIN
					-- CHECK FOR exclusive OFFERCOUNTRY and staging OfferCountry
					IF OBJECT_ID('tempdb..#STAGINGCountryExistsTEMP') IS NOT NULL
						DROP TABLE #STAGINGCountryExistsTEMP
					SELECT * INTO #STAGINGCountryExistsTEMP FROM STAGING.OfferCountry WHERE OfferId IN (SELECT ID from #STAGINGTEMP Where RowID = @INDEX)
					
					IF OBJECT_ID('tempdb..#EXCLUSIVECountryExistsTemp') IS NOT NULL
						DROP TABLE #EXCLUSIVECountryExistsTemp
					SELECT * INTO #EXCLUSIVECountryExistsTemp FROM exclusive.OfferCountry WHERE OfferId = @EXCULSIVEOFFERID
					
					IF((Select COUNT(*) from #STAGINGCountryExistsTEMP) = (SELECT COUNT(*) from #EXCLUSIVECountryExistsTemp))
					BEGIN
						IF OBJECT_ID('tempdb..#STEXOFFERCATEGORYTEMP') IS NOT NULL
							DROP TABLE #STEXOFFERCATEGORYTEMP
						SELECT * INTO #STEXOFFERCATEGORYTEMP FROM(
							SELECT CountryCode FROM #EXCLUSIVECountryExistsTemp
							UNION ALL
							SELECT COUNTRYCODE FROM #STAGINGCountryExistsTEMP
							) tbl
							GROUP BY CountryCode
							HAVING count(*) = 1
						IF(SELECT COUNT(*) FROM #STEXOFFERCATEGORYTEMP) > 0
						BEGIN
							-- Save To DUPLICATE TEMPTABLE	(if offerCountry not matching)
							INSERT INTO Staging.OfferImportAwinDuplicate(OfferImportFileId, PromotionId, [Type], Code, [Description], Starts, Ends, Terms, DeeplinkTracking, DateAdded, Title)
							(SELECT @OFFERIMPORTID, AFFILIATEREFERENCE, O.[Description], OfferCode, LongDescription, ValidFrom, ValidTo, Terms, LinkUrl, Datecreated, ShortDescription
								FROM #STAGINGTEMP T INNER JOIN Exclusive.OfferType O ON (O.Id = T.OfferTypeId) where RowID = @INDEX)
						END
						ELSE
						BEGIN
							
							DECLARE @HEADLINE_ST NVARCHAR(100)							
							DECLARE @SHORTDES_ST NVARCHAR(MAX)							
							DECLARE @LONGDES_ST NVARCHAR(MAX)							
							DECLARE @VALIDFROM_ST DATETIME							
							DECLARE @VALIDTO_ST DATETIME
							
							SELECT @HEADLINE_ST = Headline, @SHORTDES_ST = ShortDescription, @LONGDES_ST = LongDescription, @VALIDFROM_ST = ValidFrom, @VALIDTO_ST = ValidTo 
							FROM exclusive.Offer WHERE ID = @EXCULSIVEOFFERID

							-- CHECK FOR Headline,Short Description,Long Description, ValidFrom and ValidTo is Same
							SELECT @CHECKEXCULSIVEOFFERID = ID from exclusive.Offer WHERE Id = @EXCULSIVEOFFERID and Headline = @HEADLINE_ST
													and ShortDescription = @SHORTDES_ST and LongDescription = @LONGDES_ST and ValidFrom = @VALIDFROM_ST and ValidTo = @VALIDTO_ST
							IF(@CHECKEXCULSIVEOFFERID is NOT NULL)
							BEGIN
								-- Save To DUPLICATE TEMPTABLE	(if offercountry matching true and search terms are same)
								INSERT INTO Staging.OfferImportAwinDuplicate(OfferImportFileId,PromotionId,[Type],Code,[Description],Starts,Ends,Terms,DeeplinkTracking,DateAdded,Title) 
								(SELECT @OFFERIMPORTID, AFFILIATEREFERENCE, O.[Description], OfferCode, LongDescription, ValidFrom,	ValidTo, Terms, LinkUrl, Datecreated, ShortDescription 
								from #STAGINGTEMP INNER JOIN Exclusive.OfferType O ON (O.Id = OfferTypeId) where RowID = @INDEX)
							END
							ELSE
							BEGIN
								-- Save To DUPLICATE TEMPTABLE	(if offercountry matching true and search terms are different) UPdate exclusive.Offer
								UPDATE O SET
								O.ValidFrom = T.ValidFrom, O.ValidTo = T.ValidTo, O.HeadLine = T.Headline, O.ShortDescription = T.ShortDescription, O.LongDescription = T.LongDescription
								FROM Exclusive.Offer O JOIN #STAGINGTEMP T ON (O.Id = T.Id) WHERE T.RowID = @INDEX
								
								SET @SuccessRecord = @SuccessRecord + 1
							END	
						END
					END
					ELSE
					BEGIN
						-- Save To DUPLICATE TEMPTABLE	(if offerCountry count not matching)
						INSERT INTO Staging.OfferImportAwinDuplicate(OfferImportFileId, PromotionId, [Type], Code, [Description], Starts, Ends, Terms, DeeplinkTracking, DateAdded, Title)
						(SELECT @OFFERIMPORTID, AFFILIATEREFERENCE, O.[Description], OfferCode, LongDescription, ValidFrom, ValidTo, Terms, LinkUrl, Datecreated, ShortDescription
						FROM #STAGINGTEMP T INNER JOIN Exclusive.OfferType O ON (O.Id = T.OfferTypeId) where RowID = @INDEX)
					END
				END
				ELSE
				BEGIN
					-- Else insert into Exclusive.Offer (if offerID not found in Exists Offer temp table)
					INSERT INTO exclusive.Offer(MerchantId, AffiliateId, OfferTypeId, StatusId, ValidFrom, ValidTo,Validindefinately,ShortDescription,LongDescription,Instructions,Terms,Exclusions,LinkUrl,OfferCode,Reoccuring,SearchRanking,Datecreated,Headline,affiliateReference) 
					SELECT MerchantId,AffiliateId,OfferTypeId,StatusId,ValidFrom,ValidTo,Validindefinately,ShortDescription,LongDescription,Instructions,Terms,Exclusions,LinkUrl,OfferCode,Reoccuring,SearchRanking,Datecreated,Headline,affiliateReference FROM #STAGINGTEMP Where RowID = @INDEX
					-- take new Exclusive.Offer Id and insert exclusive.OfferCategory
						IF OBJECT_ID('tempdb..#STAGINGCategoryTEMP') IS NOT NULL
							DROP TABLE #STAGINGCategoryTEMP
						SELECT @NewExclusiveID = SCOPE_IDENTITY();
						SELECT IDENTITY(int, 1, 1) AS RowID,* 
						INTO #STAGINGCategoryTEMP FROM STAGING.OfferCategory WHERE OfferId = (SELECT Id from #STAGINGTEMP where RowID=@INDEX)
						WHILE (SELECT COUNT(*) From #STAGINGCategoryTEMP) > 0
						BEGIN
							SELECT TOP 1 @STAGINGCategoryROWCount = RowID from #STAGINGCategoryTEMP
							INSERT INTO Exclusive.OfferCategory(OfferId,CategoryId) 
							(SELECT @NewExclusiveID, CategoryId From #STAGINGCategoryTEMP where RowID = @STAGINGCategoryROWCount)
							DELETE #STAGINGCategoryTEMP where RowID = @STAGINGCategoryROWCount
						END
						IF OBJECT_ID('tempdb..#STAGINGCOUNTRYTemp') IS NOT NULL
							DROP TABLE #STAGINGCOUNTRYTemp
						-- insert exclusive.OfferCountry
						SELECT IDENTITY(int,1,1) as RowID, *
						INTO #STAGINGCOUNTRYTemp FROM Staging.OfferCountry WHERE OfferId = (SELECT Id from #STAGINGTEMP where RowID=@INDEX)
						WHILE(SELECT COUNT(*) FROM #STAGINGCOUNTRYTemp) > 0
						BEGIN
							SELECT TOP 1 @STAGINGCountryROWCount = RowID from #STAGINGCOUNTRYTemp
							INSERT INTO Exclusive.OfferCountry(OfferId,CountryCode,IsActive) 
							(SELECT @NewExclusiveID, CountryCode, IsActive From #STAGINGCOUNTRYTemp where RowID = @STAGINGCountryROWCount)
							DELETE #STAGINGCOUNTRYTemp where RowID = @STAGINGCountryROWCount
						END
					SET @SuccessRecord = @SuccessRecord + 1
				END
			END
			ELSE
			BEGIN
				-- Else insert into Exclusive.Offer (if OfferExists count 0)
				INSERT INTO exclusive.Offer(MerchantId, AffiliateId, OfferTypeId, StatusId, ValidFrom, ValidTo,Validindefinately,ShortDescription,LongDescription,Instructions,Terms,Exclusions,LinkUrl,OfferCode,Reoccuring,SearchRanking,Datecreated,Headline,affiliateReference) 
				SELECT MerchantId,AffiliateId,OfferTypeId,StatusId,ValidFrom,ValidTo,Validindefinately,ShortDescription,LongDescription,Instructions,Terms,Exclusions,LinkUrl,OfferCode,Reoccuring,SearchRanking,Datecreated,Headline,affiliateReference FROM #STAGINGTEMP Where RowID = @INDEX
					-- take new Exclusive.Offer Id and insert exclusive.OfferCategory
					IF OBJECT_ID('tempdb..#STAGINGCategoryNewTEMP') IS NOT NULL
						DROP TABLE #STAGINGCategoryNewTEMP
					SELECT @NewExclusiveID = SCOPE_IDENTITY();
					SELECT IDENTITY(int, 1, 1) AS RowID,* 
					INTO #STAGINGCategoryNewTEMP FROM STAGING.OfferCategory WHERE OfferId = (SELECT Id from #STAGINGTEMP where RowID=@INDEX)
					--SELECT COUNT(*) From #STAGINGCategoryNewTEMP
					WHILE (SELECT COUNT(*) From #STAGINGCategoryNewTEMP) > 0
					BEGIN
						SELECT TOP 1 @STAGINGCategoryROWCount = RowID from #STAGINGCategoryNewTEMP
						INSERT INTO Exclusive.OfferCategory(OfferId,CategoryId)
						(SELECT @NewExclusiveID, CategoryId From #STAGINGCategoryNewTEMP where RowID = @STAGINGCategoryROWCount)
						DELETE #STAGINGCategoryNewTEMP where RowID = @STAGINGCategoryROWCount
					END
					-- insert exclusive.OfferCountry
					IF OBJECT_ID('tempdb..#STAGINGCOUNTRYNewTemp') IS NOT NULL
						DROP TABLE #STAGINGCOUNTRYNewTemp
					SELECT IDENTITY(int,1,1) as RowID, *
					INTO #STAGINGCOUNTRYNewTemp FROM Staging.OfferCountry WHERE OfferId = (SELECT Id from #STAGINGTEMP where RowID=@INDEX)
					WHILE(SELECT COUNT(*) FROM #STAGINGCOUNTRYNewTemp) > 0
					BEGIN
						SELECT TOP 1 @STAGINGCountryROWCount = RowID from #STAGINGCOUNTRYNewTemp
						INSERT INTO Exclusive.OfferCountry(OfferId,CountryCode,IsActive) 
						(SELECT @NewExclusiveID, CountryCode, IsActive From #STAGINGCOUNTRYNewTemp where RowID = @STAGINGCountryROWCount)
						DELETE #STAGINGCOUNTRYNewTemp where RowID = @STAGINGCountryROWCount
					END
					SET @SuccessRecord = @SuccessRecord + 1
			END
			-- DELETE in staging.Offer with Id
			DELETE from Staging.Offer where Id = (SELECT Id from #STAGINGTEMP where RowID = @INDEX)
		COMMIT TRANSACTION OFFERS
	END TRY
	BEGIN CATCH
		PRINT 'Error in process'
		IF(@@TRANCOUNT > 0)
		BEGIN
			INSERT INTO dbo.Error(ErrorNumber, ErrorLine, ErrorMessage, ErrorSeverity, ErrorState)
			(SELECT ERROR_NUMBER(), ERROR_LINE(), ERROR_MESSAGE(), ERROR_SEVERITY(), ERROR_STATE())
			ROLLBACK TRANSACTION OFFERS
		END
	END CATCH
	SET @INDEX = @INDEX + 1
END

	-- Update Error record and success record into staging.offerimportAwin and status to Migrated state
	Update Staging.OfferImportFile SET Imported = Imported + @SuccessRecord, 
	Duplicates = (Select Count(*) from Staging.OfferImportAwinDuplicate where OfferImportFileId = @OFFERIMPORTID)
	Where Id = @OFFERIMPORTID

	--check if staging table is empty and then update the status as migrated
	if((select COUNT(*) from Staging.Offer) = 0)
	BEGIN
		Update Staging.OfferImportFile SET
		ImportStatus = (select Id from Exclusive.[Status] where [type] = 'Import' and Name='Migrated' and IsActive = 1)
		Where Id = @OFFERIMPORTID
	END

	IF OBJECT_ID('tempdb..#STAGINGTEMP') IS NOT NULL
	DROP TABLE #STAGINGTEMP
	IF OBJECT_ID('tempdb..#OFFEREXISTS') IS NOT NULL
	DROP TABLE #OFFEREXISTS
	IF OBJECT_ID('tempdb..#STAGINGCountryExistsTEMP') IS NOT NULL
	DROP TABLE #STAGINGCountryExistsTEMP
	IF OBJECT_ID('tempdb..#EXCLUSIVECountryExistsTemp') IS NOT NULL
	DROP TABLE #EXCLUSIVECountryExistsTemp
	IF OBJECT_ID('tempdb..#STEXOFFERCATEGORYTEMP') IS NOT NULL
	DROP TABLE #STEXOFFERCATEGORYTEMP
	IF OBJECT_ID('tempdb..#STAGINGCategoryNewTEMP') IS NOT NULL
	DROP TABLE #STAGINGCategoryNewTEMP
	IF OBJECT_ID('tempdb..#STAGINGCOUNTRYNewTemp') IS NOT NULL
	DROP TABLE #STAGINGCOUNTRYNewTemp
	IF OBJECT_ID('tempdb..#STAGINGCategoryTEMP') IS NOT NULL
	DROP TABLE #STAGINGCategoryTEMP
	IF OBJECT_ID('tempdb..#STAGINGCOUNTRYTemp') IS NOT NULL
	DROP TABLE #STAGINGCOUNTRYTemp

END
GO

INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190318071534_addstagingCustomerRegistration', N'2.1.4-rtm-31024')
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Staging].[CustomerRegistration](
	[CustomerPaymentId] [uniqueidentifier] NOT NULL,
	[Data] [nvarchar](max) NULL,
	[StatusId] [int] NOT NULL,
 CONSTRAINT [PK_CustomerRegistration] PRIMARY KEY CLUSTERED 
(
	[CustomerPaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [Staging].[CustomerRegistration]  WITH CHECK ADD  CONSTRAINT [FK_CustomerRegistration_Status_StatusId] FOREIGN KEY([StatusId])
REFERENCES [Exclusive].[Status] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [Staging].[CustomerRegistration] CHECK CONSTRAINT [FK_CustomerRegistration_Status_StatusId]
GO



IF NOT EXISTS (SELECT * FROM Exclusive.Status WHERE (Name = 'New' and Type='CustomerCreation' and IsActive = 1))
BEGIN
INSERT INTO Exclusive.Status(Name,Type,IsActive) VALUES('New','CustomerCreation',1)
END
GO

IF NOT EXISTS (SELECT * FROM Exclusive.Status WHERE (Name = 'Processing' and Type='CustomerCreation' and IsActive = 1))
BEGIN
INSERT INTO Exclusive.Status(Name,Type,IsActive) VALUES('Processing','CustomerCreation',1)
END
GO

IF NOT EXISTS (SELECT * FROM Exclusive.Status WHERE (Name = 'Processed' and Type='CustomerCreation' and IsActive = 1))
BEGIN
INSERT INTO Exclusive.Status(Name,Type,IsActive) VALUES('Processed','CustomerCreation',1)
END
GO