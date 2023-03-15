using System.Collections.Generic;

namespace ExclusiveCard.WebAdmin.Models.Api
{
    public class GetDealResponse
    {
        public Content Content { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }

        public GetDealResponse()
        {
            Content = new Content();
        }
    }
    public class Content
    {
        public int id { get; set; }
        public int ActionTypeId { get; set; }
        public string ActionData { get; set; }
        public object TimeStamp { get; set; }
        public List<Text> Texts { get; set; }
        public List<Image> Images { get; set; }
        public bool IsDeleted { get; set; }

        public Content()
        {
            Texts = new List<Text>();
            Images = new List<Image>();
        }
    }
    public class Text
    {
        public int Id { get; set; }
        public string TextName { get; set; }
        public string TextValue { get; set; }
        public string TextLabel { get; set; }
        public object TimeStamp { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class Image
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public string ImageHeight { get; set; }
        public string ImageWidth { get; set; }
        public object TimeStamp { get; set; }
        public bool IsDeleted { get; set; }
    }
}
