using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExclusiveCard.Services.Interfaces.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ExclusiveCard.Website.Filters
{
    public class PartnerAuthoriseAttribute: TypeFilterAttribute
    {
        public PartnerAuthoriseAttribute() : base(typeof(PartnerAuthorizeFilter))
        {

        }
    }

    public class PartnerAuthorizeFilter :  IAuthorizationFilter
    {

        private readonly IConfiguration _settings;
        private readonly IPartnerService _partnerService;

        public PartnerAuthorizeFilter(IConfiguration settings, IPartnerService partnerService)
        {
            _settings = settings;
            _partnerService = partnerService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool result = false;
            string token;
            string audience;

            //determine whether a jwt exists or not
            result= TryRetrieveToken(context.HttpContext.Request , out token, out audience);

            if (result)
            {
                result = _partnerService.ValidateLoginToken(token, audience);
            }

            if (!result)
                context.Result = new ForbidResult();
        }


        private bool TryRetrieveToken(HttpRequest request, out string token, out string audience)
        {
            token = null;
            string bearerToken = String.Empty;

            audience = request.Host.ToString();

            //IEnumerable<string> authHeaders;
            //if many tokens are returned then return false
            if (!request.Headers.ContainsKey("Authorization") || string.IsNullOrEmpty(request.Headers["Authorization"]))
            {
                return false;
            }

            bearerToken = request.Headers["Authorization"];
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        //private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        //{
        //    if (expires != null)
        //    {
        //        if (DateTime.UtcNow < expires) return true;
        //    }
        //    return false;
        //}
    }
}
