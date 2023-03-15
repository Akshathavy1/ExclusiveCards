
-- =============================================
-- Author: GSS (Winston)
-- Create date: 18 Nov 2019
-- Description: Get all the customer Withdrawals based on userId
-- =============================================
CREATE PROCEDURE[Exclusive].[SP_GetWithdrawalLog]
	@UserId NVARCHAR(36),
	@PageNumber INT,
	@PageSize INT
AS
BEGIN
	
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DECLARE @StartRow INT;
	DECLARE @ENDRow INT;

	SET @StartRow = (@PageNumber - 1) * @PageSize;
	SET @ENDRow = (@PageNumber * @PageSize) + 1;

	With TransactionsView AS (
	Select Count(*) Over() [TotalRecord], ROW_NUMBER() Over(Order By res.[Date]) as RowNumber, res.* 
	from (
	SELECT RequestedDate[Date], 'TAM'[Merchant],
	'£' + CAST(PWR.RequestedAmount AS NVARCHAR)[Value], P.[Name][Status],
	CASE WHEN P.[Name] = 'Paid' OR P.[Name] = 'Completed' THEN 1 ELSE 0 END Invested        
	FROM Exclusive.PartnerRewardWithdrawal PWR
	LEFT JOIN Exclusive.[Status] P ON(PWR.StatusId = P.Id)        
	INNER JOIN Exclusive.BankDetail BD ON (PWR.BankDetailId = BD.Id AND BD.IsDeleted = 0)
	INNER JOIN Exclusive.CustomerBankDetail CBD ON (BD.Id = CBD.BankDetailsId AND CBD.IsActive = 1 AND CBD.IsDeleted = 0)
	INNER JOIN Exclusive.Customer C ON(CBD.CustomerId = C.Id AND C.AspNetUserId = @UserId)) as res )
		SELECT * From TransactionsView Where RowNumber > @StartRow AND RowNumber < @ENDRow
		Order by [Date] DESC
END