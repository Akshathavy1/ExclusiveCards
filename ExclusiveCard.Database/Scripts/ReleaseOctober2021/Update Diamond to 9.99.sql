/****** Script to update diamond membership cost to 9.99 ******/

BEGIN TRAN

UPDATE [Exclusive].[MembershipPlan]
SET [CustomerCardPrice] = 9.99
OUTPUT INSERTED.*
WHERE 
[Id] IN
(
	SELECT MP.[Id]
	  FROM [Exclusive].[MembershipLevel] ML
	  LEFT JOIN 
	  [Exclusive].[MembershipPlan] MP
	  ON MP.[MembershipLevelId]=ML.[Id]
	  WHERE ML.[Description] = 'Diamond'
	  AND MP.[PaidByEmployer] = 0
)

--COMMIT TRAN
--ROLLBACK TRAN
