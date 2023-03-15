using System;
using AutoMapper;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Managers;
using ExclusiveCard.Services.Interfaces.Public;
using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;
using data = ExclusiveCard.Data.Models;

namespace ExclusiveCard.Services.Public
{
    public class WhiteLabelService : IWhiteLabelService
    {
        #region Private members and constructor

        //private readonly IMapper _mapper;
        private readonly IWhiteLabelManager _whiteLabelManager;

        public WhiteLabelService(IWhiteLabelManager manager)
        {
            _whiteLabelManager = manager;
        }

        //public IList<dto.WhiteLabelSettings> GetAllSiteSettings(int newsLetterId)
        //{
        //    return _whiteLabelManager.GetAll();
        //}

        public IList<dto.WhiteLabelSettings> GetAll()
        {
            return _whiteLabelManager.GetAll();
        }

        public IList<dto.WhiteLabelSettings> GetRegionSites()
        {
            return _whiteLabelManager.GetAll();
        }

        #endregion Private members and constructor

        public dto.WhiteLabelSettings GetSiteSettings(string url)
        {
            return _whiteLabelManager.GetByUrl(url);
        }

        public string WhiteLabelString(string url, string sourceString)
        {
            var data = _whiteLabelManager.GetByUrl(url);

            if (!string.IsNullOrEmpty(sourceString))
            {
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.URL) + "}}", data.URL);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.Slug) + "}}", data.Slug);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.Logo) + "}}", data.Logo);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.Name) + "}}", data.Name);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.Address) + "}}", data.Address);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.CSSFile) + "}}", data.CSSFile);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.HelpEmail) + "}}", data.HelpEmail);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.MainEmail) + "}}", data.MainEmail);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.ClaimsEmail) + "}}", data.ClaimsEmail);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.DisplayName) + "}}", data.DisplayName);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.CompanyNumber) + "}}", data.CompanyNumber);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.CardName) + "}}", data.CardName);
                sourceString = sourceString.Replace("{{" + nameof(dto.WhiteLabelSettings.isRegional) + "}}", data.isRegional.ToString());
            }

            return sourceString;
        }

        public dto.WhiteLabelSettings GetSiteSettingsById(int id)
        {
            return _whiteLabelManager.GetSiteSettingsById(id);
        }

        public async Task<List<dto.SponsorImages>> GetSponsorImagesById(int id)
        {
            return await _whiteLabelManager.GetSponsorImagesById(id);
        }

        public async Task<bool> Update(dto.WhiteLabelSettings settings)
        {
            bool res = false;

            var result = await _whiteLabelManager.Update(settings);
            if (result != null)
            {
                res = true;
            }
            return res;
        }

        //private Data.Models.WhiteLabelSettings MapToWhiteLabelSetting(dto.WhiteLabelSettings settings)
        //{
        //    var white = new Data.Models.WhiteLabelSettings();
        //    try
        //    {
        //        white.Name = settings.Name;
        //        white.DisplayName = settings.DisplayName;
        //        white.URL = settings.URL;
        //        white.Slug = settings.Slug;
        //        white.CompanyNumber = settings.CompanyNumber;
        //        white.CSSFile = settings.CSSFile;
        //        white.Logo = settings.Logo;
        //        white.ClaimsEmail = settings.ClaimsEmail;
        //        white.HelpEmail = settings.HelpEmail;
        //        white.MainEmail = settings.MainEmail;
        //        white.Address = settings.Address;
        //        white.CardName = settings.CardName;
        //        white.PrivacyPolicy = settings.PrivacyPolicy;
        //        white.Terms = settings.Terms;
        //        white.SiteType = settings.SiteType;
        //        white.CharityName = settings.CharityName;
        //        white.CharityUrl = settings.CharityUrl;
        //        white.NewsletterLogo = settings.NewsletterLogo;
        //        white.SiteOwnerId = settings.SiteOwnerId;
        //        white.RegistrationUrl = settings.RegistrationUrl;
        //        white.Id = settings.Id;

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //    return white;
        //}
    }
}