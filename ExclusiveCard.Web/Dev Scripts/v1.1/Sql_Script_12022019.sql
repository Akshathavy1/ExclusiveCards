  UPDATE [CMS].[Adverts] SET
  ImagePath = '/Adverts/Homepage/SC/SCBanner2.png' WHERE Id = 6

  UPDATE [CMS].[Adverts] SET
  ImagePath = '/Adverts/Homepage/SC/SCBanner3.png' WHERE Id = 7

  UPDATE [CMS].[Adverts] SET
  ImagePath = '/Adverts/Merchant_Page/SC/SCBanner4.png' WHERE Id = 12

  UPDATE [CMS].[Adverts] SET
  ImagePath = '/Adverts/Merchant_Page/SC/SCBanner5.png' WHERE Id = 13

  UPDATE [CMS].[Adverts] SET
  ImagePath = '/Adverts/Merchant_Page/SC/SCBanner6.png' WHERE Id = 14

--Add Role to zoe@exclusivecard.co.uk
Declare @aspUserId NVARCHAR(40)
DECLARE @userRoleId NVARCHAR(40)

SELECT @aspUserId = Id FROM Exclusive.AspNetUsers WHERE UserName = 'zoe@exclusivecard.co.uk'
SELECT @userRoleId = Id FROM Exclusive.AspNetRoles WHERE NormalizedName = 'USER'

IF((SELECT COUNT(*) FROM Exclusive.AspNetUserRoles WHERE RoleId = @userRoleId AND UserId = @aspUserId) = 0)
BEGIN
	INSERT INTO Exclusive.AspNetUserRoles VALUES(@aspUserId, @userRoleId)
END
GO

INSERT [Exclusive].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Discriminator]) VALUES (N'8598bc60-b2f2-4402-ba3f-d086de6b9b24', N'n.wilson@dwpublishing.eu', N'N.WILSON@DWPUBLISHING.EU', N'n.wilson@dwpublishing.eu', N'N.WILSON@DWPUBLISHING.EU', 0, N'AQAAAAEAACcQAAAAEHvUwBY50vpIo5eMmMhv1+tlLhp0WJeE/WNYGHHzmNjYMODOTn+wjA4Tz+QpIYCEOA==', N'QVN272QQHV7RKBPXUOXQCUF7HHVW3OVJ', N'c36965b9-b774-4360-97ab-60373579325d', NULL, 0, 0, NULL, 1, 0, N'IdentityUser')
GO
INSERT [Exclusive].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Discriminator]) VALUES (N'9aa511d0-92f3-4011-8aa0-9ad875542d14', N'david@lloydfp.co.uk', N'DAVID@LLOYDFP.CO.UK', N'david@lloydfp.co.uk', N'DAVID@LLOYDFP.CO.UK', 0, N'AQAAAAEAACcQAAAAEFy3kFgXHjKwpMooGPCbbJrIiQFQI0o2LHGltJleXA2h/oJ66RUpcbMQ47tns4nQUw==', N'ZFXKG7VOHX4WCOJAB3QBBSXVMAG3BZYO', N'f773d387-2ee3-4f74-858d-e8e7ad363711', NULL, 0, 0, NULL, 1, 0, N'IdentityUser')
GO
INSERT [Exclusive].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Discriminator]) VALUES (N'724a9e5c-4bf4-4141-97e1-3139caef319a', N'j.wilson@dwpublishing.eu', N'J.WILSON@DWPUBLISHING.EU', N'j.wilson@dwpublishing.eu', N'J.WILSON@DWPUBLISHING.EU', 0, N'AQAAAAEAACcQAAAAEENBNmRq7PZ36gJpsBqhtDZ73ZxU+JrpqDLAxX5vRNJuQ6Jx24v09wFEehAmTf21tA==', N'HXK2EAKDKEU44TKL2C3XAIYTIZSKYHPQ', N'4f8bfc84-28a7-431c-a177-3654532356ad', NULL, 0, 0, NULL, 1, 0, N'IdentityUser')
GO
INSERT [Exclusive].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Discriminator]) VALUES (N'60bb8c39-eecc-49d5-b34c-38dfce9660de', N'rothwell.adam94@gmail.com', N'ROTHWELL.ADAM94@GMAIL.COM', N'rothwell.adam94@gmail.com', N'ROTHWELL.ADAM94@GMAIL.COM', 0, N'AQAAAAEAACcQAAAAEInlkGkTt0Y5Feep0jALIe8aSf8VBQAMtt41sV3KrHwSGnekWo+Sn+eJIj840in+7w==', N'PMZW4U3IKXMJO6KRL7J6WA4OOS5D43EA', N'98211443-bd61-4d8b-a650-fdf85fcab9b3', NULL, 0, 0, NULL, 1, 0, N'IdentityUser')
GO
INSERT [Exclusive].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Discriminator]) VALUES (N'5e47e512-0bf0-449c-affb-43ca8387de36', N'tyler@exclusivecard.co.uk', N'TYLER@EXCLUSIVECARD.CO.UK', N'tyler@exclusivecard.co.uk', N'TYLER@EXCLUSIVECARD.CO.UK', 0, N'AQAAAAEAACcQAAAAECEHz3GDYPKXw3O8I6Lpp5mzZh6BkTwK9ufO+nR08DnSyfrQwHmOEPLwZOy2tVV0mg==', N'CFVBNIBBIKPS4CB75BZJSU6Z4AJOITSR', N'1bd18218-3693-4d56-ab23-025e090a2c7e', NULL, 0, 0, NULL, 1, 0, N'IdentityUser')
GO


INSERT [Exclusive].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'5e47e512-0bf0-449c-affb-43ca8387de36', N'af278518-6d80-4666-9257-f287308f9b83')
GO
INSERT [Exclusive].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'60bb8c39-eecc-49d5-b34c-38dfce9660de', N'af278518-6d80-4666-9257-f287308f9b83')
GO
INSERT [Exclusive].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'724a9e5c-4bf4-4141-97e1-3139caef319a', N'af278518-6d80-4666-9257-f287308f9b83')
GO
INSERT [Exclusive].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'8598bc60-b2f2-4402-ba3f-d086de6b9b24', N'af278518-6d80-4666-9257-f287308f9b83')
GO
INSERT [Exclusive].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'9aa511d0-92f3-4011-8aa0-9ad875542d14', N'af278518-6d80-4666-9257-f287308f9b83')
GO

SET IDENTITY_INSERT [Exclusive].[ContactDetail] ON
GO
INSERT [Exclusive].[ContactDetail] ([Id], [Address1], [Address2], [Address3], [Town], [District], [PostCode], [CountryCode], [Latitude], [Longitude], [LandlinePhone], [MobilePhone], [EmailAddress], [IsDeleted]) VALUES (7102, N'6 The Fallows', N'Birkdale', N'', N'Southport', N'Merseyside', N'PR8 5BJ', N'GB', N'', N'', N'07703822399', N'', NULL, 0)
GO
INSERT [Exclusive].[ContactDetail] ([Id], [Address1], [Address2], [Address3], [Town], [District], [PostCode], [CountryCode], [Latitude], [Longitude], [LandlinePhone], [MobilePhone], [EmailAddress], [IsDeleted]) VALUES (7103, N'6 Moss Gardens', N'Birkdale,', N'', N'Southport', N'Merseyside', N'PR8 4JD', N'GB', N'', N'', N'07809 226112', N'', NULL, 0)
GO
INSERT [Exclusive].[ContactDetail] ([Id], [Address1], [Address2], [Address3], [Town], [District], [PostCode], [CountryCode], [Latitude], [Longitude], [LandlinePhone], [MobilePhone], [EmailAddress], [IsDeleted]) VALUES (7104, N'163 Cambridge Road', N'', N'', N'Southport', N'Merseyside', N'PR9 7OR', N'GB', N'', N'', N'', N'', NULL, 0)
GO
INSERT [Exclusive].[ContactDetail] ([Id], [Address1], [Address2], [Address3], [Town], [District], [PostCode], [CountryCode], [Latitude], [Longitude], [LandlinePhone], [MobilePhone], [EmailAddress], [IsDeleted]) VALUES (7105, N'6 Moss Gardens', N'', N'', N'Southport', N'Merseyside', N'PR8 4JD', N'GB', N'', N'', N'', N'', NULL, 0)
GO
INSERT [Exclusive].[ContactDetail] ([Id], [Address1], [Address2], [Address3], [Town], [District], [PostCode], [CountryCode], [Latitude], [Longitude], [LandlinePhone], [MobilePhone], [EmailAddress], [IsDeleted]) VALUES (7106, N'15 HOGHTON STREET', N'', N'', N'SOUTHPORT', N'Merseyside', N'PR9 0NS', N'GB', N'-3.0017893', N'53.6485186', N'', N'', NULL, 0)
GO
INSERT [Exclusive].[ContactDetail] ([Id], [Address1], [Address2], [Address3], [Town], [District], [PostCode], [CountryCode], [Latitude], [Longitude], [LandlinePhone], [MobilePhone], [EmailAddress], [IsDeleted]) VALUES (7107, N'12 Maesbrook Close', N'Banks', N'', N'Southport', N'Lancashire', N'PR9 8FF', N'GB', N'-2.9148991', N'53.6735191', N'', N'', NULL, 0)
GO
SET IDENTITY_INSERT [Exclusive].[ContactDetail] OFF
Go


SET IDENTITY_INSERT [Exclusive].[Customer] ON 
GO
INSERT [Exclusive].[Customer] ([Id], [ContactDetailId], [Title], [Forename], [Surname], [IsActive], [IsDeleted], [AspNetUserId], [DateOfBirth], [DateAdded], [MarketingNewsLetter], [MarketingThirdParty]) VALUES (2102, 7102, NULL, N'Zoe', N'Quinn', 0, 0, N'b18756e6-f7ae-4a31-8926-49fe5466d54c', NULL, CAST(N'2015-11-09 17:40:17.0000000' AS DateTime2), 1, 0)
GO
INSERT [Exclusive].[Customer] ([Id], [ContactDetailId], [Title], [Forename], [Surname], [IsActive], [IsDeleted], [AspNetUserId], [DateOfBirth], [DateAdded], [MarketingNewsLetter], [MarketingThirdParty]) VALUES (2103, 7103, NULL, N'Neil', N'Wilson', 0, 0, N'8598bc60-b2f2-4402-ba3f-d086de6b9b24', NULL, CAST(N'2015-11-09 21:41:39.0000000' AS DateTime2), 1, 0)
GO
INSERT [Exclusive].[Customer] ([Id], [ContactDetailId], [Title], [Forename], [Surname], [IsActive], [IsDeleted], [AspNetUserId], [DateOfBirth], [DateAdded], [MarketingNewsLetter], [MarketingThirdParty]) VALUES (2104, 7104, NULL, N'David', N'Lloyd', 0, 0, N'9aa511d0-92f3-4011-8aa0-9ad875542d14', NULL, CAST(N'2015-11-12 20:05:34.0000000' AS DateTime2), 1, 0)
GO
INSERT [Exclusive].[Customer] ([Id], [ContactDetailId], [Title], [Forename], [Surname], [IsActive], [IsDeleted], [AspNetUserId], [DateOfBirth], [DateAdded], [MarketingNewsLetter], [MarketingThirdParty]) VALUES (2105, 7105, NULL, N'Jitka', N'Wilson', 0, 0, N'724a9e5c-4bf4-4141-97e1-3139caef319a', NULL, CAST(N'2015-11-16 19:29:44.0000000' AS DateTime2), 1, 0)
GO
INSERT [Exclusive].[Customer] ([Id], [ContactDetailId], [Title], [Forename], [Surname], [IsActive], [IsDeleted], [AspNetUserId], [DateOfBirth], [DateAdded], [MarketingNewsLetter], [MarketingThirdParty]) VALUES (2106, 7106, NULL, N'Tyler', N'Birch', 0, 0, N'5e47e512-0bf0-449c-affb-43ca8387de36', NULL, CAST(N'2018-10-15 20:12:36.0000000' AS DateTime2), 0, 1)
GO
INSERT [Exclusive].[Customer] ([Id], [ContactDetailId], [Title], [Forename], [Surname], [IsActive], [IsDeleted], [AspNetUserId], [DateOfBirth], [DateAdded], [MarketingNewsLetter], [MarketingThirdParty]) VALUES (2107, 7107, NULL, N'Adam', N'Rothwell', 0, 0, N'60bb8c39-eecc-49d5-b34c-38dfce9660de', NULL, CAST(N'2018-03-16 22:00:25.0000000' AS DateTime2), 0, 1)
GO
SET IDENTITY_INSERT [Exclusive].[Customer] OFF
Go

SET IDENTITY_INSERT [Exclusive].[MembershipCard] ON 
GO
INSERT [Exclusive].[MembershipCard] ([Id], [CustomerId], [MembershipPlanId], [CardNumber], [ValidFrom], [ValidTo], [DateIssued], [StatusId], [AgentCode], [IsActive], [IsDeleted], [PhysicalCardRequested], [CustomerPaymentProviderId], [PhysicalCardStatusId]) VALUES (1407, 2102, 1, N'EX0010028GB', CAST(N'2019-01-17 15:34:31.0000000' AS DateTime2), CAST(N'2020-01-17 00:00:00.0000000' AS DateTime2), CAST(N'2019-01-17 15:34:31.0000000' AS DateTime2), 11, N'', 1, 0, 1, N'', NULL)
GO
INSERT [Exclusive].[MembershipCard] ([Id], [CustomerId], [MembershipPlanId], [CardNumber], [ValidFrom], [ValidTo], [DateIssued], [StatusId], [AgentCode], [IsActive], [IsDeleted], [PhysicalCardRequested], [CustomerPaymentProviderId], [PhysicalCardStatusId]) VALUES (1408, 2103, 1, N'EX0010044GB', CAST(N'2018-07-04 15:31:31.0000000' AS DateTime2), CAST(N'2019-07-04 00:00:00.0000000' AS DateTime2), CAST(N'2018-07-04 15:31:31.0000000' AS DateTime2), 11, N'', 1, 0, 1, N'', NULL)
GO
INSERT [Exclusive].[MembershipCard] ([Id], [CustomerId], [MembershipPlanId], [CardNumber], [ValidFrom], [ValidTo], [DateIssued], [StatusId], [AgentCode], [IsActive], [IsDeleted], [PhysicalCardRequested], [CustomerPaymentProviderId], [PhysicalCardStatusId]) VALUES (1409, 2104, 1, N'EX0010035GB', CAST(N'2018-04-19 15:10:49.0000000' AS DateTime2), CAST(N'2019-04-19 00:00:00.0000000' AS DateTime2), CAST(N'2018-04-19 15:10:49.0000000' AS DateTime2), 11, N'', 1, 0, 1, N'', 35)
GO
INSERT [Exclusive].[MembershipCard] ([Id], [CustomerId], [MembershipPlanId], [CardNumber], [ValidFrom], [ValidTo], [DateIssued], [StatusId], [AgentCode], [IsActive], [IsDeleted], [PhysicalCardRequested], [CustomerPaymentProviderId], [PhysicalCardStatusId]) VALUES (1410, 2105, 1, N'EX0010037GB', CAST(N'2018-12-04 14:48:34.0000000' AS DateTime2), CAST(N'2019-12-04 00:00:00.0000000' AS DateTime2), CAST(N'2018-12-04 14:48:34.0000000' AS DateTime2), 11, N'', 1, 0, 1, N'', NULL)
GO
INSERT [Exclusive].[MembershipCard] ([Id], [CustomerId], [MembershipPlanId], [CardNumber], [ValidFrom], [ValidTo], [DateIssued], [StatusId], [AgentCode], [IsActive], [IsDeleted], [PhysicalCardRequested], [CustomerPaymentProviderId], [PhysicalCardStatusId]) VALUES (1411, 2106, 1, N'EX0013614GB', CAST(N'2018-10-17 14:34:45.0000000' AS DateTime2), CAST(N'2019-10-17 00:00:00.0000000' AS DateTime2), CAST(N'2018-10-17 14:34:45.0000000' AS DateTime2), 11, N'', 1, 0, 1, N'', NULL)
GO
INSERT [Exclusive].[MembershipCard] ([Id], [CustomerId], [MembershipPlanId], [CardNumber], [ValidFrom], [ValidTo], [DateIssued], [StatusId], [AgentCode], [IsActive], [IsDeleted], [PhysicalCardRequested], [CustomerPaymentProviderId], [PhysicalCardStatusId]) VALUES (1412, 2107, 1, N'EX0012161GB', CAST(N'2018-03-16 22:00:50.0000000' AS DateTime2), CAST(N'2019-03-16 00:00:00.0000000' AS DateTime2), CAST(N'2018-03-16 22:00:50.0000000' AS DateTime2), 11, N'', 1, 0, 1, N'', 35)
GO
SET IDENTITY_INSERT [Exclusive].[MembershipCard] OFF
Go