using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
    public class MerchantBranchService : IMerchantBranchService
    {
        #region Private Members
        
        private readonly IMapper _mapper;
        private readonly IMerchantBranchManager _manager;

        #endregion

        #region Constuctor

        public MerchantBranchService(IMapper mapper, IMerchantBranchManager merchantBranchManager)
        {
            _mapper = mapper;
            _manager = merchantBranchManager;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.MerchantBranch> Add(Models.DTOs.MerchantBranch branch)
        {
            MerchantBranch req = _mapper.Map<MerchantBranch>(branch);
            return MapBranchToDto(
                await _manager.Add(req));
        }

        public async Task<Models.DTOs.MerchantBranch> Update(Models.DTOs.MerchantBranch branch)
        {
            MerchantBranch req = _mapper.Map<MerchantBranch>(branch);
            return MapBranchToDto(
                await _manager.Update(req));
        }

        #endregion

        #region Reads

        public async Task<Models.DTOs.MerchantBranch> Get(int id, bool includeContact = false, bool includeMerchant = false)
        {
            return MapBranchToDto(await _manager.Get(id, includeContact, includeMerchant));
        }

        public async Task<List<Models.DTOs.MerchantBranch>> GetAll(int merchantId, bool includeContacts = false)
        {
            return MaptoBranchDtoList(await _manager.GetAll(merchantId, includeContacts));
        }

        public async Task<Models.DTOs.PagedResult<Models.DTOs.MerchantBranch>> GetPagedResult(int merchantId, int pageNo, int pageSize)
        {
            return Map(await _manager.GetBranchPagedList(merchantId, pageNo, pageSize));
        }

        #endregion

        #region Private methods

        private Models.DTOs.PagedResult<Models.DTOs.MerchantBranch> Map(PagedResult<MerchantBranch> branchPagedResult)
        {
            Models.DTOs.PagedResult<Models.DTOs.MerchantBranch> result = new Models.DTOs.PagedResult<Models.DTOs.MerchantBranch>();

            foreach (MerchantBranch branch in branchPagedResult.Results)
            {
                result.Results.Add(MapBranchToDto(branch));
            }

            result.CurrentPage = branchPagedResult.CurrentPage;
            result.PageCount = branchPagedResult.PageCount;
            result.PageSize = branchPagedResult.PageSize;
            result.RowCount = branchPagedResult.RowCount;
            return result;
        }

        private List<Models.DTOs.MerchantBranch> MaptoBranchDtoList(List<MerchantBranch> branches)
        {
            return branches.Select(MapBranchToDto).ToList();
        }

        private Models.DTOs.MerchantBranch MapBranchToDto(MerchantBranch branch)
        {
            if (branch == null)
                return null;
            if (branch.ContactDetail != null)
            {
                Models.DTOs.MerchantBranch dtoBranch = new Models.DTOs.MerchantBranch
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
                    IsDeleted = branch.IsDeleted,
                    ContactDetail = new Models.DTOs.ContactDetail
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
                    }
                };
                if (branch.Merchant != null)
                {
                    dtoBranch.Merchant = new Models.DTOs.Merchant
                    {
                        Id = branch.Merchant.Id,
                        Name = branch.Merchant.Name,
                        ContactDetailsId = branch.Merchant.ContactDetailsId,
                        ContactName = branch.Merchant.ContactName,
                        ShortDescription = branch.Merchant.ShortDescription,
                        LongDescription = branch.Merchant.LongDescription,
                        Terms = branch.Merchant.Terms,
                        WebsiteUrl = branch.Merchant.WebsiteUrl,
                        IsDeleted = branch.Merchant.IsDeleted
                    };
                }
                return dtoBranch;
            }

            else
            {
                Models.DTOs.MerchantBranch dtoBranch = new Models.DTOs.MerchantBranch
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
                    IsDeleted = branch.IsDeleted,

                };
                if (branch.Merchant != null)
                {
                    dtoBranch.Merchant = new Models.DTOs.Merchant
                    {
                        Id = branch.Merchant.Id,
                        Name = branch.Merchant.Name,
                        ContactDetailsId = branch.Merchant.ContactDetailsId,
                        ContactName = branch.Merchant.ContactName,
                        ShortDescription = branch.Merchant.ShortDescription,
                        LongDescription = branch.Merchant.LongDescription,
                        Terms = branch.Merchant.Terms,
                        WebsiteUrl = branch.Merchant.WebsiteUrl,
                        IsDeleted = branch.Merchant.IsDeleted
                    };
                }
                return dtoBranch;
            }
        }
        #endregion
    }
}
