using ExclusiveCard.Data.Constants;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.IntegrationTests.Admin
{
    class AdminOfferImportTest
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public async Task ExecuteSPMappingSTOfferToOffer()
        {
            Assert.IsTrue(false, "Old import process");
            //try
            //{
            //    //OfferService.ExecuteSPMappingSTOfferToOffer Test

            //    //1. Add Merchant (30 records)
            //    List<dto.Merchant> merchants = new List<dto.Merchant>();
            //    List<dto.Merchant> merchantsList = Common.Common.BuildMerchantsModeList();
            //    foreach (var reqMerchant in merchantsList)
            //    {
            //        dto.Merchant merchantAdded = await ServiceHelper.Instance.MerchantService.Add(reqMerchant);
            //        merchants.Add(merchantAdded);
            //    }

            //    //List<dto.Merchant> merchantsNew = await ServiceHelper.Instance.MerchantService.GetAll(); //Mapper Issue

            //    //2. Add Status List for Type Import and Offer

            //    List<dto.Status> statusOffer = Common.Common.BuildOfferStatusModel();
            //    await ServiceHelper.Instance.StatusService.AddRange(statusOffer);

            //    List<dto.Status> statusImport = Common.Common.BuildImportStatusModel();
            //    await ServiceHelper.Instance.StatusService.AddRange(statusImport);

            //    List<dto.Status> statuses = await ServiceHelper.Instance.StatusService.GetAll(); //Manual mapping

            //    //3. Add OfferTypes
            //    List<dto.OfferType> offerTypes = new List<dto.OfferType>();
            //    List<dto.OfferType> offerTypesReq = Common.Common.BuildOfferTypeModelList();
            //    foreach (var offerType in offerTypesReq)
            //    {
            //        dto.OfferType offerTypeAdded = await ServiceHelper.Instance.OfferTypeService.Add(offerType);
            //        offerTypes.Add(offerTypeAdded);
            //    }

            //    //List<dto.OfferType> offerTypes = await ServiceHelper.Instance.OfferTypeService.GetAll(); //Mapper Issue

            //    //4. Add Affiliate (AWIN)
            //    dto.Affiliate affiliateReq = Common.Common.BuildAffiliateModel();
            //    dto.Affiliate affiliate = await ServiceHelper.Instance.AffiliateService.Add(affiliateReq);

            //    //5. Add AffiliateFileMapping
            //    dto.AffiliateFileMapping affiliateFileMappingReq = Common.Common.BuildAffiliateFileMappingModel();
            //    affiliateFileMappingReq.AffiliateId = affiliate.Id;
            //    dto.AffiliateFileMapping affiliateFileMapping =
            //        await ServiceHelper.Instance.AffiliateFileMappingService.Add(affiliateFileMappingReq);

            //    //6. Add AffiliateFile
            //    List<dto.AffiliateFile> affiliateFiles = new List<dto.AffiliateFile>();
            //    List<dto.AffiliateFile> affiliateFilesReq = Common.Common.BuildAffiliateFileModelList();

            //    foreach (var affiliateFile in affiliateFilesReq)
            //    {
            //        affiliateFile.AffiliateId = affiliate.Id;
            //        affiliateFile.AffiliateFileMappingId = affiliateFileMapping.Id;
            //        dto.AffiliateFile affiliateFileAdded = await ServiceHelper.Instance.AffiliateFileService.Add(affiliateFile);
            //        affiliateFiles.Add(affiliateFileAdded);
            //    }

            //    // List<dto.AffiliateFile> affiliateFiles = await ServiceHelper.Instance.AffiliateFileService.GetAll(); //Mapper Issue

            //    //7. Add Exclusive.Offer (50 records)
            //    List<dto.Offer> offers = new List<dto.Offer>();
            //    dto.Offer exclusiveOfferReq = Common.Common.BuildOfferModel();
            //    int offerNewStatusId =
            //        statuses.FirstOrDefault(x => x.IsActive && x.Type == StatusType.Offer && x.Name == Status.Active).Id;
            //    for (int i = 0; i < 50; i++)
            //    {
            //        Random generator = new Random();
            //        int merchantId = merchants.Skip(generator.Next(merchants.Count)).FirstOrDefault().Id;
            //        int offerTypeId = offerTypes.Skip(generator.Next(offerTypes.Count)).FirstOrDefault().Id;
            //        string promotionId = generator.Next(0, 999999).ToString("D5");
            //        exclusiveOfferReq.Datecreated = DateTime.Now;
            //        exclusiveOfferReq.MerchantId = merchantId;
            //        exclusiveOfferReq.OfferTypeId = offerTypeId;
            //        exclusiveOfferReq.AffiliateReference = promotionId;
            //        exclusiveOfferReq.AffiliateId = affiliate.Id;
            //        exclusiveOfferReq.StatusId = offerNewStatusId;
            //        exclusiveOfferReq.SearchRanking = 5;

            //        exclusiveOfferReq.OfferCountries = new List<dto.OfferCountry>
            //        {
            //            new dto.OfferCountry {CountryCode = "GB"},
            //            new dto.OfferCountry {CountryCode = "SC"}
            //        };

            //        dto.Offer offerAdded = await ServiceHelper.Instance.OfferService.Add(exclusiveOfferReq);
            //        offers.Add(offerAdded);
            //    }

            //    // List<dto.Offer> offerAdded = await ServiceHelper.Instance.OfferService.GetAll(); //Mapper Issue

            //    //<-- Checking for new insert to Exclusive.Offer case (different affiliateReference)  -->
            //    //8. Add ST.OfferImportFile
            //    dto.StagingModels.OfferImportFile offerImportFile = Common.Common.BuildOfferImportFile();

            //    offerImportFile.ImportStatus = statuses
            //        .FirstOrDefault(x => x.IsActive && x.Type == StatusType.Import && x.Name == Status.Processing).Id;
            //    offerImportFile.AffiliateFileId = affiliateFiles.FirstOrDefault(x => x.FileName == "PromoCodes").Id;
            //    dto.StagingModels.OfferImportFile offerImportFileAdded =
            //        await ServiceHelper.Instance.OfferImportFileService.Add(offerImportFile);

            //    //9. Add ST.Offer (1 record for Imported case test)
            //    dto.StagingModels.Offer stagingOfferReq = Common.Common.BuildStagingOfferModel();

            //    //9.1 Checking for new insert to Exclusive.Offer case (different affiliateReference)
            //    Random newGenerator = new Random();
            //    string newPromotionId = newGenerator.Next(0, 999999).ToString("D5");
            //    int newMerchantId = merchants.Skip(newGenerator.Next(merchants.Count)).FirstOrDefault().Id;
            //    int newOfferTypeId = offerTypes.Skip(newGenerator.Next(offerTypes.Count)).FirstOrDefault().Id;
            //    stagingOfferReq.Datecreated = DateTime.Now;
            //    stagingOfferReq.MerchantId = newMerchantId;
            //    stagingOfferReq.OfferTypeId = newOfferTypeId;
            //    stagingOfferReq.AffiliateReference = newPromotionId;
            //    stagingOfferReq.AffiliateId = affiliate.Id;
            //    stagingOfferReq.StatusId = offerNewStatusId;
            //    stagingOfferReq.SearchRanking = 5;
            //    stagingOfferReq.OfferCountries = new List<dto.OfferCountry>
            //    {
            //        new dto.OfferCountry {CountryCode = "GB"},
            //        new dto.OfferCountry {CountryCode = "SC"}
            //    };

            //    dto.StagingModels.Offer stagingOfferAdded =
            //        await ServiceHelper.Instance.StagingOfferService.Add(stagingOfferReq);

            //    Assert.IsNotNull(offerImportFile, "Initialization of Offer Import File is null");
            //    Assert.IsNotNull(stagingOfferAdded, "Staging Offer not found");
            //    Assert.IsNotNull(offerImportFileAdded, "Offer import file record not found");
            //    //call StoreProcedure for new exclusive offer test case

            //    await ServiceHelper.Instance.OfferService.ExecuteSPMappingSTOfferToOffer(affiliate.Id,
            //        offerImportFileAdded.Id, 1);

            //    //List<dto.Offer> newofferList = await ServiceHelper.Instance.OfferService.GetAll(); //Mapper Issue
            //    var newOfferList = await ServiceHelper.Instance.OfferService.GetAll(false, false, false);
            //    Assert.IsTrue(newOfferList.Count() > offers.Count(), "Exclusive Offer not inserted");

            //    dto.StagingModels.OfferImportFile offerImportFileImported =
            //        await ServiceHelper.Instance.OfferImportFileService.GetById(offerImportFileAdded.Id);

            //    Assert.IsNotNull(offerImportFileImported, "OfferImportFile not found in import case.");
            //    Assert.IsTrue(offerImportFileImported.Imported == 1, "OfferImportFile failed in imported case.");

            //    //<-- End Checking for new insert to Exclusive.Offer case (different affiliateReference) -->

            //    //<-- Start Checking for update case -->
            //    //Add offerImportFile
            //    dto.StagingModels.OfferImportFile offerImportFileReq = Common.Common.BuildOfferImportFile();

            //    offerImportFileReq.ImportStatus = statuses
            //        .FirstOrDefault(x => x.IsActive && x.Type == StatusType.Import && x.Name == Status.Processing).Id;
            //    offerImportFileReq.AffiliateFileId = affiliateFiles.FirstOrDefault(x => x.FileName == "Sales").Id;
            //    dto.StagingModels.OfferImportFile offerFileUpdate =
            //        await ServiceHelper.Instance.OfferImportFileService.Add(offerImportFileReq);

            //    Assert.IsNotNull(offerImportFileReq, "Initialization of Offer Import File is null");
            //    Assert.IsNotNull(offerFileUpdate, "Offer import file record not found");

            //    //9.2 Checking for update case
            //    Assert.IsNotNull(offers, "Exclusive Offer not found");
            //    dto.Offer exclusiveOfferFilter = offers.Skip(newGenerator.Next(offers.Count)).FirstOrDefault();
            //    Assert.IsNotNull(exclusiveOfferFilter, "Exclusive Offer filter result not found");

            //    //Add Staging.Offer (1 record for Update case test)
            //    dto.StagingModels.Offer reqStagingOfferModel = new dto.StagingModels.Offer
            //    {
            //        MerchantId = exclusiveOfferFilter.MerchantId,
            //        AffiliateId = (int)exclusiveOfferFilter.AffiliateId,
            //        OfferTypeId = exclusiveOfferFilter.OfferTypeId,
            //        StatusId = exclusiveOfferFilter.StatusId,
            //        ValidFrom = exclusiveOfferFilter.ValidFrom,
            //        ValidTo = exclusiveOfferFilter.ValidTo,
            //        Validindefinately = exclusiveOfferFilter.Validindefinately,
            //        ShortDescription = exclusiveOfferFilter.ShortDescription + "Update Case Test",
            //        LongDescription = exclusiveOfferFilter.LongDescription + "Update Case Test",
            //        Instructions = exclusiveOfferFilter.Instructions,
            //        Terms = exclusiveOfferFilter.Terms,
            //        Exclusions = exclusiveOfferFilter.Exclusions,
            //        LinkUrl = exclusiveOfferFilter.LinkUrl,
            //        OfferCode = exclusiveOfferFilter.OfferCode,
            //        Reoccuring = exclusiveOfferFilter.Reoccuring,
            //        SearchRanking = exclusiveOfferFilter.SearchRanking,
            //        Datecreated = exclusiveOfferFilter.Datecreated,
            //        Headline = exclusiveOfferFilter.Headline,
            //        AffiliateReference = exclusiveOfferFilter.AffiliateReference
            //    };
            //    if (exclusiveOfferFilter.OfferCountries.Any())
            //    {
            //        List<dto.OfferCountry> stOfferCountries =
            //            new List<dto.OfferCountry>();
            //        foreach (var offerCountry in exclusiveOfferFilter.OfferCountries)
            //        {
            //            stOfferCountries.Add(new dto.OfferCountry
            //            { CountryCode = offerCountry.CountryCode, OfferId = offerCountry.OfferId });
            //        }

            //        reqStagingOfferModel.OfferCountries = stOfferCountries;
            //    }

            //    dto.StagingModels.Offer newstagingOfferAdded =
            //        await ServiceHelper.Instance.StagingOfferService.Add(reqStagingOfferModel);

            //    Assert.IsNotNull(reqStagingOfferModel, "Initialization of Staging Offer is null");
            //    Assert.IsNotNull(newstagingOfferAdded, "Staging Offer not found");

            //    await ServiceHelper.Instance.OfferService.ExecuteSPMappingSTOfferToOffer(affiliate.Id,
            //        offerFileUpdate.Id, 1);

            //    dto.Offer offerUpdate = await ServiceHelper.Instance.OfferService.Get(exclusiveOfferFilter.Id);

            //    Assert.IsNotNull(offerUpdate, "Exclusive Offer update failed.");
            //    Assert.AreNotEqual(offerUpdate.ShortDescription, exclusiveOfferFilter.ShortDescription, "Exclusive Offer no updation found for ShortDescription");
            //    Assert.AreNotEqual(offerUpdate.LongDescription, exclusiveOfferFilter.LongDescription, "Exclusive Offer no updation found for LongDescription");

            //    dto.StagingModels.OfferImportFile offerImportFileUpdate =
            //        await ServiceHelper.Instance.OfferImportFileService.GetById(offerFileUpdate.Id);

            //    Assert.IsNotNull(offerImportFileUpdate, "OfferImportFile not found in update case.");
            //    Assert.IsTrue(offerImportFileUpdate.Updates == 1, "OfferImportFile failed in update case.");

            //    //<-- End Checking for update case -->

            //    //<-- Start Checking for duplicate case -->
            //    dto.StagingModels.OfferImportFile offerImportFileNewReq = Common.Common.BuildOfferImportFile();

            //    offerImportFileNewReq.ImportStatus = statuses
            //        .FirstOrDefault(x => x.IsActive && x.Type == StatusType.Import && x.Name == Status.Processing).Id;
            //    offerImportFileNewReq.AffiliateFileId = affiliateFiles.FirstOrDefault(x => x.FileName == "Sales").Id;
            //    dto.StagingModels.OfferImportFile offerImportFileDuplicateAdded =
            //        await ServiceHelper.Instance.OfferImportFileService.Add(offerImportFileNewReq);

            //    Assert.IsNotNull(offerImportFileNewReq, "Initialization of Offer Import File is null");
            //    Assert.IsNotNull(offerImportFileDuplicateAdded, "Offer import file record not found");

            //    //9.3 Checking for duplicate records case (same affiliateReference)
            //    Assert.IsNotNull(offers, "Exclusive Offer not found");
            //    dto.Offer exclusiveOfferFilterNew = offers.Skip(newGenerator.Next(offers.Count)).FirstOrDefault();
            //    Assert.IsNotNull(exclusiveOfferFilterNew, "Exclusive Offer filter result not found");

            //    //Add Staging.Offer (1 record for duplicate case test)
            //    dto.StagingModels.Offer reqStagingOfferModelNew = new dto.StagingModels.Offer
            //    {
            //        MerchantId = exclusiveOfferFilterNew.MerchantId,
            //        AffiliateId = (int)exclusiveOfferFilterNew.AffiliateId,
            //        OfferTypeId = exclusiveOfferFilterNew.OfferTypeId,
            //        StatusId = exclusiveOfferFilterNew.StatusId,
            //        ValidFrom = exclusiveOfferFilterNew.ValidFrom,
            //        ValidTo = exclusiveOfferFilterNew.ValidTo,
            //        Validindefinately = exclusiveOfferFilterNew.Validindefinately,
            //        ShortDescription = exclusiveOfferFilterNew.ShortDescription,
            //        LongDescription = exclusiveOfferFilterNew.LongDescription,
            //        Instructions = exclusiveOfferFilterNew.Instructions,
            //        Terms = exclusiveOfferFilterNew.Terms,
            //        Exclusions = exclusiveOfferFilterNew.Exclusions,
            //        LinkUrl = exclusiveOfferFilterNew.LinkUrl,
            //        OfferCode = exclusiveOfferFilterNew.OfferCode,
            //        Reoccuring = exclusiveOfferFilterNew.Reoccuring,
            //        SearchRanking = exclusiveOfferFilterNew.SearchRanking,
            //        Datecreated = exclusiveOfferFilterNew.Datecreated,
            //        Headline = exclusiveOfferFilterNew.Headline,
            //        AffiliateReference = exclusiveOfferFilterNew.AffiliateReference
            //    };
            //    //add staging.Offer without countryCode

            //    dto.StagingModels.Offer newstagingOffer2Added =
            //        await ServiceHelper.Instance.StagingOfferService.Add(reqStagingOfferModelNew);

            //    Assert.IsNotNull(reqStagingOfferModelNew, "Initialization of Staging Offer is null");
            //    Assert.IsNotNull(newstagingOffer2Added, "Staging Offer not found");

            //    await ServiceHelper.Instance.OfferService.ExecuteSPMappingSTOfferToOffer(affiliate.Id,
            //        offerImportFileDuplicateAdded.Id, 1);

            //    //Assert case for duplication 1. Get Staging.OfferImportFile and check duplicates count 1
            //    dto.StagingModels.OfferImportFile offerImportFileDuplicate =
            //        await ServiceHelper.Instance.OfferImportFileService.GetById(offerImportFileDuplicateAdded.Id);

            //    Assert.IsNotNull(offerImportFileDuplicate, "OfferImportFile not found in duplicate case.");
            //    Assert.IsTrue(offerImportFileDuplicate.Duplicates == 1, "OfferImportFile failed in duplicate case.");
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            await Task.CompletedTask;

        }

    }
}
