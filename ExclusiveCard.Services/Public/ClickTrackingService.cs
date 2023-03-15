using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;

namespace ExclusiveCard.Services.Public
{
    public class ClickTrackingService : IClickTrackingService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IClickTrackingManager _clickTrackingManager;

        #endregion

        #region Constructor

        public ClickTrackingService(IMapper mapper, IClickTrackingManager clickTrackingManager)
        {
            _mapper = mapper;
            _clickTrackingManager = clickTrackingManager;
        }

        #endregion

        #region Writes

        //Add ClickTracking
        public async Task<Models.DTOs.ClickTracking> Add(Models.DTOs.ClickTracking membershipCard)
        {
            ClickTracking req = _mapper.Map<ClickTracking>(membershipCard);
            return _mapper.Map<Models.DTOs.ClickTracking>(
                await _clickTrackingManager.Add(req));
        }

        //Update ClickTracking
        public async Task<Models.DTOs.ClickTracking> Update(Models.DTOs.ClickTracking membershipCard)
        {
            ClickTracking req = _mapper.Map<ClickTracking>(membershipCard);
            return _mapper.Map<Models.DTOs.ClickTracking>(
                await _clickTrackingManager.Update(req));
        }

        #endregion

        #region Reads

        public Models.DTOs.ClickTracking Get(int id)
        {
            return _mapper.Map<Models.DTOs.ClickTracking>(_clickTrackingManager.Get(id));
        }

        #endregion
    }
}
