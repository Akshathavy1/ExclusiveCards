CREATE TABLE [Exclusive].[MembershipCard] (
    [Id]                        INT            IDENTITY (1, 1) NOT NULL,
    [CustomerId]                INT            NULL,
    [MembershipPlanId]          INT            NOT NULL,
    [CardNumber]                NVARCHAR (50)  NULL,
    [ValidFrom]                 DATETIME2 (7)  NOT NULL,
    [ValidTo]                   DATETIME2 (7)  NOT NULL,
    [DateIssued]                DATETIME2 (7)  NULL,
    [StatusId]                  INT            NOT NULL,
    [IsActive]                  BIT            NOT NULL,
    [IsDeleted]                 BIT            NOT NULL,
    [PhysicalCardRequested]     BIT            DEFAULT ((0)) NOT NULL,
    [CustomerPaymentProviderId] NVARCHAR (MAX) NULL,
    [PhysicalCardStatusId]      INT            NULL,
    [RegistrationCode]          INT            NULL,
    [PartnerRewardId]           INT            NULL,
    [TermsConditionsId]         INT            NULL,
    CONSTRAINT [PK_MembershipCard] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MembershipCard_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Exclusive].[Customer] ([Id]),
    CONSTRAINT [FK_MembershipCard_MembershipPlan_MembershipPlanId] FOREIGN KEY ([MembershipPlanId]) REFERENCES [Exclusive].[MembershipPlan] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MembershipCard_MembershipRegistrationCode_RegistrationCode] FOREIGN KEY ([RegistrationCode]) REFERENCES [Exclusive].[MembershipRegistrationCode] ([Id]),
    CONSTRAINT [FK_MembershipCard_PartnerRewards_PartnerRewardId] FOREIGN KEY ([PartnerRewardId]) REFERENCES [Exclusive].[PartnerRewards] ([Id]),
    CONSTRAINT [FK_MembershipCard_Status_PhysicalCardStatusId] FOREIGN KEY ([PhysicalCardStatusId]) REFERENCES [Exclusive].[Status] ([Id]),
    CONSTRAINT [FK_MembershipCard_Status_StatusId] FOREIGN KEY ([StatusId]) REFERENCES [Exclusive].[Status] ([Id]),
    CONSTRAINT [FK_MembershipCard_TermsConditions_TermsConditionsId] FOREIGN KEY ([TermsConditionsId]) REFERENCES [Exclusive].[TermsConditions] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipCard_TermsConditionsId]
    ON [Exclusive].[MembershipCard]([TermsConditionsId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipCard_PartnerRewardId]
    ON [Exclusive].[MembershipCard]([PartnerRewardId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipCard_PhysicalCardStatusId]
    ON [Exclusive].[MembershipCard]([PhysicalCardStatusId] ASC);

