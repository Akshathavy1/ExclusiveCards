/****** Script for SelectTopNRows command from SSMS  ******/
SELECT top 1000 *
  FROM [Exclusive].[AspNetUsers]  U
  left join Exclusive.AspNetUserRoles UR ON U.id = UR.UserId
  left join Exclusive.AspNetRoles R on R.id = ur.RoleId
  where 
 u.email like 'support%'
 -- r.name != 'user'


--BEGIN TRAN
--Delete Exclusive.AspNetUserRoles
--FROM [Exclusive].[AspNetUsers]  U
--  left join Exclusive.AspNetUserRoles UR ON U.id = UR.UserId
--  left join Exclusive.AspNetRoles R on R.id = ur.RoleId
--  where 
--  u.email like 'support%'

--COMMIT TRAN


--DECLARE @UserId nvarchar(450)
--SELECT @UserId = ID FROM Exclusive.AspNetUsers WHERE Email like  'support%'

--DECLARE @RoleId nvarchar(450)
--SELECT @RoleId = Id FROM Exclusive.AspNetRoles where name = 'AdminUser'

--IF @RoleId IS NOT NULL AND @UserId IS NOT NULL
--BEGIN
--	BEGIN TRAN
--	Insert Exclusive.AspNetUserRoles (userId, RoleID) Values 
--	(@UserId,  @RoleId)

--END
--Rollback TRan
--COMMIT TRAN
