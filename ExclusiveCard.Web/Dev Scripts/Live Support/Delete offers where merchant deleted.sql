Select * from Exclusive.Offer O
inner join Exclusive.Merchant M on o.MerchantId = m.Id
where o.StatusId = 1 and m.IsDeleted = 1


BEGIN TRAN

Update Exclusive.Offer 
Set StatusId = 25 -- Deleted
FROM Exclusive.Offer O
inner join Exclusive.Merchant M on o.MerchantId = m.Id
where o.StatusId = 1 and m.IsDeleted = 1

COMMIT TRAN

