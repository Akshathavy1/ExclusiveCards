using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs.StagingModels
{
    public class OfferImportError
    {
        public int Id { get; set; }

        public string ErrorMessage { get; set; }

        public int AffiliateId { get; set; }

        public int AffiliateMappingRuleId { get; set; }

        public string AffiliateValue { get; set; }

        public int OfferImportFileId { get; set; }

        public int OfferImportRecordId { get; set; }

        public DateTime ErrorDateTime { get; set; }

    }
}
