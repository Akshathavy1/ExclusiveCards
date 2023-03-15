using AutoMapper;
using ExclusiveCard.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data = ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public class WhiteLabelManager : IWhiteLabelManager
    {
        #region Private Member and Constructor

        private readonly IRepository<data.WhiteLabelSettings> _whiteLabelRepo;
        private readonly IRepository<data.SponsorImages> _imagesRepo;
        private readonly ILogger _logger;
        private readonly IMapper _mapper = null;

        public WhiteLabelManager(IRepository<data.WhiteLabelSettings> whiteLabelRepo, IRepository<data.SponsorImages> imagesRepo, IMapper mapper)
        {
            _whiteLabelRepo = whiteLabelRepo;
            _imagesRepo = imagesRepo;
            _logger = LogManager.GetCurrentClassLogger();
            _mapper = mapper;
        }

        #endregion Private Member and Constructor

        public IList<dto.WhiteLabelSettings> GetAll()
        {
            var dbWhiteLabelSettings = _whiteLabelRepo.Filter().ToList();
            var dtoWhiteLableSettings = _mapper.Map<List<dto.WhiteLabelSettings>>(dbWhiteLabelSettings);
            return dtoWhiteLableSettings;
        }

        ///<see cref="IWhiteLabelManager.GetAllAsync"/>
        public async Task<IList<dto.WhiteLabelSettings>> GetAllAsync()
        {
            var dbWhiteLabelSettings = await _whiteLabelRepo.GetAllAsync();
            var dtoWhiteLableSettings = _mapper.Map<List<dto.WhiteLabelSettings>>(dbWhiteLabelSettings);
            return dtoWhiteLableSettings;
        }

        public IList<dto.WhiteLabelSettings> GetRegionSites()
        {
            var dbWhiteLabelSettings = _whiteLabelRepo.Filter(x => x.IsRegional).ToList();
            var dtoWhiteLableSettings = _mapper.Map<List<dto.WhiteLabelSettings>>(dbWhiteLabelSettings);
            return dtoWhiteLableSettings;
        }

        public dto.WhiteLabelSettings GetSiteSettingsById(int id)
        {
            var dbWhiteLabelSettings = _whiteLabelRepo.GetById(id);
            var settings = _mapper.Map<dto.WhiteLabelSettings>(dbWhiteLabelSettings);
            return settings;
        }

        public async Task<List<dto.SponsorImages>> GetSponsorImagesById(int id)
        {
            var dbImages = await _imagesRepo.FilterNoTrackAsync(x => x.WhiteLabelId == id);
            var dtoImages = _mapper.Map<List<dto.SponsorImages>>(dbImages);
            return dtoImages;
        }

        public dto.WhiteLabelSettings GetByUrl(string url)
        {
            var dbWhiteLabelSettings = _whiteLabelRepo.GetNoTrack(x => x.URL == url);
            var dtoWhiteLableSettings = _mapper.Map<dto.WhiteLabelSettings>(dbWhiteLabelSettings);
            return dtoWhiteLableSettings;
        }

        public async Task<IList<dto.WhiteLabelSettings>> GetWhiteLabelToBeAddedToSendGrid()
        {
            var labels = await _whiteLabelRepo.FilterNoTrackAsync(x => x.MarketingContactLists == null);
            var dtoLabels = _mapper.Map<IList<dto.WhiteLabelSettings>>(labels);
            return dtoLabels;
        }

        public async Task<dto.WhiteLabelSettings> Update(dto.WhiteLabelSettings settings)
        {
            var dbSettings = _mapper.Map<data.WhiteLabelSettings>(settings);
            _whiteLabelRepo.Update(dbSettings);

            var result = _mapper.Map<dto.WhiteLabelSettings>(dbSettings);

            await Task.CompletedTask;

            return result;
        }

        #region Private Members

        private dto.SponsorImages MapToDto(Data.Models.SponsorImages req)
        {
            var customer = _mapper.Map<dto.SponsorImages>(req);
            return customer;
        }

        #endregion Private Members
    }
}