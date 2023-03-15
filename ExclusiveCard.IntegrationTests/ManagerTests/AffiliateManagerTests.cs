using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Managers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ST = ExclusiveCard.Services.Models.DTOs.StagingModels;
using DTO = ExclusiveCard.Services.Models.DTOs;
using DBST = ExclusiveCard.Data.StagingModels;
using DB = ExclusiveCard.Data.Models;
using AutoMapper;

namespace ExclusiveCard.IntegrationTests.ManagerTests
{
    class AffiliateManagerTests
    {
        IAffiliateManager<ST.AwinCSVFile, ST.AwinCSVFileError, ST.OfferImportAwin> _affiliateManager;
        IOfferImportManager _offerImportManager;
        IRepository<DBST.OfferImportFile> _offerImportFileRepo;
        IRepository<DB.AffiliateFile> _fileRepo;
        IRepository<DB.AffiliateFieldMapping> _fieldMappingRepo;
        IRepository<DBST.OfferImportAwin> _awinRepo;

        IMapper _mapper;

        const int RECORD_COUNT = 4;

        [OneTimeSetUp]
        public void Setup()
        {

            _offerImportManager = Configuration.ServiceProvider.GetService<IOfferImportManager>();
            
            _offerImportFileRepo = Configuration.ServiceProvider.GetService<IRepository<DBST.OfferImportFile>>();
            _fileRepo = Configuration.ServiceProvider.GetService<IRepository<DB.AffiliateFile>>();
            _fieldMappingRepo = Configuration.ServiceProvider.GetService<IRepository<DB.AffiliateFieldMapping>>();
            _awinRepo = Configuration.ServiceProvider.GetService<IRepository<DBST.OfferImportAwin>>();
            _mapper = Configuration.ServiceProvider.GetService<IMapper>();

            _affiliateManager = new AffiliateManagerAwin(_fileRepo, _fieldMappingRepo, _awinRepo, _mapper);
            
        }

        [OneTimeTearDown]
        public void TearDown()
        {

        }

        [Test]
        public void GetFieldMappings_Valid()
        {
            //TODO:  Create new test records for affiliates and mappings, don't rely on the ones already there
            var mappings = _affiliateManager.GetFieldMappings(1, 1);
            Assert.IsNotNull(mappings);
            Assert.IsTrue(mappings.Count > 0);

            var mapping = mappings.Where(x => x.AffiliateMappingRule != null).FirstOrDefault();
            Assert.IsNotNull(mapping);
            Assert.IsNotNull(mapping.AffiliateMappingRule);
            Assert.IsTrue(mapping.AffiliateFieldName != null);
            
        }

        [Test]
        public async Task GetStatingData_Valid()
        {
            // Create a new test file
            var fileId = await CreateStagingData();
            Assert.IsTrue(fileId > 0, "Unable to create a staging file");

            var data = _affiliateManager.GetStagingData(fileId);
            Assert.IsNotNull(data, "Null returned from get statingData");
            Assert.AreEqual(RECORD_COUNT, data.Count, "Unexpected number of records returned");
        }

        [Test]
        public void WriteThenUpdateErrorFile()
        {
            string errorFile = $"ErrorTest_{DateTime.Today.Ticks}.csv";
            //Write to file for first time...
            List<ST.AwinCSVFileError> errorList1 = new List<ST.AwinCSVFileError>();
            errorList1.Add(new ST.AwinCSVFileError() { ErrorMessage = "First error message" });
            _affiliateManager.SaveErrorFile(errorList1, errorFile);
            Assert.IsTrue(System.IO.File.Exists(errorFile), "Didn't create file: {errorFile}");

            //Write to file for second time...
            List<ST.AwinCSVFileError> errorList2 = new List<ST.AwinCSVFileError>();
            errorList2.Add(new ST.AwinCSVFileError() { ErrorMessage = "Second error message" });
            _affiliateManager.SaveErrorFile(errorList2, errorFile);
            Assert.IsTrue(System.IO.File.Exists(errorFile), "Didn't create file: {errorFile}");

        }

        private async Task<int> CreateStagingData()
        {
            

            // Go create our test data file, which has RECORD_COUNT records in.  
            var fileName = OfferImportTestHelper.CreateAWINTestFile();
            Assert.IsNotNull(fileName, "failed to create temp offer file");

            var offerImportFile = OfferImportTestHelper.CreateDtoOfferImportFile(Enums.Import.New, fileName, "Error_" + fileName);
            Assert.IsNotNull(offerImportFile, "Failed to create an OfferImportFile DTO");

            //offerImportFile.Id = OfferImportTestHelper.SaveDtoOfferImportFile(offerImportFile);
            offerImportFile = _offerImportManager.AddImportFile(offerImportFile);
            Assert.IsTrue(offerImportFile.Id > 0, "Failed to save the staging.OfferImportFile record");

            await _offerImportManager.UploadFileToStagingAsync(offerImportFile);
            Assert.AreEqual(RECORD_COUNT, offerImportFile.TotalRecords, "Failed to import the correct number of records");

            return offerImportFile.Id;
        }
    }
}
