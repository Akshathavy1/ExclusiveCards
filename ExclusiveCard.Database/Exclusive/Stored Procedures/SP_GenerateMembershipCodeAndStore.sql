
-- =============================================
-- Description: To Generate random membership codes and update MembershipRegistrationCode and RegistrationCodeSummary tables
-- =============================================





CREATE PROCEDURE [Exclusive].[SP_GenerateMembershipCodeAndStore] (
@numberofCodes AS INT,
@numberofUses AS INT,
@membershipPlanId AS INT,
@validFrom AS DATETIME,
@validTo AS DATETIME,
@storagePath AS VARCHAR(MAX)
)
AS
BEGIN
DECLARE @index int
DECLARE @registrationCodeSummaryId INT
SET @index = 0
INSERT INTO [Exclusive].RegistrationCodeSummary(MembershipPlanId,NumberOfCodes,ValidFrom,ValidTo)
VALUES(@membershipPlanId, CASE WHEN @numberofCodes > 1 THEN @numberofCodes ELSE @numberofUses END, @validFrom, @validTo)
SET @registrationCodeSummaryId = SCOPE_IDENTITY();
DECLARE @code VARCHAR(100)
DECLARE @count int
DECLARE @alphabetsToUse VARCHAR(100)
DECLARE @numbersToUse VARCHAR(100)
DECLARE @charactersToUse VARCHAR(100)
DECLARE @stringLength int
DECLARE @minLength int
DECLARE @maxLength int
DECLARE @range int
SET @alphabetsToUse = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
SET @numbersToUse = '0123456789'
SET @charactersToUse = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'
SET @stringLength = 9

WHILE @index < @numberofCodes
BEGIN
SET @count = 0
SET @code = ''

WHILE charindex(@charactersToUse,' ') > 0
BEGIN
SET @charactersToUse = replace(@charactersToUse,' ','')
END

IF @count < 1
BEGIN
SET @code = @code + SUBSTRING(@alphabetsToUse,CAST(ABS(CHECKSUM(NEWID()))*RAND(@count) as int)%LEN(@alphabetsToUse)+1,1)
SET @count = @count + 1
END

IF @count < 2
BEGIN
SET @code = @code + SUBSTRING(@numbersToUse,CAST(ABS(CHECKSUM(NEWID()))*RAND(@count) as int)%LEN(@numbersToUse)+1,1)
SET @count = @count + 1
END

WHILE @count < @stringLength
BEGIN
SET @code = @code + SUBSTRING(@charactersToUse,CAST(ABS(CHECKSUM(NEWID()))*RAND(@count) AS int)%LEN(@charactersToUse)+1,1)
SET @count = @count + 1
END
IF NOT EXISTS (SELECT * FROM [Exclusive].MembershipRegistrationCode regcode
WHERE regcode.RegistartionCode = @code)
BEGIN
INSERT INTO [Exclusive].MembershipRegistrationCode ( MembershipPlanId, NumberOfCards, RegistartionCode, ValidFrom,ValidTo,RegistrationCodeSummaryId,IsActive,IsDeleted)
VALUES (@membershipPlanId, @numberofUses, @code, @validFrom, @validTo, @registrationCodeSummaryId,1,0)

IF @@ROWCOUNT = 1
BEGIN
SET @index =@index +1
END
END
END -- END of WHILE loop for creating one code
IF @numberofCodes > 1
BEGIN
UPDATE [Exclusive].RegistrationCodeSummary
SET StoragePath = @storagePath + CONVERT(VARCHAR(50), @registrationCodeSummaryId ) + '.csv'
WHERE Id = @registrationCodeSummaryId

END



SELECT * FROM [Exclusive].MembershipRegistrationCode
WHERE RegistrationCodeSummaryId = @registrationCodeSummaryId
END