

SELECT  MAR.*, C.Forename, c.Surname, MC.*
  FROM [Exclusive].[MembershipCardAffiliateReference] MAR
inner join Exclusive.MembershipCard MC on MAR.MembershipCardId = MC.Id
Inner join Exclusive.Customer C on MC.CustomerId = C.Id
where MembershipCardId = 1416


Select * from Exclusive.CashbackTransaction -- Where accountType = 'R'
Select * from Exclusive.CashbackSummary --  Where AccountType = 'R'

Select * from Exclusive.MembershipPlan