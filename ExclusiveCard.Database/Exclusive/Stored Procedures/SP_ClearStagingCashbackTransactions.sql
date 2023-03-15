CREATE Procedure [Exclusive].[SP_ClearStagingCashbackTransactions]	                       
AS
BEGIN
	SET NOCOUNT ON;

	INSERT [Staging].[WebJobsHistory] (JobName, [Status]) VALUES ('SP_ClearStagingCashbackTransactions', 'OK')

	TRUNCATE TABLE [Staging].[CashbackTransaction]
	
END