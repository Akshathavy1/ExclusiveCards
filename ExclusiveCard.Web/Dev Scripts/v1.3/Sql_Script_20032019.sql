GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190319054748_MembershipPlanButtons', N'2.1.8-servicing-32085')
GO

ALTER TABLE Exclusive.MembershipPlanPaymentProvider
ADD SubscribeAppRef NVARCHAR(50) NULL
GO

ALTER TABLE Exclusive.MembershipPlanPaymentProvider
ADD SubscribeAppAndCardRef NVARCHAR(50) NULL
GO

INSERT INTO Exclusive.MembershipPlan values(null, 1, 30, 10, '2019-03-01', '2019-12-31', 3, 0.00, 100, 0, 0, 'GBP', 'Exclusive Plan 3',1)
GO
INSERT INTO Exclusive.MembershipPlan values(null, 1, 30, 10, '2019-03-01', '2019-12-31', 6, 0.00, 100, 0, 0, 'GBP', 'Exclusive Plan 6',1)
GO

DECLARE @Plan1 INT
DECLARE @Plan2 INT
DECLARE @Plan3 INT
DECLARE @PaymentData INT
DECLARE @PaymentData2 INT
DECLARE @PaymentData3 INT
DECLARE @PayPalProvider INT

SET @PaymentData = 0

--select paypal provider Id
SELECT @PayPalProvider = ID FROM Exclusive.Paymentprovider where Name = 'PayPal' AND IsActive = 1
--get first basic plan
select @Plan1 = Id FROM [Exclusive].[MembershipPlan] where [Description] = 'Personal v2'
select @Plan2 = Id FROM [Exclusive].[MembershipPlan] where [Description] = 'Exclusive Plan 3'
select @Plan3 = Id FROM [Exclusive].[MembershipPlan] where [Description] = 'Exclusive Plan 6'
  
--check if payment data exists
select @PaymentData = COUNT(*) FROM Exclusive.MembershipPlanPaymentProvider WHERE MembershipPlanId = @Plan1 AND PaymentProviderId = @PayPalProvider
select @PaymentData2 = COUNT(*) FROM Exclusive.MembershipPlanPaymentProvider WHERE MembershipPlanId = @Plan2 AND PaymentProviderId = @PayPalProvider
select @PaymentData3 = COUNT(*) FROM Exclusive.MembershipPlanPaymentProvider WHERE MembershipPlanId = @Plan3 AND PaymentProviderId = @PayPalProvider

if(@Plan1 > 0 AND @PaymentData = 0)
BEGIN
INSERT INTO Exclusive.MembershipPlanPaymentProvider(MembershipPlanId, PaymentProviderId, SubscribeAppRef, SubscribeAppAndCardRef) VALUES(@Plan1, @PayPalProvider, 'X6R9QBW8BUTR8', 'U8BLKLK4X3M8C')
END


if(@Plan2 > 0 AND @PaymentData2 = 0)
BEGIN
INSERT INTO Exclusive.MembershipPlanPaymentProvider(MembershipPlanId, PaymentProviderId, SubscribeAppRef, SubscribeAppAndCardRef) VALUES(@Plan2, @PayPalProvider, 'SU88QHNEQPVJY', '8AZJ6MZYSBG8W')
END


if(@Plan3 > 0 AND @PaymentData3 = 0)
BEGIN
INSERT INTO Exclusive.MembershipPlanPaymentProvider(MembershipPlanId, PaymentProviderId, SubscribeAppRef, SubscribeAppAndCardRef) VALUES(@Plan3, @PayPalProvider, 'BKAEUM788ZRQ6', '83DPTZBFKST4J')
END
GO

if((SELECT COUNT(*) FROM Exclusive.Affiliate) = 0)
BEGIN
	INSERT INTO Exclusive.Affiliate VALUES('WebGains')
END
GO


INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190320101121_PlanIdInSubscription', N'2.1.4-rtm-31024')
GO

ALTER TABLE Exclusive.PayPalSubscription
ADD MembershipPlanId INT NULL

GO

/****** Object:  Index [IX_PayPalSubscription_MembershipPlanId]    Script Date: 20-03-2019 16:00:24 ******/
CREATE NONCLUSTERED INDEX [IX_PayPalSubscription_MembershipPlanId] ON [Exclusive].[PayPalSubscription]
(
	[MembershipPlanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [Exclusive].[PayPalSubscription]  WITH CHECK ADD  CONSTRAINT [FK_PayPalSubscription_MembershipPlan_MembershipPlanId] FOREIGN KEY([MembershipPlanId])
REFERENCES [Exclusive].[MembershipPlan] ([Id])
GO

ALTER TABLE [Exclusive].[PayPalSubscription] CHECK CONSTRAINT [FK_PayPalSubscription_MembershipPlan_MembershipPlanId]
GO



--For Testing only
--INSERT INTO Exclusive.MembershipPlanPaymentProvider(MembershipPlanId, PaymentProviderId, SubscribeAppRef, SubscribeAppAndCardRef) VALUES(5, 1, null, null)
--INSERT INTO [ExclusiveCard].[Exclusive].[MembershipRegistrationCode] VALUES(4, 'ExcDisc3', '2019-03-01', '2019-12-31', 10, null, 1, 0)
--INSERT INTO [ExclusiveCard].[Exclusive].[MembershipRegistrationCode] VALUES(5, 'ExcDisc6', '2019-03-01', '2019-12-31', 10, null, 1, 0)