using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Public
{
    public class LocalisationService : ILocalisationService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly ILocalisationManager _manager;

        #endregion

        #region Constructor

        public LocalisationService(IMapper mapper, ILocalisationManager localisationManager)
        {
            _mapper = mapper;
            _manager = localisationManager;
        }

        #endregion

        #region Writes

        public async Task<DTOs.Localisation> Add(DTOs.Localisation locale)
        {
            Localisation req = _mapper.Map<Localisation>(locale);
            return _mapper.Map<DTOs.Localisation>(await _manager.Add(req));
        }

        public async Task<DTOs.Localisation> Update(DTOs.Localisation locale)
        {
            Localisation req = _mapper.Map<Localisation>(locale);
            return _mapper.Map<DTOs.Localisation>(await _manager.Update(req));
        }

        #endregion

        #region Reads

        public DTOs.Localisation Get(int id)
        {
            return _mapper.Map<DTOs.Localisation>(_manager.Get(id));
        }

        public async Task<List<DTOs.Localisation>> GetAll(string localisationCode)
        {
            return _mapper.Map<List<DTOs.Localisation>>(await _manager.GetAll(localisationCode));
        }

        public string GetByContext(string context, string localisationCode)
        {
            return _manager.GetByContext(context, localisationCode);
        }

        #endregion
    }
}
