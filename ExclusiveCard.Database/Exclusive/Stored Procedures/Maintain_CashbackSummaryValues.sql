CREATE PROCEDURE [Exclusive].[Maintain_CashbackSummaryValues]
AS

/* 
	QUERY TO  FIX DISCREPENCIES BETWEEN SUMMARY AND ACTUAL CASHBACK TRANSACTION VALUES 

	Updates
			2021 06 07	AL	Fixed Cashback withdrawal calculation

*/

INSERT [Staging].[WebJobsHistory] (JobName, [Status]) VALUES ('Maintain_CashbackSummaryValues', 'OK')

UPDATE Exclusive.CashbackSummary   
	SET PendingAmount = ISNull(P.PendingAmount, 0), 
		ConfirmedAmount = ISNULL(C.ConfirmedAmount, 0), 
		[ReceivedAmount] = (
			case when S.AccountType='R' THEN
				ISNULL(R.ReceivedAmount, 0) --- ISNULL(O.Deductions, 0) 
			when S.AccountType='B' THEN
				ISNULL(S.ReceivedAmount, 0) 
			when S.AccountType='D' THEN
				ISNULL(S.ReceivedAmount, 0) 
			ELSE 0 END), 
		PaidAmount = ISNULL(I.InvestedAmount, 0)
FROM 
Exclusive.CashbackSummary S --Summary
Left join	(
				SELECT MembershipCardId, AccountType, sum(CashbackAmount) PendingAmount
				FROM Exclusive.CashbackTransaction 
				WHERE StatusId = 21
				Group by MembershipCardId, AccountType
			) P --Pending
			ON S.MembershipCardId = p.MembershipCardId AND S.AccountType = P.AccountType

Left join 	(
				Select MembershipCardId, AccountType, sum(CashbackAmount) ConfirmedAmount
				From Exclusive.CashbackTransaction 
				Where StatusId = 20
				Group by MembershipCardId, AccountType
			) C --Confirmed
			ON S.MembershipCardId = C.MembershipCardId AND S.AccountType = C.AccountType

Left join 	(
				SELECT MembershipCardId, AccountType, SUM(CashbackAmount) ReceivedAmount
				From Exclusive.CashbackTransaction 
				Where StatusId = 19 AND (PaymentStatusId IS NULL or PaymentStatusId != 63)
				Group by MembershipCardId, AccountType
			) R --Received
			ON S.MembershipCardId = R.MembershipCardId AND S.AccountType = R.AccountType

--Left join   (
--				SELECT MC.Id MembershipCardId, 'R' AccountType, CONVERT(decimal(5,2), ISNULL(CP.[Amount],0)) Deductions
--				FROM
--				(
--					SELECT MAX(MC.Id) as id, MC.CustomerId
--					FROM [Exclusive].[MembershipCard] MC 
--					WHERE 
--						MC.IsActive = 1
--						AND MC.IsDeleted =0
--					GROUP BY MC.CustomerId
--				) MC
--				LEFT JOIN 
--				(
--					SELECT SUM([Amount]) [Amount], CustomerId
--					FROM
--					[Exclusive].[CashbackPayout] CP
--					WHERE 
--					CP.[StatusId] = 17 
--					GROUP BY CustomerId
--				) CP
--				ON MC.Customerid = CP.Customerid
--			) O -- out (paid out to customer)
--			ON S.MembershipCardId = O.MembershipCardId AND S.AccountType = O.AccountType 

Left join   (
				Select MembershipCardId, AccountType, sum(CashbackAmount) InvestedAmount
				From Exclusive.CashbackTransaction 
				Where StatusId = 55  OR (StatusId = 19 and PaymentStatusId = 63)  -- Add received txns which have been sucessfully processed from TAM into the invested total
				Group by MembershipCardId, AccountType
			) I -- Invested (With TAM)
			ON S.MembershipCardId = I.MembershipCardId AND S.AccountType = I.AccountType

RETURN 0
