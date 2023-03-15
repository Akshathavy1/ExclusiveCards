using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class ContactDetail
    {
        public int Id { get; set; }

        public string Address1 { get; set; } = null;

        public string Address2 { get; set; } = null;

        public string Address3 { get; set; } = null;

        public string Town { get; set; }

        public string District { get; set; }

        public string PostCode { get; set; }

        public string CountryCode { get; set; }

        public string Latitude { get; set; }
        
        public string Longitude { get; set; }

        public string LandlinePhone { get; set; }

        public string MobilePhone { get; set; }

        public string EmailAddress { get; set; }

        public bool IsDeleted { get; set; }

        //TODO:  Find out WTF Merchant, bankDetails, Partners  properties are doing on a Contact Detail DTO. Need removing ASAP.
        // There is no business reason to find an address then try and look up every bank  or customer or partner that may use it. Madness. 
        // This is not a GEO Location business or Google Maps FFS
        
        public ICollection<Merchant> Merchants { get; set; }

        public ICollection<MerchantBranch> MerchantBranches { get; set; }

        public ICollection<Customer> Customers { get; set; }

        public ICollection<BankDetail> BankDetails { get; set; }

        public ICollection<PartnerDto> Partners { get; set; }
    }
}
