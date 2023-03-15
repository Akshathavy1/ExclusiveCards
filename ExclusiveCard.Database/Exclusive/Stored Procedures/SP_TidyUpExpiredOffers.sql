
CREATE PROCEDURE Exclusive.SP_TidyUpExpiredOffers

AS 

BEGIN

INSERT [Staging].[WebJobsHistory] (JobName, [Status]) VALUES ('SP_TidyUpExpiredOffers', 'OK')

-- 1) Mark Offers inactive when they pass ValidTo Date

DECLARE @ActiveStatusId INT
DECLARE @InActiveStatusId INT
SELECT @ActiveStatusId = Id FROM Exclusive.Status Where Type = 'Offer' and Name = 'Active' And IsActive  = 1
SELECT @InActiveStatusId = Id FROM Exclusive.Status Where Type = 'Offer' and Name = 'InActive' AND IsActive  = 1

UPDATE Exclusive.Offer
Set StatusId = @InActiveStatusId
WHERE Statusid = @ActiveStatusId -- Active
And ValidTo < GetDate()
And Validindefinately =0 

-- 2)  Delete offers that are no longer valid from the OfferListItem tables
Delete CMS.OfferListItem
FROM  CMS.OfferListItem  OLI
Inner join Exclusive.Offer O on OLI.OfferId = O.Id
Where O.ValidTo < GetDate()

-- 3)  Delete offers from lists if their merchant has been deleted
DELETE CMS.OfferListItem
from CMS.OfferListItem  OLI
Inner join Exclusive.Offer O on OLI.OfferId = O.Id
inner join Exclusive.Merchant M on O.MerchantId = M.Id
Where M.IsDeleted = 1

-- 4)  Delete offers from merchants that have been deleted
Delete Exclusive.Offer  
from Exclusive.Offer O 
inner join Exclusive.Merchant M on O.MerchantId = M.Id
Where M.IsDeleted = 1



END