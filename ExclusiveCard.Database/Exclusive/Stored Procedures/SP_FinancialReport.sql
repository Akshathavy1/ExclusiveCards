Create Procedure [Exclusive].[SP_FinancialReport]
	@PageNumber INT=0,
	@PageSize INT=0,
	@StartDate DateTime2 ,
	@EndDate DateTime2
AS
BEGIN
	SET NOCOUNT ON;
	declare @Percentage decimal(18,2);
	DECLARE @StartRow INT;
	DECLARE @ENDRow INT;

	SET @Percentage = 10;
	SET @StartRow = (@PageNumber - 1) * @PageSize;
	SET @ENDRow = (@PageNumber * @PageSize) + 1;

	With CustomerView AS (
	Select Count(*) Over() [TotalRecord], ROW_NUMBER() Over(Order By res.Description) as RowNumber, res.* 
	from (
		SELECT [Description], SUM([Exclusive Commission]) [ExclusiveCommission]
			,[Beneficiary]
			,SUM([Beneficiary Commission]) [BeneficiaryCommission]
			,SUM([TalkSPORT Commission]) [TalkSPORTCommission],Clicks [ClickCount], CustomerCount [CustomerCount], SUM(CashbackAmount) [CashbackAmount]
		FROM
		(
			SELECT MP.[Description]
				, [Exclusive Commission]
				,CASE WHEN (C.[CharityName] IS NULL AND MP.MembershipPlanTypeId = 3) 
							OR (C.[CharityName] IS NULL AND P.[Name] = 'Exclusive Sport') THEN
				P.[Name]
				ELSE
				C.[CharityName] END
				AS [Beneficiary]
				,[Beneficiary Commission]
				,CASE WHEN [SiteClanId] IS NULL AND P.[Name] <> 'Exclusive Sport' THEN 
					NULL
				ELSE CAST([TalkSPORT Commission] as DECIMAL(10,2)) END
				AS [TalkSPORT Commission],T.MembershipCount AS Clicks, CUS.CustomerCount AS CustomerCount, Amount.CashbackAmount AS CashbackAmount
			FROM
			(
				SELECT MC.[MembershipPlanId] [MembershipPlanId],
						C.[SiteClanId] [SiteClanId],C.Id AS CustomerId,
					 SUM(
						CASE WHEN [AccountType] = 'B' THEN 
							[CashbackAmount]
						ELSE 0 END
						)
					AS [Beneficiary Commission],
					SUM(
						CASE WHEN [AccountType] = 'D' THEN 
							[CashbackAmount]
						ELSE 0 END
						)
					AS [Exclusive Commission],
					SUM(
						CASE WHEN [AccountType] = 'D' THEN 
						@Percentage * ([CashbackAmount] / 100.0)
						ELSE 0 END
						)
					AS [TalkSPORT Commission]
				  FROM [Exclusive].[CashbackTransaction] CT
				  LEFT JOIN [Exclusive].[MembershipCard] MC
				  ON CT.[MembershipCardId] = MC.Id
				  LEFT JOIN [Exclusive].[Customer] C
				  ON MC.CustomerId = C.Id
				  WHERE (CT.[AccountType] = 'B' OR CT.[AccountType] = 'D') AND CT.DateReceived >= @StartDate AND CT.DateReceived	<=@EndDate
				  GROUP BY MC.[MembershipPlanId], C.[SiteClanId],c.Id
			)as [Plan Summary] 
		  LEFT JOIN [Exclusive].[MembershipPlan] MP ON [Plan Summary].[MembershipPlanId] = MP.Id
 
		  LEFT JOIN (SELECT  MP1.Description,COUNT(CT.Id) AS MembershipCount FROM [Exclusive].[MembershipCard] MC
					  JOIN [Exclusive].[MembershipPlan] MP1 ON MC.MembershipPlanId=MP1.Id
					  JOIN [Exclusive].[ClickTracking] CT ON CT.MembershipCardId=MC.Id
					  group by MP1.Description) T
		  ON MP.[Description]=T.Description


		   LEFT JOIN (SELECT  MP1.Description,COUNT(CT.Id) AS CustomerCount FROM [Exclusive].[MembershipCard] MC
					  JOIN [Exclusive].[MembershipPlan] MP1 ON MC.MembershipPlanId=MP1.Id
					  JOIN [Exclusive].[Customer] CT ON CT.Id=MC.CustomerId
					  group by MP1.Description) Cus
		   ON MP.[Description]=Cus.Description 

		   LEFT JOIN (SELECT  MP1.Description,SUM(CBT.CashbackAmount) AS CashbackAmount FROM [Exclusive].[CashbackTransaction] CBT
			 JOIN [Exclusive].[MembershipCard] MC ON CBT.MembershipCardId =MC.Id
		     JOIN [Exclusive].[MembershipPlan] MP1 ON MC.MembershipPlanId=MP1.Id
			 WHERE CBT.AccountType='R' AND CBT.StatusId=20 AND CBT.CashbackAmount IS NOT NULL AND MP1.MembershipPlanTypeId = 3
			 group by MP1.Description) Amount			  
			ON MP.[Description]=[Amount].[Description]


			LEFT JOIN [CMS].[SiteClan] SC
			ON [Plan Summary].SiteClanId = SC.Id
			LEFT JOIN [CMS].[Charity] C
			ON SC.CharityId = C.Id
			LEFT JOIN [Exclusive].[Partner] P
			ON MP.CardProviderId = P.Id
			WHERE (C.[CharityName] IS NULL AND MP.MembershipPlanTypeId = 3)
			  OR C.[CharityName] IS NOT NULL OR P.[Name] = 'Exclusive Sport' 
			 ) Results
			
			GROUP BY [Description],[Beneficiary],Clicks,CustomerCount, CashbackAmount
		 ) as res )
		SELECT * From CustomerView Where RowNumber > @StartRow AND RowNumber < @ENDRow
		Order by RowNumber
END