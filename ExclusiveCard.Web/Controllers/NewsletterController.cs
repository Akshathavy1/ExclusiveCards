using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.Controllers
{
    [Authorize(Roles = "AdminUser, BackOfficeUser")]
    [SessionTimeout]
    public class NewsletterController : BaseController
    {
        #region Private Members
        private readonly IMarketingService _marketingService;

        #endregion

        #region Constructor

        public NewsletterController( IMarketingService marketingService
                                   )
        {
            _marketingService = marketingService;
        }

        #endregion
        #region Public Members

        public async Task<IActionResult> Index()
        {
            NewsletterViewModel newsletter=new  NewsletterViewModel();
            newsletter.ListofNewsletterName = new List<SelectListItem>();
            newsletter = await GetAllNewsletter();
            
            return View("AmendNewsletters", newsletter);
        }
       
        async Task<NewsletterViewModel> GetAllNewsletter()
        {
            NewsletterViewModel newsletterVM = new NewsletterViewModel();
            newsletterVM.ListofNewsletterName = new List<SelectListItem>();
            var newsletters = await _marketingService.GetNewsletters(); // _newsletterService.GetAllNewsletterNames();
            if (newsletters != null)
            {
                await Task.WhenAll(newsletters.Select(async (newsletter) =>
                {
                    newsletterVM.ListofNewsletterName.Add(
                            new SelectListItem()
                            {
                                Text = newsletter.Name,
                                Value = newsletter.Id.ToString()
                            });
                    await Task.CompletedTask;
                }));
            }
            newsletterVM.OfferListId = 0;
            newsletterVM.Enable = false;
            newsletterVM.EmailTemplateId = 0;

            return newsletterVM;
        }
       
        [HttpGet]
        [ActionName("AmendNewsletters")]
        public IActionResult AmendNewsletters()
        {
            try
            {
                NewsletterViewModel newsletter = new NewsletterViewModel();
                return View("AmendNewsletters", newsletter);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred. Please try again."));
            }
        }

        
        [HttpGet]
        [ActionName("GetEmailTemplate")]
        public async  Task<IActionResult> GetEmailTemplate(int letterId)
        {
            try
            {
                NewsletterViewModel newsLetter = new NewsletterViewModel();
                if (letterId != 0)
                {
                    newsLetter = await MapToNewsLetterViewModel(letterId);
                }
                else
                {
                    newsLetter = await GetAllNewsletter();
                }
               

                return View("AmendNewsletters", newsLetter);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred. Please try again."));
            }
        }

        [HttpGet]
        [ActionName("GetWhiteLabels")]
        public async Task<IActionResult> GetWhiteLabels(int letterId)
        {
            try
            {
                NewsletterViewModel newsLetter = new NewsletterViewModel();
                if (letterId != 0)
                {
                    newsLetter = await MapToNewsLetterViewModel(letterId);
                }
                else
                {
                    newsLetter = await GetAllNewsletter();
                }

                return View("Preview", newsLetter);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred. Please try again."));
            }
        }

        [HttpGet]
        [ActionName("RenderNewsletter")]
        public async Task<IActionResult> RenderNewsletter(int campaignId)
        {
            string html = string.Empty;
            try
            {
                var email = await _marketingService.GetTestEmail(campaignId);
                if (email != null)
                {
                    html = email.BodyHtml;
                }

                return Json(html);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred. Please try again."));
            }
        }

        [HttpGet]
        [ActionName("SendTestEmail")]
        public async Task<IActionResult> SendTestEmail(int campaignId, string recipient)
        {
           string res = string.Empty;
            try
            {
                var email = await _marketingService.GetTestEmail(campaignId);

                if (email != null)
                {
                    email.EmailTo = new List<string>() { recipient };
                    res = await _marketingService.SendEmail(email);
                }
                return Json(res);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred. Please try again."));
            }
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save(NewsletterViewModel request)
        {
            // Method to Save Newsletter Details
            try
            {
                if (request.Id == 0)
                {
                    //response = await _newsletterService.Add(letter);
                    throw new Exception("newsletter does not exist");
                }
                else
                {
                    Newsletter response = new Newsletter();
                    var templateResponse = new EmailTemplate();

                    Newsletter letter = new Newsletter
                    {
                        Id = request.Id,
                        Name = request.Name,
                        //NewsLetterId = request.NewsLetterId,
                        Schedule = request.Schedule,
                        Enabled = request.Enable,
                        EmailTemplateId = request.EmailTemplateId,
                        OfferListId = request.OfferListId,
                    };
                    IList<MarketingCampaign> newsletterCampaignList = new List<MarketingCampaign>();
                    foreach (var item in request.MarketingCampaigns)
                    {
                        MarketingCampaign ncl = new MarketingCampaign();
                        ncl.Id = item.Id;
                        ncl.Enabled = item.Enabled;
                        ncl.NewsletterId = letter.Id;
                        newsletterCampaignList.Add(ncl);
                    }
                    EmailTemplate emailTemplate = new EmailTemplate
                    {
                        Id = request.EmailTemplateId,
                        TemplateTypeId = request.TemplateTypeId,
                        Subject = request.Subject,
                        BodyHtml = request.BodyHtml,
                        BodyText = request.BodyText,
                        HeaderHtml = request.HeaderHtml,
                        FooterHtml = request.FooterHtml,
                        IsDeleted = false,
                        EmailName = request.EmailName
                    };

                    response = await _marketingService.Update(letter);
                    if (response != null)
                    {
                        templateResponse = await _marketingService.Update(emailTemplate);
                        var tmp = await _marketingService.Update(newsletterCampaignList.ToList());
                        request.MarketingCampaigns = tmp;

                        //Update provider with new details...
                        await _marketingService.ManageMarketingContacts();
                        await _marketingService.ManageMarketingEvents();

                    }

                }
                return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error Saving Newsletter"));
            }
        }

        public async Task<IActionResult> Preview()
        {
            NewsletterViewModel newsletter = new NewsletterViewModel();
            newsletter.ListofNewsletterName = new List<SelectListItem>();
            newsletter = await GetAllNewsletter();

            return View("Preview", newsletter);
        }

        #endregion 

        private async Task<NewsletterViewModel> MapToNewsLetterViewModel(int letterId)
        {
            try
            {
                NewsletterViewModel model = new NewsletterViewModel();
                if (letterId > 0)
                {
                    model.WhiteLabelSettings = await _marketingService.GetWhiteLabelsByNewsletter(letterId);
                    model.MarketingCampaigns = await _marketingService.GetCampaignsByNewsletter(letterId);
                    model.ListofNewsletterName = await MapNewslettersToList();
                    var letter = await _marketingService.GetNewsLetter(letterId);
                    model.OfferListId = letter.OfferListId;
                    model.Enable = letter.Enabled;
                    model.EmailTemplateId = letter.EmailTemplateId;
                    model.Id = letterId;
                    model.Schedule = letter.Schedule;
                    var template = await _marketingService.GetEmailTemplateById(model.EmailTemplateId);
                    model.Subject = template.Subject;
                    model.BodyHtml = template.BodyHtml;
                    model.BodyText = template.BodyText;
                    model.HeaderHtml = template.HeaderHtml;
                    model.FooterHtml = template.FooterHtml;
                    model.TemplateTypeId = template.TemplateTypeId;
                    model.EmailName = template.EmailName;
                   
                }

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<SelectListItem>> MapNewslettersToList()
        {
            var newsletters = await _marketingService.GetNewsletters();
            var listofNewsletterNames = new List<SelectListItem>();

            await Task.WhenAll(newsletters.Select(async (newsletter) =>
            {
                listofNewsletterNames.Add( new SelectListItem()
                                            {
                                                Text = newsletter.Name,
                                                Value = newsletter.Id.ToString()
                                            });
                await Task.CompletedTask;
            }));

            return listofNewsletterNames;
        }

        //private IList<WhiteLabelSettings> MapToWhiteLabelSetting(IList<NewsletterCampaignLink> settings)
        //{
        //    IList<WhiteLabelSettings> whiteLabelSetting = new List<WhiteLabelSettings>();

        //    foreach (var item in settings)
        //    {
        //        //if (!item.WhiteLabelName.ToLower().Contains(" slot")) //Exclude the deployment slot
        //        {
        //            var white = new WhiteLabelSettings();
        //            white.Name = item.WhiteLabelName;
        //            whiteLabelSetting.Add(white);
        //        }
        //    }
        //    return whiteLabelSetting;
        //}

       
    }
}
