CREATE TABLE [Exclusive].[MembershipPlanBenefits] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [MembershipPlanId] INT           NOT NULL,
    [DisplayOrder]     INT           NOT NULL,
    [Description]      NVARCHAR (50) NULL,
    CONSTRAINT [PK_MembershipPlanBenefits] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MembershipPlanBenefits_MembershipPlan_MembershipPlanId] FOREIGN KEY ([MembershipPlanId]) REFERENCES [Exclusive].[MembershipPlan] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipPlanBenefits_MembershipPlanId]
    ON [Exclusive].[MembershipPlanBenefits]([MembershipPlanId] ASC);

