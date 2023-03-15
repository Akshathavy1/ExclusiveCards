SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED


-- Account and membership card
Select DISTINCT  C.Forename, C.Surname, A.UserName, MC.CardNumber, MC.ValidTo,  CD.Address1, CD.Address2, CD.Address3, CD.District, CD.PostCode, CD.CountryCode
from Exclusive.Customer C 
left join Exclusive.ContactDetail CD on C.ContactDetailId = CD.Id
left join Exclusive.MembershipCard MC On C.Id = MC.CustomerId
left  join Exclusive.AspNetUsers A on C.AspNetUserId = A.id
left join Exclusive.CashbackSummary CS ON MC.Id  = CS.MembershipCardId
where A.UserName in (
 'ste_vo1000@hotmail.com',
 'KIRSTYK802@HOTMAIL.CO.UK',
 'CLAUDIACHADFORD@GMAIL.COM'
)
AND MC.StatusId = 11
AND VALIDTO > GetDAte()



