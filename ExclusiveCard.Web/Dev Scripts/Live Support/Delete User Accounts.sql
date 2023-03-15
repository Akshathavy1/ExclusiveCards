SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED


-- STEP 1,  Confirm customer and cards to be deleted

-- Account and membership card
Select C.*, CD.*,  A.*, MC.*, CS.*
--  C.Forename, C.Surname, A.UserName, MC.CardNumber, MC.ValidTo,  CD.Address1, CD.Address2, CD.Address3, CD.District, CD.PostCode, CD.CountryCode
from Exclusive.Customer C 
left join Exclusive.ContactDetail CD on C.ContactDetailId = CD.Id
left join Exclusive.MembershipCard MC On C.Id = MC.CustomerId
left  join Exclusive.AspNetUsers A on C.AspNetUserId = A.id
left join Exclusive.CashbackSummary CS ON MC.Id  = CS.MembershipCardId
where 
A.UserName in ('cmoncherry@airseychelles.com', 'fagathine@airseychelles.com')

--MC.CardNumber like '%13642%' OR MC.CardNumber like '%13636%' OR MC.CardNumber like '%13633%'


-- STEP 2,  Set Membership Cards for account deletion here. Run each card in turn.

DECLARE @MembershipCardNumberToDelete NVARCHAR(11) = 'EX0013767GB'

DECLARE @CustId INT
DECLARE @CardId INT
DECLARE @ASPNetId NVARCHAR(450)
DECLARE @ContactID INT 

SELECT @CustId = C.ID, @ASPNetId = c.AspNetUserId, @ContactID = C.ContactDetailId
from Exclusive.Customer C 
left join Exclusive.ContactDetail CD on C.ContactDetailId = CD.Id
left join Exclusive.MembershipCard MC On C.Id = MC.CustomerId
left  join Exclusive.AspNetUsers A on C.AspNetUserId = A.id
where MC.CardNumber =  @MembershipCardNumberToDelete

SELECT @CustId, @ASPNetId, @ContactID

BEGIN TRAN

IF @CustId IS NOT NULL
BEGIN


DELETE Exclusive.CashbackSummary
from Exclusive.Customer C 
left join Exclusive.ContactDetail CD on C.ContactDetailId = CD.Id
left join Exclusive.MembershipCard MC On C.Id = MC.CustomerId
left  join Exclusive.AspNetUsers A on C.AspNetUserId = A.id
left join Exclusive.CashbackSummary CS ON MC.Id  = CS.MembershipCardId
WHERE C.id = @CustId

DELETE Exclusive.MembershipCard
from Exclusive.Customer C 
left join Exclusive.ContactDetail CD on C.ContactDetailId = CD.Id
left join Exclusive.MembershipCard MC On C.Id = MC.CustomerId
left  join Exclusive.AspNetUsers A on C.AspNetUserId = A.id
left join Exclusive.CashbackSummary CS ON MC.Id  = CS.MembershipCardId
WHERE C.id = @CustId

DELETE Exclusive.Customer
WHERE Id = @CustId

DELETE Exclusive.ContactDetail
WHere Id = @ContactID



DELETE Exclusive.AspNetUserRoles
WHERE userid = @ASPNetId

DELETE Exclusive.AspNetUsers
Where Id = @ASPNetId

END
COMMIT TRAN
ROLLBACK TRAN

