SElect * from Exclusive.AspNetUsers where email like 'test23%'
Select * from Exclusive.AspNetUserRoles where UserId = 'c7f21842-0a90-4668-a6bc-36108e5f075a'
select * from Exclusive.customer
Select * from Exclusive.ContactDetail where id= 7812
Select * from Exclusive.MembershipCard where CustomerId = 2603
select * from Exclusive.CashbackTransaction where MembershipCardId in (2952, 2953)
select * from Exclusive.CashbackSummary where MembershipCardId in (2952, 2953)
select * from Exclusive.CashbackPAyout where CustomerId = 2603
select * from Exclusive.MembershipCardAffiliateReference where MembershipCardId in (2952, 2953)
SELECT * from Exclusive.PartnerRewards where id = 3
SELECT * from Exclusive.PartnerRewardWithdrawal where PartnerRewardId = 3
Select * from Exclusive.PayPalSubscription Where CustomerId = 2603
Select * from EXclusive.CustomerBankDetail where CustomerId = 2603
select * from Exclusive.CustomerPayment where  CustomerId = 2603
Select * from Exclusive.CustomerSecurityQuestion Where CustomerId = 2603
select * from Exclusive.ContactDetail CD INNER JOIN Exclusive.Customer C on C.ContactDetailId = CD.ID Where C.Id = 2603


BEGIN TRAN
DELETE Exclusive.ClickTracking 
DELETE Exclusive.CashbackTransaction 
DELETE Exclusive.CashbackSummary 
DELETE Exclusive.CashbackPAyout 
DELETE Exclusive.MembershipCardAffiliateReference 
DELETE Exclusive.PartnerRewardWithdrawal 
DELETE Exclusive.PayPalSubscription 
DELETE EXclusive.CustomerBankDetail
DELETE Exclusive.CustomerPayment 
DELETE Exclusive.CustomerSecurityQuestion 
DELETE Exclusive.MembershipCard 
DELETE Exclusive.customer 
DELETE Exclusive.PartnerRewards
DELETE Exclusive.ContactDetail 
DELETE Exclusive.[LoginUserToken ]
DELETE Exclusive.AspNetUsers 
DELETE Exclusive.AspNetUserRoles 

--COMMIT TRAN
--ROLLBACK TRAN


--BEGIN TRAN

--DELETE Exclusive.Offer where OfferTypeId not in (1,5,7)
--select * from Exclusive.Offer
--COMMIT TRAN

Delete Exclusive.Files
DELETE [Exclusive].[MembershipPendingToken]
DELETE Exclusive.PaymentNotification

DELETE [Staging].[CashbackTransactionErrors]
DELETE Staging.CashbackTransaction
DELETE Staging.CustomerRegistration
DELETE Staging.OfferImportFile
DELETE Staging.TransactionFile