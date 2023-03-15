using System.Collections.Generic;
using System.Linq;
using AutoMapper;

using dto = ExclusiveCard.Services.Models.DTOs;
using db = ExclusiveCard.Data.Models;
using System;

namespace ExclusiveCard.Services
{
    // TODO:  REWRITE All BESPOKE MAPPING  USING AUTOMAPPER. 
    // Start reducing our dependancies on this:
    // Remove external calls to this whenever you work on related features
    // Set methods to private when there's no more external calls
    // Remove methods that are no longer needed 
    [Obsolete("Learn how to use automapper and stop writing manual mappings")]
    public static class ManualMappings
    {

        //Deleted MapCustomer(db.Customer customer) - the bank deatail/customer looping in this caused crashes!! Bug #1323

        //Set to private - use automapper
        private static dto.ContactDetail MapContactDetail(db.ContactDetail con)
        {
            if (con == null)
                return null;
            dto.ContactDetail dto = new dto.ContactDetail
            {
                Id = con.Id,
                Address1 = con.Address1,
                Address2 = con.Address2,
                Address3 = con.Address3,
                Town = con.Town,
                District = con.District,
                PostCode = con.PostCode,
                CountryCode = con.CountryCode,
                Latitude = con.Latitude,
                Longitude = con.Longitude,
                LandlinePhone = con.LandlinePhone,
                MobilePhone = con.MobilePhone,
                EmailAddress = con.EmailAddress,
                IsDeleted = con.IsDeleted
            };
            return dto;
        }

        public static db.CustomerBankDetail MapCustomerBankDetailReq(dto.CustomerBankDetail detail)
        {
            if (detail == null)
                return null;
            return new db.CustomerBankDetail
            {
                CustomerId = detail.CustomerId,
                BankDetailsId = detail.BankDetailsId,
                MandateAccepted = detail.MandateAccepted,
                DateMandateAccepted = detail.DateMandateAccepted,
                IsActive = detail.IsActive,
                IsDeleted = detail.IsDeleted
            };
        }

        public static List<dto.BankDetail> MapBankDetails(List<db.BankDetail> details)
        {
            if (details == null || details.Count == 0)
                return null;

            List<dto.BankDetail> dtos = new List<dto.BankDetail>();
            dtos.AddRange(details.Select(detail =>
            {
                if (detail != null) return MapBankDetail(detail);
                return null;
            }));
            return dtos;
        }

        public static dto.BankDetail MapBankDetail(db.BankDetail detail)
        {
            if (detail == null)
                return null;

            dto.BankDetail dto = new dto.BankDetail
            {
                Id = detail.Id,
                BankName = detail.BankName,
                ContactDetailId = detail.ContactDetailId,
                SortCode = detail.SortCode,
                AccountNumber = detail.AccountNumber,
                AccountName = detail.AccountName,
                IsDeleted = detail.IsDeleted
            };

            
            return dto;
        }

        public static db.BankDetail MapBankDetailReq(dto.BankDetail detail)
        {
            if (detail == null)
                return null;

            return new db.BankDetail
            {
                Id = detail.Id,
                BankName = detail.BankName,
                ContactDetailId = detail.ContactDetailId,
                SortCode = detail.SortCode,
                AccountNumber = detail.AccountNumber,
                AccountName = detail.AccountName,
                IsDeleted = detail.IsDeleted
            };
        }

        public static List<dto.MembershipCard> MapMembershipCards(ICollection<db.MembershipCard> cards)
        {
            if (cards == null)
                return null;

            List<dto.MembershipCard> dtos = new List<dto.MembershipCard>();

            foreach (var card in cards)
            {
                dtos.Add(MapMembershipCard(card));
            }

            return dtos;
        }

        //Set to private - use automapper
        private static dto.MembershipCard MapMembershipCard(db.MembershipCard card)
        {
            if (card == null)
                return null;

            dto.MembershipCard dto = new dto.MembershipCard
            {
                Id = card.Id,
                CustomerId = card.CustomerId,
                MembershipPlanId = card.MembershipPlanId,
                CardNumber = card.CardNumber,
                ValidFrom = card.ValidFrom,
                ValidTo = card.ValidTo,
                DateIssued = card.DateIssued,
                StatusId = card.StatusId,
                PhysicalCardRequested = card.PhysicalCardRequested,
                CustomerPaymentProviderId = card.CustomerPaymentProviderId,
                IsActive = card.IsActive,
                IsDeleted = card.IsDeleted,
                PhysicalCardStatusId = card.PhysicalCardStatusId,
                RegistrationCode = card.RegistrationCode,
                PartnerRewardId = card.PartnerRewardId,
                TermsConditionsId = card.TermsConditionsId
            };

            return dto;
        }


        public static dto.PagedResult<dto.Files> MapPartnerTransactionFiles(db.PagedResult<db.Files> data)
        {
            if (data == null)
                return null;

            dto.PagedResult<dto.Files> response = new dto.PagedResult<dto.Files>
            {
                CurrentPage = data.CurrentPage,
                PageCount = data.PageCount,
                PageSize = data.PageSize,
                RowCount = data.RowCount,
                Results = new List<dto.Files>()
            };
            if (data.Results?.Count > 0)
            {
                foreach (var file in data.Results)
                {
                    response.Results.Add(MapFile(file));
                }
            }

            return response;
        }

        public static List<dto.Files> MapFiles(List<db.Files> data)
        {
            if (data == null || data.Count == 0)
                return null;

            List<dto.Files> dtos = new List<dto.Files>();

            dtos.AddRange(data.Select(MapFile));

            return dtos;
        }

        //Set to private - use automapper
        private static dto.Files MapFile(db.Files data)
        {
            if (data == null)
                return null;

            var file = new dto.Files
            {
                Id = data.Id,
                Name = data.Name,
                PartnerId = data.PartnerId,
                Type = data.Type,
                StatusId = data.StatusId,
                PaymentStatusId = data.PaymentStatusId,
                TotalAmount = data.TotalAmount,
                CreatedDate = data.CreatedDate,
                ChangedDate = data.ChangedDate,
                PaidDate = data.PaidDate,
                UpdatedBy = data.UpdatedBy,
                ConfirmedAmount = data.ConfirmedAmount
            };

            if (data.Status != null)
            {
                file.Status = new dto.Status
                {
                    Id = data.Status.Id,
                    Name = data.Status.Name,
                    Type = data.Status.Type,
                    IsActive = data.Status.IsActive
                };
            }

            if (data.PaymentStatus != null)
            {
                file.PaymentStatus = new dto.Status
                {
                    Id = data.PaymentStatus.Id,
                    Name = data.PaymentStatus.Name,
                    Type = data.PaymentStatus.Type,
                    IsActive = data.PaymentStatus.IsActive
                };
            }

            if (data.Partner != null)
            {
                file.Partner = new dto.PartnerDto();
                file.Partner = MapPartner(data.Partner);
            }

            return file;
        }

        public static List<dto.PartnerDto> MapPartners(List<db.Partner> data)
        {
            if (data == null)
                return null;

            var response = new List<dto.PartnerDto>();
            foreach (var partner in data)
            {
                response.Add(MapPartner(partner));
            }

            return response;
        }

        //Set to private - use automapper
        private static dto.PartnerDto MapPartner(db.Partner data)
        {
            if (data == null)
                return null;

            return new dto.PartnerDto
            {
                Id = data.Id,
                Name = data.Name,
                ContactDetailId = data.ContactDetailId,
                BankDetailsId = data.BankDetailsId,
                IsDeleted = data.IsDeleted,
                Type = data.Type,
                ImagePath = data.ImagePath,
                ManagementURL = data.ManagementURL
            };
        }

        public static List<dto.MembershipPlanBenefits> MapPlanBenefitsList(List<db.MembershipPlanBenefits> data,
            IMapper mapper)
        {
            if (data == null || data.Count == 0)
                return null;
            List<dto.MembershipPlanBenefits> dtos = new List<dto.MembershipPlanBenefits>();
            if (data.Count > 0)
            {
                foreach (var membershipPlanBenefit in data)
                {
                    dtos.Add(mapper.Map<dto.MembershipPlanBenefits>(membershipPlanBenefit));
                }
            }

            return dtos;
        }

        public static List<dto.TamDataModel> MapTamDataModels(List<dto.TamWithdrawalDataModel> data)
        {
            List<dto.TamDataModel> tamDataModels = new List<dto.TamDataModel>();
            if (data == null)
                return null;
            foreach (var da in data)
            {
                tamDataModels.Add(MapDataModel(da));
            }
            return tamDataModels;
        }

        //Set to private - use automapper
        private static dto.TamDataModel MapDataModel(dto.TamWithdrawalDataModel data)
        {
            if (data == null)
                return null;
            return new dto.TamDataModel
            {
                TransType = data.TransType,
                UniqueReference =data.UniqueReference,
                FundType = data.FundType,
                Title = data.Title,
                Forename = data.Forename,
                Surname = data.Surname,
                NINumber = data.NINumber,
                Amount = data.Amount,
                IntroducerCode = data.IntroducerCode,
                ProcessState = data.ProcessState
            };
        }
        

        public static dto.PagedResult<dto.Merchant> MapMerchantPagedResult(db.PagedResult<db.Merchant> data)
        {
            if (data == null)
                return null;

            dto.PagedResult<dto.Merchant> resp = new dto.PagedResult<dto.Merchant>
            {
                CurrentPage = data.CurrentPage,
                PageCount = data.PageCount,
                PageSize = data.PageSize,
                RowCount = data.RowCount
            };

            if (data.Results?.Count > 0)
            {
                resp.Results = new List<dto.Merchant>();
                resp.Results = MapMerchantsList(data.Results.ToList());
            }

            return resp;
        }

        //Set to private - use automapper
        private static List<dto.Merchant> MapMerchantsList(List<db.Merchant> data)
        {
            if (data == null || data.Count == 0)
                return null;

            List<dto.Merchant> resp = new List<dto.Merchant>();
            foreach (var merchant in data)
            {
                resp.Add(MapMerchant(merchant));
            }
            return resp;
        }

        //Set to private - use automapper
        private static dto.Merchant MapMerchant(db.Merchant data)
        {
            if (data == null)
                return null;

            dto.Merchant resp = new dto.Merchant
            {
                Id = data.Id,
                Name = data.Name,
                ContactDetailsId = data.ContactDetailsId,
                ContactName = data.ContactName,
                ShortDescription = data.ShortDescription,
                LongDescription = data.LongDescription,
                Terms = data.Terms,
                WebsiteUrl = data.WebsiteUrl,
                IsDeleted = data.IsDeleted,
                FeatureImageOfferText = data.FeatureImageOfferText,
                BrandColour = data.BrandColour
            };

            if (data.ContactDetail != null)
            {
                resp.ContactDetail = new dto.ContactDetail();
                resp.ContactDetail = MapContactDetail(data.ContactDetail);
            }

            if (data.MerchantSocialMediaLinks?.Count > 0)
            {
                resp.MerchantSocialMediaLinks = new List<dto.MerchantSocialMediaLink>();
                resp.MerchantSocialMediaLinks = MapMerchantSocialMediaLinks(data.MerchantSocialMediaLinks.ToList());
            }

            if (data.MerchantBranches?.Count > 0)
            {
                resp.MerchantBranches = new List<dto.MerchantBranch>();
                resp.MerchantBranches = MapMerchantBranches(data.MerchantBranches.ToList());
            }

            if (data.MerchantImages?.Count > 0)
            {
                resp.MerchantImages = new List<dto.MerchantImage>();
                resp.MerchantImages = MapMerchantImages(data.MerchantImages.ToList());
            }

            return resp;
        }

        public static List<dto.MerchantSocialMediaLink> MapMerchantSocialMediaLinks(
            List<db.MerchantSocialMediaLink> data)
        {
            if (data == null || data.Count == 0)
                return null;
            List<dto.MerchantSocialMediaLink> resp = new List<dto.MerchantSocialMediaLink>();

            foreach (var link in data)
            {
                resp.Add(MapMerchantSocialMediaLink(link));
            }

            return resp;
        }

        public static dto.MerchantSocialMediaLink MapMerchantSocialMediaLink(db.MerchantSocialMediaLink data)
        {
            if (data == null)
                return null;
            dto.MerchantSocialMediaLink resp = new dto.MerchantSocialMediaLink
            {
                Id = data.Id,
                MerchantId = data.MerchantId,
                SocialMediaCompanyId = data.SocialMediaCompanyId,
                SocialMediaURI = data.SocialMediaURI
            };
            if (data.SocialMediaCompany != null)
            {
                resp.SocialMediaCompany = MapSocialMediaCompany(data.SocialMediaCompany);
            }

            return resp;
        }

        public static List<dto.SocialMediaCompany> MapSocialMediaCompanies(List<db.SocialMediaCompany> data)
        {
            if (data == null || data.Count == 0)
                return null;
            List<dto.SocialMediaCompany> resp = new List<dto.SocialMediaCompany>();
            foreach (var company in data)
            {
                resp.Add(MapSocialMediaCompany(company));
            }

            return resp;
        }

        //Set to private - use automapper
        private static dto.SocialMediaCompany MapSocialMediaCompany(db.SocialMediaCompany data)
        {
            if (data == null)
                return null;
            return new dto.SocialMediaCompany
            {
                Id = data.Id,
                Name = data.Name,
                IsEnabled = data.IsEnabled
            };
        }

        //Set to private - use automapper
        private static List<dto.MerchantBranch> MapMerchantBranches(List<db.MerchantBranch> data)
        {
            if (data == null)
                return null;
            List<dto.MerchantBranch> resp = new List<dto.MerchantBranch>();
            foreach (var branch in data)
            {
               resp.Add(MapMerchantBranch(branch)); 
            }
            return resp;
        }

        //Set to private - use automapper
        private static dto.MerchantBranch MapMerchantBranch(db.MerchantBranch data)
        {
            if (data == null)
                return null;

            dto.MerchantBranch resp = new dto.MerchantBranch
            {
                Id = data.Id,
                ContactDetailsId = data.ContactDetailsId,
                MerchantId = data.MerchantId,
                Name = data.Name,
                ShortDescription = data.ShortDescription,
                LongDescription = data.LongDescription,
                Notes = data.Notes,
                Mainbranch = data.Mainbranch,
                IsDeleted = data.IsDeleted
            };

            if (data.ContactDetail != null)
            {
                resp.ContactDetail = new dto.ContactDetail();
                resp.ContactDetail = MapContactDetail(data.ContactDetail);
            }

            return resp;
        }

        public static List<dto.MerchantImage> MapMerchantImages(List<db.MerchantImage> data)
        {
            if (data == null)
                return null;

            List<dto.MerchantImage> resp = new List<dto.MerchantImage>();
            foreach (var image in data)
            {
                resp.Add(MapMerchantImage(image));
            }

            return resp;
        }

        public static dto.MerchantImage MapMerchantImage(db.MerchantImage data)
        {
            if (data == null)
                return null;
            return new dto.MerchantImage
            {
                Id = data.Id,
                MerchantId = data.MerchantId,
                ImagePath = data.ImagePath,
                DisplayOrder = data.DisplayOrder,
                TimeStamp = data.TimeStamp,
                ImageType = data.ImageType
            };
        }

    }
}
