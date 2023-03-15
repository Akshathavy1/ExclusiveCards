namespace ExclusiveCard.Services.Models.DTOs
{
   public class CustomerSecurityQuestion
    {
        public int CustomerId { get; set; }

        public int SecurityQuestionId { get; set; }

        public string Answer { get; set; }

        public  Customer Customer { get; set; }

        public  SecurityQuestion SecurityQuestion { get; set; }
    }
}
