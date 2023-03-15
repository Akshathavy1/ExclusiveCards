DECLARE @username nvarchar(50) = 'reidac@southport.ac.uk'

-- Account and membership card
Select  C.*, CD.*,  A.*, MC.*, CS.*
from Exclusive.Customer C 
left join Exclusive.ContactDetail CD on C.ContactDetailId = CD.Id
left join Exclusive.MembershipCard MC On C.Id = MC.CustomerId
left  join Exclusive.AspNetUsers A on C.AspNetUserId = A.id
left join Exclusive.CashbackSummary CS ON MC.Id  = CS.MembershipCardId
where 
a.UserName like  @username
--MC.cardNumber like '%13757%'
--c.forename like 'And%'
--c.surname = 'Rothwell'

-- Payments and Paypal
Select CP.* , PS.*
from Exclusive.Customer C 
left  join Exclusive.AspNetUsers A on C.AspNetUserId = A.id
left join Exclusive.CustomerPayment CP on C.Id = CP.CustomerId
left join Exclusive.PayPalSubscription PS on C.id = PS.CustomerId
where a.UserName like   @username


