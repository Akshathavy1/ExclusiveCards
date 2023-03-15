using AutoMapper;
using ExclusiveCard.Data.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

using db = ExclusiveCard.Data.Models;

using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public class PlanManager : IPlanManager
    {
        #region Private Fields and Constructor

        private readonly IRepository<db.MembershipPlan> _membershipPlanRepo;
        private readonly IRepository<db.PlanAgentHistory> _planAgentHistoryRepo;
        private readonly IMapper _mapper;

        public PlanManager(IRepository<db.MembershipPlan> membershipPlanRepo, IRepository<db.PlanAgentHistory> planAgentHistoryRepo, IMapper mapper)
        {
            _membershipPlanRepo = membershipPlanRepo;
            _planAgentHistoryRepo = planAgentHistoryRepo;
            _mapper = mapper;
        }

        #endregion Private Fields and Constructor

        /// <see cref="IPlanManager.CreatePlanAsync(dto.MembershipPlan)"/>
        public async Task<dto.MembershipPlan> CreatePlanAsync(dto.MembershipPlan membershipPlan)
        {
            var dbModel = _mapper.Map<db.MembershipPlan>(membershipPlan);
            _membershipPlanRepo.Create(dbModel);
            await _membershipPlanRepo.SaveChangesAsync();
            var dtoMembershipPlan = _mapper.Map<dto.MembershipPlan>(dbModel);

            await CreateHistory(dtoMembershipPlan.Id, dtoMembershipPlan.AgentCodeId);

            return dtoMembershipPlan;
        }

        /// <see cref="IPlanManager.GetAllPlansAsync(int)"/>
        public async Task<List<dto.MembershipPlan>> GetAllPlansAsync(int cardProviderId)
        {
            var dbMembershipPlans = await _membershipPlanRepo.FilterNoTrackAsync(x => (x.CardProviderId == cardProviderId)
                                        && x.IsActive && !x.IsDeleted);
            var dtoMembershipPlans = _mapper.Map<List<dto.MembershipPlan>>(dbMembershipPlans);
            return dtoMembershipPlans;
        }

        /// <see cref="IPlanManager.GetAllPlansAsync(int, int)"/>
        public async Task<List<dto.MembershipPlan>> GetAllPlansAsync(int whiteLabelId, int cardProviderId)
        {
            var dbMembershipPlans = await _membershipPlanRepo.FilterNoTrackAsync(x => (x.WhitelabelId == whiteLabelId)
                                        && (x.CardProviderId == cardProviderId)
                                        && x.IsActive && !x.IsDeleted);
            var dtoMembershipPlans = _mapper.Map<List<dto.MembershipPlan>>(dbMembershipPlans);
            return dtoMembershipPlans;
        }

        /// <see cref="IPlanManager.GetPlanById(int)"/>
        public dto.MembershipPlan GetPlanById(int planId)
        {
            dto.MembershipPlan dtoPlan = null;
            var dbPlan = _membershipPlanRepo.GetNoTrack(x => x.Id == planId);
            if (dbPlan != null)
            {
                dtoPlan = _mapper.Map<dto.MembershipPlan>(dbPlan);
                return dtoPlan;
            }
            return dtoPlan;
        }

        /// <see cref="IPlanManager.UpdatePlanAsync(dto.MembershipPlan)"/>
        public async Task<bool> UpdatePlanAsync(dto.MembershipPlan membershipPlan)
        {
            #region History Record checks
            var dboriginal = _membershipPlanRepo.GetNoTrack(x => x.Id == membershipPlan.Id);
            int originalCode = dboriginal.AgentCodeId ?? 0;
            int updatedCode = membershipPlan.AgentCodeId ?? 0;
            #endregion

            var dbModel = _mapper.Map<db.MembershipPlan>(membershipPlan);
            _membershipPlanRepo.Update(dbModel);
            var updated = await _membershipPlanRepo.SaveChangesAsync();

            #region History Record checks
            if (originalCode != updatedCode)
            {
                await CreateHistory(membershipPlan.Id, membershipPlan.AgentCodeId);
            }
            #endregion

            return updated != 0;
        }

        #region Private
        async Task CreateHistory(int membershipPlanId, int? agentCodeId)
        {
            var dbHist = new db.PlanAgentHistory()
            {
                AgentCodeId = agentCodeId,
                MembershipPlanId = membershipPlanId,
                Assigned = System.DateTime.UtcNow
            };
            _planAgentHistoryRepo.Create(dbHist);
            await _planAgentHistoryRepo.SaveChangesAsync();
        }
        #endregion
    }
}