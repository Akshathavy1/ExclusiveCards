using System;
using System.Collections.Generic;
using System.Transactions;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public interface IMembershipManager
    {
        /// <summary>
        /// Returns the membership card that the customer was set up against
        /// NO active or expired checks are performed
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        dto.MembershipCard GetOriginalMembershipCard(int customerId);
        dto.MembershipCard GetActiveMembershipCard(string aspNetUserId);

        dto.MembershipCard GetActiveMembershipCard(int customerId);

        dto.MembershipCard GetDiamondMembershipCard(string aspNetUserId);

        dto.MembershipCard GetDiamondMembershipCard(int customerId);

        dto.MembershipPendingToken CreatePendingToken(dto.MembershipPlan plan, string registrationCode);

        dto.MembershipPlan GetMembershipPlanFromPendingToken(Guid? pendingToken, out int? registrationCodeId);

        dto.MembershipPlan GetMembershipPlanFromRegistrationCode(string registrationCode);

        dto.MembershipPlan GetDiamondMembershipPlan(int standardPlanId);

        dto.MembershipPlan GetDiamondMembershipPlan(string CardProviderName, int CardPrividerType, int PlanTypeId);

        dto.MembershipPlan GetMembershipPlan(int planId);

        List<dto.MembershipCard> CreateMembershipCards(dto.MembershipPlan plan, int customerId, int registrationCodeId, int? termsConditionsId, int? partnerRewardId, string countrCode);

        dto.MembershipCard UpgradeToDiamond(dto.MembershipCard standardCard, string paymentDetails,decimal paymentAmount);

        dto.MembershipCard Renew(dto.MembershipCard expiredCard, string paymentDetails, int duration = 0);

        dto.PartnerDto GetCardProvider(int cardId);

        dto.MembershipPlan GetTalkSportRegistrationCode(int whiteLabelId, int membershipPlanTypeId);
    }
}