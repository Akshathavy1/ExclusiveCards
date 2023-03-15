using System.ComponentModel.DataAnnotations;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class CustomTagList
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Tags { get; set; }
    }
}
