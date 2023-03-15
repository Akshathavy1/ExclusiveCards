--TODO: Needs MembershipPlanRegistrationCode and plan name to be changed to appropriate names

IF((SELECT 1 FROM Exclusive.[Status] WHERE [Name] = 'Created' AND [TYPE] = 'FileStatus') IS NULL)
BEGIN
	INSERT INTO Exclusive.[Status] VALUES ('Created', 'FileStatus', 1)
END
GO

IF((SELECT 1 FROM Exclusive.[Status] WHERE [Name] = 'Sent' AND [TYPE] = 'FileStatus') IS NULL)
BEGIN
	INSERT INTO Exclusive.[Status] VALUES ('Sent', 'FileStatus', 1)
END
GO

IF((SELECT 1 FROM Exclusive.[Status] WHERE [Name] = 'Error' AND [TYPE] = 'FileStatus') IS NULL)
BEGIN
	INSERT INTO Exclusive.[Status] VALUES ('Error', 'FileStatus', 1)
END
GO

IF((SELECT 1 FROM Exclusive.[Status] WHERE [Name] = 'TransactionError' AND [TYPE] = 'FileStatus') IS NULL)
BEGIN
	INSERT INTO Exclusive.[Status] VALUES ('TransactionError', 'FileStatus', 1)
END
GO

IF((SELECT 1 FROM Exclusive.[Status] WHERE [Name] = 'Processed' AND [TYPE] = 'FileStatus') IS NULL)
BEGIN
	INSERT INTO Exclusive.[Status] VALUES ('Processed', 'FileStatus', 1)
END
GO

IF((SELECT 1 FROM Exclusive.[Status] WHERE [Name] = 'Paid' AND [TYPE] = 'FilePayment') IS NULL)
BEGIN
	INSERT INTO Exclusive.[Status] VALUES ('Paid', 'FilePayment', 1)
END
GO

IF((SELECT 1 FROM Exclusive.[Status] WHERE [Name] = 'Unpaid' AND [TYPE] = 'FilePayment') IS NULL)
BEGIN
	INSERT INTO Exclusive.[Status] VALUES ('Unpaid', 'FilePayment', 1)
END
GO


--CREATE MEMBERSHIP PLAN TYPE AS Partner Deferred
IF((SELECT 1 FROM Exclusive.MembershipPlanType WHERE [Description] = 'Partner Deferred') IS NULL)
BEGIN
	INSERT INTO Exclusive.MembershipPlanType VALUES('Partner Deferred', 1)
END
GO

--CREATE A PARTNER FOR MEMBERSHIP TYPE Partner Deferred
IF((SELECT 1 FROM Exclusive.[Partner] WHERE [Name] = 'TAM') IS NULL)
BEGIN
	INSERT INTO Exclusive.[Partner] VALUES ('TAM', NULL, NULL, 0)
END
GO

DECLARE @PlanTypeId INT
DECLARE @PartnerId INT

SELECT @PlanTypeId = ID FROM Exclusive.MembershipPlanType WHERE [Description] = 'Partner Deferred'
SELECT @PartnerId = ID FROM Exclusive.[Partner] WHERE [Name] = 'TAM'

IF((SELECT 1 FROM Exclusive.MembershipPlan WHERE MembershipPlanTypeId = @PlanTypeId AND PartnerId = @PartnerId) IS NULL)
BEGIN
	INSERT INTO Exclusive.MembershipPlan VALUES (@PartnerId, @PlanTypeId, 365, 10, GETDATE(), '2021-12-31', 3.0, 2.0, 80, 20, 0, 'GBP', 'Partner Deferred', 1)
END


DECLARE @PlanId INT
SELECT @PlanId = ID FROM Exclusive.MembershipPlan WHERE MembershipPlanTypeId = @PlanTypeId AND PartnerId = @PartnerId

IF((SELECT 1 FROM Exclusive.MembershipRegistrationCode WHERE RegistartionCode = 'ExcTam3') IS NULL)
BEGIN
	INSERT INTO Exclusive.MembershipRegistrationCode VALUES (@PlanId, 'ExcTam3', GETDATE(), '2021-12-31', 10, NULL, 1, 0)
END
GO

GO
CREATE TABLE [Exclusive].[Files] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NULL,
    [PartnerId] int NULL,
    [Type] nvarchar(15) NULL,
    [StatusId] int NOT NULL,
    [PaymentStatusId] int NOT NULL,
    [TotalAmount] decimal(18, 2) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [ChangedDate] datetime2 NULL,
    [PaidDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Files_Partner_PartnerId] FOREIGN KEY ([PartnerId]) REFERENCES [Exclusive].[Partner] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Files_Status_PaymentStatusId] FOREIGN KEY ([PaymentStatusId]) REFERENCES [Exclusive].[Status] ([Id]),
    CONSTRAINT [FK_Files_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [Exclusive].[Status] ([Id])
);

GO

CREATE INDEX [IX_Files_PartnerId] ON [Exclusive].[Files] ([PartnerId]);

GO

CREATE INDEX [IX_Files_PaymentStatusId] ON [Exclusive].[Files] ([PaymentStatusId]);

GO

CREATE INDEX [IX_Files_StatusId] ON [Exclusive].[Files] ([StatusId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190812131439_FilesSchema', N'2.1.11-servicing-32099');

GO

ALTER TABLE [Exclusive].[CashbackTransaction] ADD [FileId] int NULL;

GO

ALTER TABLE [Exclusive].[CashbackSummary] ADD [PaidAmount] decimal(18, 2) NOT NULL DEFAULT 0.0;

GO

CREATE INDEX [IX_CashbackTransaction_FileId] ON [Exclusive].[CashbackTransaction] ([FileId]);

GO

ALTER TABLE [Exclusive].[CashbackTransaction] ADD CONSTRAINT [FK_CashbackTransaction_Files_FileId] FOREIGN KEY ([FileId]) REFERENCES [Exclusive].[Files] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190813072719_NewFieldsAmended', N'2.1.11-servicing-32099');

GO

ALTER TABLE [Exclusive].[Customer] ADD [NINumber] nvarchar(10) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190813100749_NINumberField', N'2.1.11-servicing-32099');

GO

ALTER TABLE [Exclusive].[Partner] ADD [Type] int NOT NULL DEFAULT 1;

GO

ALTER TABLE [Exclusive].[MembershipCard] ADD [PartnerRewardId] int NULL;

GO

CREATE TABLE [Exclusive].[PartnerRewards] (
    [Id] int NOT NULL IDENTITY,
    [RewardKey] nvarchar(20) NULL,
    [PartnerId] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_PartnerRewards] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PartnerRewards_Partner_PartnerId] FOREIGN KEY ([PartnerId]) REFERENCES [Exclusive].[Partner] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_MembershipCard_PartnerRewardId] ON [Exclusive].[MembershipCard] ([PartnerRewardId]);

GO

CREATE INDEX [IX_PartnerRewards_PartnerId] ON [Exclusive].[PartnerRewards] ([PartnerId]);

GO

ALTER TABLE [Exclusive].[MembershipCard] ADD CONSTRAINT [FK_MembershipCard_PartnerRewards_PartnerRewardId] FOREIGN KEY ([PartnerRewardId]) REFERENCES [Exclusive].[PartnerRewards] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190816095808_PartnerRewards', N'2.1.11-servicing-32099');

GO

SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

-- =============================================
-- Author:		Winston (GSS)
-- Create date: 13 Aug 2019
-- Description:	To Get customer data with cashback amount to be pushed to SFTP
-- =============================================
CREATE PROCEDURE Exclusive.SP_GetTransactionReport
	@FileId INT,
	@PartnerId INT
AS
BEGIN
	declare @maxerr int 
	                    
	BEGIN TRANSACTION
		                    
		UPDATE CT SET
		CT.FileId = @FileId
		FROM Exclusive.CashbackTransaction CT WITH(NOLOCK)
		INNER JOIN Exclusive.MembershipCard MC WITH(NOLOCK) ON (CT.MembershipCardId = MC.Id)
		INNER JOIN Exclusive.MembershipPlan MP WITH(NOLOCK) ON (MC.MembershipPlanId = MP.Id)
		INNER JOIN Exclusive.MembershipPlanType MPT WITH(NOLOCK) ON (MP.MembershipPlanTypeId = MPT.Id AND MPT.[Description] = 'Partner Deferred')
		INNER JOIN Exclusive.[Partner] P WITH(NOLOCK) ON (MP.PartnerId = P.Id)
		INNER JOIN Exclusive.[Status] S ON (CT.StatusId = S.Id AND S.IsActive = 1 AND S.[Name] = 'Received' AND S.[Type] = 'Cashback')
		INNER JOIN Exclusive.CashbackSummary CS WITH(NOLOCK) ON (MC.Id = CS.MembershipCardId AND CS.AccountType in ('C', 'R') AND CT.CurrencyCode = CS.CurrencyCode)
		INNER JOIN Exclusive.Customer C WITH(NOLOCK) ON (MC.CustomerId = C.Id)
		WHERE CT.FileId IS NULL AND P.Id = @PartnerId AND CT.AccountType in ('C', 'R') AND (CS.ReceivedAmount - CS.PaidAmount) >= 10 AND C.NINumber IS NOT NULL

	SET @maxerr = @@ERROR

	IF(@maxerr <> 0)
		BEGIN
			ROLLBACK
		END
	ELSE
		BEGIN
			COMMIT
		END

	SELECT 'SUBS' TransType, PR.RewardKey UniqueReference, 'TAMICVBAL' FundType, LEFT(C.Title, 20) Title,
	LEFT(C.Forename, 50) Forename, LEFT(C.Surname, 50) Surname, C.NINumber NINumber, (S.Received - S.PaidAmount) Amount, '' IntroducerCode, '' ProcessState
	FROM Exclusive.Customer C WITH(NOLOCK)
	INNER JOIN Exclusive.MembershipCard MC WITH(NOLOCK) ON (C.Id = MC.CustomerId)
	INNER JOIN Exclusive.PartnerRewards PR WITH(NOLOCK) ON (MC.PartnerRewardId = PR.Id)
	INNER JOIN
	(select MembershipCardId CardId, SUM(ReceivedAmount) Received, SUM(PaidAmount) PaidAmount
	FROM Exclusive.CashbackSummary CS WITH(NOLOCK) WHERE CS.AccountType IN ('C', 'R')
	GROUP BY CS.MembershipCardId) S ON (S.CardId = MC.Id)
	WHERE (S.Received - S.PaidAmount) > 10 AND C.NINumber IS NOT NULL
END

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190819095403_SP_GetTransactionReport', N'2.1.11-servicing-32099');

GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Exclusive].[Files]') AND [c].[name] = N'UpdatedBy');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Exclusive].[Files] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Exclusive].[Files] ALTER COLUMN [UpdatedBy] nvarchar(450) NULL;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190821122144_FileFieldModification', N'2.1.11-servicing-32099');

GO

