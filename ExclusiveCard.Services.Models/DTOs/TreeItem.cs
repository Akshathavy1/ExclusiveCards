using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class TreeItem<T>
    {
        public T Item { get; set; }
        public IEnumerable<TreeItem<T>> Children { get; set; }
    }
}
