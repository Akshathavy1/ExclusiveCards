using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using data = ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.IntegrationTests.Common
{
    public static class Common
    {
        public static ExclusiveUser BuildUserModel()
        {
            return new ExclusiveUser
            {
                UserName = "test1@email.com",
                Email = "test1@email.com"
            };
        }

        public static dto.Customer BuildCustomerModel()
        {
            return new dto.Customer
            {
                Title = "Mr",
                Forename = "Customer",
                Surname = "Test",
                DateOfBirth = new DateTime(1990, 01, 23, 0, 0, 0),
                IsActive = true,
                IsDeleted = false,
                DateAdded = DateTime.UtcNow,
                MarketingNewsLetter = false,
                MarketingThirdParty = false,
                NINumber = "4154785295",
                ContactDetail = new dto.ContactDetail
                {
                    Address1 = "Address 1",
                    Address2 = "Address 2",
                    Address3 = "Address 3",
                    Town = "Town",
                    District = "District",
                    PostCode = "123456",
                    CountryCode = "IN",
                    Latitude = "",
                    Longitude = "",
                    LandlinePhone = "91802370152",
                    MobilePhone = "91805049948",
                    EmailAddress = "test1@email.com",
                    IsDeleted = false
                }
            };
        }

        public static dto.MembershipPlanType BuildMembershipPlanTypeModel()
        {
            return new dto.MembershipPlanType
            {
                Description = "Personal",
                IsActive = true
            };
        }

        public static dto.MembershipPlan BuildMembershipPlanModel()
        {
            return new dto.MembershipPlan
            {
                MembershipPlanTypeId = 1,
                NumberOfCards = 10,
                ValidFrom = DateTime.UtcNow.AddMonths(-2),
                ValidTo = DateTime.UtcNow.AddYears(1),
                CustomerCardPrice = 19.99m,
                PartnerCardPrice = 0m,
                CustomerCashbackPercentage = 100,
                DeductionPercentage = 0,
                Description = "Personal-V2",
                CurrencyCode = "GBP",
                Duration = 365,
                IsActive = true,
                IsDeleted = false
            };
        }

        public static dto.MembershipRegistrationCode BuildMembershipRegistrationCodeModel()
        {
            return new dto.MembershipRegistrationCode
            {
                RegistartionCode = "ExFree19",
                ValidFrom = DateTime.UtcNow.AddMonths(-3),
                ValidTo = DateTime.UtcNow.AddYears(1),
                NumberOfCards = 10,
                EmailHostName = null,
                IsActive = true,
                IsDeleted = false
            };
        }

        public static dto.MembershipCard BuildMembershipCardModel()
        {
            return new dto.MembershipCard
            {
                StatusId = 20,
                PhysicalCardStatusId = 11,
                MembershipPlanId = 2,
                CardNumber = "EX0000001GB",
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddYears(1),
                DateIssued = DateTime.UtcNow,
                AgentCode = null,
                PhysicalCardRequested = false,
                CustomerPaymentProviderId = null,
                IsActive = true,
                IsDeleted = false
            };
        }

        public static dto.Merchant BuildMerchantModel()
        {
            return new dto.Merchant
            {
                Name = "MerchantName",
                ContactName = "Name",
                ShortDescription = "text",
                LongDescription = "text",
                Terms = "text",
                WebsiteUrl = "www.text.com",
                IsDeleted = false,
                ContactDetail = new dto.ContactDetail
                {
                    Address1 = "Address 1",
                    Address2 = "Address 2",


                    Address3 = "Address 3",
                    Town = "Town",
                    District = "District",
                    PostCode = "123456",
                    CountryCode = "IN",
                    Latitude = "",
                    Longitude = "",
                    LandlinePhone = "91802370152",
                    MobilePhone = "91805049948",
                    EmailAddress = "test1@email.com",
                    IsDeleted = false
                }
            };
        }

        public static dto.Offer BuildOfferModel()
        {
            return new dto.Offer
            {
                ValidFrom = new DateTime(1990, 01, 23, 0, 0, 0),
                ValidTo = new DateTime(1990, 01, 23, 0, 0, 0),
                Validindefinately = true,
                ShortDescription = "test",
                LongDescription = "test",
                Instructions = "test",
                Terms = "test",
                Exclusions = "test",
                LinkUrl = "www.test.com",
                OfferCode = "1234",
            };
        }

        public static List<dto.Status> BuildOfferStatusModel()
        {
            List<dto.Status> iStatus = new List<dto.Status>
            {
                new dto.Status {Name = "Active", Type = "Offer", IsActive = true},
                new dto.Status {Name = "Inactive", Type = "Offer", IsActive = true},
                new dto.Status {Name = "Needs Review", Type = "Offer", IsActive = true},
                new dto.Status {Name = "Disabled", Type = "Offer", IsActive = true},
                new dto.Status {Name = "To Be Deleted", Type = "Offer", IsActive = true},
                new dto.Status {Name = "Deleted", Type = "Offer", IsActive = true},
                new dto.Status {Name = "Qued For Deletion", Type = "Offer", IsActive = true}
            };

            // await ServiceHelper.Instance.StatusService.DeleteRangeAsync(); (FK reference issue)

            return iStatus;
        }

        public static List<dto.Merchant> BuildMerchantsModeList()
        {
            return new List<dto.Merchant>
            {
                new dto.Merchant {Name = "Virgin Experience Days", IsDeleted = false},

                new dto.Merchant {Name = "Breens Solicitors", IsDeleted = false},
                new dto.Merchant {Name = "Chill Factore", IsDeleted = false},
                new dto.Merchant {Name = "Woodbank Office Solutions", IsDeleted = false},
                new dto.Merchant {Name = "Land Rover Experience", IsDeleted = false},
                new dto.Merchant {Name = "Banks Printers", IsDeleted = false},
                new dto.Merchant {Name = "Barringtons Wedding Cars", IsDeleted = false},
                new dto.Merchant {Name = "Beef And Pudding", IsDeleted = false},
                new dto.Merchant {Name = "Bella & Mr Tom", IsDeleted = false},
                new dto.Merchant {Name = "Bierkeller", IsDeleted = false},
                new dto.Merchant {Name = "Browns Brasserie & Bar", IsDeleted = false},
                new dto.Merchant {Name = "Casa Italia", IsDeleted = false},
                new dto.Merchant {Name = "Casa Matta", IsDeleted = false},
                new dto.Merchant {Name = "Cheshire Oaks Designer Outlet", IsDeleted = false},
                new dto.Merchant {Name = "Circo Liverpool", IsDeleted = false},
                new dto.Merchant {Name = "The Dutch Flower Shop", IsDeleted = false},
                new dto.Merchant {Name = "Eat Elite", IsDeleted = false},
                new dto.Merchant {Name = "Eighty Eight Bar", IsDeleted = false},
                new dto.Merchant {Name = "Enterprise Car Hire", IsDeleted = false},
                new dto.Merchant {Name = "Fabuluxe", IsDeleted = false},
                new dto.Merchant {Name = "Fuego's Mexican Restaurant", IsDeleted = false},
                new dto.Merchant {Name = "Jason James Salon", IsDeleted = false},
                new dto.Merchant {Name = "Kitten Beachwear Limited", IsDeleted = false},
                new dto.Merchant {Name = "Little Black Dress", IsDeleted = false},
                new dto.Merchant {Name = "The Office Bar & Restaurant", IsDeleted = false},
                new dto.Merchant {Name = "Ollie & Darsh", IsDeleted = false},
                new dto.Merchant {Name = "One Up Repairs", IsDeleted = false},
                new dto.Merchant {Name = "Portland Hall Spa", IsDeleted = false},
                new dto.Merchant {Name = "Remedy Bar & Cafe", IsDeleted = false},
                new dto.Merchant {Name = "Reputation Menswear", IsDeleted = false}
            };
        }

        public static dto.StagingModels.OfferImportFile BuildOfferImportFile()
        {
            return new dto.StagingModels.OfferImportFile
            {
                DateImported = DateTime.Now,
                FilePath = "Testing",
                ErrorFilePath = null,
                CountryCode = "GB"
            };
        }

        //public static dto.StagingModels.Offer BuildStagingOfferModel()
        //{
        //    return new dto.StagingModels.Offer
        //    {
        //        ValidFrom = new DateTime(1990, 01, 23, 0, 0, 0),
        //        ValidTo = new DateTime(1990, 01, 23, 0, 0, 0),
        //        Validindefinately = true,
        //        ShortDescription = "test",
        //        LongDescription = "test",
        //        Instructions = "test",
        //        Terms = "test",
        //        Exclusions = "test",
        //        LinkUrl = "www.test.com",
        //        OfferCode = "1234",
        //    };
        //}

        public static dto.AffiliateFileMapping BuildAffiliateFileMappingModel()
        {
            return new dto.AffiliateFileMapping { Description = "FileMapping" };
        }

        public static List<dto.AffiliateFile> BuildAffiliateFileModelList()
        {
            return new List<dto.AffiliateFile>
            {
                new dto.AffiliateFile {FileName = "Sales", Description = "Sales", StagingTable = "OfferImportAwin"},
                new dto.AffiliateFile
                    {FileName = "PromoCodes", Description = "Voucher Code", StagingTable = "OfferImportAwin"}
            };
        }

        public static dto.Affiliate BuildAffiliateModel()
        {
            return new dto.Affiliate
            {
                Name = "AWIN"
            };
        }

        public static List<dto.OfferType> BuildOfferTypeModelList()
        {
            return new List<dto.OfferType>
            {
                new dto.OfferType {Description = "Cashback", IsActive = true, SearchRanking = 1},
                new dto.OfferType {Description = "Voucher Code", IsActive = true, SearchRanking = 3},
                new dto.OfferType {Description = "High Street", IsActive = true, SearchRanking = 5},
                new dto.OfferType {Description = "Sales", IsActive = true, SearchRanking = 4},
                new dto.OfferType {Description = "Standard Cashback", IsActive = true, SearchRanking = 2},
                new dto.OfferType {Description = "Restaurant", IsActive = true, SearchRanking = 6},
            };
        }

        public static List<dto.Category> BuildCategoryListModel()
        {
            return new List<dto.Category>
            {
                new dto.Category
                    {Name = "Fashion", ParentId = 0, IsActive = true, DisplayOrder = 1, UrlSlug = "fashion"}
            };
        }


        

        public static dto.Customer MapCustomerToReq(dto.Customer cust)
        {
            if (cust == null)
                return null;

            return new dto.Customer
            {
                Id = cust.Id,
                AspNetUserId = cust.AspNetUserId,
                ContactDetailId = cust.ContactDetailId,
                Title = cust.Title,
                Forename = cust.Forename,
                Surname = cust.Surname,
                DateOfBirth = cust.DateOfBirth,
                IsActive = cust.IsActive,
                IsDeleted = cust.IsDeleted,
                DateAdded = cust.DateAdded,
                MarketingNewsLetter = cust.MarketingNewsLetter,
                MarketingThirdParty = cust.MarketingThirdParty
            };
        }

        public static dto.ContactDetail MapContactDetailToReq(dto.ContactDetail det)
        {
            if (det == null)
                return null;

            return new dto.ContactDetail
            {
                Id = det.Id,
                Address1 = det.Address1,
                Address2 = det.Address2,
                Address3 = det.Address3,
                Town = det.Town,
                District = det.District,
                PostCode = det.PostCode,
                CountryCode = det.CountryCode,
                Latitude = det.Latitude,
                Longitude = det.Longitude,
                LandlinePhone = det.LandlinePhone,
                MobilePhone = det.MobilePhone,
                EmailAddress = det.EmailAddress,
                IsDeleted = det.IsDeleted
            };
        }

        public static async Task<List<dto.MembershipCard>> GenerateMembershipcards(int partnerId,
            int reqMembershipcard = 10)
        {
            List<dto.MembershipCard> membershipCards = new List<dto.MembershipCard>();
            //Create Role
            IdentityRole iRole = new IdentityRole { Name = Roles.User.ToString(), NormalizedName = "User" };
            var roleResult = await ServiceHelper.Instance.UserService.CreateRoleAsync(iRole);
            for (int i = 1; i <= reqMembershipcard; i++)
            {
                //Initialize user model
                data.ExclusiveUser user = new data.ExclusiveUser
                {
                    UserName = $"test{i}@email.com",
                    Email = $"test{i}@email.com"
                };
                //Add user
                data.ExclusiveUser respUser = await Customer.CreateUser(user, Roles.User);
                //Create customer Model
                dto.Customer customer = new dto.Customer
                {
                    Title = "Mr",
                    Forename = $"{i}Customer",
                    Surname = $"{(i < 10 ? "Test" + i.ToString() : i.ToString() + "Test")}",
                    DateOfBirth = new DateTime(1990, 01, 23, 0, 0, 0),
                    IsActive = true,
                    IsDeleted = false,
                    DateAdded = DateTime.UtcNow,
                    MarketingNewsLetter = false,
                    MarketingThirdParty = false,
                    NINumber = $"41547852{(i < 10 ? "0" + i.ToString() : i.ToString())}",
                    ContactDetail = new dto.ContactDetail
                    {
                        Address1 = "Address 1",
                        Address2 = "Address 2",
                        Address3 = "Address 3",
                        Town = "Town",
                        District = "District",
                        PostCode = "123456",
                        CountryCode = "IN",
                        Latitude = "",
                        Longitude = "",
                        LandlinePhone = "91802370152",
                        MobilePhone = "91805049948",
                        EmailAddress = user.Email,
                        IsDeleted = false
                    }
                };

                dto.Customer respCustomer = null;

                if (!string.IsNullOrEmpty(respUser.Id))
                {
                    respCustomer = await Customer.CreateCustomer(respUser.Id, customer);
                }

                //Create PartnerReward model
                DateTime current = DateTime.Now;
                string month = current.Month.ToString().Length == 1
                    ? $"0{current.Month.ToString()}"
                    : current.Month.ToString();
                string day = current.Day.ToString().Length == 1 ? $"0{current.Day.ToString()}" : current.Day.ToString();
                //Create PartnerReward
                dto.PartnerRewards reward = new dto.PartnerRewards
                {
                    RewardKey =
                        $"{current.Year}{month}{day}{current.Hour}{current.Minute}{current.Second}{customer?.Forename.Substring(0, 1).ToUpper()}{customer?.Surname.Substring(0, 1).ToUpper()}",
                    PartnerId = partnerId,
                    CreatedDate = current
                };
                dto.PartnerRewards resPartnerRewards =
                    await ServiceHelper.Instance.PartnerRewardService.AddAsync(reward);
                //Create Membership card
                dto.MembershipCard membershipCard = new dto.MembershipCard
                {
                    StatusId = 20,
                    PhysicalCardStatusId = 11,
                    MembershipPlanId = 2,
                    CardNumber = $"EX00000{(i < 10 ? "0" + i.ToString() : i.ToString())}GB",
                    ValidFrom = DateTime.UtcNow,
                    ValidTo = DateTime.UtcNow.AddYears(1),
                    DateIssued = DateTime.UtcNow,
                    AgentCode = null,
                    PhysicalCardRequested = false,
                    CustomerPaymentProviderId = null,
                    IsActive = true,
                    IsDeleted = false,
                    PartnerRewardId = resPartnerRewards.Id
                };
                dto.MembershipCard membershipCardAdded =
                    await Customer.CreatMembershipCard(respCustomer.Id, 4, 1.ToString(), membershipCard);
                membershipCardAdded.PartnerReward = resPartnerRewards;
                membershipCards.Add(membershipCardAdded);
            }

            return membershipCards;
        }

        public static dto.BankDetail BuildBankDetail()
        {
            return new dto.BankDetail
            {
                SortCode = "12-34-56",
                AccountNumber = "030101509356",
                AccountName = "Winston Ernest Peter",
                IsDeleted = false
            };
        }

        public static dto.CustomerBankDetail BuildCustomerBankDetail(int customerId, int bankDetailId)
        {
            return new dto.CustomerBankDetail
            {
                CustomerId = customerId,
                BankDetailsId = bankDetailId,
                MandateAccepted = false,
                DateMandateAccepted = null,
                IsDeleted = false,
                IsActive = true
            };
        }
    }
}
