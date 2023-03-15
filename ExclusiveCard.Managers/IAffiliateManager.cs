using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DTO = ExclusiveCard.Services.Models.DTOs;
using ST = ExclusiveCard.Services.Models.DTOs.StagingModels;

namespace ExclusiveCard.Managers
{
    public interface IAffiliateManager<TFile, TErrors, TData>  
    {
        List<TFile> ReadFileFromCSV(string filePath);

        Task<Tuple<List<TData>, List<TErrors>>> MapToSchemaAsync(List<TFile> records, ST.OfferImportFile offerImportFile, string cultureInfoCountry);

        int SaveErrorFile(List<TErrors> errorList, string errorfileName);
        Task<int> AppendToErrorFile(List<ST.OfferImportError> recordErrorList, string errorfile);

        List<TData> GetStagingData(int affiliateFileId);

        int SaveStagingData(List<TData> data);

        List<DTO.AffiliateFieldMapping> GetFieldMappings(int affiliateId, int affiliateFileId);

    }
}
