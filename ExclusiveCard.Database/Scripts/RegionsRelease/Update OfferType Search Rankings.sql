/*
SELECT TOP (1000) [Id]
,[Description]      
,[IsActive]
,[ActionTextLocalisationId]
,[TitleLocalisationId]
,[SearchRanking]
FROM [Exclusive].[OfferType]
where IsActive = 1
order by SearchRanking

BEGIN TRAN
UPDATE  [Exclusive].[OfferType] SET SearchRanking = 1 where Description = 'Local Offer'
UPDATE  [Exclusive].[OfferType] SET SearchRanking = 2 where Description = 'Standard Cashback'
UPDATE  [Exclusive].[OfferType] SET SearchRanking = 3 where Description = 'Cashback'

-- COMMIT TRAN

*/