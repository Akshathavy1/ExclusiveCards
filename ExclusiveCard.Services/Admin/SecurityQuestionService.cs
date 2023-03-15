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
    public class SecurityQuestionService : ISecurityQuestionService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly ISecurityQuestionManager _securityQuestionManager;

        #endregion

        #region Constructor

        public SecurityQuestionService(IMapper mapper, ISecurityQuestionManager securityQuestionManager)
        {
            _mapper = mapper;
            _securityQuestionManager = securityQuestionManager;
        }

        #endregion

        #region Writes

        //Add Customer
        public async Task<Models.DTOs.SecurityQuestion> Add(DTOs.SecurityQuestion securityQuestion)
        {
            SecurityQuestion req = _mapper.Map<SecurityQuestion>(securityQuestion);
            return _mapper.Map<Models.DTOs.SecurityQuestion>(
                await _securityQuestionManager.Add(req));
        }

        //Update Customer
        public async Task<Models.DTOs.SecurityQuestion> Update(DTOs.SecurityQuestion securityQuestion)
        {
            SecurityQuestion req = _mapper.Map<SecurityQuestion>(securityQuestion);
            return _mapper.Map<Models.DTOs.SecurityQuestion>(
                await _securityQuestionManager.Update(req));
        }

        #endregion

        #region Reads

        public async Task<List<Models.DTOs.SecurityQuestion>> GetAll()
        {
            return MapToDtos(await _securityQuestionManager.GetAll());
        }

        #endregion

        private List<Models.DTOs.SecurityQuestion> MapToDtos(List<SecurityQuestion> data)
        {
            if (data == null || data.Count == 0)
                return null;

            List<Models.DTOs.SecurityQuestion> dtos = new List<Models.DTOs.SecurityQuestion>();

            dtos.AddRange(data.Select(MapToDto));

            return dtos;
        }

        private Models.DTOs.SecurityQuestion MapToDto(SecurityQuestion data)
        {
            if (data == null)
                return null;
            return new Models.DTOs.SecurityQuestion
            {
                Id = data.Id,
                IsActive = data.IsActive,
                Question = data.Question
            };
        }
    }
}
