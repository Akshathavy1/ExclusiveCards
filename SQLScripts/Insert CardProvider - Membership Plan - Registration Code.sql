/*
MBS2020  :   Mike Black   : Standard code only   :   81/19 split   no limit
CJS2020   :   Chris Jones : Standard code only   :   81/19 split   no limit
NWS2020 :    Neil Wilson :  Standard code only  :    81/19 split   no limit
ZQS2020  :    Zoe Quinn  :  Standard code only  :    81/19 split   no limit
ARS2020   :   Adam Rothwell  :  Standard code only  :  81/19 split  no limit
TBS2020   :   Tyler Birch  :   Standard code only  :    81/19 split   no limit
*/

-- Edit these values for standard plans

Declare @cardProvider Varchar(max) = 'Mike Black'
Declare @registrationCode Varchar(max) = 'MBS2020'
Declare @customerCashbackPercentage INT = 81
Declare @deductionPercentage INT = 19
-- End editing

-- Add the records
BEGIN TRAN

  --Create Partner
  DECLARE @CardProviderId INT = null
  INSERT Exclusive.Partner (name, IsDeleted, Type ) Values (@cardProvider, 0, 1)
  SELECT @CardProviderId = SCOPE_IDENTITY()



  DECLARE @PlanID INT = null
  INSERT [Exclusive].[MembershipPlan] ([PartnerId], [MembershipPlanTypeId], [Duration] ,[NumberOfCards]  ,[ValidFrom]  ,[ValidTo]   ,[CustomerCardPrice]  ,[PartnerCardPrice]
      ,[CustomerCashbackPercentage]  ,[DeductionPercentage]  ,[IsDeleted]  ,[CurrencyCode]   ,[Description]   ,[IsActive]  ,[MembershipLevelId]   ,[PaidByEmployer]   ,[MinimumValue]
      ,[PaymentFee]   ,[CardProviderId] )

  VALUES (1, 4, 36500, 100000000, GetDate(), '31 Dec 2119', 0, 0, @customerCashbackPercentage, @deductionPercentage, 0, 'GBP', @CardProvider + ' Standard Rewards Plan', 1, 1, 0, 10, 1, @CardProviderId)
  SELECT @planId = SCOPE_IDENTITY()

  INSERT Exclusive.MembershipRegistrationCode (MembershipPlanId,[RegistartionCode] , ValidFrom, ValidTo, NumberOfCards, IsActive, IsDeleted)
  VALUES (@PlanID, @registrationCode, GetDate(), '31 Dec 2119', 100000000, 1, 0)

  INSERT Exclusive.MembershipPlanPaymentProvider (MEmbershipPlanId, PaymentProviderId, OneOffPaymentRef)
  VALUES (@PlanId, 2, 'ZRFM9WT7LQE9A')

COMMIT TRAN
-- END of Add

-- Check the results
SELECT * 
FROM Exclusive.Partner 
order by id desc

Select * 
from [Exclusive].[MembershipPlan] MP
where isACtive = 1
order by id desc

Select  P.Description, C.* 
from [Exclusive].[MembershipRegistrationCode] C
inner join Exclusive.MembershipPlan P ON C.MembershipPlanId = P.Id
where p.IsActive = 1
order by p.id desc


SElect P.Description,   PP.* 
from [Exclusive].[MembershipPlanPaymentProvider] PP
inner join Exclusive.MembershipPlan P ON PP.MembershipPlanId = P.Id
where p.IsActive = 1
order by p.id desc



