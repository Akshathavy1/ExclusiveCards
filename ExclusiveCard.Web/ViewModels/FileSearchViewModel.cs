using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class FileSearchViewModel
    {
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDate { get; set; }
        public int State { get; set; }
        public List<SelectListItem> States { get; set; }
        public string Type { get; set; }
        public List<SelectListItem> Types { get; set; }
        public int Page { get; set; }

        public FilesListViewModel FileList { get; set; }

        public FileSearchViewModel()
        {
            States = new List<SelectListItem>();
            Types = new List<SelectListItem>();
            FileList = new FilesListViewModel();
        }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedFrom { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedTo { get; set; }
    }
}
