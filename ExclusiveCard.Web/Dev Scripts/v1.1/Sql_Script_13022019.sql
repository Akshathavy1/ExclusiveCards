INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20190213083044_FieldAddition', N'2.1.4-rtm-31024')
GO

ALTER TABLE [Exclusive].[MembershipCard]
ADD RegistrationCode INT NULL
GO

ALTER TABLE [Exclusive].[MembershipCard]  WITH CHECK ADD  CONSTRAINT [FK_MembershipCard_Status_StatusId] FOREIGN KEY([StatusId])
REFERENCES [Exclusive].[Status] ([Id])
GO

ALTER TABLE [Exclusive].[MembershipCard] CHECK CONSTRAINT [FK_MembershipCard_Status_StatusId]
GO


ALTER TABLE [Exclusive].[MembershipCard]  WITH CHECK ADD  CONSTRAINT [FK_MembershipCard_MembershipRegistrationCode_RegistrationCode] FOREIGN KEY([RegistrationCode])
REFERENCES [Exclusive].[MembershipRegistrationCode] ([Id])
GO

ALTER TABLE [Exclusive].[MembershipCard] CHECK CONSTRAINT [FK_MembershipCard_MembershipRegistrationCode_RegistrationCode]
GO

ALTER TABLE [Exclusive].[MembershipPlan]  WITH CHECK ADD  CONSTRAINT [FK_MembershipPlan_MembershipPlanType_MembershipPlanTypeId] FOREIGN KEY([MembershipPlanTypeId])
REFERENCES [Exclusive].[MembershipPlanType] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [Exclusive].[MembershipPlan] CHECK CONSTRAINT [FK_MembershipPlan_MembershipPlanType_MembershipPlanTypeId]
GO
