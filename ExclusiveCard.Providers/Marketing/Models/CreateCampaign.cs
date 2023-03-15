using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Providers.Marketing.Models
{
    public class CampaignError
    {
        public string message { get; set; }
        public string field { get; set; }

    }

    public class CampaignResponse
    {
        public string title { get; set; }
        public int id { get; set; }
        public string status { get; set; }

        
    }

    public class CreateCampaign
    {
        public string title { get; set; }
        public List<string> list_ids { get; set; }
        public string subject { get; set; }
        public int? sender_id { get; set; }

        //public List<string> segment_ids { get; set; }
        //public List<categories> categories { get; set; }
        //public int suppression_group_id { get; set; }
        //public string custom_unsubscribe_url { get; set; }
        //public string ip_pool { get; set; }
        public string html_content { get; set; }
        //public string plain_content { get; set; }

        //public string editor { get; set; }
        //public string status { get; set; }
        public int? id { get; set; }

    }

    public class Segments
    {
        public int id { get; set; }
    }

    public class categories
    {
        public string scope { get; set; }
    }

    public class CampaignSchedule
    {
        public int send_at { get; set; }
    }
    public class ScheduleResponse
    {
        public int send_at { get; set; }
    }


}