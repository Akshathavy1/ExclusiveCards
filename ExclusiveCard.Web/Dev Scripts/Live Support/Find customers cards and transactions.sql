SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

-- Account and membership card
Select DISTINCT  C.Id, C.Forename, C.Surname, A.UserName, A.id,  MC.Id, MC.CardNumber, MC.StatusId,  MC.ValidTo,  CD.Address1, CD.Address2, CD.Address3, CD.District, CD.PostCode, CD.CountryCode
from Exclusive.Customer C 
left join Exclusive.ContactDetail CD on C.ContactDetailId = CD.Id
left join Exclusive.MembershipCard MC On C.Id = MC.CustomerId
left  join Exclusive.AspNetUsers A on C.AspNetUserId = A.id
left join Exclusive.CashbackSummary CS ON MC.Id  = CS.MembershipCardId
where 
c.id = 1199
or username like '%chris@ex%'
 or mc.id = 1413
--AND MC.StatusId = 11
--AND VALIDTO > GetDAte()


Select * 
from Exclusive.CashbackTransaction CT
Where MembershipCardId = 1413

Select * 
from Exclusive.MembershipCardAffiliateReference
Where CardReference like '%UMafrDNCM0MEdTsurQYFv9HyhkkbPC%' 
--MembershipCardId = 1413

Select * 
from Exclusive.ClickTracking
where DeeplinkURL like '%UMafrDNCM0MEdTsurQYFv9HyhkkbPC%' 
order by DateTime Desc
--and MembershipCardId != 1413
--where MembershipCardId = 1413

Select *
from Staging.CashbackTransaction
Where MembershipCardReference like '%UMafrDNCM0MEdTsurQYFv9HyhkkbPC%' 

