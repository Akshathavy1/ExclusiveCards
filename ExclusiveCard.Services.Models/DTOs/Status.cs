using ExclusiveCard.Data.StagingModels;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Models.DTOs
{
    public class Status
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool IsActive { get; set; }


    }
}
