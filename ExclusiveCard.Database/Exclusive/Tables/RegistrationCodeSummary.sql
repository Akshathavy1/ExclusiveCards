CREATE TABLE [Exclusive].[RegistrationCodeSummary]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [MembershipPlanId] INT            NOT NULL,
    [ValidFrom]        DATETIME2 (7)  NOT NULL,
    [ValidTo]          DATETIME2 (7)  NOT NULL,
    [NumberOfCodes]    INT            NOT NULL,
    [StoragePath]    NVARCHAR (512) NULL
)
