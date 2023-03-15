using Newtonsoft.Json;

namespace ExclusiveCard.Website.Models
{
    public class PlanUpgradeViewModel
    {
        public int MembershipPlanId { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public PlanUpgradeViewModel FromJson(string data)
        {
            //Deserialise the entity to this entity,
            PlanUpgradeViewModel model =
                JsonConvert.DeserializeObject<PlanUpgradeViewModel>(data);
            //and return the entity
            return model;
        }
    }

}
