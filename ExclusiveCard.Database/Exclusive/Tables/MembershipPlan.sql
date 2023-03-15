CREATE TABLE [Exclusive].[MembershipPlan] (
    [Id]                         INT             IDENTITY (1, 1) NOT NULL,
    [PartnerId]                  INT             NULL,
    [MembershipPlanTypeId]       INT             NOT NULL,
    [Duration]                   INT             NOT NULL,
    [NumberOfCards]              INT             NOT NULL,
    [ValidFrom]                  DATETIME2 (7)   NOT NULL,
    [ValidTo]                    DATETIME2 (7)   NOT NULL,
    [CustomerCardPrice]          DECIMAL (18, 2) NOT NULL,
    [PartnerCardPrice]           DECIMAL (18, 2) NOT NULL,
    [CustomerCashbackPercentage] REAL            NOT NULL,
    [DeductionPercentage]        REAL            NOT NULL,
    [IsDeleted]                  BIT             NOT NULL,
    [CurrencyCode]               NVARCHAR (3)    NULL,
    [Description]                NVARCHAR (500)  NULL,
    [IsActive]                   BIT             DEFAULT ((0)) NOT NULL,
    [MembershipLevelId]          INT             NULL,
    [PaidByEmployer]             BIT             DEFAULT ((0)) NOT NULL,
    [MinimumValue]               DECIMAL (18, 2) DEFAULT ((0.0)) NOT NULL,
    [PaymentFee]                 DECIMAL (18, 2) DEFAULT ((0.0)) NOT NULL,
    [CardProviderId]             INT             NULL,
    [BenefactorPercentage]       REAL DEFAULT ((0)) NOT NULL,
    [WhitelabelId]             INT   DEFAULT (1)  NOT NULL,
    [AgentCodeId]              INT   NULL,
    [SiteCategoryId]             INT   NULL,
    CONSTRAINT [PK_MembershipPlan] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MembershipPlan_MembershipLevel_MembershipLevelId] FOREIGN KEY ([MembershipLevelId]) REFERENCES [Exclusive].[MembershipLevel] ([Id]),
    CONSTRAINT [FK_MembershipPlan_MembershipPlanType_MembershipPlanTypeId] FOREIGN KEY ([MembershipPlanTypeId]) REFERENCES [Exclusive].[MembershipPlanType] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MembershipPlan_Partner_CardProviderId] FOREIGN KEY ([CardProviderId]) REFERENCES [Exclusive].[Partner] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MembershipPlan_Partner_PartnerId] FOREIGN KEY ([PartnerId]) REFERENCES [Exclusive].[Partner] ([Id]),
    CONSTRAINT [FK_MembershipPlan_Whitelabel_WhitelabelId] FOREIGN KEY ([WhitelabelId]) REFERENCES [CMS].[WhitelabelSettings] ([Id]),
    CONSTRAINT [FK_MembershipPlan_AgentCode_AgentCodeId] FOREIGN KEY ([AgentCodeId]) REFERENCES [Exclusive].[AgentCode] ([Id]),
    CONSTRAINT [FK_MembershipPlan_SiteCategory_SiteCategoryId] FOREIGN KEY ([SiteCategoryId]) REFERENCES [CMS].[SiteCategory] ([Id]),
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipPlan_MembershipLevelId]
    ON [Exclusive].[MembershipPlan]([MembershipLevelId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipPlan_CardProviderId]
    ON [Exclusive].[MembershipPlan]([CardProviderId] ASC);

