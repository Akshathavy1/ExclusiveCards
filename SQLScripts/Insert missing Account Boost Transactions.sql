-- Script to insert missing Account Boost transactions

-- Find the Payment notification .... Search by date to find it
Select TOP 10 * from Exclusive.PaymentNotification WHERE TransactionType = 'web_accept' order by id desc

--  TODO ... Set the payment Id and date of txn
DECLARE @PaymentId varchar(50) = 'c1bc011e-07e5-4cc9-92a0-147af25e1d31'
DECLARE @TxnDate DATETIME = '2020-02-04 17:31:46' -- Date/Time IPN received



-- Find the payment in progress
DECLARE @ASPNetUserId  varchar(50)
SELECT @ASPNetUserId = AspNetUserId from Staging.CustomerRegistration where CustomerPaymentId = @PaymentId and StatusId = 43 -- Status New

-- Confirm customer name
DECLARE @CustId Int
SELECT @CustId = Id from Exclusive.Customer Where AspNetUserId = @ASPNetUserId
SELECT * from Exclusive.Customer Where AspNetUserId = @ASPNetUserId

-- Find the Card Id
DECLARE @CardId INT   
Select Top 1  @CardId = MC.Id from Exclusive.MembershipCard MC
inner join Exclusive.MembershipPlan MP on MC.MembershipPlanId = MP.Id
INNER JOIN Exclusive.MembershipLevel ML ON MP.MembershipLevelId = ML.Id
WHERE MC.CustomerId= @CustId And MC.IsActive = 1 ORDER BY ML.Level Desc 

SELECT @CardId

-- Check txns before adding
Select top 10 * from [Exclusive].[CashbackTransaction] order by id desc




  BEGIN TRAN

  INSERT [Exclusive].[CashbackTransaction] ([MembershipCardId] ,[PartnerId] ,[AccountType] ,[TransactionDate] ,[PurchaseAmount] ,[CurrencyCode] ,[Summary] ,[Detail],[StatusId] ,[DateReceived]  ,[CashbackAmount]  )
  VALUES 
   (@CardId, 1, 'R', @TxnDate, 11, 'GBP','Account Boost Deposit','Investment',55,@TxnDate, 10)
  ,(@CardId, 1, 'D', @TxnDate, 11, 'GBP','Account Boost Deposit','Investment',55,@TxnDate, 1)
  

UPDATE  Staging.CustomerRegistration SET StatusId = 45 where CustomerPaymentId = @PaymentId and StatusId = 43 -- Status New  

-- Confirm they were inserted
Select top 10 * from [Exclusive].[CashbackTransaction]  order by id desc


--COMMIT TRAN
--ROLLBACK TRAN