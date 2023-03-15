/****** Script to update diamond membership cost to 6.99 ******/

BEGIN TRAN

UPDATE [Exclusive].[MembershipPlan]
SET [CustomerCardPrice] = 6.99
OUTPUT INSERTED.*
WHERE 
[Id] IN
(
	SELECT MP.[Id]
	  FROM [Exclusive].[MembershipLevel] ML
	  LEFT JOIN 
	  [Exclusive].[MembershipPlan] MP
	  ON MP.[MembershipLevelId]=ML.[Id]
	  LEFT JOIN [Exclusive].[Partner] P
	  ON MP.[CardProviderId] = P.Id
	  WHERE ML.[Description] = 'Diamond'
	  AND MP.[CustomerCardPrice] = 14.99
	  AND MP.[PaidByEmployer] = 0
	  AND P.[Name] = 'Exclusive Media'
)

--COMMIT TRAN
--ROLLBACK TRAN
