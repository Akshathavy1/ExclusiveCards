using ExclusiveCard.Enums;
using ExclusiveCard.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;
using serve = ExclusiveCard.Website.App.ServiceHelper;

namespace ExclusiveCard.Website.Helpers
{
    public class OffersHelper : CommonHelper
    {
        //Get list of OfferList & then get offerSummaries for each offerList with pagesize not more than 100
        public static async Task GetAndMapOffersForCategories(string countryCode, OffersViewModel model, string userId)
        {
            try
            {
                //check if user is signed in
                var membershipStatus = string.Empty;
                if (!string.IsNullOrEmpty(userId) && new Guid(userId) != Guid.Empty)
                {
                    dto.Customer custData = await serve.Instance.CustomerService.Get(userId);
                    List<dto.MembershipCard> mCards = await serve.Instance.MembershipCardService.GetAll(custData.Id);
                    List<dto.Status> status = await serve.Instance.StatusService.GetAll();
                    membershipStatus = status?.FirstOrDefault(x => x.IsActive && x.Type == Data.Constants.StatusType.MembershipCard
                                                                           && x.Id == mCards?.FirstOrDefault(y => y.IsActive && !y.IsDeleted)?.StatusId)?.Name;
                }

                
                model.OfferLists = CacheHelper.Get<List<dto.OfferList>>(serve.Instance.Cache, Keys.Keys.OfferLists);
                if (model.OfferLists == null || model.OfferLists.Count == 0)
                {
                    model.OfferLists = await serve.Instance.OffersService.GetAllOfferLists();
                    CacheHelper.Set(serve.Instance.Cache, model.OfferLists, Keys.Keys.OfferLists);
                }

                List<dto.OfferType> types =
                    CacheHelper.Get<List<dto.OfferType>>(serve.Instance.Cache, Keys.Keys.OfferTypes);
                if (types == null || types.Count == 0)
                {
                    types = await serve.Instance.OffersService.GetAllOfferType();
                    CacheHelper.Set(serve.Instance.Cache, types, Keys.Keys.OfferTypes);
                }

                List<dto.Public.OfferSummary> highstreet = new List<dto.Public.OfferSummary>();
                if (model.OfferLists != null && model.OfferLists.Count > 0)
                {
                    foreach (dto.OfferList offer in model.OfferLists)
                    {
                        switch (offer.ListName)
                        {
                            case Keys.Keys.CashBackOfferList:
                                switch (offer.PermissionLevel)
                                {
                                    case (int)PermissionLevel.Everyone:
                                    case (int)PermissionLevel.LoggedIn when !string.IsNullOrEmpty(userId) && new Guid(userId) != Guid.Empty:
                                    case (int)PermissionLevel.PendingAndActiveCards when membershipStatus == Data.Constants.Status.Active || membershipStatus == Data.Constants.Status.Pending:
                                    case (int)PermissionLevel.ActiveCards when membershipStatus == Data.Constants.Status.Active:
                                        model.ShowCashbackPanel = true;
                                        break;
                                    default:
                                        model.ShowCashbackPanel = false;
                                        break;
                                }
                                if (model.ShowCashbackPanel)
                                {
                                    model.CashBackOfferSummaries = CacheHelper.Get<List<dto.Public.OfferSummary>>(
                                        serve.Instance.Cache,
                                        Keys.Keys.CashBackOfferList, countryCode);
                                    if (model.CashBackOfferSummaries == null)
                                    {
                                        model.CashBackOfferSummaries = new List<dto.Public.OfferSummary>();
                                        model.CashBackOfferSummaries.AddRange(
                                            await serve.Instance.OffersService.GetByOfferListCountry(offer.Id,
                                                countryCode,
                                                HomePageSize, null));
                                        CacheHelper.Set(serve.Instance.Cache, model.CashBackOfferSummaries,
                                            Keys.Keys.CashBackOfferList, countryCode);

                                    }
                                }
                                break;
                            case Keys.Keys.VoucherOfferList:
                                switch (offer.PermissionLevel)
                                {
                                    case (int)PermissionLevel.Everyone:
                                    case (int)PermissionLevel.LoggedIn when !string.IsNullOrEmpty(userId) && new Guid(userId) != Guid.Empty:
                                    case (int)PermissionLevel.PendingAndActiveCards when membershipStatus == Data.Constants.Status.Active || membershipStatus == Data.Constants.Status.Pending:
                                    case (int)PermissionLevel.ActiveCards when membershipStatus == Data.Constants.Status.Active:
                                        model.ShowVoucherPanel = true;
                                        break;
                                    default:
                                        model.ShowVoucherPanel = false;
                                        break;
                                }
                                if (model.ShowVoucherPanel)
                                {
                                    model.VoucherCodeOfferSummaries = CacheHelper.Get<List<dto.Public.OfferSummary>>(
                                        serve.Instance.Cache,
                                        Keys.Keys.VoucherOfferList, countryCode);
                                    if (model.VoucherCodeOfferSummaries == null)
                                    {
                                        model.VoucherCodeOfferSummaries = new List<dto.Public.OfferSummary>();
                                        model.VoucherCodeOfferSummaries.AddRange(
                                            await serve.Instance.OffersService.GetByOfferListCountry(offer.Id,
                                                countryCode,
                                                HomePageSize, null));
                                        CacheHelper.Set(serve.Instance.Cache, model.VoucherCodeOfferSummaries,
                                            Keys.Keys.VoucherOfferList, countryCode);
                                    }
                                }
                                break;
                            case Keys.Keys.DailyOfferList:
                                switch (offer.PermissionLevel)
                                {
                                    case (int)PermissionLevel.Everyone:
                                    case (int)PermissionLevel.LoggedIn when !string.IsNullOrEmpty(userId) && new Guid(userId) != Guid.Empty:
                                    case (int)PermissionLevel.PendingAndActiveCards when membershipStatus == Data.Constants.Status.Active || membershipStatus == Data.Constants.Status.Pending:
                                    case (int)PermissionLevel.ActiveCards when membershipStatus == Data.Constants.Status.Active:
                                        model.ShowDealsPanel = true;
                                        break;
                                    default:
                                        model.ShowDealsPanel = false;
                                        break;
                                }
                                if (model.ShowDealsPanel)
                                {
                                    model.DailyDealsOfferSummaries =
                                        CacheHelper.Get<List<dto.Public.OfferSummary>>(serve.Instance.Cache,
                                            Keys.Keys.DailyOfferList, countryCode);
                                    if (model.DailyDealsOfferSummaries == null)
                                    {
                                        model.DailyDealsOfferSummaries = new List<dto.Public.OfferSummary>();
                                        model.DailyDealsOfferSummaries.AddRange(
                                            await serve.Instance.OffersService.GetByOfferListCountry(offer.Id,
                                                countryCode,
                                                HomePageSize, null));
                                        CacheHelper.Set(serve.Instance.Cache, model.DailyDealsOfferSummaries,
                                            Keys.Keys.DailyOfferList, countryCode);
                                    }
                                }
                                break;
                            case Keys.Keys.EndingSoonList:
                                switch (offer.PermissionLevel)
                                {
                                    case (int)PermissionLevel.Everyone:
                                    case (int)PermissionLevel.LoggedIn when !string.IsNullOrEmpty(userId) && new Guid(userId) != Guid.Empty:
                                    case (int)PermissionLevel.PendingAndActiveCards when membershipStatus == Data.Constants.Status.Active || membershipStatus == Data.Constants.Status.Pending:
                                    case (int)PermissionLevel.ActiveCards when membershipStatus == Data.Constants.Status.Active:
                                        model.ShowEndingSoonPanel = true;
                                        break;
                                    default:
                                        model.ShowEndingSoonPanel = false;
                                        break;
                                }
                                if (model.ShowEndingSoonPanel)
                                {
                                    model.EndingSoonOfferSummaries =
                                        CacheHelper.Get<List<dto.Public.OfferSummary>>(serve.Instance.Cache,
                                            Keys.Keys.EndingSoonList, countryCode);
                                    if (model.EndingSoonOfferSummaries == null)
                                    {
                                        model.EndingSoonOfferSummaries = new List<dto.Public.OfferSummary>();
                                        model.EndingSoonOfferSummaries.AddRange(
                                            await serve.Instance.OffersService.GetByOfferListCountry(offer.Id,
                                                countryCode,
                                                HomePageSize, null, true));
                                        CacheHelper.Set(serve.Instance.Cache, model.EndingSoonOfferSummaries,
                                            Keys.Keys.EndingSoonList, countryCode);
                                    }
                                }
                                break;
                            case Keys.Keys.SalesOfferList:
                                switch (offer.PermissionLevel)
                                {
                                    case (int)PermissionLevel.Everyone:
                                    case (int)PermissionLevel.LoggedIn when !string.IsNullOrEmpty(userId) && new Guid(userId) != Guid.Empty:
                                    case (int)PermissionLevel.PendingAndActiveCards when membershipStatus == Data.Constants.Status.Active || membershipStatus == Data.Constants.Status.Pending:
                                    case (int)PermissionLevel.ActiveCards when membershipStatus == Data.Constants.Status.Active:
                                        model.ShowSalesPanel = true;
                                        break;
                                    default:
                                        model.ShowSalesPanel = false;
                                        break;
                                }
                                if (model.ShowSalesPanel)
                                {
                                    model.SalesOfferSummaries =
                                        CacheHelper.Get<List<dto.Public.OfferSummary>>(serve.Instance.Cache,
                                            Keys.Keys.SalesOfferList, countryCode);
                                    if (model.SalesOfferSummaries == null)
                                    {
                                        model.SalesOfferSummaries = new List<dto.Public.OfferSummary>();
                                        model.SalesOfferSummaries.AddRange(
                                            await serve.Instance.OffersService.GetByOfferListCountry(offer.Id,
                                                countryCode,
                                                HomePageSize, null));
                                        CacheHelper.Set(serve.Instance.Cache, Keys.Keys.SalesOfferList, countryCode);
                                    }
                                }
                                break;
                            case Keys.Keys.HighStreetOfferList:
                                switch (offer.PermissionLevel)
                                {
                                    case (int)PermissionLevel.Everyone:
                                    case (int)PermissionLevel.LoggedIn when !string.IsNullOrEmpty(userId) && new Guid(userId) != Guid.Empty:
                                    case (int)PermissionLevel.PendingAndActiveCards when membershipStatus == Data.Constants.Status.Active || membershipStatus == Data.Constants.Status.Pending:
                                    case (int)PermissionLevel.ActiveCards when membershipStatus == Data.Constants.Status.Active:
                                        model.ShowHighStreetPanel = true;
                                        break;
                                    default:
                                        model.ShowHighStreetPanel = false;
                                        break;
                                }
                                if (model.ShowHighStreetPanel)
                                {
                                    highstreet = CacheHelper.Get<List<dto.Public.OfferSummary>>(serve.Instance.Cache,
                                        Keys.Keys.HighStreetOfferList, countryCode);
                                    if (highstreet == null || highstreet.Count == 0)
                                    {
                                        highstreet = new List<dto.Public.OfferSummary>();
                                        highstreet.AddRange(
                                            await serve.Instance.OffersService.GetByOfferListCountry(offer.Id,
                                                countryCode,
                                                HomePageSize, null));
                                        CacheHelper.Set(serve.Instance.Cache, highstreet, Keys.Keys.HighStreetOfferList,
                                            countryCode);
                                    }
                                }
                                break;
                            case Keys.Keys.RestaurantOfferList:
                                switch (offer.PermissionLevel)
                                {
                                    case (int)PermissionLevel.Everyone:
                                    case (int)PermissionLevel.LoggedIn when !string.IsNullOrEmpty(userId) && new Guid(userId) != Guid.Empty:
                                    case (int)PermissionLevel.PendingAndActiveCards when membershipStatus == Data.Constants.Status.Active || membershipStatus == Data.Constants.Status.Pending:
                                    case (int)PermissionLevel.ActiveCards when membershipStatus == Data.Constants.Status.Active:
                                        model.ShowRestuarantPanel = true;
                                        break;
                                    default:
                                        model.ShowRestuarantPanel = false;
                                        break;
                                }
                                if (model.ShowRestuarantPanel)
                                {
                                    model.RestaurantOfferSummaries =
                                        CacheHelper.Get<List<Tuple<dto.Public.OfferSummary, dto.Public.OfferSummary>>>(
                                            serve.Instance.Cache, Keys.Keys.RestaurantOfferList, countryCode);
                                    if (model.RestaurantOfferSummaries == null ||
                                        model.RestaurantOfferSummaries.Count == 0)
                                    {
                                        model.RestaurantOfferSummaries =
                                            new List<Tuple<dto.Public.OfferSummary, dto.Public.OfferSummary>>();
                                        model.RestaurantOfferSummaries = GetHighStreetDeals(
                                            await serve.Instance.OffersService.GetByOfferListCountry(offer.Id,
                                                countryCode,
                                                HomePageSize, null), 2);
                                        CacheHelper.Set(serve.Instance.Cache, model.RestaurantOfferSummaries,
                                            Keys.Keys.RestaurantOfferList, countryCode);
                                    }
                                }
                                break;
                        }
                    }
                }

                if (highstreet.Count > 0)
                {
                    model.HighStreetOfferSummaries = GetHighStreetDeals(highstreet, 4);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get lIst of OfferList along with offerSummaries
        public static async Task GetAndMapOffers(string countryCode, OffersViewModel model)
        {
            List<dto.OfferList> offerLists = await serve.Instance.OffersService.GetOffersForCountry(countryCode);

            List<dto.Public.OfferSummary> highstreet = new List<dto.Public.OfferSummary>();
            if (offerLists != null && offerLists.Count > 0)
            {
                foreach (dto.OfferList offer in offerLists)
                {
                    switch (offer.ListName)
                    {
                        case Keys.Keys.CashBackOfferList:
                            model.CashBackOfferSummaries.AddRange(MapToOfferSummary(offer));
                            break;
                        case Keys.Keys.VoucherOfferList:
                            model.VoucherCodeOfferSummaries.AddRange(MapToOfferSummary(offer));
                            break;
                        case Keys.Keys.DailyOfferList:
                            model.DailyDealsOfferSummaries.AddRange(MapToOfferSummary(offer));
                            break;
                        case Keys.Keys.EndingSoonList:
                            model.DailyDealsOfferSummaries.AddRange(MapToOfferSummary(offer));
                            break;
                        case Keys.Keys.HighStreetOfferList:
                            highstreet.AddRange(MapToOfferSummary(offer));
                            break;
                        case Keys.Keys.RestaurantOfferList:
                            model.RestaurantOfferSummaries = GetHighStreetDeals(MapToOfferSummary(offer).ToList(), 2);
                            break;
                    }
                }
            }

            if (highstreet.Count > 0)
            {
                model.HighStreetOfferSummaries = GetHighStreetDeals(highstreet, 4);
            }
        }

        //Map Highstreet deals into dictionary
        public static List<Tuple<dto.Public.OfferSummary, dto.Public.OfferSummary>> GetHighStreetDeals(
            List<dto.Public.OfferSummary> list, int? count)
        {
            List<Tuple<dto.Public.OfferSummary, dto.Public.OfferSummary>> highStreet =
                new List<Tuple<dto.Public.OfferSummary, dto.Public.OfferSummary>>();
            int maxCount = list.Count;
            if (count.HasValue)
            {
                if (list.Count > count.Value)
                {
                    maxCount = count.Value;
                }
            }

            for (var i = 0; i < maxCount; i = i + 2)
            {
                int j = i + 1;
                if (list[i] != null && (j) == list.Count)
                {
                    highStreet.Add(new Tuple<dto.Public.OfferSummary, dto.Public.OfferSummary>(list[i], null));
                }
                else if (list[i] != null && list[i + 1] != null)
                {
                    highStreet.Add(
                        new Tuple<dto.Public.OfferSummary, dto.Public.OfferSummary>(list[i], list[i + 1]));
                }
            }
            return highStreet;
        }

        public static async Task GetMerchantOffersAndMapToPagedData(int merchantId, string countryCode, int currentPage, MerchantOffersViewModel model)
        {
            try
            {
                
                model.Merchant = serve.Instance.MerchantService.Get(merchantId, false, false, true, true);
                
                ////TODO:this has to be removed in next release
                dto.PagedResult<dto.MerchantBranch> paging = await serve.Instance.MerchantBranchService.GetPagedResult(merchantId,currentPage,BranchPageSize);
                model.PagedMerchantBranch.MerchantBranches = paging.Results.ToList();
                model.PagedMerchantBranch.PagingView.CurrentPage = paging.CurrentPage;
                model.PagedMerchantBranch.PagingView.PageCount = paging.PageCount;
                model.PagedMerchantBranch.PagingView.PageSize = paging.PageSize;
                model.PagedMerchantBranch.PagingView.RowCount = paging.RowCount;

                //OfferLists
                var offerLists = CacheHelper.Get<List<dto.OfferList>>(serve.Instance.Cache, Keys.Keys.OfferLists);
                if (offerLists == null || offerLists.Count == 0)
                {
                    offerLists = await serve.Instance.OffersService.GetAllOfferLists();
                    CacheHelper.Set(serve.Instance.Cache, offerLists, Keys.Keys.OfferLists);
                }

                //Offer Types
                List<dto.OfferType> types = CacheHelper.Get<List<dto.OfferType>>(serve.Instance.Cache, Keys.Keys.OfferTypes);
                if (types == null || types.Count == 0)
                {
                    types = await serve.Instance.OffersService.GetAllOfferType();
                    CacheHelper.Set(serve.Instance.Cache, types, Keys.Keys.OfferTypes);
                }

                //get data for offer Types
                if (types != null && types.Count > 0)
                {
                    foreach (dto.OfferType type in types)
                    {
                        switch (type.Description)
                        {
                            case Keys.Keys.CashbackType:
                                //model.Cashback = MapToPagedOffersViewModel(await offerService.GetMerchantOffersByType(merchantId, type.Id, countryCode, PageSize, currentPage));
                                model.Cashback =
                                    await serve.Instance.OffersService.GetListofMerchantOffersByTypeAsync(
                                        merchantId, type.Id, countryCode);
                                break;
                            case Keys.Keys.RestaurantType:
                                //model.HighStreetRestaurant = MapToPagedOffersViewModel(await offerService.GetMerchantOffersByType(merchantId, type.Id, countryCode, PageSize, currentPage));
                                model.HighStreetRestaurant = await serve.Instance.OffersService.GetListofMerchantOffersByTypeAsync(
                                    merchantId, type.Id, countryCode);
                                break;
                            case Keys.Keys.VoucherType:
                                //model.Voucher = MapToPagedOffersViewModel(await offerService.GetMerchantOffersByType(merchantId, type.Id, countryCode, PageSize, currentPage));
                                model.Voucher = await serve.Instance.OffersService.GetListofMerchantOffersByTypeAsync(
                                    merchantId, type.Id, countryCode);
                                break;
                            case Keys.Keys.StandardType:
                                //model.Standard = MapToPagedOffersViewModel(await offerService.GetMerchantOffersByType(merchantId, type.Id, countryCode, PageSize, currentPage));
                                var standard = await serve.Instance.OffersService.GetListofMerchantOffersByTypeAsync(
                                    merchantId, type.Id, countryCode);
                                model.FeaturedOffer = standard.FirstOrDefault();
                                //if more than 1 standard cashback offers are there, then it should be appended below the Cashback offers
                                if (standard?.Count > 1)
                                {
                                    for (int i = 1; i < standard.Count; i++)
                                    {
                                        model.Cashback.Add(standard[i]);
                                    }
                                }
                                break;
                            case Keys.Keys.SalesType:
                                //model.Sales = MapToPagedOffersViewModel(await offerService.GetMerchantOffersByType(merchantId, type.Id, countryCode, PageSize, currentPage));
                                model.Sales = await serve.Instance.OffersService.GetListofMerchantOffersByTypeAsync(
                                    merchantId, type.Id, countryCode);
                                break;
                            case Keys.Keys.HighStreet:
                                //model.HighStreet = MapToPagedOffersViewModel(await offerService.GetMerchantOffersByType(merchantId, type.Id, countryCode, PageSize, currentPage));
                                model.HighStreet = await serve.Instance.OffersService.GetListofMerchantOffersByTypeAsync(
                                    merchantId, type.Id, countryCode);
                                break;
                        }
                    }
                }

                //Get data for Offer Lists
                if (offerLists != null && offerLists.Count > 0)
                {
                    foreach (var list in offerLists)
                    {
                        //Temporary Fix
                        switch (list.ListName)
                        {
                            case Keys.Keys.DailyOfferList:
                                model.DailyDeals =
                                    CacheHelper.Get<List<dto.Public.OfferSummary>>(serve.Instance.Cache,
                                        Keys.Keys.DailyOfferList, countryCode);
                                if (model.DailyDeals == null)
                                {
                                    model.DailyDeals = new List<dto.Public.OfferSummary>();
                                    model.DailyDeals.AddRange(
                                        await serve.Instance.OffersService.GetByOfferListCountry(list.Id, countryCode,
                                            HomePageSize, null));
                                    CacheHelper.Set(serve.Instance.Cache, model.DailyDeals, Keys.Keys.DailyOfferList, countryCode);
                                }
                                break;
                            case Keys.Keys.EndingSoonList:
                                model.EndingSoon =
                                    CacheHelper.Get<List<dto.Public.OfferSummary>>(serve.Instance.Cache,
                                        Keys.Keys.EndingSoonList, countryCode);
                                if (model.EndingSoon == null)
                                {
                                    model.EndingSoon = new List<dto.Public.OfferSummary>();
                                    model.EndingSoon.AddRange(
                                        await serve.Instance.OffersService.GetByOfferListCountry(list.Id, countryCode,
                                            HomePageSize, null));
                                    CacheHelper.Set(serve.Instance.Cache, model.EndingSoon, Keys.Keys.EndingSoonList, countryCode);
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public static async Task<PagedOffersViewModel> GetMerchantOffersAndMapToPagedData(int merchantId, string countryCode, int currentPage, string offerType)
        //{
        //    try
        //    {
        //        List<dto.OfferType> types = CacheHelper.Get<List<dto.OfferType>>(serve.Instance.Cache, Keys.Keys.OfferTypes);
        //        if (types == null || types.Count == 0)
        //        {
        //            types = await serve.Instance.OffersService.GetAllOfferType();
        //            CacheHelper.Set(serve.Instance.Cache, types, Keys.Keys.OfferTypes);
        //        }

        //        if (types != null && types.Count > 0)
        //        {
        //            foreach (dto.OfferType type in types)
        //            {
        //                if (!string.IsNullOrEmpty(offerType) && offerType == type.Description)
        //                {
        //                    switch (type.Description)
        //                    {
        //                        case Keys.Keys.CashbackType:
        //                            return MapToPagedOffersViewModel(await serve.Instance.OffersService.GetMerchantOffersByType(merchantId, type.Id, countryCode, PageSize, currentPage));
        //                        case Keys.Keys.RestaurantType:
        //                            return MapToPagedOffersViewModel(await serve.Instance.OffersService.GetMerchantOffersByType(merchantId, type.Id, countryCode, PageSize, currentPage));
        //                        case Keys.Keys.VoucherType:
        //                            return MapToPagedOffersViewModel(await serve.Instance.OffersService.GetMerchantOffersByType(merchantId, type.Id, countryCode, PageSize, currentPage));
        //                        case Keys.Keys.StandardType:
        //                            return MapToPagedOffersViewModel(await serve.Instance.OffersService.GetMerchantOffersByType(merchantId, type.Id, countryCode, PageSize, currentPage));
        //                        case Keys.Keys.SalesType:
        //                            return MapToPagedOffersViewModel(await serve.Instance.OffersService.GetMerchantOffersByType(merchantId, type.Id, countryCode, PageSize, currentPage));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return null;
        //}

        ////Display All offers
        //public static async Task<OffersDisplayViewModel> MapToDisplayOffer(OffersDisplayViewModel offersDisplayView, string countryCode,
        //    string keyword, List<int> categoryIds, string merchant, List<int> offerTypeIds, string[] searchCategory,
        //    string[] searchOfferTypes, string listName, bool parentCategory, int currentPage)
        //{
        //    if (searchCategory != null)
        //    {
        //        categoryIds = CategoryStringArrayToList(searchCategory, parentCategory);
        //    }
        //    if (categoryIds.Count > 0)
        //    {
        //        offersDisplayView.CategoryIds = categoryIds;
        //    }
        //    if (searchOfferTypes != null)
        //    {
        //        offerTypeIds = await OfferTypeStringArrayToList(searchOfferTypes);
        //    }
        //    if (offerTypeIds.Count > 0)
        //    {
        //        offersDisplayView.OfferTypeIds = offerTypeIds;
        //    }
        //    if (!string.IsNullOrEmpty(keyword))
        //    {
        //        offersDisplayView.Keywords = keyword;
        //    }
        //    if (!string.IsNullOrEmpty(merchant))
        //    {
        //        offersDisplayView.MerchantName = merchant;
        //    }
        //    List<dto.Category> categories = serve.Instance.PCategoryService.GetAll();
        //    foreach (dto.Category category in categories)
        //    {
        //        category.IsActive = false;
        //    }
        //    if (offersDisplayView.CategoryIds.Count > 0)
        //    {
        //        foreach (int id in offersDisplayView.CategoryIds)
        //        {
        //            foreach (dto.Category category in categories)
        //            {
        //                if (category.Id == id)
        //                {
        //                    category.IsActive = true;
        //                }
        //            }
        //        }
        //    }
        //    List<dto.OfferType> offerTypes = await serve.Instance.OffersService.GetAllOfferType();
        //    foreach (dto.OfferType offerType in offerTypes)
        //    {
        //        offerType.IsActive = false;
        //    }
        //    if (offersDisplayView.OfferTypeIds.Count > 0)
        //    {
        //        foreach (int id in offersDisplayView.OfferTypeIds)
        //        {
        //            foreach (dto.OfferType offerType in offerTypes)
        //            {
        //                if (offerType.Id == id)
        //                {
        //                    offerType.IsActive = true;
        //                }
        //            }
        //        }
        //    }

        //    List<dto.Merchant> merchants = await serve.Instance.MerchantService.GetAll();
        //    List<SelectListItem> ListofMerchantName = merchants.Select(merchantNew => new SelectListItem()
        //    {
        //        Text = merchantNew.Name,
        //        Value = merchantNew.Id.ToString()
        //    }).ToList();
        //    offersDisplayView.ListOfMerchantName = ListofMerchantName;

        //    List<dto.Tag> tags = await serve.Instance.TagService.GetAll();

        //    dto.OfferSearchCriteria criteria = new dto.OfferSearchCriteria
        //    {
        //        KeyWord = offersDisplayView.Keywords,
        //        Categories = offersDisplayView.CategoryIds,
        //        MerchantName = offersDisplayView.MerchantName,
        //        OfferTypes = offersDisplayView.OfferTypeIds
        //    };
        //    if (!string.IsNullOrEmpty(listName))
        //    {
        //        criteria.SortOrder = 1;
        //    }
        //    offersDisplayView.Categories = categories;
        //    offersDisplayView.Tags = tags;
        //    offersDisplayView.OfferTypes = offerTypes;
        //    if (searchOfferTypes != null)
        //    {
        //        if (offerTypeIds.Count > 0)
        //        {
        //            //Daliy Deals or Ending Soon offer types case (Return: No record found)
        //            //    offersDisplayView.OffersResultView = await MapToOffersResultView(cache, offersService, countryCode,
        //            //currentPage, criteria, offersDisplayView.OffersResultView);
        //            offersDisplayView.PagedOffersView = MapToPagedOffersViewModel(await serve.Instance.OffersService.PagedSearch(new dto.OfferSearchCriteria
        //            {
        //                MerchantId = criteria.MerchantId,
        //                AffiliateId = criteria.AffiliateId,
        //                Categories = criteria.Categories,
        //                OfferStatus = criteria.OfferStatus,
        //                KeyWord = criteria.KeyWord,
        //                ValidFrom = criteria.ValidFrom,
        //                ValidTo = criteria.ValidTo,
        //                CountryCode = countryCode,
        //                OfferTypes = criteria.OfferTypes,
        //                MerchantName = criteria.MerchantName,
        //                PageSize = PageSize,
        //                CurrentPage = currentPage,
        //                SortOrder = criteria.SortOrder
        //            }));
        //        }
        //    }
        //    else
        //    {
        //        //    offersDisplayView.OffersResultView = await MapToOffersResultView(cache, offersService, countryCode,
        //        //currentPage, criteria, offersDisplayView.OffersResultView);
        //        offersDisplayView.PagedOffersView = MapToPagedOffersViewModel(await serve.Instance.OffersService.PagedSearch(new dto.OfferSearchCriteria
        //        {
        //            MerchantId = criteria.MerchantId,
        //            AffiliateId = criteria.AffiliateId,
        //            Categories = criteria.Categories,
        //            OfferStatus = criteria.OfferStatus,
        //            KeyWord = criteria.KeyWord,
        //            ValidFrom = criteria.ValidFrom,
        //            ValidTo = criteria.ValidTo,
        //            CountryCode = countryCode,
        //            OfferTypes = criteria.OfferTypes,
        //            MerchantName = criteria.MerchantName,
        //            PageSize = PageSize,
        //            CurrentPage = currentPage,
        //            SortOrder = criteria.SortOrder
        //        }));
        //    }

            

        //    return offersDisplayView;
        //}

        //OfferTypes wise display offer
        //public static async Task<OffersResultViewModel> MapToOffersResultView(string countryCode,
        //    int currentPage, dto.OfferSearchCriteria criteria, OffersResultViewModel resultModel)
        //{
        //    List<dto.OfferType> types = CacheHelper.Get<List<dto.OfferType>>(serve.Instance.Cache, Keys.Keys.OfferTypes);
        //    if (types == null || types.Count == 0)
        //    {
        //        types = await serve.Instance.OffersService.GetAllOfferType();
        //        CacheHelper.Set(serve.Instance.Cache, types, Keys.Keys.OfferTypes);
        //    }

        //    if (types != null && types.Count > 0)
        //    {
        //        foreach (dto.OfferType type in types)
        //        {
        //            switch (type.Description)
        //            {
        //                case Keys.Keys.CashbackType:
        //                    resultModel.CashbackOffers = MapToPagedOffersViewModel(await serve.Instance.OffersService.PagedSearch(new dto.OfferSearchCriteria
        //                    {
        //                        MerchantId = criteria.MerchantId,
        //                        AffiliateId = criteria.AffiliateId,
        //                        Categories = criteria.Categories,
        //                        OfferStatus = criteria.OfferStatus,
        //                        KeyWord = criteria.KeyWord,
        //                        ValidFrom = criteria.ValidFrom,
        //                        ValidTo = criteria.ValidTo,
        //                        CountryCode = countryCode,
        //                        OfferType = type.Id,
        //                        OfferTypes = criteria.OfferTypes,
        //                        MerchantName = criteria.MerchantName,
        //                        PageSize = PageSize,
        //                        CurrentPage = currentPage,
        //                        SortOrder = criteria.SortOrder
        //                    }));
        //                    break;
        //                case Keys.Keys.RestaurantType:
        //                    resultModel.HighStreetandRestaurantOffers = MapToPagedOffersViewModel(await serve.Instance.OffersService.PagedSearch(new dto.OfferSearchCriteria
        //                    {
        //                        MerchantId = criteria.MerchantId,
        //                        AffiliateId = criteria.AffiliateId,
        //                        Categories = criteria.Categories,
        //                        OfferStatus = criteria.OfferStatus,
        //                        KeyWord = criteria.KeyWord,
        //                        ValidFrom = criteria.ValidFrom,
        //                        ValidTo = criteria.ValidTo,
        //                        CountryCode = countryCode,
        //                        OfferType = type.Id,
        //                        OfferTypes = criteria.OfferTypes,
        //                        MerchantName = criteria.MerchantName,
        //                        PageSize = PageSize,
        //                        CurrentPage = currentPage,
        //                        SortOrder = criteria.SortOrder
        //                    }));
        //                    break;
        //                case Keys.Keys.VoucherType:
        //                    resultModel.VoucherCodeOffers = MapToPagedOffersViewModel(await serve.Instance.OffersService.PagedSearch(new dto.OfferSearchCriteria
        //                    {
        //                        MerchantId = criteria.MerchantId,
        //                        AffiliateId = criteria.AffiliateId,
        //                        Categories = criteria.Categories,
        //                        OfferStatus = criteria.OfferStatus,
        //                        KeyWord = criteria.KeyWord,
        //                        ValidFrom = criteria.ValidFrom,
        //                        ValidTo = criteria.ValidTo,
        //                        CountryCode = countryCode,
        //                        OfferType = type.Id,
        //                        OfferTypes = criteria.OfferTypes,
        //                        MerchantName = criteria.MerchantName,
        //                        PageSize = PageSize,
        //                        CurrentPage = currentPage,
        //                        SortOrder = criteria.SortOrder
        //                    }));
        //                    break;
        //                case Keys.Keys.StandardType:
        //                    resultModel.StandardOffers = MapToPagedOffersViewModel(await serve.Instance.OffersService.PagedSearch(new dto.OfferSearchCriteria
        //                    {
        //                        MerchantId = criteria.MerchantId,
        //                        AffiliateId = criteria.AffiliateId,
        //                        Categories = criteria.Categories,
        //                        OfferStatus = criteria.OfferStatus,
        //                        KeyWord = criteria.KeyWord,
        //                        ValidFrom = criteria.ValidFrom,
        //                        ValidTo = criteria.ValidTo,
        //                        CountryCode = countryCode,
        //                        OfferType = type.Id,
        //                        OfferTypes = criteria.OfferTypes,
        //                        MerchantName = criteria.MerchantName,
        //                        PageSize = PageSize,
        //                        CurrentPage = currentPage,
        //                        SortOrder = criteria.SortOrder
        //                    }));
        //                    break;
        //                case Keys.Keys.SalesType:
        //                    resultModel.SalesOffers = MapToPagedOffersViewModel(await serve.Instance.OffersService.PagedSearch(new dto.OfferSearchCriteria
        //                    {
        //                        MerchantId = criteria.MerchantId,
        //                        AffiliateId = criteria.AffiliateId,
        //                        Categories = criteria.Categories,
        //                        OfferStatus = criteria.OfferStatus,
        //                        KeyWord = criteria.KeyWord,
        //                        ValidFrom = criteria.ValidFrom,
        //                        ValidTo = criteria.ValidTo,
        //                        CountryCode = countryCode,
        //                        OfferType = type.Id,
        //                        OfferTypes = criteria.OfferTypes,
        //                        MerchantName = criteria.MerchantName,
        //                        PageSize = PageSize,
        //                        CurrentPage = currentPage,
        //                        SortOrder = criteria.SortOrder
        //                    }));
        //                    break;
        //            }
        //        }
        //    }
        //    return resultModel;
        //}

        //offersDetail view for main search text
        //public static async Task<OffersDisplayViewModel> MapToSearchDisplayOffer(
        //    OffersDisplayViewModel offersDisplayView, string countryCode, string searchTerm, int currentPage)
        //{
        //    List<dto.Category> categories = serve.Instance.PCategoryService.GetAll();
        //    foreach (dto.Category category in categories)
        //    {
        //        category.IsActive = false;
        //    }
        //    List<dto.OfferType> offerTypes = await serve.Instance.OffersService.GetAllOfferType();
        //    foreach (dto.OfferType offerType in offerTypes)
        //    {
        //        offerType.IsActive = false;
        //    }
        //    offersDisplayView.Categories = categories;
        //    offersDisplayView.OfferTypes = offerTypes;
        //    List<dto.Merchant> merchants = await serve.Instance.MerchantService.GetAll();
        //    List<SelectListItem> ListofMerchantName = merchants.Select(merchantNew => new SelectListItem()
        //    {
        //        Text = merchantNew.Name,
        //        Value = merchantNew.Id.ToString()
        //    }).ToList();
        //    offersDisplayView.ListOfMerchantName = ListofMerchantName;
        //    //offersDisplayView.OffersResultView = await MapToOffersSearchResultView(cache, offersService, countryCode,
        //    //    currentPage, searchTerm, offersDisplayView.OffersResultView);
        //    offersDisplayView.PagedOffersView = MapToPagedOffersViewModel(await serve.Instance.OffersService.PagedMainSearch(new dto.OfferSearchCriteria
        //    {
        //        KeyWord = searchTerm,
        //        CountryCode = countryCode,
        //        PageSize = PageSize,
        //        CurrentPage = currentPage
        //    }));

        //    return offersDisplayView;
        //}

        //OffersResultView for Main Search text
        //public static async Task<OffersResultViewModel> MapToOffersSearchResultView(IMemoryCache cache, IOffersService offersService, string countryCode,
        //    int currentPage, string searchTerm, OffersResultViewModel resultModel)
        //{
        //    List<dto.OfferType> types = CacheHelper.Get<List<dto.OfferType>>(cache, Keys.Keys.OfferTypes);
        //    if (types == null || types.Count == 0)
        //    {
        //        types = await offersService.GetAllOfferType();
        //        CacheHelper.Set(cache, types, Keys.Keys.OfferTypes);
        //    }

        //    if (types != null && types.Count > 0)
        //    {
        //        foreach (dto.OfferType type in types)
        //        {
        //            switch (type.Description)
        //            {
        //                case Keys.Keys.CashbackType:
        //                    resultModel.CashbackOffers = MapToPagedOffersViewModel(await offersService.PagedMainSearch(new dto.OfferSearchCriteria
        //                    {
        //                        KeyWord = searchTerm,
        //                        CountryCode = countryCode,
        //                        OfferType = type.Id,
        //                        PageSize = PageSize,
        //                        CurrentPage = currentPage
        //                    }));
        //                    break;
        //                case Keys.Keys.RestaurantType:
        //                    resultModel.HighStreetandRestaurantOffers = MapToPagedOffersViewModel(await offersService.PagedMainSearch(new dto.OfferSearchCriteria
        //                    {
        //                        KeyWord = searchTerm,
        //                        CountryCode = countryCode,
        //                        OfferType = type.Id,
        //                        PageSize = PageSize,
        //                        CurrentPage = currentPage
        //                    }));
        //                    break;
        //                case Keys.Keys.VoucherType:
        //                    resultModel.VoucherCodeOffers = MapToPagedOffersViewModel(await offersService.PagedMainSearch(new dto.OfferSearchCriteria
        //                    {
        //                        KeyWord = searchTerm,
        //                        CountryCode = countryCode,
        //                        OfferType = type.Id,
        //                        PageSize = PageSize,
        //                        CurrentPage = currentPage
        //                    }));
        //                    break;
        //                case Keys.Keys.StandardType:
        //                    resultModel.StandardOffers = MapToPagedOffersViewModel(await offersService.PagedMainSearch(new dto.OfferSearchCriteria
        //                    {
        //                        KeyWord = searchTerm,
        //                        CountryCode = countryCode,
        //                        OfferType = type.Id,
        //                        PageSize = PageSize,
        //                        CurrentPage = currentPage
        //                    }));
        //                    break;
        //                case Keys.Keys.SalesType:
        //                    resultModel.SalesOffers = MapToPagedOffersViewModel(await offersService.PagedMainSearch(new dto.OfferSearchCriteria
        //                    {
        //                        KeyWord = searchTerm,
        //                        CountryCode = countryCode,
        //                        OfferType = type.Id,
        //                        PageSize = PageSize,
        //                        CurrentPage = currentPage
        //                    }));
        //                    break;
        //            }
        //        }
        //    }
        //    return resultModel;
        //}

        //OffersDetail view for pagination of main search
        public static async Task<PagedOffersViewModel> MapToSearchOffersDetailView(string countryCode,
           int currentPage, dto.OfferSearchCriteria criteria)
        {
            PagedOffersViewModel viewModel = MapToPagedOffersViewModel(await serve.Instance.OffersService.PagedMainSearch(new dto.OfferSearchCriteria
            {
                KeyWord = criteria.KeyWord,
                CountryCode = countryCode,
                OfferType = criteria.OfferType,
                PageSize = PageSize,
                CurrentPage = currentPage
            }));
            return viewModel;
        }

        //Paged Result for Search Term
        public static async Task<PagedOffersViewModel> MainSearchAndMapOffer(string countryCode,
            dto.OfferSearchCriteria criteria)
        {
            PagedOffersViewModel model = await MapToSearchOffersDetailView(countryCode, criteria.CurrentPage, criteria);
            return model;
        }

        public static async Task MaptoGetPagedBranchContact (int currentPage, int merchantId, PagedBranchContactViewModel model)
        {
            dto.PagedResult<dto.MerchantBranch> paging = await serve.Instance.MerchantBranchService.GetPagedResult(merchantId, currentPage, BranchPageSize);
            model.MerchantBranches = paging.Results.ToList();
            model.PagingView.CurrentPage = paging.CurrentPage;
            model.PagingView.PageCount = paging.PageCount;
            model.PagingView.PageSize = paging.PageSize;
            model.PagingView.RowCount = paging.RowCount;
        }
        
        #region Private Methods

        private static IEnumerable<dto.Public.OfferSummary> MapToOfferSummary(dto.OfferList offer)
        {
            return (from item in offer.OfferListItems
                    where item.Offer?.Merchant != null
                    select new dto.Public.OfferSummary
                    {
                        OfferId = item.OfferId,
                        OfferHeadline = item.Offer?.Headline,
                        OfferShortDescription = item.Offer?.ShortDescription,
                        OfferTerms = item.Offer?.Terms,
                        OfferExclusions = item.Offer?.Exclusions,
                        OfferCode = item.Offer?.OfferCode,
                        MerchantId = item.Offer.Merchant.Id,
                        MerchantName = item.Offer?.Merchant?.Name,
                        MerchantLogoPath = item.Offer.Merchant?.MerchantImages?.FirstOrDefault()?.ImagePath,
                    }).ToList();
        }

        private static PagedOffersViewModel MapToPagedOffersViewModel(dto.PagedResult<dto.Public.OfferSummary> result)
        {
            return new PagedOffersViewModel
            {
                OfferSummaries = result.Results.ToList(),
                CurrentPageNumber = result.CurrentPage,
                PagingView = new PagingViewModel
                {
                    CurrentPage = result.CurrentPage,
                    PageCount = result.PageCount,
                    PageSize = result.PageSize,
                    RowCount = result.RowCount
                }
            };
        }

        private static List<int> CategoryStringArrayToList(String[] request, bool parentCategory)
        {
            List<int> response = new List<int>();
            foreach (string str in request)
            {
                dto.Category category = serve.Instance.PCategoryService.GetByUrlSlug(str);
                if (category != null)
                {
                    response.Add(category.Id);
                    if (parentCategory || category.ParentId == 0)
                    {
                        List<dto.Category> categories = serve.Instance.PCategoryService.GetByParentId(category.Id);
                        foreach (dto.Category subCategory in categories)
                        {
                            response.Add(subCategory.Id);
                        }
                    }
                }
            }
            return response;
        }

        private static async Task<List<int>> OfferTypeStringArrayToList(string[] request)
        {
            List<int> response = new List<int>();
            foreach (string str in request)
            {
                dto.OfferType offerType = await serve.Instance.OffersService.GetOfferType(str);
                if (offerType != null)
                {
                    response.Add(offerType.Id);
                }
            }
            return response;
        }
        
        #endregion
    }
}