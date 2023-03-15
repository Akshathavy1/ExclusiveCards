CREATE TABLE [Exclusive].[MembershipPlanType] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [Description]       NVARCHAR (100) NULL,
    [IsActive]          BIT            NOT NULL,
    [TermsConditionsId] INT            NULL,
    CONSTRAINT [PK_MembershipPlanType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MembershipPlanType_TermsConditions_TermsConditionsId] FOREIGN KEY ([TermsConditionsId]) REFERENCES [Exclusive].[TermsConditions] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MembershipPlanType_TermsConditionsId]
    ON [Exclusive].[MembershipPlanType]([TermsConditionsId] ASC);

