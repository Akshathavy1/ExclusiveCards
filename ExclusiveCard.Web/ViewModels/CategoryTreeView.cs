using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class CategoryTreeView
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [DataType("nvarchar")]
        public string Name { get; set; }

        public int ParentId { get; set; }

        public bool IsChecked { get; set; } = false;
        
    }
}
