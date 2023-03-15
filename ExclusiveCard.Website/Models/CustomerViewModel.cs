using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace ExclusiveCard.Website.Models
{
    public class CustomerViewModel
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "Title is required")]
        //[DisplayName("*Title:")]

        //public string Title { get; set; }

        [Required(ErrorMessage = "Forename is required")]
        [DisplayName("*Forename:")]

        public string Forename { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [DisplayName("*Surname:")]

        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DisplayName("*Email:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Email is not valid")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Confirm Email is required")]
        [DisplayName("*Confirm Email:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Confirm Email is not valid email address")]
        [Compare("Email", ErrorMessage = "Email and confirm email do not match.")]
        public string Confirmemail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&^#])[A-Za-z\\d@$!%*^?&#]{8,48}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        [DisplayName("*Password:")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
        [DisplayName("*Confirm Password:")]
        public string ConfirmPassword { get; set; }

        //[Required(ErrorMessage = "Date of Birth is required")]
        [DisplayName("*Date Of Birth:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [ValidDateCheck(ErrorMessage = "Date must be after 01 Jan 1900 and before or equal to current date")]
        public DateTime? Dateofbirth { get; set; }
        //[RegularExpression("^[A-Z]{1,2}[0-9][A-Z0-9]? ?[0-9][A-Z]{2}$")]
        //[Required(ErrorMessage = "Postcode is required")]
        //[DisplayName("*Postcode:")]

        //public string Postcode { get; set; }

       
        //[Required(ErrorMessage = "Question is required")]
        //[DisplayName("*Choose a Question:")]
        //public int QuestionId { get; set; }
        //public List<SelectListItem> ListofQuestion { get; set; }
       

        //[Required(ErrorMessage = "Answer is required")]
        //[DisplayName("*Security Question Answer")]

        //public string Answer { get; set; }

        public bool Tick { get; set; }


        [Required(ErrorMessage = "Address 1 is required")]
        [DisplayName("*Address 1:")]

        public string Addressone { get; set; }

        
        [DisplayName("Address 2:")]

        public string Addresstwo { get; set; }

       
        [DisplayName("Address 3:")]

        public string Addressthree { get; set; }

        [Required(ErrorMessage = "Town is required")]
        [DisplayName("*Town:")]

        public string Town { get; set; }

        [DisplayName("County:")]

        public string County { get; set; }


        [Required(ErrorMessage = "Country is required")]
        [DisplayName("*Country:")]
        public string CountryId { get; set; }
        public List<SelectListItem> ListofCountry { get; set; }

        public string Token { get; set; }

        public int MembershipPlanId { get; set; }

        public string SubscribeAppRef { get; set; }

        public string SubscribeAppAndCardRef { get; set; }

        public decimal CardCost { get; set; }

        public PaymentViewModel PaymentProvider { get; set; }

        public string PartnerLogo { get; set; }
        public string PlanDescription { get; set; }
        public List<Services.Models.DTOs.MembershipPlanBenefits> PlanBenefits { get; set; }
        public Services.Models.DTOs.TermsConditions TermsAndConditions { get; set; }
        [DisplayName("National Insurance Number")]
        public string NationalInsuranceNumber { get; set; }

        public bool PaidByEmployer { get; set; }
        public string OneOfPaymentRef { get; set; }
        public decimal MinimumValue { get; set; }
        public decimal PaymentFee { get; set; }
        public bool DiamondUpgrade { get; set; }
        public bool InstantBoost { get; set; }
        public bool MarketingPreference { get; set; }

        public int? SiteClanId { get; set; }

        public CustomerViewModel()
        {
            //ListofQuestion = new List<SelectListItem>();
            ListofCountry = new List<SelectListItem>();
            PaymentProvider = new PaymentViewModel();
            PlanBenefits = new List<Services.Models.DTOs.MembershipPlanBenefits>();
            TermsAndConditions = new Services.Models.DTOs.TermsConditions();
        }

        public string EncryptedPassword { get; set; }

        public string ToJson()
        {
            //Now encrypt password and set to EncryptedPassword
            EncryptedPassword = Encrypt(Password);
            Password = "Dummy@1234";
            ConfirmPassword = "Dummy@1234";
            //PropertyRenameAndIgnoreSerializerContractResolver jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();
            //jsonResolver.IgnoreProperty(typeof(CustomerViewModel), "Password");
            //jsonResolver.IgnoreProperty(typeof(CustomerViewModel), "ConfirmPassword");

            //JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            //{
            //    ContractResolver = jsonResolver
            //};
            //Now serialise the entity to Json  this
            return JsonConvert.SerializeObject(this);
        }

        public CustomerViewModel FromJson(string data)
        {
            //Deserialise the entity to this entity,
            CustomerViewModel model =
                JsonConvert.DeserializeObject<CustomerViewModel>(data);
            //Drcrypt the EncryptedPassword and set that to Password
            model.Password = Decrypt(model.EncryptedPassword);
            model.ConfirmPassword = Decrypt(model.EncryptedPassword);
            //and return the entity
            return model;
        }

        private string Encrypt(string encryptString)
        {
            string encryptionKey = "A0B1C2D3E4FGH5IJK6LMNO7PQRST8UVWXY9Z";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        private static string Decrypt(string cipherText)
        {
            string encryptionKey = "A0B1C2D3E4FGH5IJK6LMNO7PQRST8UVWXY9Z";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
