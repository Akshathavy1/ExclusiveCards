Create Procedure [Exclusive].[SP_CustomerSearch]
	@PageNumber INT,
	@PageSize INT,
	@Username nvarchar(100) = null,
	@Forename nvarchar(100) = null,
	@Surname nvarchar(100) = null,
	@Cardnumber nvarchar(20) = null,
	@CardStatus INT = null,
	@RegistrationCode nvarchar(30) = null,
	@DateOfBrith DateTime = null,
	@DateOfIssue DateTime = null,
	@Postcode nvarchar(10) = null
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @StartRow INT;
	DECLARE @ENDRow INT;

	SET @StartRow = (@PageNumber - 1) * @PageSize;
	SET @ENDRow = (@PageNumber * @PageSize) + 1;

	With CustomerView AS (
	Select Count(*) Over() [TotalRecord], ROW_NUMBER() Over(Order By res.Id) as RowNumber, res.* 
	from (
		 Select Distinct C.Id, A.UserName, C.Forename, C.Surname, C.DateOfBirth, D.PostCode, MC.CardNumber
		 From Exclusive.Customer C 
		 INNER JOIN Exclusive.ContactDetail D ON C.ContactDetailId = D.Id 
		 INNER JOIN Exclusive.AspNetUsers A ON C.AspNetUserId = A.Id
		 LEFT JOIN Exclusive.MembershipCard MC ON C.Id = MC.CustomerId
		 LEFT JOIN Exclusive.MembershipPlan MP ON MC.MembershipPlanId = MP.Id 
		 LEFT JOIN Exclusive.MembershipRegistrationCode MRC ON MP.Id = MRC.MembershipPlanId 
		 where C.IsDeleted = 0
		AND (@Username Is Null OR A.UserName like '%'+@Username+'%') 
		And (@Forename Is Null OR C.Forename like '%'+@Forename+'%') 
		And (@Surname Is Null OR C.Surname like '%'+@Surname+'%') 
		And (@Cardnumber Is Null OR MC.CardNumber like '%'+@Cardnumber+'%') 
		AND (@CardStatus is NUll OR MC.StatusId = @CardStatus)
		And (@RegistrationCode Is Null OR MRC.RegistartionCode =@RegistrationCode) 
		And (@DateOfBrith IS NULL OR Convert(date,C.DateOfBirth) = Convert(date,@DateOfBrith)) 
		AND (@DateOfIssue IS NUll OR Convert(date,MC.DateIssued) = Convert(date,@DateOfIssue))
		And (@Postcode Is Null OR D.PostCode like '%'+@Postcode+'%')) as res )
		SELECT * From CustomerView Where RowNumber > @StartRow AND RowNumber < @ENDRow
		Order by RowNumber
END