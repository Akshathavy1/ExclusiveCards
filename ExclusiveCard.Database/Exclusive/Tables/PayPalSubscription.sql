CREATE TABLE [Exclusive].[PayPalSubscription] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [CustomerId]        INT             NOT NULL,
    [PayPalId]          NVARCHAR (30)   NULL,
    [PayPalStatusId]    INT             NOT NULL,
    [NextPaymentDate]   DATETIME2 (7)   NULL,
    [NextPaymentAmount] DECIMAL (18, 2) NOT NULL,
    [PaymentType]       NVARCHAR (30)   NULL,
    [MembershipPlanId]  INT             NULL,
    CONSTRAINT [PK_PayPalSubscription] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PayPalSubscription_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Exclusive].[Customer] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PayPalSubscription_MembershipPlan_MembershipPlanId] FOREIGN KEY ([MembershipPlanId]) REFERENCES [Exclusive].[MembershipPlan] ([Id]),
    CONSTRAINT [FK_PayPalSubscription_Status_PayPalStatusId] FOREIGN KEY ([PayPalStatusId]) REFERENCES [Exclusive].[Status] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PayPalSubscription_PayPalStatusId]
    ON [Exclusive].[PayPalSubscription]([PayPalStatusId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PayPalSubscription_CustomerId]
    ON [Exclusive].[PayPalSubscription]([CustomerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PayPalSubscription_MembershipPlanId]
    ON [Exclusive].[PayPalSubscription]([MembershipPlanId] ASC);

