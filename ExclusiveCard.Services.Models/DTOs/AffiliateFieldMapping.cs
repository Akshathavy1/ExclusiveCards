namespace ExclusiveCard.Services.Models.DTOs
{
    public class AffiliateFieldMapping
    {
        public int Id { get; set; }

        //public int AffiliateFileId { get; set; }

        public int AffiliateFileMappingId { get; set; }

        public string AffiliateFieldName { get; set; }

        public string ExclusiveTable { get; set; }

        public string ExclusiveFieldName { get; set; }

        public bool IsList { get; set; }

        public string Delimiter { get; set; }

        //public bool IsDataMapped { get; set; }

        public int? AffiliateMappingRuleId { get; set; }

        public int AffiliateTransformId { get; set; }

        public int AffiliateMatchTypeId { get; set; }

        //public AffiliateFile AffiliateFile { get; set; }
        public AffiliateMappingRule AffiliateMappingRule { get; set; }

        public AffiliateFileMapping AffiliateFileMapping { get; set; }
    }
}
