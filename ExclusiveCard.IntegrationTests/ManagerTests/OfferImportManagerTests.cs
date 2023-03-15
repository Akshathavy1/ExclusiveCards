using System;
using System.Threading.Tasks;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Managers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using st = ExclusiveCard.Data.StagingModels;
using dto = ExclusiveCard.Services.Models.DTOs;
using AutoMapper;

namespace ExclusiveCard.IntegrationTests.ManagerTests
{
    class OfferImportManagerTests
    {
        IOfferImportManager _offerImportManager;
        IRepository<st.OfferImportFile> _offerImportFileRepo;
        IMapper _mapper;

        const int AWIN_AFFILIATE_ID = 1;
        const string TEMP_FILE_PATH = "C:\\Temp\\";

        [OneTimeSetUp]
        public void Setup()
        {
            _offerImportManager = Configuration.ServiceProvider.GetService<IOfferImportManager>();
            _offerImportFileRepo = Configuration.ServiceProvider.GetService<IRepository<st.OfferImportFile>>();
            _mapper = Configuration.ServiceProvider.GetService<IMapper>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {

        }

        [Test]
        public void GetImportFiles_Valid()
        {
            int newFileId = 0;

            newFileId = EnsureImportFileRecordsExist(Enums.Import.New);

            var fileRecs = _offerImportManager.GetImportFiles(Enums.Import.New);
            Assert.IsNotNull(fileRecs, "Import File records not found - method returned null");
            Assert.IsTrue(fileRecs.Count >= 1, "Import file records not found, count < 1");

            if (newFileId != 0)
                CleanUpImportFileRecord(newFileId);
        }

        [Test]
        public void GetImportFiles_ValidNoTrack()
        {
            int newFileId = 0;

            newFileId = EnsureImportFileRecordsExist(Enums.Import.New);

            var fileRecs = _offerImportManager.GetImportFiles(Enums.Import.New, true);
            Assert.IsNotNull(fileRecs, "Import File records not found - method returned null");
            Assert.IsTrue(fileRecs.Count >= 1, "Import file records not found, count < 1");

            if (newFileId != 0)
                CleanUpImportFileRecord(newFileId);
        }

        [Test]
        public void GetImportFiles_Invalid_NoRecords()
        {
            int fileStatusId = (int)Enums.Import.New;
            // pick some random statusID that will never appear in the Import File table
            int changedId = (int) Enums.MembershipCardStatus.Cancelled;
            // Make the statusId for any existing records with our chosen status negative
            ChangeFileImportStatusId(fileStatusId, changedId);

            // Should be no records now
            var fileRecs = _offerImportManager.GetImportFiles(Enums.Import.New);
            Assert.IsNotNull(fileRecs, "Expected empty recordset but Null was returned.");
            Assert.IsTrue(fileRecs.Count == 0, "Expected 0 records but unexpected data has been returned");

            // Go put the status back 
            ChangeFileImportStatusId(changedId, fileStatusId);
        }

        [Test]
        public async Task UploadToStagingAsync_Valid()
        {
            const int RECORD_COUNT = 4;

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

        }

        [Test]
        public void UploadToStagingAsync_InvalidCSVFormat()
        {

            // Go create our test data file, which has RECORD_COUNT records in.  
            var fileName = OfferImportTestHelper.CreateAWINTestFile_MissingField();
            Assert.IsNotNull(fileName, "failed to create temp offer file");

            var offerImportFile = OfferImportTestHelper.CreateDtoOfferImportFile(Enums.Import.New, fileName, "Error_" + fileName);
            Assert.IsNotNull(offerImportFile, "Failed to create an OfferImportFile DTO");

            //offerImportFile.Id = OfferImportTestHelper.SaveDtoOfferImportFile(offerImportFile);
            offerImportFile = _offerImportManager.AddImportFile(offerImportFile);
            Assert.IsTrue(offerImportFile.Id > 0, "Failed to save the staging.OfferImportFile record");

            try
            {
                Task.WaitAll(_offerImportManager.UploadFileToStagingAsync(offerImportFile));
                Assert.Fail("The UploadFileToStagingAsync method failed to recognise the invalid file format");
            }
            catch
            { }
        }

        [Test]
        public async Task UploadToStagingAsync_InvalidFileData()
        {
            const int TOTAL_RECORD_COUNT = 3;
            const int VALID_RECORD_COUNT = 2;
            const int INVALID_RECORD_COUNT = 1;

            // Go create our test data file, which has RECORD_COUNT records in.  
            var fileName = OfferImportTestHelper.CreateAWINTestFile_InvalidData();
            Assert.IsNotNull(fileName, "failed to create temp offer file");

            var offerImportFile = OfferImportTestHelper.CreateDtoOfferImportFile(Enums.Import.New, fileName, "Error_" + fileName);
            Assert.IsNotNull(offerImportFile, "Failed to create an OfferImportFile DTO");

            //offerImportFile.Id = OfferImportTestHelper.SaveDtoOfferImportFile(offerImportFile);
            offerImportFile = _offerImportManager.AddImportFile(offerImportFile);
            Assert.IsTrue(offerImportFile.Id > 0, "Failed to save the staging.OfferImportFile record");

            await _offerImportManager.UploadFileToStagingAsync(offerImportFile);
            Assert.AreEqual(TOTAL_RECORD_COUNT, offerImportFile.TotalRecords, "Failed to process the correct number of records");
            Assert.AreEqual(VALID_RECORD_COUNT, offerImportFile.Staged, "Failed to stage the correct number of records");
            Assert.AreEqual(INVALID_RECORD_COUNT, offerImportFile.Failed, "Failed to record the correct number of invalid records");
        }

        [Test]
        public async Task MigrateOffersFromStagingAsync_Valid()
        {
            const int RECORD_COUNT = 4;

            // Create a new test file
            var fileName = OfferImportTestHelper.CreateAWINTestFile();
            Assert.IsNotNull(fileName, "failed to create temp offer file");

            // Add a record to the OfferImportFile table for our test file
            var offerImportFile = OfferImportTestHelper.CreateDtoOfferImportFile(Enums.Import.New, fileName, "Error_" + fileName);
            Assert.IsNotNull(offerImportFile, "Failed to create an OfferImportFile DTO");
            //offerImportFile.Id = OfferImportTestHelper.SaveDtoOfferImportFile(offerImportFile);
            offerImportFile = _offerImportManager.AddImportFile(offerImportFile);
            Assert.IsTrue(offerImportFile.Id > 0, "Failed to save the staging.OfferImportFile record");
            offerImportFile.AffiliateFile = OfferImportTestHelper.CreateDtoAffiliateFile();


            // Save the data in our test file to staging
            await _offerImportManager.UploadFileToStagingAsync(offerImportFile);
            Assert.AreEqual(RECORD_COUNT, offerImportFile.TotalRecords, "Failed to import the correct number of records");

            // Now go and migrate it and lets see what happens
            await _offerImportManager.MigrateOffersFromStagingAsync(offerImportFile);
        }


        #region Private Methods

        /// <summary>
        /// Checks to see if any file records exist for specified status and creates one if not
        /// </summary>
        /// <param name="fileStatusEnum">File Import status</param>
        /// <returns>0 returned if files already existed else the fileId of the newly created file</returns>
        private int EnsureImportFileRecordsExist(Enums.Import fileStatusEnum)
        {
            // Use FileId to identify any created file, so we can clean up afterwards
            int fileId = 0; 
            var importStatus = (int)fileStatusEnum;
            var fileRecs = _offerImportFileRepo.FilterNoTrack(x => x.ImportStatus == importStatus);

            if (fileRecs == null)
            {
                st.OfferImportFile fileRecord = CreateOfferImportFile(fileStatusEnum, TEMP_FILE_PATH + "TestFile1.csv");

                _offerImportFileRepo.Create(fileRecord);
                _offerImportFileRepo.SaveChanges();
                fileId = fileRecord.Id;
            }

            return fileId;
        }

        private st.OfferImportFile CreateOfferImportFile(Enums.Import importStatus, string fileName, string errorFilePath = null)
        {
            return new st.OfferImportFile()
            {
                AffiliateFileId = AWIN_AFFILIATE_ID,
                DateImported = DateTime.Now,
                FilePath = fileName,
                ErrorFilePath = errorFilePath,
                ImportStatus = (int)importStatus,
                Staged = 0,
                TotalRecords = 0,
                Failed = 0,
                Imported = 0,
                Duplicates = 0,
                Updates = 0,
                CountryCode = "GB"
            };
        }


        /// <summary>
        /// Removes specifield file record from the Staging.OfferImportFile table. 
        /// </summary>
        /// <param name="fileId"></param>
        private void CleanUpImportFileRecord(int fileId)
        {
            _offerImportFileRepo.Delete(fileId);
            _offerImportFileRepo.SaveChanges();
        }


        /// <summary>
        /// Changes the status Id for all records currently set to this status  to a new value. 
        /// </summary>
        /// <param name="fileStatusId">Current status Id</param>
        /// <param name="changedStatusId">New status Id</param>
        private void ChangeFileImportStatusId(int fileStatusId, int changedStatusId)
        {

            var fileRecs = _offerImportFileRepo.Filter(x => x.ImportStatus == fileStatusId);

            if (fileRecs != null)
            {
                foreach (var rec in fileRecs)
                {
                    rec.ImportStatus = changedStatusId;
                }
                _offerImportFileRepo.SaveChanges();
            }
            
        }

        

        #endregion

    }
}
