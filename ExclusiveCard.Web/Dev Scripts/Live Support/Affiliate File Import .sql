SEt transaction isolation level read uncommitted

Select * from Staging.OfferImportFile

Select count(*) From Staging.OfferImportAwin
Select count(*) from Staging.Offer 
Select count(*) from Exclusive.Offer  -- 5843

--EXEC Exclusive.SP_TriggerStagingToExclusive 200

--Select count(*) from Staging.Offer 
--Select count(*) from Exclusive.Offer  -- 5843
