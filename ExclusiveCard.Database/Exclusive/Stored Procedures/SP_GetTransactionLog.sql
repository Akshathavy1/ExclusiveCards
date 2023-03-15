
-- =============================================
-- Author:		GSS (Winston)
-- Create date: 05 Sept 2019
-- Description:	Get all the cashback transactions and customer Withdrawals based on userId
-- =============================================
CREATE PROCEDURE [Exclusive].[SP_GetTransactionLog]
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
    SELECT Count(*) Over() [TotalRecord], ROW_NUMBER() Over(Order By res.[Date]) as RowNumber, res.* 
    FROM 
    (
        SELECT CT.TransactionDate [Date], M.[Name] Merchant, ISNull(CT.CashbackAmount,0) [Value], 
            ISNull(CT.PurchaseAmount,0) [PurchaseAmount], CT.[Summary], ISNull(B.CashbackAmount,0) [Donated], S.[Name] [Status], 
            CASE WHEN P.[Name] = 'Paid' OR P.[Name] = 'Completed' THEN 1 ELSE 0 END [Invested]
        FROM Exclusive.CashbackTransaction CT
        INNER JOIN Exclusive.[Status] S ON (CT.StatusId = S.Id)
        LEFT JOIN Exclusive.[Status] P ON (CT.PaymentStatusId = P.Id)
        LEFT JOIN Exclusive.Merchant M ON (CT.MerchantId = M.Id)
        INNER JOIN Exclusive.MembershipCard MC ON (CT.MembershipCardId = MC.Id)
        INNER JOIN Exclusive.Customer C ON (MC.CustomerId = C.Id AND C.AspNetUserId = @UserId)
        LEFT JOIN 
        (
            SELECT MembershipCardId, PurchaseAmount, TransactionDate, CurrencyCode, DateReceived, CashbackAmount
            FROM Exclusive.CashbackTransaction where AccountType='B'
        ) B
        ON 
            B.TransactionDate= CT.TransactionDate AND 
            B.CurrencyCode=CT.CurrencyCode AND 
            --B.DateReceived=CT.DateReceived AND 
            B.PurchaseAmount = CT.PurchaseAmount AND
            B.MembershipCardId = CT.MembershipCardId
        WHERE CT.AccountType in ('C', 'R')
		UNION
		SELECT 
		(	CASE 
			WHEN CP.[StatusId] =17 THEN
				CP.[PayoutDate]
			ELSE
				CP.[RequestedDate]
			END
			) [Date]
		,'Cashback Withdrawal' [Merchant]
		,ISNull(CP.Amount,0) [Value]
		, 0 [PurchaseAmount]
		,'Cashback Withdrawal' [Summary]
		, 0 [Donated]
		, S.[Name] [Status]
		, 0 [Invested]
		FROM [Exclusive].[CashbackPayout] CP
		INNER JOIN Exclusive.[Status] S ON (CP.StatusId = S.Id)
		INNER JOIN Exclusive.Customer C ON (CP.CustomerId = C.Id AND C.AspNetUserId = @UserId)

	)as res 
    ) 
    
    SELECT * From TransactionsView Where RowNumber > @StartRow AND RowNumber < @ENDRow
    Order by [Date] DESC

 

END