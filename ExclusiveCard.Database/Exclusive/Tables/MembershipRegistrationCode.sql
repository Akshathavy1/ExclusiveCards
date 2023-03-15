CREATE TABLE [Exclusive].[MembershipRegistrationCode] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [MembershipPlanId] INT            NOT NULL,
    [RegistartionCode] NVARCHAR (30)  NULL,
    [ValidFrom]        DATETIME2 (7)  NOT NULL,
    [ValidTo]          DATETIME2 (7)  NOT NULL,
    [NumberOfCards]    INT            NOT NULL,
    [EmailHostName]    NVARCHAR (512) NULL,
    [IsActive]         BIT            NOT NULL,
    [IsDeleted]        BIT            NOT NULL,
    [RegistrationCodeSummaryId]    INT            NULL,
    CONSTRAINT [PK_MembershipRegistrationCode] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MembershipRegistrationCode_MembershipPlan_MembershipPlanId] FOREIGN KEY ([MembershipPlanId]) REFERENCES [Exclusive].[MembershipPlan] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MembershipRegistrationCode_RegistrationCodeSummary_RegistrationCodeSummaryId] FOREIGN KEY ([RegistrationCodeSummaryId]) REFERENCES [Exclusive].[RegistrationCodeSummary] ([Id]) ON DELETE CASCADE
);
GO
 CREATE NONCLUSTERED INDEX [IX_MembershipRegistrationCode_RegistrationCode]
    ON [Exclusive].[MembershipRegistrationCode]([RegistartionCode] ASC);

GO
