using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.Data.Models
{
    [Table("ContactDetail", Schema = "Exclusive")]
    public class ContactDetail
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string Address1 { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string Address2 { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string Address3 { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string Town { get; set; }

        [MaxLength(128)]
        [DataType("nvarchar")]
        public string District { get; set; }

        [MaxLength(16)]
        [DataType("nvarchar")]
        public string PostCode { get; set; }

        [MaxLength(3)]
        [DataType("nvarchar")]
        public string CountryCode { get; set; }

        [MaxLength(16)]
        [DataType("nvarchar")]
        public string Latitude { get; set; }


        [MaxLength(16)]
        [DataType("nvarchar")]
        public string Longitude { get; set; }

        [MaxLength(16)]
        [DataType("nvarchar")]
        public string LandlinePhone { get; set; }

        [MaxLength(16)]
        [DataType("nvarchar")]
        public string MobilePhone { get; set; }

        [MaxLength(512)]
        [DataType("nvarchar")]
        public string EmailAddress { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<Merchant> Merchants { get; set; }

        public virtual ICollection<MerchantBranch> MerchantBranches { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }

        public virtual ICollection<BankDetail> BankDetails { get; set; }

        public virtual ICollection<Partner> Partners { get; set; } 
    }
}
