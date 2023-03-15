using System;

namespace ExclusiveCard.Data.Models
{
    public class SPCustomerSearch
    {
        public Int64 TotalRecord { get; set; }
        public Int64 RowNumber { get; set; }
        public int Id { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PostCode { get; set; }
        public string UserName { get; set; }
        public string CardNumber { get; set; }
    }
}
