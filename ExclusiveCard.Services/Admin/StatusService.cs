using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Admin
{
    [Obsolete("Use the Status Enum rather than looking up status in a service call") ]
    public class StatusService : IStatusServices
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IStatusManager _statusManager;

        #endregion

        #region Contructor

        public StatusService(IMapper mapper, IStatusManager statusManager)
        {
            _mapper = mapper;
            _statusManager = statusManager;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.Status> Add(DTOs.Status status)
        {
            Status req = _mapper.Map<Status>(status);
            return _mapper.Map<Models.DTOs.Status>(await _statusManager.Add(req));

        }

        public async Task<List<Models.DTOs.Status>> AddRange(List<DTOs.Status> statuses)
        {
            List<Status> req = _mapper.Map<List<Status>>(statuses);
            return _mapper.Map<List<Models.DTOs.Status>>(
                await _statusManager.AddRange(req));
        }

        #endregion


        #region Reads

        //Get all status
        public async Task<List<Models.DTOs.Status>> GetAll(string type = null)
        {
            return MapToDTO(await _statusManager.GetAll(type));
        }

        //delete status if exists
        public async Task DeleteRangeAsync()
        {
            await _statusManager.DeleteRangeAsync();
        }

        #endregion

        private List<Models.DTOs.Status> MapToDTO(List<Data.Models.Status> status)
        {
            if (status == null || status.Count == 0)
            {
                return new List<Models.DTOs.Status>();
            }
            List<Models.DTOs.Status> stats = new List<Models.DTOs.Status>();

            stats.AddRange(status.Select(st => new Models.DTOs.Status
            {
                Id = st.Id,
                Name = st.Name,
                Type = st.Type,
                IsActive = st.IsActive
            }));

            return stats;
        }
    }
}
