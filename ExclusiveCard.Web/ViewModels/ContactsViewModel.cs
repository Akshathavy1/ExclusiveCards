using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class ContactsViewModel
    {
        public int? Id { get; set; }

        [DisplayName("Address 1:")]
        [MaxLength(128)]
        public string Address1 { get; set; }

        [DisplayName("Address 2:")]
        [MaxLength(128)]
        public string Address2 { get; set; }

        [MaxLength(128)]
        [DisplayName("Address 3:")]
        public string Address3 { get; set; }

        [MaxLength(128)]
        [DisplayName("Town:")]
        public string Town { get; set; }

        [MaxLength(128)]
        [DisplayName("County/District:")]
        public string District { get; set; }

        [MaxLength(16)]
        [DisplayName("Postal/Zip Code")]
        public string PostCode { get; set; }

        [MaxLength(3)]
        [DisplayName("Country:")]
        public string CountryCode { get; set; }

        [MaxLength(16)]
        [DisplayName("Latitude:")]
        public string Latitude { get; set; }


        [MaxLength(16)]
        [DisplayName("Longitude:")]
        public string Longitude { get; set; }

        [MaxLength(16)]
        [DisplayName("Landline:")]
        public string LandlinePhone { get; set; }

        [MaxLength(16)]
        [DisplayName("Mobile:")]
        public string MobilePhone { get; set; }

        [MaxLength(512)]
        [DisplayName("Email:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
