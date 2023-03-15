
-- =============================================
-- Author:		Completely rewritten from the top by IJWAA (Ian)
-- Create date: 13 Aug 2019.  Fixed date 26/01/20
-- Description:	To Get customer data with cashback amount to SEND TO REWARD PARTNER
-- =============================================
CREATE PROCEDURE [Exclusive].[SP_GetTransactionReport]
	@PartnerId INT
AS
BEGIN

		INSERT [Staging].[WebJobsHistory] (JobName, [Status], Detail) VALUES ('TAM Send Transactions', 'OK', 'SP_GetTransactionReport' )

		DECLARE @FileId INT
		
		--BEGIN TRY
		BEGIN TRY
			--BEGIN TRANSACTION
			BEGIN TRANSACTION T1
				Declare @maxerr int
				DECLARE @rowUpdated INT
				DECLARE @Name NVARCHAR(100)
				DECLARE @NoRecordId INT

				--CREATE FILENAME
				SET @Name = 'ER-SUBS-' + CAST(YEAR(GETDATE()) AS NVARCHAR) + '-' + CAST(FORMAT(GETDATE(),'MM') AS NVARCHAR) + '-' + CAST(FORMAT(GETDATE(),'dd') AS NVARCHAR) + '.txt'

				--CREATE FILE
				INSERT INTO Exclusive.Files([Name], [PartnerId], [Type], [StatusId], [PaymentStatusId], CreatedDate, ChangedDate, [Location]) 
				VALUES(@Name, @PartnerId, 'PartnerTrans', 56, 62, GETDATE(), GETDATE(), 'TAM-1')
				--56 is for CREATED OF FILE STATUS
				--62 Unpaid of FILE PAYMENT

				--GET THE FILE RECORD ID
				SELECT @FileId = CAST(scope_identity() AS int)

				--Update Transactions which will be picked for sending to TAM investment
				UPDATE	CT 
				SET		CT.FileId = @FileId
				 FROM Exclusive.CashbackTransaction CT 
				INNER JOIN Exclusive.MembershipCard MC ON CT.MembershipCardId = MC.Id 
				INNER JOIN 
				(	SELECT  MC.CustomerId, Sum(CashbackAmount) TotalCashback
					FROM	Exclusive.CashbackTransaction CT
					inner join Exclusive.MembershipCard MC ON CT.MembershipCardId = MC.Id  	
					Inner Join Exclusive.Customer C ON MC.CustomerId = C.Id			
					WHERE	FileId IS NULL 
							AND AccountType = 'R'
							AND CashbackAmount > 0
							AND ( CT.StatusId = 19 or CT.StatusId = 55)  -- Received or UserPaid of Cashback Type
					Group by MC.CustomerId 
				) T on MC.CustomerId = T.CustomerId
				INNER JOIN Exclusive.MembershipPlan MP ON MC.MembershipPlanId = MP.Id AND MP.PartnerId = 1   ---- THIS IS A REWARD PARTNER
				INNER JOIN Exclusive.Customer C WITH(NOLOCK) ON (MC.CustomerId = C.Id)
				INNER JOIN Exclusive.AspNetUsers U WITH(NOLOCK) ON (C.AspNetUserId = U.Id)
				WHERE	CT.FileId IS NULL 
						AND CT.AccountType = 'R'
						AND CT.CashbackAmount > 0
						AND (CT.StatusId = 19 or CT.StatusId = 55)  -- Received or UserPaid of Cashback Type
						AND T.TotalCashback >= MP.MinimumValue 		
						AND C.NINumber IS NOT NULL 
						AND U.EmailConfirmed = 1

	
				SET @rowUpdated = @@ROWCOUNT
				SET @maxerr = @@ERROR

				-- TODO:  Log an error message somewhere (maybe pass it back as a varchar out PARAM on the stored proc) if an error did occur

		

				-- if no error
				IF(@rowUpdated > 0)
				BEGIN

					--TODO:   To be truely generic, this proc should only return a list of customer details and the balance to send.
					--        The TAM specific fields should be added later [bacl in c# world ] when the file output is generated perhaps.
			
					SELECT  @FileId FileId, 'SUBS' TransType, PR.RewardKey UniqueReference, 'TAMICVBAL' FundType, LEFT(C.Title, 20) Title,
							LEFT(C.Forename, 50) Forename, LEFT(C.Surname, 50) Surname, C.NINumber NINumber, SUM(CT.CashbackAmount) Amount, '' IntroducerCode, '' ProcessState
					FROM	Exclusive.CashbackTransaction CT 
							INNER JOIN Exclusive.MembershipCard MC ON (CT.MembershipCardId = MC.Id)
							INNER JOIN Exclusive.Customer C ON MC.CustomerId = C.Id			
							INNER JOIN Exclusive.PartnerRewards PR WITH(NOLOCK) ON (MC.PartnerRewardId = PR.Id)
					WHERE	CT.FileId = @FileId
					GROUP BY   PR.RewardKey,  LEFT(C.Title, 20),LEFT(C.Forename, 50), LEFT(C.Surname, 50), C.NINumber
						
					COMMIT TRANSACTION T1
				END	
				ELSE
				BEGIN
					--UPDATE FILES TO NORECORD STATUS
					UPDATE Exclusive.Files SET
					StatusId = 65 where Id = @FileId --65	NoRecords of FileStatus type

					COMMIT TRANSACTION T1
				END				
		END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION T1
		END CATCH
END