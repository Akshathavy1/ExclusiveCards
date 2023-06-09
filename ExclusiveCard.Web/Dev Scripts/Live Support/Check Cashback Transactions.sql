-- Staging Transactions
  SELECT Distinct MC.Id,  CT.MerchantName, CT.Checked, CT.Name, CT.StatusId,  CT.SourceRevenue
  FROM [Staging].[CashbackTransaction] CT
  left Join [Exclusive].[MembershipCardAffiliateReference] AR on CT.MembershipCardReference = AR.CardReference
  left Join Exclusive.MembershipCard MC ON AR.MembershipCardID = MC.Id
  Where CT.RecordStatusId = 7
  order by MC.Id, CT.Checked 

  -- Cashback TRansactions
  SELECT CT.MembershipCardId, CT.TransactionDate, M.Name Merchant,  CT.Detail, CT.CashbackAmount , CT.StatusId
  From Exclusive.CashbackTransaction CT
  left JOIN Exclusive.Merchant M ON CT.MerchantId = M.Id
  Where CashbackAmount != 0
  order by CT.MembershipCardId, CT.TransactionDate


  -- from Cashback Summmary
  Select * 
  From Exclusive.CashbackSummary CS
  where PendingAmount != 0
  order by CS.MembershipCardId

  -- Totals from Cashback TRansations
  Select MembershipCardId, statusid, sum(CashbackAmount)
  from Exclusive.CashbackTransaction
  Group By MembershipCardId, statusId
  Having sum(CashbackAmount) > 0 
  order by MembershipCardId, statusId

  

  /*
  SELECT *
  FROM [Staging].[CashbackTransaction]
  where TransactionFileId > 5

  */