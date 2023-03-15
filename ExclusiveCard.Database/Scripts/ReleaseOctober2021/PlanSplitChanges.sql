/****** Update Plan % Values  ******/

DECLARE @BenefitReward int
SELECT @BenefitReward = Id FROM [Exclusive].[MembershipPlanType] Where [Description] = 'BenefitRewards'
DECLARE @PartnerReward int
SELECT @PartnerReward = Id FROM [Exclusive].[MembershipPlanType] Where [Description] = 'PartnerReward'

BEGIN TRAN

  UPDATE [Exclusive].[MembershipPlan]
  SET [CustomerCashbackPercentage] = 80,
	[DeductionPercentage] = 20,
	[BenefactorPercentage] = 0
  WHERE [MembershipPlanTypeId] = @PartnerReward

  UPDATE [Exclusive].[MembershipPlan]
  SET [CustomerCashbackPercentage] = 55,
	[DeductionPercentage] = 20,
	[BenefactorPercentage] = 25
  WHERE [MembershipPlanTypeId] = @BenefitReward

SELECT * FROM [Exclusive].[MembershipPlan]
  WHERE [MembershipPlanTypeId] IN (@BenefitReward, @PartnerReward)
  ORDER BY [MembershipPlanTypeId]

 --COMMIT TRAN
 --ROLLBACK TRAN
