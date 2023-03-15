using System;

namespace ExclusiveCard.IntegrationTests.ViewModels
{
    public class TestCustomerViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string Confirmemail { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public DateTime? Dateofbirth { get; set; }

        public string Postcode { get; set; }

        public int QuestionId { get; set; }

        public string Answer { get; set; }

        public bool Tick { get; set; }
        public string Addressone { get; set; }

        public string Addresstwo { get; set; }

        public string Addressthree { get; set; }
        public string Town { get; set; }

        public string County { get; set; }
        public string CountryId { get; set; }

        public string Token { get; set; }

        public int MembershipPlanId { get; set; }

        public string SubscribeAppRef { get; set; }

        public string SubscribeAppAndCardRef { get; set; }

        public decimal CardCost { get; set; }

    }
}
