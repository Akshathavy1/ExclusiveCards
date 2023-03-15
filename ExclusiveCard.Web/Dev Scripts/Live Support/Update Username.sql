DECLARE @oldUser nvarchar(50) = 'mcasba53@gmail.com'
Declare @NewUser  nvarchar(50)  = 'mcasba63@gmail.com'

Select * from Exclusive.AspNetUsers
where username = @OldUser

Select * from Exclusive.AspNetUsers
where username = @NewUser

/*
BEGIN TRAN

Update Exclusive.AspNetUsers
Set UserName = @NewUser, 
NormalizedUserName = UPPER(@NewUser), 
Email = @NewUser, 
NormalizedEmail  = UPPER(@NewUser)
where username = @OldUser

Select * from Exclusive.AspNetUsers
where username = @OldUser

Select * from Exclusive.AspNetUsers
where username = @NewUser

--COMMIT TRAN

*/