DECLARE @CustomerId INT = 841
DECLARE @PayPalId nvarchar(30) = 'I-TDEGS4L8SPPR'
Declare @Active INT = 37
DECLARE @NextPaymentDate DATETIME = '17 Mar 2020'
DECLARE @NextPaymentAmount decimal(18,2) = 19.99
DECLARE @PaymentType NVARCHAR(10) = 'Yearly'
Declare @Id INT


SELECT @Id = id FROM [Exclusive].[PayPalSubscription] Where PayPalId = @PayPalId and CustomerId = @CustomerId
SELECT @Id
BEGIN TRAN

if @Id is null
BEGIN 

INSERT [Exclusive].[PayPalSubscription]
 ([CustomerId] ,[PayPalId], [PayPalStatusId],[NextPaymentDate]  ,[NextPaymentAmount]  ,[PaymentType])

values ( @CustomerId, @PayPalId, @Active, @NextPaymentDate, @NextPaymentAmount, @PaymentType)

END

ELSE

BEGIN
	UPDATE  [Exclusive].[PayPalSubscription] 
	Set NextPaymentDate = @NextPaymentDate, NextPaymentAmount = @NextPaymentAmount
	WHERE Id = @Id
END
COMMIT TRAN
