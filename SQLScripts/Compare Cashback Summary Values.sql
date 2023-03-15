Select S.MembershipCardId, S.AccountType, ISNull(P.PendingAmount, 0) PendingAmount, ISNULL(C.ConfirmedAmount, 0) ConfirmedAmount, ISNULL(R.ReceivedAmount, 0) ReceivedAmount, ISNULL(I.InvestedAmount, 0) InvestedAmount
,S2.PendingAmount, S2.ConfirmedAmount, S2.ReceivedAmount, s2.PaidAmount
From Exclusive.CashbackSummary S

Left join 	(Select MembershipCardId, AccountType, sum(CashbackAmount) PendingAmount
			 From Exclusive.CashbackTransaction 
			 Where statusId = 21
			Group by MembershipCardId, AccountType) P ON S.MembershipCardId = p.MembershipCardId AND S.AccountType = P.AccountType

Left join 	(Select MembershipCardId, AccountType, sum(CashbackAmount) ConfirmedAmount
			 From Exclusive.CashbackTransaction 
			 Where statusId = 20
			Group by MembershipCardId, AccountType) C ON S.MembershipCardId = C.MembershipCardId AND S.AccountType = C.AccountType

Left join 	(Select MembershipCardId, AccountType, sum(CashbackAmount) ReceivedAmount
			 From Exclusive.CashbackTransaction 
			 Where statusId = 19 AND (PaymentStatusId IS NULL or PaymentStatusId != 63)
			Group by MembershipCardId, AccountType) R ON S.MembershipCardId = R.MembershipCardId AND S.AccountType = R.AccountType

Left join 	(Select MembershipCardId, AccountType, sum(CashbackAmount) InvestedAmount
			 From Exclusive.CashbackTransaction 
			 Where statusId = 55  OR (StatusId = 19 and PaymentStatusId = 63)
			Group by MembershipCardId, AccountType) I ON S.MembershipCardId = I.MembershipCardId AND S.AccountType = I.AccountType

INNER JOiN Exclusive.CashbackSummary S2 on S.MembershipCardId = S2.MembershipCardId and S.AccountType = S2.AccountType

WHERE P.PendingAmount != S2.PendingAmount OR R.ReceivedAmount != S2.ReceivedAmount OR C.ConfirmedAmount != S2.ConfirmedAmount OR I.InvestedAmount != S2.PaidAmount
