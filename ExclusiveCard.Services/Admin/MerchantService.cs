using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
    public class MerchantService : IMerchantService
    {
        #region Private Members

        private readonly IMerchantManager _merchantManager;

        #endregion

        #region Contructor

        public MerchantService(IMerchantManager merchantManager)
        {
            _merchantManager = merchantManager;
        }

        #endregion

        #region Writes

        //Add merchant
        public async Task<Models.DTOs.Merchant> Add(Models.DTOs.Merchant merchant)
        {
            Merchant req = MapToData(merchant);
            return Map(await _merchantManager.Add(req), false, false, false, false);
        }

        //Update merchant
        public async Task<Models.DTOs.Merchant> Update(Models.DTOs.Merchant merchant)
        {
            Merchant req = MapToData(merchant);
            return Map(await _merchantManager.Update(req), false, false, false, false);
        }

        #endregion

        #region Reads

        //Get merchant
        public Models.DTOs.Merchant Get(int id, bool includeBranch = false, bool includeBranchContact = false,
            bool includeImage = false, bool includeSocialMedia = false)
        {
            return Map(_merchantManager.Get(id, includeBranch, includeBranchContact, includeImage,
                includeSocialMedia), includeBranch, includeBranchContact, includeImage, includeSocialMedia);
        }
        

        public Models.DTOs.Merchant GetByName (string name)
        {
            return Map(_merchantManager.GetByName(name), false, false, false, false);
        }

        //Get all merchants
        public async Task<List<Models.DTOs.Merchant>> GetAll(
            bool includeBranch = false, bool includeImage = false, bool includeSocialMedia = false)
        {
            bool includeContacts = includeBranch;
            return MapList(await _merchantManager.GetAll(includeBranch, includeImage, includeSocialMedia), includeBranch, includeContacts, includeImage, includeSocialMedia);
        }

        //Get Paged List of Merchants includes search by merchant name
        public async Task<Models.DTOs.PagedResult<Models.DTOs.Merchant>> GetPagedResult(string searchText, int pageNo, int pageSize, MerchantsSortOrder sortOrder)
        {
            return ManualMappings.MapMerchantPagedResult(await _merchantManager.GetPagedListAsync(searchText, pageNo, pageSize, sortOrder));
        }

        #endregion

        #region Private Members

        private List<Models.DTOs.Merchant> MapList(List<Merchant> merchants, bool includeBranch, bool includeBranchContact = false,
            bool includeImage = false, bool includeSocialMedia = false)
        {
            if (merchants == null || merchants.Count == 0)
                return null;
            List<Models.DTOs.Merchant> dtoMerchants = new List<Models.DTOs.Merchant>();
            dtoMerchants.AddRange(merchants.Select(merchant => Map(merchant, includeBranch, includeBranchContact, includeImage, includeSocialMedia)));
            return dtoMerchants;
        }

        private Models.DTOs.Merchant Map(Merchant merchant, bool includeBranch, bool includeBranchContact,
            bool includeImage, bool includeSocialMedia)
        {
            if (merchant == null)
                return null;
            Models.DTOs.Merchant dtoMerchant = new Models.DTOs.Merchant
            {
                Id = merchant.Id,
                Name = merchant.Name,
                ContactDetailsId = merchant.ContactDetailsId,
                ContactName = merchant.ContactName,
                ShortDescription = merchant.ShortDescription,
                LongDescription = merchant.LongDescription,
                Terms = merchant.Terms,
                WebsiteUrl = merchant.WebsiteUrl,
                IsDeleted = merchant.IsDeleted,
                FeatureImageOfferText = merchant.FeatureImageOfferText,
                BrandColour = merchant.BrandColour
            };
            if (includeBranch && merchant.MerchantBranches.Any())
            {
                dtoMerchant.MerchantBranches = new List<Models.DTOs.MerchantBranch>();
                foreach (MerchantBranch branch in merchant.MerchantBranches)
                {
                    Models.DTOs.MerchantBranch b = new Models.DTOs.MerchantBranch
                    {
                        Id = branch.Id,
                        ContactDetailsId = branch.ContactDetailsId,
                        MerchantId = branch.MerchantId,
                        Name = branch.Name,
                        ShortDescription = branch.ShortDescription,
                        LongDescription = branch.LongDescription,
                        Notes = branch.Notes,
                        Mainbranch = branch.Mainbranch,
                        DisplayOrder = branch.DisplayOrder,
                        IsDeleted = branch.IsDeleted
                    };
                    if (includeBranchContact && branch.ContactDetail != null)
                    {
                        b.ContactDetail = new Models.DTOs.ContactDetail
                        {
                            Id = branch.ContactDetail.Id,
                            Address1 = branch.ContactDetail.Address1,
                            Address2 = branch.ContactDetail.Address2,
                            Address3 = branch.ContactDetail.Address3,
                            Town = branch.ContactDetail.Town,
                            District = branch.ContactDetail.District,
                            PostCode = branch.ContactDetail.PostCode,
                            CountryCode = branch.ContactDetail.CountryCode,
                            Latitude = branch.ContactDetail.Latitude,
                            Longitude = branch.ContactDetail.Longitude,
                            LandlinePhone = branch.ContactDetail.LandlinePhone,
                            MobilePhone = branch.ContactDetail.MobilePhone,
                            EmailAddress = branch.ContactDetail.EmailAddress,
                            IsDeleted = branch.ContactDetail.IsDeleted
                        };
                    }
                    dtoMerchant.MerchantBranches.Add(b);
                }
            }

            if (includeImage && merchant.MerchantImages.Any())
            {
                dtoMerchant.MerchantImages = new List<Models.DTOs.MerchantImage>();
                foreach (MerchantImage image in merchant.MerchantImages)
                {
                    dtoMerchant.MerchantImages.Add(new Models.DTOs.MerchantImage
                    {
                        Id = image.Id,
                        MerchantId = image.MerchantId,
                        ImagePath = image.ImagePath,
                        DisplayOrder = image.DisplayOrder,
                        ImageType = image.ImageType
                    });
                }
            }

            if (includeSocialMedia && merchant.MerchantSocialMediaLinks.Any())
            {
                dtoMerchant.MerchantSocialMediaLinks = new List<Models.DTOs.MerchantSocialMediaLink>();
                foreach (MerchantSocialMediaLink link in merchant.MerchantSocialMediaLinks)
                {
                    dtoMerchant.MerchantSocialMediaLinks.Add(new Models.DTOs.MerchantSocialMediaLink
                    {
                        Id = link.Id,
                        MerchantId = link.MerchantId,
                        SocialMediaCompanyId = link.SocialMediaCompanyId,
                        SocialMediaURI = link.SocialMediaURI
                    });
                }
            }

            return dtoMerchant;
        }

        private Merchant MapToData(Models.DTOs.Merchant merchant)
        {
            if (merchant == null)
                return null;

            Merchant dtoMerchant = new Merchant
            {
                Id = merchant.Id,
                Name = merchant.Name,
                ContactDetailsId = merchant.ContactDetailsId,
                ContactName = merchant.ContactName,
                ShortDescription = merchant.ShortDescription,
                LongDescription = merchant.LongDescription,
                Terms = merchant.Terms,
                WebsiteUrl = merchant.WebsiteUrl,
                IsDeleted = merchant.IsDeleted,
                FeatureImageOfferText = merchant.FeatureImageOfferText,
                BrandColour = merchant.BrandColour
            };
            if (merchant.MerchantBranches != null && merchant.MerchantBranches.Any())
            {
                dtoMerchant.MerchantBranches = new List<MerchantBranch>();
                foreach (Models.DTOs.MerchantBranch branch in merchant.MerchantBranches)
                {
                    MerchantBranch b = new MerchantBranch
                    {
                        Id = branch.Id,
                        ContactDetailsId = branch.ContactDetailsId,
                        MerchantId = branch.MerchantId,
                        Name = branch.Name,
                        ShortDescription = branch.ShortDescription,
                        LongDescription = branch.LongDescription,
                        Notes = branch.Notes,
                        Mainbranch = branch.Mainbranch,
                        DisplayOrder = branch.DisplayOrder,
                        IsDeleted = branch.IsDeleted
                    };
                    if (branch.ContactDetail != null)
                    {
                        b.ContactDetail = new ContactDetail
                        {
                            Id = branch.ContactDetail.Id,
                            Address1 = branch.ContactDetail.Address1,
                            Address2 = branch.ContactDetail.Address2,
                            Address3 = branch.ContactDetail.Address3,
                            Town = branch.ContactDetail.Town,
                            District = branch.ContactDetail.District,
                            PostCode = branch.ContactDetail.PostCode,
                            CountryCode = branch.ContactDetail.CountryCode,
                            Latitude = branch.ContactDetail.Latitude,
                            Longitude = branch.ContactDetail.Longitude,
                            LandlinePhone = branch.ContactDetail.LandlinePhone,
                            MobilePhone = branch.ContactDetail.MobilePhone,
                            EmailAddress = branch.ContactDetail.EmailAddress,
                            IsDeleted = branch.ContactDetail.IsDeleted
                        };
                    }
                    dtoMerchant.MerchantBranches.Add(b);
                }
            }

            if (merchant.MerchantImages != null && merchant.MerchantImages.Any())
            {
                dtoMerchant.MerchantImages = new List<MerchantImage>();
                foreach (Models.DTOs.MerchantImage image in merchant.MerchantImages)
                {
                    dtoMerchant.MerchantImages.Add(new MerchantImage
                    {
                        Id = image.Id,
                        MerchantId = image.MerchantId,
                        ImagePath = image.ImagePath,
                        DisplayOrder = image.DisplayOrder,
                        ImageType = image.ImageType
                    });
                }
            }

            if (merchant.MerchantSocialMediaLinks != null && merchant.MerchantSocialMediaLinks.Any())
            {
                dtoMerchant.MerchantSocialMediaLinks = new List<MerchantSocialMediaLink>();
                foreach (Models.DTOs.MerchantSocialMediaLink link in merchant.MerchantSocialMediaLinks)
                {
                    dtoMerchant.MerchantSocialMediaLinks.Add(new MerchantSocialMediaLink
                    {
                        Id = link.Id,
                        MerchantId = link.MerchantId,
                        SocialMediaCompanyId = link.SocialMediaCompanyId,
                        SocialMediaURI = link.SocialMediaURI
                    });
                }
            }

            return dtoMerchant;
        }

        #endregion
    }
}



