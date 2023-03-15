using System;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class FileViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public string Location { get; set; }
        public DateTime CreatedFrom { get; set; }
        public DateTime CreatedTo { get; set; }
    }
}
