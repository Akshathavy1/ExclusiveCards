using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Providers;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.WebAdmin.Models;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace ExclusiveCard.WebAdmin.Controllers
{
    [Authorize(Roles = "AdminUser, BackOfficeUser")]
    public class FileController : BaseController
    {
        #region Private Methods

        private readonly IPartnerTransactionService _partnerTransactionService;
        private readonly IStatusServices _statusServices;
        private readonly IOptions<TypedAppSettings> _setting;
        private readonly IUserService _userService;
        private readonly IAzureStorageProvider _azureStorageProvider;

        #endregion

        public FileController(IPartnerTransactionService partnerTransactionService,
            IStatusServices statusServices, IOptions<TypedAppSettings> setting,
            IUserService userService, IAzureStorageProvider azureStorageProvider)
        {
            _partnerTransactionService = partnerTransactionService;
            _statusServices = statusServices;
            _setting = setting;
            _userService = userService;
            _azureStorageProvider = azureStorageProvider;
        }

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index(int? page, DateTime? dateFrom, DateTime? dateTo, int? state, string fileType)
        {
            if(dateFrom==null)
            dateFrom = DateTime.UtcNow.AddDays(-14);
            if (dateTo == null)
            dateTo = DateTime.UtcNow;
            FileSearchViewModel model = new FileSearchViewModel
            {
                Page = 1,
                CreatedFrom = dateFrom,
                CreatedTo = dateTo,
                Type = fileType
            };

            try
            {
                int pageSize = 20;
                if (page.HasValue && page > 0)
                {
                    model.Page = page.Value;
                }

                int.TryParse(_setting.Value.PageSize, out pageSize);
                if (state.HasValue && state > 0)
                    model.State = state.Value;
                else
                    state = null;

                model.States = Enum.GetValues(typeof(FileStatus)).Cast<FileStatus>()
                    .Where(v => v.ToString() != FileStatus.NoRecords.ToString()).Select(v => new SelectListItem
                    {
                        Text = Regex.Replace(v.ToString(), "(\\B[A-Z]+?(?=[A-Z][^A-Z])|\\B[A-Z]+?(?=[^A-Z]))", " $1"),
                        Value = ((int) v).ToString()
                    }).ToList();

                model.Types = (from object eVal in Enum.GetValues(typeof(FileType))
                    select new SelectListItem { Text = Enum.GetName(typeof(FileType), eVal), Value = eVal.ToString() }).ToList();
                var files = await _partnerTransactionService.GetPagedFileResults(model.Page, pageSize, state, fileType, dateFrom, dateTo);
                await MapToList(files, model.FileList);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View("Error", new ErrorViewModel {RequestId = "Error occurred while retrieving files data."});
            }
            return View(model);
        }

        [HttpPost]
        [ActionName("Update")]
        public async Task<IActionResult> Update(int fileId)
        {
            try
            {
                if (fileId > 0)
                {
                    var file = await _partnerTransactionService.GetByIdAsync(fileId);
                    if (file != null)
                    {
                        var user = await _userService.GetUserAsync(HttpContext.User);
                        file.StatusId = (int) FileStatus.Sent;//status.FirstOrDefault(x => x.Name == Data.Constants.Status.Sent).Id;
                        file.UpdatedBy = user.Id;
                        file.ChangedDate = DateTime.UtcNow;
                        await _partnerTransactionService.UpdateAsync(file);
                    }
                    else
                    {
                        return Json(JsonResponse<string>.ErrorResponse("File not found."));
                    }
                }
                else
                {
                    return Json(JsonResponse<string>.ErrorResponse("Error updating file data to sent."));
                }

                return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error updating file data to sent."));
            }
        }

        [HttpGet]
        [ActionName("Download")]
        public async Task<IActionResult> Download(int fileId)
        {
            try
            {
                if (fileId > 0)
                {
                    var file = await _partnerTransactionService.GetByIdAsync(fileId);
                    if (file != null)
                    {
                        //Get current path
                        var destinationPath = Path.Combine(
                            Directory.GetCurrentDirectory(), _setting.Value.FileDownload);

                        if (!Directory.Exists(destinationPath))
                        {
                            Directory.CreateDirectory(destinationPath);
                        }
                        else
                        {
                            string[] files = Directory.GetFiles(destinationPath);

                            // Copy the files and overwrite destination files if they already exist.
                            foreach (string s in files)
                            {
                                // Use static Path methods to extract only the file name from the path.
                                var fileName = Path.GetFileName(s);
                                System.IO.File.Delete(Path.Combine(destinationPath, fileName));
                            }
                        }

                        //Download to local folder
                        if (file.Type == FileType.Love2Shop.ToString())
                        {
                            await _azureStorageProvider.DownloadFile(_setting.Value.BlobConnectionString,
                                _setting.Value.PartnerContainerName, file.Location, destinationPath, file.Name, Logger);
                        }
                        else if(file.Status.Name == Data.Constants.Status.Created || file.Status.Name == Data.Constants.Status.Sent)
                        {
                            await _azureStorageProvider.DownloadFile(_setting.Value.BlobConnectionString,
                                    _setting.Value.PartnerContainerName, file.Location, destinationPath, file.Name, Logger);
                        }
                        else
                        {
                            await _azureStorageProvider.DownloadFile(_setting.Value.BlobConnectionString,
                                _setting.Value.PartnerContainerName, file.Location, destinationPath, file.Name, Logger);
                        }

                        //download to user system
                        var path = $"{destinationPath}/{file.Name}";

                        var memory = new MemoryStream();
                        using (var stream = new FileStream(path, FileMode.Open))
                        {
                            await stream.CopyToAsync(memory);
                        }
                        memory.Position = 0;
                        return File(memory, "text/csv", Path.GetFileName(path));
                    }
                    else
                    {
                        return Json(JsonResponse<string>.ErrorResponse("Error finding file."));
                    }
                }
                else
                {
                    return Json(JsonResponse<string>.ErrorResponse("Error finding file."));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error downloading file."));
            }
        }

        #region Private Methods

        private async Task MapToList(ExclusiveCard.Services.Models.DTOs.PagedResult<Services.Models.DTOs.Files> source, FilesListViewModel dest)
        {
            if (source != null)
            {
                dest.CurrentPageNumber = source.CurrentPage;
                dest.Paging = new PagingViewModel
                {
                    CurrentPage = source.CurrentPage,
                    PageCount = source.PageCount,
                    PageSize = source.PageSize,
                    RowCount = source.RowCount
                };
                await Task.WhenAll(source.Results.Select(async item =>
                {
                    if (item.Status.Name != "NoRecords")
                    {
                        dest.Files.Add(new FileViewModel
                        {
                            Id = item.Id,
                            CreatedDate = item.CreatedDate,
                            Name = item.Name,
                            Type = item.Type,
                            State = item.Status.Name,
                            Location = item.Location,
                            CreatedFrom=item.CreatedFrom,
                            CreatedTo=item.CreatedTo                            
                        });
                    }

                    await Task.CompletedTask;
                }));
            }
        }

        #endregion
    }
}
