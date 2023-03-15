--/****** Script to add summary records for pre-existing registration codes ******/

IF NOT EXISTS (SELECT 1 FROM [Exclusive].[RegistrationCodeSummary])
BEGIN
BEGIN TRAN

--Populate the RegistrationCodeSummary table with details from MembershipRegistrationCode
--Existing records are all shared codes so create one record per entry
SET IDENTITY_INSERT [Exclusive].[RegistrationCodeSummary] ON

INSERT INTO [Exclusive].[RegistrationCodeSummary]
([Id],[MembershipPlanId],[ValidFrom],[ValidTo],[NumberOfCodes])
SELECT [Id],[MembershipPlanId],[ValidFrom],[ValidTo], 1
FROM [Exclusive].[MembershipRegistrationCode]
WHERE [RegistrationCodeSummaryId] IS NULL

SET IDENTITY_INSERT [Exclusive].[RegistrationCodeSummary] OFF

--Update the MembershipRegistrationCode
UPDATE [Exclusive].[MembershipRegistrationCode]
SET [RegistrationCodeSummaryId] = [Id]
WHERE [RegistrationCodeSummaryId] IS NULL

--Validate
SELECT * FROM [Exclusive].[RegistrationCodeSummary]
SELECT * FROM [Exclusive].[MembershipRegistrationCode]

--COMMIT TRAN
--ROLLBACK TRAN
END
ELSE
PRINT 'Records already exist, check [RegistrationCodeSummary] table'
