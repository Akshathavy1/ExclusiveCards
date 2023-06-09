
  DECLARE @Duration int = 365 --- VALid for this number of days
  DECLARE @ValidFrom DateTime = '01 Feb 2020'
  DECLARE @ValidTo DateTime = '28 Feb 2120'  --  Make it valid for next 100 years, i.e. practically forever
  DECLARE @Price decimal (18,2) = 19.95  --- Price to charge the customer
  DECLARE @Description varchar(50) = 'Seychelles Personal Membership v3'   -- Name of Plan
  DECLARE @RegistrationCode varchar(50) = 'SEYPER20' --- Code to type in
  DECLARE @PlanId Int
  

  INSERT [Exclusive].[MembershipPlan]
   ( 
      [MembershipPlanTypeId]
      ,[Duration]
      ,[NumberOfCards]
      ,[ValidFrom]
      ,[ValidTo]
      ,[CustomerCardPrice]
      ,[PartnerCardPrice]
      ,[CustomerCashbackPercentage]
      ,[PartnerCashbackPercentage]
      ,[IsDeleted]
      ,[CurrencyCode]
      ,[Description]
      ,[IsActive])
	Values 
	(1, @Duration, 10000000, @ValidFrom, @ValidTo, @Price, 0, 100, 0, 0, 'GBP', @Description, 1)
	SELECT @PlanID = SCOPE_IDENTITY()

	INSERT [Exclusive].[MembershipRegistrationCode]
	(
	 [MembershipPlanId]
      ,[RegistartionCode]
      ,[ValidFrom]
      ,[ValidTo]
      ,[NumberOfCards]
      ,[IsActive]
      ,[IsDeleted]
	)
	VALUES (@PlanId, @RegistrationCode, @ValidFrom, @ValidTo, 10000000, 1, 0)

