using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class MerchantImageViewModel
    {
        public int Id { get; set; }

        public int MerchantId { get; set; }

        [MaxLength(512)]
        public string ImagePath { get; set; }

        public short DisplayOrder { get; set; }
        public int ImageType { get; set; }

        public List<string> ImagePaths { get; set; }
        public MerchantImageViewModel()
        {
            ImagePaths = new List<string>();
        }
    }
}
