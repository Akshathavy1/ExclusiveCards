using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Services.Models.DTOs.Public;
using ExclusiveCard.Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using dto = ExclusiveCard.Services.Models.DTOs;
namespace ExclusiveCard.Website.Controllers
{
    public class MerchantBranchController : BaseController
    {
        private readonly Services.Interfaces.Admin.IMerchantService _merchantService;
        private readonly Services.Interfaces.Admin.IContactDetailService _contactDetailService;
        private readonly Services.Interfaces.Admin.IMerchantBranchService _merchantBranchService;
        private readonly IOfferBranchServices _offerBranchServices;
        private readonly IOptions<TypedAppSettings> _settings;
        public MerchantBranchController(Services.Interfaces.Admin.IMerchantService merchantService,
            Services.Interfaces.Admin.IContactDetailService contactDetailService,
            Services.Interfaces.Admin.IMerchantBranchService merchantBranchService,
            IOfferBranchServices offerBranchServices, IOptions<TypedAppSettings> settings)
        {
            _merchantService = merchantService;
            _contactDetailService = contactDetailService;
            _merchantBranchService = merchantBranchService;
            _offerBranchServices = offerBranchServices;
            _settings = settings;
        }
        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index(int offerid, int merchantid,string logo)
        {
            MerchantOffersViewModel model = new MerchantOffersViewModel();
            var merchants = await _merchantService.GetAll();
            ViewBag.Logo = logo;
            ViewBag.GoogleApi = _settings.Value.GoogleApiKey;
            List<dto.MerchantBranch> merchantBranches = new List<dto.MerchantBranch>();
            merchantBranches = await _merchantBranchService.GetAll(merchantid);
            foreach (var item in merchantBranches)
            {
                if (item.ContactDetail == null)
                {
                    item.ContactDetail = await _contactDetailService.Get((int)item.ContactDetailsId);
                }
            }
            if (offerid != 0)
            {
                var branch = await _offerBranchServices.GetofferBranch((int)offerid);

                List<dto.MerchantBranch> offerMerchantBranches = new List<dto.MerchantBranch>();
                foreach (var id in branch)
                {
                    offerMerchantBranches.Add(merchantBranches.FirstOrDefault(x => x.Id == id));
                }
                if (offerMerchantBranches != null)
                {
                    foreach (var id in offerMerchantBranches)
                    {
                        if (id.ContactDetail != null)
                        {
                            if (id.ContactDetail.Address1 != null || id.ContactDetail.Address2 != null || id.ContactDetail.Address3 != null)
                            {
                                model.MerchantBranches.Add(id);
                            }


                        }
                    }
                }

            }

            return View("Index1", model.MerchantBranches);
        }
    }
}
