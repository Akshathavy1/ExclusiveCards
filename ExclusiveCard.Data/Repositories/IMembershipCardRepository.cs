using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Data.Repositories
{
    public interface IMembershipCardRepository : IRepository<MembershipCard>
    {
        void CreateMembershipCard(MembershipCard card, MembershipPlanTypeEnum planType);

        void CreateCashBackTransactions(CashbackTransaction cashBackTransaction);
    }
}
