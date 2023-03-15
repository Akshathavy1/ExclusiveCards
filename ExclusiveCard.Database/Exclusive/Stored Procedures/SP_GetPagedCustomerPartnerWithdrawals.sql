
-- =============================================
-- Author:		GSS (Winston)
-- Create date: 13 Sept 2019
-- Description:	Get Paged Customer Partner Reward Withdrawals
-- =============================================
CREATE PROCEDURE [Exclusive].[SP_GetPagedCustomerPartnerWithdrawals]
	--@PartnerId INT,
	@FromDate DATETIME,
	@ToDate DATETIME,
	@PageNumber INT,
	@PageSize INT
AS
BEGIN

	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	DECLARE @StartRow INT;
	DECLARE @ENDRow INT;

	SET @StartRow = (@PageNumber - 1) * @PageSize;
	SET @ENDRow = (@PageNumber * @PageSize) + 1;

	With RewardView AS (
	Select Count(*) Over() [TotalRecord], ROW_NUMBER() Over(Order By res.ContactName) as RowNumber, res.* 
	from (
	SELECT 'Customer' + CAST(C.Id AS NVARCHAR) 'ContactName', U.UserName 'EmailAddress', 
	'TAM' + CAST(PRW.Id AS NVARCHAR) 'InvoiceNumber', PRW.WithdrawnDate 'InvoiceDate', 
	DATEADD(day, 5, PRW.WithdrawnDate) 'DueDate', PRW.ConfirmedAmount 'Total', 
	BD.SortCode + ' | ' + BD.AccountNumber 'Description', 1 'Quantity',
	PRW.ConfirmedAmount 'UnitAmount', 851 'AccountCode', 0 'TaxType',
	0 'TaxAmount', 'GBP' 'Currency'
	FROM [Exclusive].PartnerRewardWithdrawal PRW
	INNER JOIN Exclusive.BankDetail BD ON (PRW.BankDetailId = BD.Id AND BD.IsDeleted = 0)
	INNER JOIN Exclusive.CustomerBankDetail CBD ON (BD.Id = CBD.BankDetailsId AND CBD.IsDeleted = 0 AND CBD.IsActive = 1)
	INNER JOIN Exclusive.Customer C ON (CBD.CustomerId = C.Id)
	INNER JOIN Exclusive.AspNetUsers U ON (C.AspNetUserId = U.Id) 
	INNER JOIN [Exclusive].[Status] S ON (PRW.StatusId = S.Id)
	WHERE S.[Name] = 'Paid' AND S.[Type] = 'WithdrawalStatus' AND PRW.PartnerRewardId IS NULL AND CONVERT(NVARCHAR(8), PRW.WithdrawnDate, 11) >= CONVERT(NVARCHAR(8), @FromDate, 11)
	AND CONVERT(NVARCHAR(8), PRW.WithdrawnDate, 11) <= CONVERT(NVARCHAR(8), @ToDate, 11)) as res )
		SELECT * From RewardView Where RowNumber > @StartRow AND RowNumber < @ENDRow
		Order by InvoiceDate ASC
END