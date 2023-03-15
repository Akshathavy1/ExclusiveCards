
CREATE Procedure [Exclusive].[SP_ExpireMembershipCards]
	                       
AS
BEGIN
	SET NOCOUNT ON;

	INSERT [Staging].[WebJobsHistory] (JobName, [Status]) VALUES ('SP_ExpireMembershipCards', 'OK')

	DECLARE @Status INT
	SELECT @Status = Id From [Exclusive].[Status] s where s.Type = 'MembershipCard' AND s.Name = 'Expired' AND s.IsActive = 1

	IF(@Status IS NOT NULL)
	BEGIN
		UPDATE MC SET 
		StatusId = @Status
		FROM [Exclusive].[MembershipCard] MC
		WHERE ValidTo < GETUTCDATE()
	END
END