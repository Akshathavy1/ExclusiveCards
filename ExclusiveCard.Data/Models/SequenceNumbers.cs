using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExclusiveCard.Data.Models
{
    [Table("SequenceNumbers", Schema = "Exclusive")]
    public class SequenceNumbers
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public long Value { get; set; }
    }
}
