using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Admin
{
    public class TagService : ITagService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly ITagManager _tagManager;

        #endregion

        #region Contructor

        public TagService(IMapper mapper, ITagManager tagManager)
        {
            _mapper = mapper;
            _tagManager = tagManager;
        }

        #endregion

        #region Writes

        //Add tag
        public async Task<Models.DTOs.Tag> Add(DTOs.Tag tag)
        {
            Tag req = _mapper.Map<Tag>(tag);
            return _mapper.Map<Models.DTOs.Tag>(
                await _tagManager.Add(req));
        }

        //Update tag
        public async Task<Models.DTOs.Tag> Update(DTOs.Tag tag)
        {
            Tag req = _mapper.Map<Tag>(tag);
            return _mapper.Map<Models.DTOs.Tag>(
                await _tagManager.Update(req));
        }

        #endregion

        #region Reads

        //Get all merchants
        public async Task<List<Models.DTOs.Tag>> GetAll()
        {
            return _mapper.Map<List<Models.DTOs.Tag>>(await _tagManager.GetAll());
        }

        #endregion
    }
}
