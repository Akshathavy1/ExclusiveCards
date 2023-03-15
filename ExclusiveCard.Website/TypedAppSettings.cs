namespace ExclusiveCard.Website
{
    public class TypedAppSettings
    {
        public string AppUrl { get; set; }
        public int PageSize { get; set; }
        public int HomePageSize { get; set; }
        public int BranchPageSize { get; set; }
        public int RelatedOfferPageSize { get; set; }
        public int TransactionCount { get; set; }
        public int MaxCashbackAmount { get; set; }
        public string ContainerName { get; set; }
        public string ImageCategory { get; set; }
        public string BlobConnectionString { get; set; }
        public string NoReplyEmailAddress { get; set; }
        public string SendGridAPIKey { get; set; }
        public string GoogleApiKey { get; set; }

        public string PayPalButton_SubscribeApp { get; set; }
        public string PayPalButton_SubscribeAppAndCard { get; set; }
        //public string Facebook { get; set; }
        //public string Twitter { get; set; }
        //public string Instagram { get; set; }
        public string PayPalLink { get; set; }
        public string PayPalSubscribeButton { get; set; }
        public string PayPalSubscribeText { get; set; }
        public string ActiveMembersipPlan { get; set; }
        public decimal PhysicalMembershipCard { get; set; }
        public string AdminEmail { get; set; }
        public int ReportPageSize { get; set; }
        public int OfferCount { get; set; }
        public int OffersScreenCount { get; set; }
        public string OfferSort { get; set; }
    }
}
