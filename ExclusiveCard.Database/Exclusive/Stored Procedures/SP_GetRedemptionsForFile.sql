
-- =============================================
-- Author:		GSS (Winston)
-- Create date: 04 Dec 2019
-- Description:	Get Love2Shop Offer Redemption requests for file creation
-- Updates: Moved the select to the end so that it runs in all scenarios (BUG 577)
--			Restructured data gathering so that an Exclusive.Files record is always created (Bug 568)
--			Fixed @Name file name calculation to avoid duplicates
-- =============================================
CREATE PROCEDURE [Exclusive].[SP_GetRedemptionsForFile]
	@BlobLove2Shop NVARCHAR(100)
AS
BEGIN	
	DECLARE @UpdateDate DATETIME
	DECLARE @CompleteId INT
	DECLARE @NoRecordsId INT
	DECLARE @RequestedId INT
	DECLARE @CreatedId INT
	DECLARE @Count INT
	DECLARE @FileId INT
	
	INSERT [Staging].[WebJobsHistory] (JobName, [Status], Detail) VALUES ('Love2Shop SendFile', 'OK', 'sp_GetRedemptionsForFile')

	SET @Count = 0
	
	--CHECK IF DATA EXISTS IN Requested state
	SELECT @RequestedId = Id FROM Exclusive.[Status] WITH(NOLOCK) WHERE [Name] = 'Requested' AND [Type] = 'OfferRedemptionStatus'

	SELECT @Count = COUNT(MembershipCardId) FROM Exclusive.OfferRedemption WITH(NOLOCK) WHERE [State] = @RequestedId

	--GENERATE FILE NAME
	DECLARE @Name NVARCHAR(100)
	--This was just wrong, dates like 11th Jan and 1st Nov would generate the same file name...
	--SET @Name = 'Love2shop_' + CAST(YEAR(GETDATE()) AS NVARCHAR) + CAST(MONTH(GETDATE()) AS NVARCHAR) + CAST(DAY(GETDATE()) AS NVARCHAR) + '.csv'
	SET @Name = 'Love2shop_' + CONVERT(NVARCHAR(8),GETDATE(), 112) + '.csv'

	--GET FILE CREATED STATUS FROM STATUS TABLE
	SELECT @CreatedId = Id FROM Exclusive.[Status] WITH(NOLOCK) where [Name] = 'CREATED' AND [Type] = 'FileStatus'

	SET @UpdateDate = GETDATE()

	--CREATE FILE RECORD
	INSERT INTO Exclusive.Files([Name], [Type], [StatusId], CreatedDate, ChangedDate, [Location]) 
	VALUES(@Name, 'Love2Shop', @CreatedId, @UpdateDate, @UpdateDate, @BlobLove2Shop)
				
	--GET THE FILE RECORD ID
	SELECT @FileId = CAST(scope_identity() AS int)

	IF(@Count > 0)
	BEGIN

		BEGIN TRANSACTION
			--GET NEW STATUS ID TO WHICH THE FETCHED RECORDS ARE TO BE UPDATED
			SELECT @CompleteId = Id FROM Exclusive.[Status] WITH(NOLOCK) WHERE [Name] = 'Complete' AND [Type] = 'OfferRedemptionStatus'
				
			--UPDATE THE OFFER REDEMPTION RECORDS IN REQUESTED STATE TO COMPLETE AND UPDATEDDATE TO CURRENT DATE
			UPDATE Exclusive.OfferRedemption SET
			[State] = @CompleteId, FileId = @FileId, UpdatedDate = @UpdateDate 
			WHERE [State] = @RequestedId
		COMMIT

	END
	ELSE
	BEGIN

		BEGIN TRANSACTION
			--GET NO Records STATUS FROM STATUS TABLE
			SELECT @NoRecordsId = Id FROM Exclusive.[Status] WITH(NOLOCK) where [Name] = 'NoRecords' AND [Type] = 'FileStatus'

			--UPDATE FILES TO NO RECORD STATUS
			UPDATE Exclusive.Files SET
			StatusId = @NoRecordsId where Id = @FileId --65	NoRecords of FileStatus type
		COMMIT
	END	
	
	--GET ALL THE RECORDS WITH FILE ID AND UPDATEDDATE IS CURRENT DATE
	select @Name [FileName], O.RedemptionAccountNumber AccountNumber, R.CustomerRef CustomerRef, C.Forename + ' ' + C.Surname [Name],
	replace(CD.Address1,',',' ') Add1, replace(CD.Address2,',',' ') Add2, replace(CD.Address3,',',' ') Add3, 
	replace(CD.Town,',',' ') Add4, replace(CD.PostCode,',',' ') Postcode, '' Country,
	O.RedemptionProductCode ProductCode, 1 Quantity, 0 'Value', 0 'Total', '' ActivationPIN, '' CustomerNotes
	FROM Exclusive.OfferRedemption R
		INNER JOIN Exclusive.MembershipCard MC ON (R.MembershipCardId = MC.Id)
		INNER JOIN Exclusive.Customer C ON (MC.CustomerId = C.Id)
		LEFT JOIN Exclusive.ContactDetail CD ON (C.ContactDetailId = CD.Id)
		INNER JOIN Exclusive.Offer O ON (R.OfferId = O.Id)
	WHERE R.[FileId] = @FileId

END