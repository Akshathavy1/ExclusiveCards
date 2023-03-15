CREATE TABLE [Exclusive].[PartnerRewardWithdrawal] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [PartnerRewardId] INT             NOT NULL,
    [StatusId]        INT             NOT NULL,
    [RequestedAmount] DECIMAL (18, 2) NOT NULL,
    [ConfirmedAmount] DECIMAL (18, 2) NULL,
    [BankDetailId]    INT             DEFAULT ((0)) NOT NULL,
    [FileId]          INT             NULL,
    [RequestedDate]   DATETIME2 (7)   DEFAULT ('0001-01-01T00:00:00.0000000Z') NOT NULL,
    [WithdrawnDate]   DATETIME2 (7)   NULL,
    [ChangedDate]     DATETIME2 (7)   DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [UpdatedBy]       NVARCHAR (450)  NULL,
    CONSTRAINT [PK_PartnerRewardWithdrawal] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PartnerRewardWithdrawal_BankDetail_BankDetailId] FOREIGN KEY ([BankDetailId]) REFERENCES [Exclusive].[BankDetail] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PartnerRewardWithdrawal_Files_FileId] FOREIGN KEY ([FileId]) REFERENCES [Exclusive].[Files] ([Id]),
    CONSTRAINT [FK_PartnerRewardWithdrawal_PartnerRewards_PartnerRewardId] FOREIGN KEY ([PartnerRewardId]) REFERENCES [Exclusive].[PartnerRewards] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PartnerRewardWithdrawal_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [Exclusive].[Status] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PartnerRewardWithdrawal_FileId]
    ON [Exclusive].[PartnerRewardWithdrawal]([FileId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PartnerRewardWithdrawal_PartnerRewardId]
    ON [Exclusive].[PartnerRewardWithdrawal]([PartnerRewardId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PartnerRewardWithdrawal_StatusId]
    ON [Exclusive].[PartnerRewardWithdrawal]([StatusId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PartnerRewardWithdrawal_BankDetailId]
    ON [Exclusive].[PartnerRewardWithdrawal]([BankDetailId] ASC);

