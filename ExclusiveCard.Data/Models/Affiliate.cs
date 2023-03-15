using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Data.Models
{
    [Table("Affiliate", Schema = "Exclusive")]
   public  class Affiliate
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<AffiliateFile> AffiliateFiles { get; set; }


    }
}
