
 DECLARE @CustomerId INT = 1272
 DECLARE @PAymentDate DATETIME = '02 Mar 2019'
 DECLARE @Amount Decimal(18,2) = 19.99

 DECLARE @MembershipCardId  INT
 DECLARE @PAymentProviderId INT = 2 -- PayPAl
 
 SELECT @MembershipCardId = ID
 FROM Exclusive.MembershipCard where CustomerId = @CustomerId  And StatusId = 11

 IF @MembershipCardId IS NOT NULL
 BEGIN

  INSERT [Exclusive].[CustomerPayment]  
  ( [CustomerId],[MembershipCardId],[PaymentProviderId],[PaymentDate],[Amount],[CurrencyCode],[Details],
  [CashbackTransactionId],[PaymentNotificationId],[PaymentProviderRef]) 
  VALUES (@CustomerId, @MembershipCardId, @PAymentProviderId, @PAymentDate, @Amount, 'GBP', 'Annual Membership', null, null, null)

  END