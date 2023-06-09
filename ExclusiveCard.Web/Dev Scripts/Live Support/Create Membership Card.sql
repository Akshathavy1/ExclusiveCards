 -- SET THESE VALUES 
  DECLARE @CustomerId INT = 1272
  DECLARE @ValidFrom DATETIME = '02 Mar 2019'
  DECLARE @MembershipPlanId INT = 2 -- Personal 2019
  
  DECLARE @CardNumber nvarchar(11) = 'EX0012142GB'
  DECLARE @ValidTo DATETIME =  Dateadd (YYYY, 1, @ValidFrom)
  DECLARE @DateIssued DATETIME = GetDAte()
  DECLARE @StatusId  INT = 11 -- Active / 
  DECLARE @CustomerPAymentProviderId INT = 2 -- PayPAl
  DECLARE @MembershipCardId INT

  if @CardNumber IS NULL
  BEGIN

  SELECT @Cardnumber = 'EX' +  replace(str(Convert(int, substring(max(CardNumber),3, 7) + 1), 7), ' ', '0') + 'GB'
  From Exclusive.MembershipCard
  Where SUBSTRING(cardNumber, 1, 2) = 'EX'
  
  END 


  INSERT [Exclusive].[MembershipCard]  (
  [CustomerId]
      ,[MembershipPlanId]
      ,[CardNumber]
      ,[ValidFrom]
      ,[ValidTo]
      ,[DateIssued]
      ,[StatusId]
      ,[AgentCode]
      ,[IsActive]
      ,[IsDeleted]
      ,[PhysicalCardRequested]
      ,[CustomerPaymentProviderId]
      ,[PhysicalCardStatusId]
      ,[RegistrationCode]
	  )

	  VALUES (@CustomerId, @MembershipPlanId, @CardNumber, @ValidFrom, @ValidTo, @DateIssued, @StatusId, null, 1, 0, 0, @CustomerPAymentProviderId, null, null)
   SELECT @MembershipCardId  = SCOPE_IDENTITY()

  SELECT 'New Membership Card = ' + @CardNumber


 
  INSERT [Exclusive].[CashbackSummary]
  (   [AffiliateId] ,[MembershipCardId] ,[PartnerId] ,[AccountType] ,[CurrencyCode] ,[PendingAmount] ,[ConfirmedAmount] ,[ReceivedAmount] 
  ,[FeeDue])
  VALUES (null, @MembershipCardId, null, 'C', 'GBP', 0, 0, 0, 0)
