using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class OfferList
    {
        public int Id { get; set; }

        public string ListName { get; set; }

        public string Description { get; set; }

        public int MaxSize { get; set; }

        public bool IsActive { get; set; }

        public bool IncludeShowAllLink { get; set; }

        public string ShowAllLinkCaption { get; set; }

        public short PermissionLevel { get; set; }

        public ICollection<OfferListItem> OfferListItems { get; set; }
    }
}
