using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class TamDataModel
    {
        public int FileId { get; set; }
        public string TransType { get; set; }
        public string UniqueReference { get; set; }
        public string FundType { get; set; }
        [MaxLength(20)]
        public string Title { get; set; }
        [MaxLength(50)]
        public string Forename { get; set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        [MaxLength(10)]
        public string NINumber { get; set; }
        public decimal Amount { get; set; }
        public string IntroducerCode { get; set; }
        public string ProcessState { get; set; }
        public string Password { get; set; }
    }
}
