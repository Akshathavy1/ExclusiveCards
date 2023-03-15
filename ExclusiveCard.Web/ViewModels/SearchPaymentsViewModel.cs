using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class SearchPaymentsViewModel
    {
        [DisplayName("Payment Status")]
        public int StatusId { get; set; }
        public List<SelectListItem> Status { get; set; }
        [DisplayName("Partner")]
        public int PartnerId { get; set; }
        public List<SelectListItem> Partners { get; set; }
        public TransactionsList PartnerTransactions { get; set; }
        
        public SearchPaymentsViewModel()
        {
            Status = new List<SelectListItem>();
            Partners = new List<SelectListItem>();
        }
    }
}
