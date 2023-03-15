
CREATE PROCEDURE [Exclusive].[SP_GetWithdrawalReport]
    @StatusId INT
AS
BEGIN

INSERT [Staging].[WebJobsHistory] (JobName, [Status], Detail) VALUES ('TAM Send Withdrawal Requests', 'OK', 'SP_GetWithdrawalReport' )

	SELECT 'WTHD' TransType, PR.RewardKey UniqueReference, 'TAMICVBAL' FundType, LEFT(C.Title, 20) Title,
	LEFT(C.Forename, 50) Forename, LEFT(C.Surname, 50) Surname, C.NINumber NINumber, S.RequestedAmount Amount, '' IntroducerCode, '' ProcessState,
	s.Id as PartnerRewardWithdrawalId, s.PartnerRewardId, s.ConfirmedAmount, s.FileId, s.BankDetailId, S.RequestedDate RequestedDate
	FROM Exclusive.PartnerRewardWithdrawal S WITH(NOLOCk)
	INNER JOIN Exclusive.PartnerRewards PR WITH(NOLOCK) ON (S.PartnerRewardId = PR.Id)
	INNER JOIN Exclusive.BankDetail BD WITH(NOLOCK) ON (S.BankDetailId = BD.Id AND BD.IsDeleted = 0)
	INNER JOIN Exclusive.CustomerBankDetail CBD WITH(NOLOCK) ON (BD.Id = CBD.BankDetailsId AND CBD.IsActive = 1 AND CBD.IsDeleted = 0)
	INNER JOIN Exclusive.Customer C WITH(NOLOCK) ON (CBD.CustomerId = C.Id)
	Where S.StatusId = @StatusId and s.FileId IS NULL and S.RequestedAmount > 0 and C.NINumber IS NOT NULL
END