using System;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class MerchantImage
    {
        public int Id { get; set; }

        public int MerchantId { get; set; }

        public string ImagePath { get; set; }

        public short DisplayOrder { get; set; }

        public DateTime TimeStamp { get; set; }
        public int ImageType { get; set; }

        public Merchant Merchant { get; set; }
    }
}
