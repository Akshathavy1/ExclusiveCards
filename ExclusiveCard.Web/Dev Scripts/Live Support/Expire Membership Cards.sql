SET Transaction isolation level read uncommitted

--Select * from Exclusive.Status where type = 'MembershipCard'

Select * from Exclusive.MembershipCard
Where statusId = 11 and VAlidTo < GetDate()


BEGIN TRAN

Update Exclusive.MembershipCard
Set Statusid = 10
Where  statusId = 11 and VAlidTo < GetDate()

COMMIT TRAN


Select * from Exclusive.MembershipCard
Where statusId = 11 and VAlidTo < GetDate()
