namespace ExclusiveCard.Services.Models.DTOs
{
    public class Localisation
    {
        public int Id { get; set; }
        
        public string LocalisedText { get; set; }
        
        public string Context { get; set; }

        public string LocalisationCode { get; set; }
    }
}
