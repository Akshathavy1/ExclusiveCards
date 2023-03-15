using System;
using System.Collections.Generic;
using System.Text;

namespace ExclusiveCard.Services.Models.DTOs.StagingModels
{
    public class AwinCSVFileError : AwinCSVFile
    {
        public string ErrorMessage { get; set; }
    }
}
