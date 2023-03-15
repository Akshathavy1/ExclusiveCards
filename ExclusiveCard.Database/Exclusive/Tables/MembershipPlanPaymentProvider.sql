CREATE TABLE [Exclusive].[MembershipPlanPaymentProvider] (
    [MembershipPlanId]       INT           NOT NULL,
    [PaymentProviderId]      INT           NOT NULL,
    [SubscribeAppRef]        NVARCHAR (50) NULL,
    [SubscribeAppAndCardRef] NVARCHAR (50) NULL,
    [OneOffPaymentRef]       NVARCHAR (50) NULL,
    CONSTRAINT [PK_MembershipPlanPaymentProvider] PRIMARY KEY CLUSTERED ([MembershipPlanId] ASC, [PaymentProviderId] ASC),
    CONSTRAINT [FK_MembershipPlanPaymentProvider_MembershipPlan_MembershipPlanId] FOREIGN KEY ([MembershipPlanId]) REFERENCES [Exclusive].[MembershipPlan] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MembershipPlanPaymentProvider_Paymentprovider_PaymentProviderId] FOREIGN KEY ([PaymentProviderId]) REFERENCES [Exclusive].[Paymentprovider] ([Id]) ON DELETE CASCADE
);

