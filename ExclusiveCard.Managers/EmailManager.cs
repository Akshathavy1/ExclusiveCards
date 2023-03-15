
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Enums;
using ExclusiveCard.Providers.Email;
using ExclusiveCard.Services.Models.DTOs;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using db = ExclusiveCard.Data.Models;

namespace ExclusiveCard.Managers
{
    public class EmailManager : IEmailManager
    {
        private string _fromName;
        private string _fromEmail;
 
        private const short TO = 0;
        private const short CC = 1;
        private const short BCC = 2;

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailProvider _emailProvider;
        private readonly IRepository<db.EmailTemplate> _templateRepo;

        public EmailManager(IConfiguration configuration, IEmailProvider emailProvider, IRepository<db.EmailTemplate> templateRepo)
        {
            _configuration = configuration;
            _fromEmail = _configuration["NoReplyEmailAddress"];
            _fromName = _configuration["SenderName"];

            _emailProvider = emailProvider;
            _templateRepo = templateRepo;
            _logger = LogManager.GetLogger(GetType().FullName);
        }

        #region public methods

        public async Task<string> SendEmailAsync(Email email)
        {
            email.EmailFrom = email.EmailFrom ?? _fromEmail;
            email.EmailFromName = email.EmailFromName ?? _fromName;

            var emailSent = await _emailProvider.SendEmailAsync(email);
            return emailSent;
        }

        public async Task SendEmailAsync(string emailId, int emailTemplateType, object emailTemplateData, string subject = null, List<string> attachments = null)
        {
            try
            {
                var  emailTemplate = _templateRepo.Get(x => x.TemplateTypeId == emailTemplateType);
                if (emailTemplate != null)
                {
                    _logger.Info($"Sending email to {emailId} TemplateType={emailTemplate.EmailName}");
                    
                    var bodyHtml = ReplaceTokensInTemplate(emailTemplate.BodyHtml, emailTemplateData);
                    string bodyPlainText = null;
                    if (emailTemplate.BodyText != null)
                        bodyPlainText = ReplaceTokensInTemplate(emailTemplate.BodyText, emailTemplateData);
                    
                    Email email = new Email
                    {
                        EmailFrom = _fromEmail,
                        EmailFromName = _fromName,
                        Subject = subject ?? emailTemplate.Subject,
                        EmailTo = new List<string>() { emailId },
                        BodyHtml = bodyHtml,
                        BodyPlainText = bodyPlainText
                    };
                                      

                    await SendEmailAsync(email);
                }
                else
                {
                    _logger.Error($"EMAILPROVIDER: Email Template Type: {emailTemplateType} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _logger.Log(LogLevel.Error, ex, $"EMAILPROVIDER: Error sending email: {ex.Message}.");
            }
        }

        //public async Task<string> TestSendEmailAsync(string emailId, int emailTemplateId, object emailTemplateData, string subject = null, List<string> attachments = null)
        //{
        //    string res=String.Empty;
        //    try
        //    {

        //        var emailTemplate = _templateRepo.Get(x => x.Id == emailTemplateId);
        //        if (emailTemplate != null)
        //        {
        //            Email email = new Email
        //            {
        //                EmailFrom = _fromEmail,
        //                EmailFromName = _fromName,
        //                Subject = subject ?? emailTemplate.Subject,
        //                EmailTo = new List<string>() { emailId },
        //                BodyHtml = emailTemplateData.ToString(),
        //            };

        //           res= await SendEmailAsync(email);
        //        }
        //        else
        //        {
        //            _logger.Error($"EMAILPROVIDER: Email Template Type: {emailTemplateId} not found.");
        //        }

                
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex);
        //        _logger.Log(LogLevel.Error, ex, $"EMAILPROVIDER: Error sending email: {ex.Message}.");
        //    }
        //    return res;
        //}

      
        #endregion

        #region PrivateMethods

         private string ReplaceTokensInTemplate(string template, object tokens)
        {
            if (!string.IsNullOrEmpty(template) && tokens != null)
            {
                Type type = tokens.GetType();
                if (typeof(IDictionary).IsAssignableFrom(type))
                {
                    if (tokens is IDictionary tokensDict)
                    {
                        foreach (object key in tokensDict.Keys)
                        {
                            template = ReplaceTokenInTemplate(template, key, tokensDict[key]);
                        }
                    }
                }
                else
                {
                    foreach (PropertyInfo prop in type.GetProperties().Where(p => p.GetIndexParameters().Length == 0))
                    {
                        template = ReplaceTokenInTemplate(template, prop.Name, prop.GetValue(tokens));
                    }
                }
            }

            return template;
        }

        private static string ReplaceTokenInTemplate(string template, object name, object value)
        {
            return template.Replace($"[{name}]", value?.ToString());
        }

        public List<string> CreateFromEmailTemplate(int emailTemplateType, object emailTemplateData)
        {
            List<string> email = new List<string>();
            try
            {
                var emailTemplate = _templateRepo.Get(x => x.Id == emailTemplateType);
                if (emailTemplate != null)
                {
                    email = new List<string>() { "", "", "" };
                    var headerHtml = ReplaceTokensInTemplate(emailTemplate.HeaderHtml, emailTemplateData);
                    var bodyHtml = ReplaceTokensInTemplate(emailTemplate.BodyHtml, emailTemplateData);
                    var footerHtml = ReplaceTokensInTemplate(emailTemplate.FooterHtml, emailTemplateData);
                    string emailTemp = headerHtml + bodyHtml + footerHtml;
                    //email.Add(emailTemp);
                    email[(int)EmailTemplateSections.HTMLContent] = emailTemp;
                    //email.Add(emailTemplate.Subject);
                    email[(int)EmailTemplateSections.Subject] = emailTemplate.Subject;
                    //email.Add(emailTemplate.BodyText);
                    email[(int)EmailTemplateSections.BodyText] = emailTemplate.BodyText;
                    return email;
                }
                else
                {
                    _logger.Error($"EMAILPROVIDER: Email Template Type: {emailTemplateType} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _logger.Log(LogLevel.Error, ex, $"EMAILPROVIDER: Error sending email: {ex.Message}.");
            }
            return email;
        }

        
        #endregion
    }
}
