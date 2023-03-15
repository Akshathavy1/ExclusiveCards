
-- =============================================
-- Author:		GSS (Winston)
-- Create date: 30 Aug 2019
-- Description:	Get data to prepare view model for withdrawal request
-- =============================================
CREATE PROCEDURE [Exclusive].[SP_GetRewardWithdrawalData]
	@MembershipCardId INT
AS
BEGIN
	--Get reward exists in Created/Processing state
	DECLARE @Reward TABLE
	(
		Id INT,
		PartnerRewardId INT,
		Requested DECIMAL
	)

	INSERT INTO @Reward
	SELECT PRW.Id, PRW.PartnerRewardId, SUM(PRW.RequestedAmount) Requested 
	FROM Exclusive.PartnerRewardWithdrawal PRW
	WHERE PRW.StatusId IN (SELECT Id FROM Exclusive.[Status] S WHERE S.IsActive = 1 
	AND S.[Type] = 'WithdrawalStatus' AND (S.[Name] = 'Pending' OR S.[Name] = 'FileCreated' OR S.[Name] = 'Sent'))
	GROUP BY PRW.Id, PRW.PartnerRewardId
	
	SELECT CASE WHEN P.Id IS NULL THEN 0 else 1 END RequestExists, MC.CustomerId CustomerId, 
	B.Id BankDetailId, MC.PartnerRewardId PartnerRewardId, 
	CASE WHEN P.Requested IS NULL THEN PR.LatestValue ELSE PR.LatestValue - P.Requested END AvailableFund, 
	B.AccountName 'Name', B.AccountNumber AccountNumber, B.SortCode SortCode
	FROM Exclusive.MembershipCard MC
	LEFT JOIN Exclusive.PartnerRewards PR ON (MC.PartnerRewardId = PR.Id)
	LEFT JOIN Exclusive.CustomerBankDetail CB ON (MC.CustomerId = CB.CustomerId)
	LEFT JOIN Exclusive.BankDetail B ON (CB.BankDetailsId = B.Id)
	LEFT JOIN @Reward P ON (PR.Id = P.PartnerRewardId)
	WHERE MC.Id = @MembershipCardId
	                    
END