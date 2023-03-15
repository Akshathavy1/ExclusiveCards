using System.Collections.Generic;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class FilesListViewModel
    {
        public List<FileViewModel> Files { get; set; }
        public PagingViewModel Paging { get; set; }
        public int? CurrentPageNumber { get; set; }

        public FilesListViewModel()
        {
            Files = new List<FileViewModel>();
            Paging = new PagingViewModel();
        }
    }
}
