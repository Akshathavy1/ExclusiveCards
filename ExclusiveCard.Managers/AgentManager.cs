using AutoMapper;
using ExclusiveCard.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using db = ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public class AgentManager : IAgentManager
    {
        #region Private Fields and Constructor

        private readonly IRepository<db.AgentCode> _agentRepo;
        private readonly IMapper _mapper;

        public AgentManager(IRepository<db.AgentCode> agentRepo, IMapper mapper)
        {
            _agentRepo = agentRepo;
            _mapper = mapper;
        }

        #endregion Private Fields and Constructor

        /// <see cref="IAgentManager.CreateAgentAsync(AgentCode)"/>
        public async Task<dto.AgentCode> CreateAgentAsync(dto.AgentCode agentCode)
        {
            if (agentCode.StartDate == null)
            {
                agentCode.StartDate = DateTime.UtcNow;
            }
            agentCode.EndDate = null;
            agentCode.IsDeleted = false;
            var dbModel = _mapper.Map<db.AgentCode>(agentCode);
            _agentRepo.Create(dbModel);
            await _agentRepo.SaveChangesAsync();
            var dtoAgent = _mapper.Map<dto.AgentCode>(dbModel);
            return dtoAgent;
        }

        /// <see cref="IAgentManager.GetAllAgentsAsync"/>
        public async Task<List<dto.AgentCode>> GetAllAgentsAsync()
        {
            var dbAgents = await _agentRepo.FilterNoTrackAsync(a => !a.IsDeleted);
            var dtoAgents = _mapper.Map<List<dto.AgentCode>>(dbAgents);
            return dtoAgents;
        }

        /// <see cref="IAgentManager.UpdateAgentAsync(dto.AgentCode)"/>
        public async Task<bool> UpdateAgentAsync(dto.AgentCode agentCode)
        {
            var dbModel = _mapper.Map<db.AgentCode>(agentCode);
            _agentRepo.Update(dbModel);
            var updated = await _agentRepo.SaveChangesAsync();
            return updated != 0;
        }
    }
}