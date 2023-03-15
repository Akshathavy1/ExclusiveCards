using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;
using Db = ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using ExclusiveCard.Enums;
using System;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ExclusiveCard.Managers
{
    /// <summary>
    /// The UserManager is responsible for user account management, providing a wrapper around Microsoft Identity and the ASPNetUsers table(s)
    /// It includes methods for login/logout, role validation and  account creation  (ASPNETUser + ASPNETRoles only)
    /// Handles various roles including Website Users (User), Admin and Partner
    /// Partners, website users and admin users each require an ASPNETUser account to login via Websites or APIs
    /// Entity specific tables such as Partner or Customer will include a foreign key to the ASPNETUsers table, and have a separate
    /// manager class (PartnerManager , CustomerManager ).
    /// </summary>
    public class UserManager : IUserManager
    {
        #region Private Members and Constructor

        private readonly string _Issuer;
        private readonly string _Secret;
        private int _tokenDuration = 60;

        private readonly IConfiguration _settings;
        private readonly SignInManager<Db.ExclusiveUser> _signInManager;
        private readonly UserManager<Db.ExclusiveUser> _identityUserManager;
        private IRepository<Db.ExclusiveUser> _identityRepo;
        private readonly IMapper _mapper;
        private IRepository<Db.LoginUserToken> _tokenRepo;
        private IRepository<Db.Partner> _partnerRepo;
        private readonly IMembershipManager _membershipManager;
        private readonly IWhiteLabelManager _whiteLabel;

        public UserManager(SignInManager<Db.ExclusiveUser> signInManager,
                            UserManager<Db.ExclusiveUser> userManager,
                            IRepository<Db.ExclusiveUser> identityRepo,
                            IRepository<Db.LoginUserToken> tokenRepo,
                            IRepository<Db.Partner> partnerRepo,
                            IMapper mapper
                            , IConfiguration settings, IMembershipManager membershipManager, IWhiteLabelManager whiteLabel
            )
        {
            _signInManager = signInManager;
            _identityUserManager = userManager;
            _identityRepo = identityRepo;
            _tokenRepo = tokenRepo;
            _partnerRepo = partnerRepo;
            _mapper = mapper;
            _settings = settings;
            _whiteLabel = whiteLabel;
            _Secret = Convert.ToString(_settings["Authentication:Secret"]);
            _Issuer = Convert.ToString(_settings["Authentication:Issuer"]);
            if (int.TryParse(_settings["Authentication:TokenDuration-Minutes"], out int duration))
                _tokenDuration = duration;
            _membershipManager = membershipManager;
        }

        #endregion Private Members and Constructor

        #region public methods

        public async Task<SignInResult> AdminLoginAsync(string userName, string password)
        {
            SignInResult result = null;

            // Check user role is assigned
            var user = _identityRepo.Get(x => x.UserName == userName);
            var userRoles = await _identityUserManager.GetRolesAsync(user);
            var role = userRoles?.Where(x => x.Contains(Roles.AdminUser.ToString())
                                          || x.Contains(Roles.BackOfficeUser.ToString())
                                          || x.Contains(Roles.RootUser.ToString()))
                                .FirstOrDefault();
            if (role != null)
            {
                result = await _signInManager.PasswordSignInAsync(userName, password, false, true);
            }

            return result;
        }

        public async Task<string> PartnerLoginAsync(string userName, string password, string audience)
        {
            string authToken = null;

            var user = _identityRepo.Include(c => c.Partner).FirstOrDefault(x => x.UserName == userName && x.Partner != null);

            // Check user has partner role
            // var isPartner = await _identityUserManager.IsInRoleAsync(user, Enums.Roles.PartnerAPI.ToString());
            //if (isPartner && user.Partner?.IsDeleted == false)
            //{
            // Attempt to login
            var result = await _signInManager.PasswordSignInAsync(userName, password, false, false);
            if (result.Succeeded)
            {
                // If logged in, get the auth token
                authToken = GenerateToken(userName, Enums.Roles.PartnerAPI.ToString(), audience);

                // Sign out partner - we don't need to stay logged in once the token has been prepared
                await _signInManager.SignOutAsync();
            }

            //}

            return authToken;
        }

        public async Task PartnerCustomerSignInAsync(string customerUserName)
        {
            var customer = _identityRepo.Include(c => c.Partner).FirstOrDefault(x => x.UserName == customerUserName);
            //string thePartner = null;
            //thePartner = customer.Partner?.Name;

            // Attempt to login
            if (customer != null)
            {
                // Sign in customer
                await _signInManager.SignInAsync(customer, true);
            }
        }

        public bool ValidatePartnerToken(string token, string audience)
        {
            var result = ValidateToken(token, audience);
            if (result)
            {
                var roleClaim = GetClaim(token, "Role");
                result = (string.Compare(roleClaim, Roles.PartnerAPI.ToString(), true) == 0);
            }

            return result;
        }

        public async Task<UserAccountDetails> CustomerLoginAsync(string userName, string password)
        {
            var accountDetails = new UserAccountDetails();

            // Check customer exists is assigned
            var user = _identityRepo.Include(c => c.Customer).FirstOrDefault(x => x.UserName == userName);
            if (user == null)
            {
                accountDetails.Id = -10; //user Not found
                return accountDetails;
            }

            // Check customer has User role
            var isUserRole = await _identityUserManager.IsInRoleAsync(user, Enums.Roles.User.ToString());
            if (isUserRole)
            {
                // Login
                var result = await _signInManager.PasswordSignInAsync(userName, password, false, true);
                if (result.Succeeded)
                {
                    var customer = user?.Customer;

                    //TODO:  Work out if we need these details
                    if (customer != null)
                    {
                        accountDetails.Id = customer.Id;
                        accountDetails.Title = customer.Title;
                        accountDetails.Forename = customer.Forename;
                        accountDetails.Surname = customer.Surname;
                        accountDetails.SiteClanId = customer.SiteClanId;
                        if (customer.DateOfBirth.HasValue)
                        {
                            accountDetails.DateOfBirth = customer.DateOfBirth.Value;
                        }

                        var activeCard = _membershipManager.GetActiveMembershipCard(user.Id);
                        if (activeCard != null)
                        {
                            accountDetails.MembershipLevelId = activeCard.MembershipPlan?.MembershipLevelId;
                        }
                    }
                }
                else if (result.IsLockedOut)
                {
                    accountDetails.Id = -1;
                }
            }

            return accountDetails;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Db.ExclusiveUser dbUser)
        {
            var token = await _identityUserManager.GenerateEmailConfirmationTokenAsync(dbUser);
            return token;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(ExclusiveUser user)
        {
            var dbUser = await _identityUserManager.FindByNameAsync(user.UserName);
            return await GenerateEmailConfirmationTokenAsync(dbUser);
        }

        public async Task<IdentityResult> ConfirmEmailTokenAsync(Db.ExclusiveUser user, string token)
        {
            return await _identityUserManager.ConfirmEmailAsync(user, token);
        }

        public async Task<IdentityResult> ConfirmEmailTokenAsync(ExclusiveUser user, string token)
        {
            var dbUser = await _identityUserManager.FindByNameAsync(user.UserName);
            return await _identityUserManager.ConfirmEmailAsync(dbUser, token);
        }

        public async Task<bool> CheckExistsAsync(string userName)
        {
            bool result = true;

            var user = await _identityUserManager.FindByNameAsync(userName);
            if (user == null)
                result = false;

            return result;
        }

        /// <summary>
        /// Adds a new user into the Exclusive.ASPNETUsers table.
        /// A valid password must be included or password left as null
        /// Passwords must follow complexity rules (include upper case, lower case, symbol and number)
        /// If password left as null a random password will be assigned and user will need to use Forgot password link to reset it
        /// </summary>
        /// <param name="user">An Exclusive User DTO</param>
        /// <param name="password">If password left as null a random password will be assigned and user will need to use Forgot password link to reset it/param>
        /// <param name="role">Name of a role to assign. Default value is "User". Roles must exist in ASPNETRoles table or an error will occur.</param>
        /// <returns>The Id of the new user record (ASPNetUsers.Id) if successful. If null, an error has happened.</returns>
        public async Task<string> CreateAsync(ExclusiveUser user, string password = null, string role = "User")
        {
            var dbUser = _mapper.Map<Db.ExclusiveUser>(user);

            // If no password provided, generate a random guid
            if (password == null)
                password = "AbC#" + DateTime.UtcNow.Ticks.ToString();

            var result = await _identityUserManager.CreateAsync(dbUser, password);
            if (result.Succeeded)
            {
                if (role != null)
                {
                    await _identityUserManager.AddToRoleAsync(dbUser, role);
                }
            }
            else
            {
                string errors = string.Empty;
                foreach (var err in result.Errors)
                {
                    errors += err.Description;
                }
                throw new Exception("Create User Failed. " + errors);
            }

            return dbUser.Id;
        }

        public async Task<ExclusiveUser> GetUserAsync(string userName)
        {
            ExclusiveUser dtoUser = null;

            var dbUser = await _identityUserManager.FindByNameAsync(userName);

            if (dbUser != null)
                dtoUser = _mapper.Map<ExclusiveUser>(dbUser);

            return dtoUser;
        }

        public async Task<ExclusiveUser> GetUserAsync(System.Security.Claims.ClaimsPrincipal principal)
        {
            ExclusiveUser dtoUser = null;

            var signedIn = _signInManager.IsSignedIn(principal);
            if (signedIn)
            {
                var dbUser = await _identityUserManager.GetUserAsync(principal);
                if (dbUser != null)
                    dtoUser = _mapper.Map<ExclusiveUser>(dbUser);
            }

            return dtoUser;
        }

        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            claimType = claimType.ToLower();
            var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;

            return stringClaimValue;
        }

        #endregion public methods

        #region private methods

        private string GenerateToken(string userName, string role, string audience)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_Secret));
            var expiryDate = DateTime.UtcNow.AddMinutes(_tokenDuration);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, userName)
                      , new Claim(ClaimTypes.Role.ToString(), role) }),
                Expires = expiryDate,
                Issuer = _Issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool ValidateToken(string token, string audience)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_Secret));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _Issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        //private LoginToken CreateLoginToken(string AspNetUserId, long loginTimeOutMinutes)
        //{
        //    DateTime expiryDate = DateTime.UtcNow.AddMinutes(loginTimeOutMinutes);

        //    LoginToken token = new LoginToken()
        //    {
        //        AspNetUserId = AspNetUserId,
        //        ExpiresTimestamp = expiryDate
        //    };
        //    string authToken = token.ToBase64String();

        //    token.TokenValue = Guid.NewGuid();

        //    var tokenRecord = new Db.LoginUserToken()
        //        {
        //            AspNetUserId = AspNetUserId,
        //            Token = authToken,
        //            TokenValue = token.TokenValue
        //    };
        //        _tokenRepo.Create(tokenRecord);
        //        _tokenRepo.SaveChanges();

        //    return token;
        //}

        #endregion private methods
    }
}