namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class ErrorTransferStagingOfferViewModel
    {
        public int Id { get; set; }

        public int MerchantId { get; set; }

        public int AffiliateId { get; set; }

        public int OfferTypeId { get; set; }

        public int StatusId { get; set; }

        public string ValidFrom { get; set; }

        public string ValidTo { get; set; }

        public int Validindefinately { get; set; }
        
        public string ShortDescription { get; set; }
        
        public string LongDescription { get; set; }
        
        public string Instructions { get; set; }
        
        public string Terms { get; set; }
        
        public string Exclusions { get; set; }
        
        public string LinkUrl { get; set; }
        
        public string OfferCode { get; set; }

        public int Reoccuring { get; set; }

        public int SearchRanking { get; set; }

        public string Datecreated { get; set; }
        
        public string Headline { get; set; }

        public string ErrorMessage { get; set; }
    }
}
