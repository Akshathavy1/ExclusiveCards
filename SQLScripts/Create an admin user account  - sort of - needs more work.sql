--INSERT [Exclusive].[AspNetUsers] (
--	 [Id]
--      ,[UserName]
--      ,[NormalizedUserName]
--      ,[Email]
--      ,[NormalizedEmail]
--      ,[EmailConfirmed]
--      --,[PasswordHash]
--      --,[SecurityStamp]
--      --,[ConcurrencyStamp]
--      --,[PhoneNumber]
--      ,[PhoneNumberConfirmed]
--      ,[TwoFactorEnabled]
--      --,[LockoutEnd]
--      ,[LockoutEnabled]
--      ,[AccessFailedCount]
--	  ) 
--  VALUES (  NewID(), 'info@ijustwantanapp.com', 'INFO@IJUSTWANTANAPP.COM', 'info@ijustwantanapp.com', 'INFO@IJUSTWANTANAPP.COM', 1, 0,0, 0, 0)

DECLARE @ContactDetail Int  
 insert Exclusive.ContactDetail
 (    [EmailAddress], [IsDeleted]) VALUES ('INFO@IJUSTWANTANAPP.COM', 0)
 SELECT @ContactDetail = @@IDENTITY



  insert Exclusive.Customer (
      [ContactDetailId]
      --,[Title]
      ,[Forename]
      ,[Surname]
      ,[IsActive]
      ,[IsDeleted]
      ,[AspNetUserId]
      --,[DateOfBirth]
      ,[DateAdded]
      ,[MarketingNewsLetter]
      ,[MarketingThirdParty]
      ,[MarketingThirdPart]
      --,[NINumber]
	  )
	VALUES (@ContactDetail, 'Ij', 'WAA', 1, 0, 'E059C272-4343-4F6A-A80F-A29733DEC2B5', getdate(), 0, 0,0)


