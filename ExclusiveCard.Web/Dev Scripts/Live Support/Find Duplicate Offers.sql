Select AffiliateId, M.Name, Headline, O.ShortDescription, O.LongDescription
--, ValidFrom, VAlidTo
, count(*) NumberOfOffers
From Exclusive.Offer O
Left Join Exclusive.Merchant M on O.MerchantId = M.Id
WHere AffiliateId Is NOT NULL and o.StatusId = 1 and M.IsDeleted = 0 and  (ValidTo > GetDate() or Validindefinately = 1)
Group by AffiliateId, M.Name, Headline, O.ShortDescription, O.LongDescription
--, ValidFrom, ValidTo
Having count(*) > 1