using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ExclusiveCard.Website.Models
{
    public class AddOnCardViewModel
    {
        [Required]
        public int Id { get; set; }
        public string Token { get; set; }
        public int MembershipPlanId { get; set; }
        public string SubscribeAppRef { get; set; }
        public string SubscribeAppAndCardRef { get; set; }
        public decimal CardCost { get; set; }
        public PaymentViewModel PaymentProvider { get; set; }
        public bool PhysicalCardRequested { get; set; }
        public string CountryId { get; set; }

        public AddOnCardViewModel()
        {
            PaymentProvider = new PaymentViewModel();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public AddOnCardViewModel FromJson(string data)
        {
            return JsonConvert.DeserializeObject<AddOnCardViewModel>(data);
        }
    }
}
