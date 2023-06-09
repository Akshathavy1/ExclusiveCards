/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [Id]
      ,[TransactionFileId]
      ,[ResultsId]
      ,[SourceId]
      ,[NetworkId]
      ,[NetworkName]
      ,[ConnectionId]
      ,[MerchantId]
      ,[MerchantName]
      ,[OrderId]
      ,[Country]
      ,[Clicked]
      ,[Sold]
      ,[Checked]
      ,[Referrer]
      ,[BasketId]
      ,[BaskedSourceId]
      ,[Name]
      ,[Currency]
      ,[PriceTotal]
      ,[Revenue]
      ,[SourcePriceTotal]
      ,[SourceRevenue]
      ,[StatusId]
      ,[MembershipCardReference]
      ,[SourceCurrency]
      ,[RecordStatusId]
  FROM [Staging].[CashbackTransaction]
  where MembershipCardReference is null and transactionfileid = 134