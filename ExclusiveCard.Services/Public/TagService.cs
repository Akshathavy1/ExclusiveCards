using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Public
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
       
        #region Reads

        //Get all merchants
        public async Task<List<Models.DTOs.Tag>> GetAll()
        {
            return _mapper.Map<List<Models.DTOs.Tag>>(await _tagManager.GetAll());
        }        
        #endregion
    }
}