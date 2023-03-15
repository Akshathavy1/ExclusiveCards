using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class LocalOfferViewModel
    {
        public int Id { get; set; }
        public int MerchantId { get; set; }
        public string MerchantName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Action { get; set; }
    }
}
