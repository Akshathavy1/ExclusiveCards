using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class MerchantViewModel
    {
        [DisplayName("ID:")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DisplayName("*Name:")]
        [MaxLength(128)]
        public string Name { get; set; }

        [DisplayName("Short Description:")]
        [MaxLength(128)]
        public string ShortDescription { get; set; }

        [DisplayName("Long Description:")]
        [MaxLength(8000)]
        public string LongDescription { get; set; }

        [DisplayName("Terms:")]
        [MaxLength(Int32.MaxValue)]
        public string Terms { get; set; }

        [DisplayName("Website:")]
        [MaxLength(512)]
        public string WebSite { get; set; }

        public List<SocialMediaItem> SocialMediaLinks { get; set; }

        public MerchantImage Images { get; set; }

        public List<MerchantBranch> Branches { get; set; }

        public MerchantBranchListViewModel MerchantBranchList { get; set; }

        public int? ContactDetailId { get; set; }

        [DisplayName("Landline:")]
        [MaxLength(16)]
        public string LandlinePhone { get; set; }

        [DisplayName("Mobile:")]
        [MaxLength(16)]
        public string MobilePhone { get; set; }

        [DisplayName("Email:")]
        [MaxLength(512)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [DisplayName("Feature Image Offer Text:")]
        public bool FeatureImageOfferText { get; set; }
        [DisplayName("Brand Colour:")]
        public string BrandColor { get; set; }

        public CarouselModel Carousel { get; set; }

        public List<MerchantImageViewModel> MerchantImages { get; set; }
        public MerchantImageViewModel FeatureImage { get; set; }
        public MerchantImageViewModel DisabledLogo { get; set; }

        public MerchantViewModel()
        {
            SocialMediaLinks = new List<SocialMediaItem>();
            Branches = new List<MerchantBranch>();
            Images = new MerchantImage();
            MerchantBranchList = new MerchantBranchListViewModel();
            Carousel = new CarouselModel();
            MerchantImages = new List<MerchantImageViewModel>();
        }
    }
}
