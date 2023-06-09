/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) MC.ID MembershipCardId, C.ID CustId, C.Forename, C.Surname, CT.* , C.*
  FROM [Exclusive].[CashbackTransaction]  CT
  inner join Exclusive.MembershipCard MC on CT.MembershipCardId = MC.id
  left join Exclusive.Customer  C on MC.CustomerId = C.ID
  Where Summary like 'Account Boost%' and AccountType = 'R'
 -- and FileId = 208
  --and MembershipCardId in (2971, 2974)
  order by CT.Id Desc 

  Select * from Exclusive.Files 
  where type = 'PartnerTrans' -- Love2Shop
  order by id desc

  Select * from Exclusive.Customer C 
  inner join Exclusive.MembershipCard MC on MC.CustomerId = C.Id
  Where Mc.Id in (3045, 3021)

  Select * from Exclusive.Status Where Type = 'Cashback'
  Select * from Exclusive.Status Where Type = 'FilePayment'


