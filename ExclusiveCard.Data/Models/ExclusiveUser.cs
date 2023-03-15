using Microsoft.AspNetCore.Identity;

namespace ExclusiveCard.Data.Models
{
    public class ExclusiveUser : IdentityUser
    {
        public virtual Customer  Customer { get; set; }
        public virtual Partner Partner { get; set; }

    }
}
