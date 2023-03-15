using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Website.Models
{
    public class SettingsViewModel
    {
        public int CustomerId { get; set; }
        public int? ContactDetailId { get; set; }
        //[Required(ErrorMessage = "Forename is required")]
        [DisplayName("First Name")]
        public string Forename { get; set; }
        //[Required(ErrorMessage = "SurnameR")]
        [DisplayName("Surname")]
        public string Surname { get; set; }
        //[Required(ErrorMessage = "EmailR")]
        [DisplayName("Email")]
        //[DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "EmailV")]
        public string Email { get; set; }
        //[Required(ErrorMessage = "DOBR")]
        [DisplayName("DOB")]
        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //[ValidDateCheck(ErrorMessage = "DOBCheck")]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "PostcodeE")]
        [RegularExpression("^[A-Z]{1,2}[0-9][A-Z0-9]? ?[0-9][A-Z]{2}$")]
        [DisplayName("Postcode")]
        public string Postcode { get; set; }

        [DisplayName("NiN")]
        [RegularExpression(@"^\s*[a-zA-Z]{2}(?:\s*\d\s*){6}[a-zA-Z]{1}?\s*$")]
        public string NationalInsuranceNumber { get; set; }

        [MaxLength(128)]
        [DisplayName("Address Line 1")]
        public string Address1 { get; set; }
        [MaxLength(128)]
        [DisplayName("Address Line 2")]
        public string Address2 { get; set; }
        [MaxLength(128)]
        [DisplayName("Address Line 3")]
        public string Address3 { get; set; }
        [MaxLength(128)]
        [DisplayName("District")]
        public string District { get; set; }
        [MaxLength(128)]
        [DisplayName("Town")]
        public string Town { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "CurrentError")]
        [DisplayName("Current")]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&^#])[A-Za-z\\d@$!%*?&^#]{8,48}$")]
        [Required(ErrorMessage = "NewError")]
        [DisplayName("New")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "ConfirmError")]
        [DisplayName("Confirm")]
        public string ConfirmPassword { get; set; }
        public bool MarketingNewsLetter { get; set; }

        [Required(ErrorMessage = "Question is required")]
        [DisplayName("*Choose a Question:")]
        public int QuestionId { get; set; }
        public List<SelectListItem> ListofQuestion { get; set; }


        [Required(ErrorMessage = "Answer is required")]
        [DisplayName("*Security Question Answer")]

        public string Answer { get; set; }
    }
}
