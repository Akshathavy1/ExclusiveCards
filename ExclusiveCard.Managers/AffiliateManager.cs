using System;
using System.Collections.Generic;
using System.Linq;
using DTO = ExclusiveCard.Services.Models.DTOs;
using DB = ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using AutoMapper;

namespace ExclusiveCard.Managers
{
    public abstract class AffiliateManager<TFile, TErrors, TData> : IAffiliateManager<TFile, TErrors, TData>
    {
        #region Private fields and Constructor

        IRepository<DB.AffiliateFieldMapping> _fieldMappingRepo;
        IRepository<DB.AffiliateFile> _fileRepo;

        protected IMapper _mapper;

        public AffiliateManager(IRepository<DB.AffiliateFile> fileRepo,   IRepository<DB.AffiliateFieldMapping> fieldMappingRepo, IMapper mapper)
        {
            _fieldMappingRepo = fieldMappingRepo;
            _fileRepo = fileRepo;
            _mapper = mapper;
        }

        #endregion

        public List<DTO.AffiliateFieldMapping> GetFieldMappings(int affiliateId, int affiliateFileId)
        {
            List<DTO.AffiliateFieldMapping> dtoFieldMappings = null;
            var dbFile = _fileRepo.FilterNoTrack(x => x.AffiliateId == affiliateId && x.Id == affiliateFileId).FirstOrDefault();
            if (dbFile != null)
            {
                var dbFieldMappings = _fieldMappingRepo.Include(c => c.AffiliateMappingRule.AffiliateMappings)                                                    
                                                       .Where(x => x.AffiliateFileMappingId == dbFile.AffiliateFileMappingId)
                                                       .ToList();
                dtoFieldMappings = _mapper.Map<List<DTO.AffiliateFieldMapping>>(dbFieldMappings);
            }

            return dtoFieldMappings;
        }

        public abstract System.Threading.Tasks.Task<Tuple<List<TData>, List<TErrors>>> MapToSchemaAsync(List<TFile> records, DTO.StagingModels.OfferImportFile offerImportFile, string cultureInfoCountry);


        public abstract List<TFile> ReadFileFromCSV(string filePath);


        public abstract int SaveStagingData(List<TData> data);

        public abstract List<TData> GetStagingData(int fileId);

        public abstract int SaveErrorFile(List<TErrors> errorList, string errorfileName);

        public abstract System.Threading.Tasks.Task<int> AppendToErrorFile(List<DTO.StagingModels.OfferImportError> recordErrorList, string errorfile);

    }
}
