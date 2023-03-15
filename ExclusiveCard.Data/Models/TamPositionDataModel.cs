using System;

namespace ExclusiveCard.Data.Models
{
    public class TamPositionDataModel
    {
        public DateTime VALUATION_DATE { get; set; }
        public string VALUATION_TYPE { get; set; }
        public string PROVIDERREFERENCE { get; set; }
        public string PORTFOLIO_CCY { get; set; }
        public string ACCOUNT { get; set; }
        public string ASSET_SEDOL_CODE { get; set; }
        public string ASSET_ISIN_CODE { get; set; }
        public string ASSET_CCY { get; set; }
        public string ASSET_NAME { get; set; }
        public string ASSET_CLASS { get; set; }
        public string ASSET_DOM { get; set; }
        public decimal UNIT_AMOUNT { get; set; }
        public decimal UNIT_PRICE { get; set; }
        public decimal ASSET_VALUE { get; set; }
        public int DI { get; set; }
    }
}
