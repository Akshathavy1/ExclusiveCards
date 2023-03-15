
-- =============================================
-- Author:		GSS (Winston)
-- Create date: 24 Sept 2019
-- Description:	To Check whether any cashback withdrawal request is in progress and get other details
-- =============================================
CREATE PROCEDURE [Exclusive].[SP_GetCashoutRequestData]
	@UserId NVARCHAR(40)
AS
BEGIN
	
	SET NOCOUNT ON;

	--DECLARE @Requested TABLE
	--(
	--	PayId INT,
	--	CustomerId INT
	--)

	--INSERT INTO @Requested
	--	SELECT CP.Id, CP.CustomerId 
	--		FROM Exclusive.CashbackPayout CP WITH(NOLOCK)
	--		WHERE CP.StatusId = (SELECT ID FROM Exclusive.[Status] S WHERE S.[Type] = 'Cashback' AND S.[Name] = 'Requested')

	--DECLARE @Payout TABLE
	--(
	--	CustomerId INT,
	--	Withdrawn DECIMAL
	--)
	--INSERT INTO @Payout
	--	SELECT CP.CustomerId, SUM(CP.Amount) Withdrawn
	--	FROM Exclusive.CashbackPayout CP WITH(NOLOCK)
	--		INNER JOIN Exclusive.Customer C WITH(NOLOCK) ON (CP.CustomerId = C.Id AND C.AspNetUserId = @UserId)
	--		INNER JOIN Exclusive.[Status] S WITH(NOLOCK) ON (CP.StatusId = S.Id AND S.[Name] = 'PaidOut' AND S.[Type] = 'Cashback')
	--	GROUP BY CP.CustomerId

SELECT 
	CASE WHEN R.Requested IS NULL THEN 0 ELSE 1 END RequestExists, N.CustomerId CustomerId,
	N.BankDetailId [BankDetailId], N.PartnerRewardId [PartnerRewardId],
	(ISNULL(AvailableFund,0) - ISNULL(Withdrawn,0)) - ISNULL(RequestedAmount,0) AvailableFund,
	N.[Name] [Name], N.AccountNumber [AccountNumber], n.SortCode [SortCode]
FROM
(
	SELECT TOP(1)
		MC.CustomerId CustomerId, 
		B.Id BankDetailId, 
		MC.PartnerRewardId PartnerRewardId,
		SUM(CS.ReceivedAmount) AvailableFund,
		B.AccountName [Name], 
		B.AccountNumber AccountNumber, 
		B.SortCode SortCode
	FROM Exclusive.CashbackSummary CS WITH(NOLOCK)
		LEFT JOIN Exclusive.MembershipCard MC WITH(NOLOCK) ON (CS.MembershipCardId = MC.Id)
		LEFT JOIN Exclusive.Customer C WITH(NOLOCK) ON (MC.CustomerId = C.Id)
		LEFT JOIN Exclusive.CustomerBankDetail CB WITH(NOLOCK) ON (C.Id = CB.CustomerId)
		LEFT JOIN Exclusive.BankDetail B WITH(NOLOCK) ON (CB.BankDetailsId = B.Id AND B.IsDeleted=0)
	WHERE C.AspNetUserId = @UserId AND AccountType='R'
	GROUP BY MC.CustomerId, B.Id, MC.PartnerRewardId, B.AccountName, B.AccountNumber, B.SortCode
	ORDER BY B.Id DESC
) N
LEFT JOIN 
(
	SELECT CP.CustomerId, SUM(CP.Amount) Withdrawn
	FROM Exclusive.CashbackPayout CP WITH(NOLOCK)
		INNER JOIN Exclusive.Customer C WITH(NOLOCK) ON (CP.CustomerId = C.Id AND C.AspNetUserId = @UserId)
		INNER JOIN Exclusive.[Status] S WITH(NOLOCK) ON (CP.StatusId = S.Id AND S.[Name] = 'PaidOut' AND S.[Type] = 'Cashback')
	GROUP BY CP.CustomerId
) PO
ON (N.CustomerId = PO.CustomerId)
LEFT JOIN 
(
	SELECT CP.CustomerId [CustomerId],SUM(CP.Amount) [RequestedAmount], COUNT(CP.Id) Requested
		FROM Exclusive.CashbackPayout CP WITH(NOLOCK)
		INNER JOIN Exclusive.Customer C WITH(NOLOCK) ON (CP.CustomerId = C.Id AND C.AspNetUserId = @UserId)
		INNER JOIN Exclusive.[Status] S WITH(NOLOCK) ON (CP.StatusId = S.Id AND S.[Name] = 'Requested' AND S.[Type] = 'Cashback')
		GROUP BY CP.CustomerId
) R 
ON (N.CustomerId = R.CustomerId)

END