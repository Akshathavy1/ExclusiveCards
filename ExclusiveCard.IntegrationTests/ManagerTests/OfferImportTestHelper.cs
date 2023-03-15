using ExclusiveCard.Data.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using st = ExclusiveCard.Data.StagingModels;
using dto = ExclusiveCard.Services.Models.DTOs;
using AutoMapper;

namespace ExclusiveCard.IntegrationTests.ManagerTests
{
    internal class OfferImportTestHelper
    {
        const int AWIN_AFFILIATE_ID = 1;
        const int AWIN_AFFILIATE_FILETYPE_ID = 1;
  
        /// <summary>
        /// Creates a csv file in AWIN format,  with some authentic offers. 
        /// </summary>
        /// <returns>The path and filename that has been created</returns>
        public static string CreateAWINTestFile()
        {
            string line1 = @"""Promotion ID"" ,""Advertiser"",""Advertiser ID"",""Type"",""Code"",""Description"",""Starts"",""Ends"",""Categories"",""Regions"",""Terms"",""Deeplink Tracking"",""Deeplink"",""Commission Groups"",""Commission"",""Exclusive"",""Date Added"",""Title""";
            string line2 = @"""537731"",""Boots.com"",""2041"",""Promotions Only"","""",""Collect £10 worth of points for every £60 you spend online"",""07/02/2019 13:51:00"",""08/02/2019 23:59:00"",""Cosmetics"",""United Kingdom"",""Spend £60, for £10 worth of points"",""http://www.awin1.com/cread.php?awinaffid=245049&awinmid=2041&p=https%3A%2F%2Fwww.boots.com%2F"",""https://www.boots.com/"","""","""",""No"",""07/02/2019 13:51:57"",""Collect £10 worth of points for every £60 you spend online""";
            string line4 = @"""537711"",""Gemondo Jewellery"",""2421"",""Vouchers Only"",""loverings"",""Take advantage of 20% off all rings at Gemondo when using the discount code LOVERINGS"",""07/02/2019 12:54:00"",""10/02/2019 23:59:00"",""Women's Accessories,Anniversary Gifts,Birthday Gifts,Women's Jewellery"",""Australia,Ireland,United Kingdom,United States of America"",""Offer must end on the 10th February 2019 at 23.59pm GMT. Only valid on selected rings available at www.gemondo.com, www.gemondo.ie, us.gemondo.com and www.gemondo.com.au. Cannot be used in conjunction with any other offers. See full terms and conditions at Gemondo.com and the Gemondo AWIN profile"",""http://www.awin1.com/cread.php?awinaffid=245049&awinmid=2421&p=https%3A%2F%2Fwww.gemondo.com%2Frings"",""https://www.gemondo.com/rings"","""","""",""No"",""07/02/2019 12:54:59"",""20% off Rings at Gemondo Jewellery""";
            string line3 = @"""537665"",""edX (Global)"",""6798"",""Promotions Only"","""",""Explore and understand your own theories of learning and leadership."",""07/02/2019 12:11:00"",""02/03/2019 12:07:00"",""Adult Books,Adult Gifts,Games, Puzzles & Learning"",""United Kingdom,United States of America"",""See website"",""http://www.awin1.com/cread.php?awinaffid=245049&awinmid=6798&p=https%3A%2F%2Fwww.edx.org%2Fcourse%2Fleaders-of-learning-1"",""https://www.edx.org/course/leaders-of-learning-1"","""","""",""No"",""07/02/2019 12:11:42"",""Leaders of Learning. Free Course""";
            string line5 = @"""537663"",""Temple Spa"",""9004"",""Promotions Only"","""",""Valentine's Day Skincare, Spa & Beauty Gift Ideas Including Great Offers & Discounts"",""07/02/2019 11:58:00"",""12/03/2019 23:59:00"",""Other Experiences,Pampering,Anniversary Gifts,Birthday Gifts,Other Occasions,Valentine's Day,Wedding Gifts,Cosmetics,Skincare"",""Ireland,United Kingdom"",""FREE UK DELIVERY ON ORDERS OVER £50. Individual collections, offers and discounts have their own terms and conditions please check website for details."",""http://www.awin1.com/cread.php?awinaffid=245049&awinmid=9004&p=https%3A%2F%2Fwww.templespa.com%2Fshop%2Fgifts%2Fproduct-type%2Fromance"",""https://www.templespa.com/shop/gifts/product-type/romance"","""","""",""No"",""07/02/2019 11:58:12"",""Valentine's Day Skincare, Spa & Beauty Gift Ideas Including Great Offers & Discounts""";

            string fileName = "ImportTest" + DateTime.Now.Ticks.ToString() + ".csv";
            using (StreamWriter stream = File.CreateText(fileName))
            {
                stream.WriteLine(line1);
                stream.WriteLine(line2);
                stream.WriteLine(line4);
                stream.WriteLine(line3);
                stream.WriteLine(line5);
            }

            return fileName;
        }

        /// <summary>
        /// creates a record that mimics AWIN data held in the staging table, after it has been imported from the CSV file
        /// </summary>
        /// <returns>A list of AWIN data held in  the AWIN data type </returns>
        public static List<dto.StagingModels.OfferImportAwin> CreateAWINStagingData()
        {
            var dataList = new List<dto.StagingModels.OfferImportAwin>();

            var dataItem = new dto.StagingModels.OfferImportAwin()
            {
                Advertiser = "Boots.com",
                AdvertiserId = 2041,
                Categories = "Cosmetics",
                Code = "",
                Description = "Collect £10 worth of points for every £60 you spend online",
                DateAdded = DateTime.Parse("07/02/2019 13:51:57"),
                Deeplink = "https://www.boots.com/",
                DeeplinkTracking = "http://www.awin1.com/cread.php?awinaffid=245049&awinmid=2041&p=https%3A%2F%2Fwww.boots.com%2F",
                Ends = DateTime.Parse("08/02/2019 23:59:00"),
                PromotionId = "537731",
                Regions = "United Kingdom",
                Starts = DateTime.Parse("07/02/2019 13:51:00"),
                Terms = "Spend £60, for £10 worth of points",
                Title = "Collect £10 worth of points for every £60 you spend online",
                Type = "Promotions Only"
            };

            dataList.Add(dataItem);

            return dataList;
        }


        public static string CreateAWINTestFile_MissingField()
        {
            // Create file with missing Advertiser field
            string line1 = @"""Promotion ID"" ,""Advertiser ID"",""Type"",""Code"",""Description"",""Starts"",""Ends"",""Categories"",""Regions"",""Terms"",""Deeplink Tracking"",""Deeplink"",""Commission Groups"",""Commission"",""Exclusive"",""Date Added"",""Title""";
            string line2 = @"""537731"",""2041"",""Promotions Only"","""",""Collect £10 worth of points for every £60 you spend online"",""07/02/2019 13:51:00"",""08/02/2019 23:59:00"",""Cosmetics"",""United Kingdom"",""Spend £60, for £10 worth of points"",""http://www.awin1.com/cread.php?awinaffid=245049&awinmid=2041&p=https%3A%2F%2Fwww.boots.com%2F"",""https://www.boots.com/"","""","""",""No"",""07/02/2019 13:51:57"",""Collect £10 worth of points for every £60 you spend online""";

            string fileName = "ImportTest" + DateTime.Now.Ticks.ToString() + ".csv";
            using (StreamWriter stream = File.CreateText(fileName))
            {
                stream.WriteLine(line1);
                stream.WriteLine(line2);
            }

            return fileName;
        }

        public static string CreateAWINTestFile_InvalidData()
        {
            string line1 = @"""Promotion ID"" ,""Advertiser"",""Advertiser ID"",""Type"",""Code"",""Description"",""Starts"",""Ends"",""Categories"",""Regions"",""Terms"",""Deeplink Tracking"",""Deeplink"",""Commission Groups"",""Commission"",""Exclusive"",""Date Added"",""Title""";
            string line2 = @"""537731"",""Boots.com"",""2041"",""Promotions Only"","""",""Collect £10 worth of points for every £60 you spend online"",""07/02/2019 13:51:00"",""08/02/2019 23:59:00"",""Cosmetics"",""United Kingdom"",""Spend £60, for £10 worth of points"",""http://www.awin1.com/cread.php?awinaffid=245049&awinmid=2041&p=https%3A%2F%2Fwww.boots.com%2F"",""https://www.boots.com/"","""","""",""No"",""07/02/2019 13:51:57"",""Collect £10 worth of points for every £60 you spend online""";
            string line3 = @"""537665"",""edX (Global)"",""6798"",""Promotions Only"","""",""Explore and understand your own theories of learning and leadership."",""AnIvalidDate"",""AnotherInvalidDate"",""Adult Books,Adult Gifts,Games, Puzzles & Learning"",""United Kingdom,United States of America"",""See website"",""http://www.awin1.com/cread.php?awinaffid=245049&awinmid=6798&p=https%3A%2F%2Fwww.edx.org%2Fcourse%2Fleaders-of-learning-1"",""https://www.edx.org/course/leaders-of-learning-1"","""","""",""No"",""07/02/2019 12:11:42"",""Leaders of Learning. Free Course""";
            string line4 = @"""537663"",""Temple Spa"",""9004"",""Promotions Only"","""",""Valentine's Day Skincare, Spa & Beauty Gift Ideas Including Great Offers & Discounts"",""07/02/2019 11:58:00"",""12/03/2019 23:59:00"",""Other Experiences,Pampering,Anniversary Gifts,Birthday Gifts,Other Occasions,Valentine's Day,Wedding Gifts,Cosmetics,Skincare"",""Ireland,United Kingdom"",""FREE UK DELIVERY ON ORDERS OVER £50. Individual collections, offers and discounts have their own terms and conditions please check website for details."",""http://www.awin1.com/cread.php?awinaffid=245049&awinmid=9004&p=https%3A%2F%2Fwww.templespa.com%2Fshop%2Fgifts%2Fproduct-type%2Fromance"",""https://www.templespa.com/shop/gifts/product-type/romance"","""","""",""No"",""07/02/2019 11:58:12"",""Valentine's Day Skincare, Spa & Beauty Gift Ideas Including Great Offers & Discounts""";

            string fileName = "ImportTest" + DateTime.Now.Ticks.ToString() + ".csv";
            using (StreamWriter stream = File.CreateText(fileName))
            {
                stream.WriteLine(line1);
                stream.WriteLine(line2);
                stream.WriteLine(line3);
                stream.WriteLine(line4);
            }

            return fileName;
        }

        public static  dto.StagingModels.OfferImportFile CreateDtoOfferImportFile(Enums.Import importStatus, string fileName, string errorFilePath = null)
        {
            return new dto.StagingModels.OfferImportFile()
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

        public static dto.AffiliateFile CreateDtoAffiliateFile()
        {
            var dtoAffiliateFile = new dto.AffiliateFile()
            {
                Id = AWIN_AFFILIATE_FILETYPE_ID,
                AffiliateId = AWIN_AFFILIATE_ID,
                Description = "Test AWIN Sales and Promotions File",
                StagingTable = "OfferImportAWIN",
                FileName = "AWIN TEST FILE"
            };

            return dtoAffiliateFile;
        }

        //public static int SaveDtoOfferImportFile(ExclusiveCard.Services.Models.DTOs.StagingModels.OfferImportFile offerFileDTO)
        //{

        //    IRepository<ExclusiveCard.Data.StagingModels.OfferImportFile> offerImportFileRepo = Configuration.ServiceProvider.GetService<IRepository<ExclusiveCard.Data.StagingModels.OfferImportFile>>();
        //    var mapper = Configuration.ServiceProvider.GetService<IMapper>();
        //    var dbOfferFile = mapper.Map<ExclusiveCard.Data.StagingModels.OfferImportFile>(offerFileDTO);
        //    offerImportFileRepo.Create(dbOfferFile);
        //    offerImportFileRepo.SaveChanges();
        //    return dbOfferFile.Id;
        //}
    }
}
