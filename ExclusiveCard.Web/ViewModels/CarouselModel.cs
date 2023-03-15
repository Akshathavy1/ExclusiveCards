using System.Collections.Generic;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class CarouselModel
    {
        public List<string> Images { get; set; }
        public HttpPostedFileBase File { get; set; }

        public CarouselModel()
        {
            Images = new List<string>();
        }
    }
}
