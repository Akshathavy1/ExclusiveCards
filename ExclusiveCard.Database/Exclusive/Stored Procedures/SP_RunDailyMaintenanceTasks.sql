
CREATE Procedure [Exclusive].[SP_RunDailyMaintenanceTasks]
	                       
AS
BEGIN
	SET NOCOUNT ON;

	INSERT [Staging].[WebJobsHistory] (JobName, [Status]) VALUES ('SP_RunDailyMaintenanceTasks', 'OK')

	EXEC [Exclusive].[SP_ClearStagingCashbackTransactions]
	EXEC [Exclusive].[SP_ExpireMembershipCards]
	EXEC [Exclusive].[SP_TidyUpExpiredOffers]
	EXEC [Exclusive].[Maintain_CashbackSummaryValues]
END