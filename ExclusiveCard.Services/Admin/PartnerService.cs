using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using ExclusiveCard.Data.Constants;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Providers;
using ExclusiveCard.Services.Interfaces;
using ExclusiveCard.Services.Interfaces.Admin;
using NLog;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Admin
{
    public class PartnerService : IPartnerService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IPartnerManager _partnerManager;
        private readonly IPartnerTransactionService _partnerTransactionService;
        private readonly ISFTPProvider _sftpProvider;
        private readonly Managers.IEmailManager _emailManager;
        private readonly IPartnerRewardWithdrawalService _partnerRewardWithdrawalService;
        private readonly IAzureStorageProvider _azureStorageProvider;
        private readonly ILogger _logger;

        #endregion Private Members

        #region Constructor

        public PartnerService(IMapper mapper, IPartnerManager partnerManager,
            IPartnerTransactionService partnerTransactionService,
            ISFTPProvider sftpProvider, Managers.IEmailManager emailManager,
            IPartnerRewardWithdrawalService partnerRewardWithdrawalService,
            IAzureStorageProvider azureStorageProvider)
        {
            _mapper = mapper;
            _partnerManager = partnerManager;
            _partnerTransactionService = partnerTransactionService;
            _sftpProvider = sftpProvider;
            _emailManager = emailManager;
            _partnerRewardWithdrawalService = partnerRewardWithdrawalService;
            _azureStorageProvider = azureStorageProvider;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion Constructor

        public async Task<string> SendPartnerReport(int partnerId, string adminEmail, string inFolder,
            string blobConnectionString, string container, string ftpUrl, string ftpUsername, string ftpPassword)
        {
            dto.Files fileReq = new dto.Files();
            try
            {
                bool result = false;
                if (partnerId == 0)
                    return "PartnerId is required.";
                var partner = await GetByIdAsync(partnerId);
                var currentPath = Path.Combine(
                    Directory.GetCurrentDirectory(), TemporaryFilePath.TempFileIN);

                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }

                dto.ExternalFile externalFile = new dto.ExternalFile();
                var fileStream = new StringBuilder();
                decimal totalFileAmount = new decimal(0);

                //Check if the file exists with Processing state
                var fileExistsWithProcessing = _partnerTransactionService.CheckIfFileExistsWithProcessingState(partnerId);
                if (fileExistsWithProcessing)
                {
                    return "File exists in processing state. Could not create new file.";
                }
                else
                {
                    //check if file is created and call get file data
                    int fileId;

                    var fileData = await _partnerTransactionService.GetTransactionReport(partnerId);

                    if (fileData?.Count > 0)
                    {
                        fileId = fileData.FirstOrDefault().FileId;
                        //Get file from DB
                        fileReq = await _partnerTransactionService.GetByIdAsync(fileId);
                        //assign filename to external object
                        externalFile.FileName = fileReq.Name;
                        totalFileAmount = fileData.Select(x => x.Amount).Sum();
                        //Generate file stream
                        GenerateFileDataStream(fileStream, fileData);
                        //update the file amount
                        if (totalFileAmount > 0)
                        {
                            fileReq.Id = fileId;
                            fileReq.TotalAmount = totalFileAmount;
                            fileReq.ChangedDate = DateTime.UtcNow;
                            await _partnerTransactionService.UpdateAsync(fileReq);
                        }

                        //FileStream to be passed in to story 8
                        using (FileStream
                            fs = new FileStream(Path.Combine(currentPath, externalFile.FileName), FileMode.Create))
                        {
                            using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                            {
                                w.Write(fileStream.ToString());
                            }
                        }
                    }
                    else
                    {
                        return "No data found to create file.";
                    }
                }

                //upload file to SFTP in folder
                //upload to blob
                await _azureStorageProvider.SaveFile(blobConnectionString, container, currentPath,
                    $"{partner.Name}-{partnerId}", externalFile.FileName, true, _logger);

                try
                {
                    result = _sftpProvider.UploadFile(ftpUrl, ftpUsername, ftpPassword, $"{inFolder}", externalFile, adminEmail);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    //email sent for error in processing file
                    var resa = _emailManager.SendEmailAsync(new dto.Email
                    {
                        EmailTo = new List<string>() { adminEmail },
                        Subject = $"Partner Report processing failure - uploading file failed",
                        BodyHtml = $"Dear Admin,<br/><p>Failed to uploading file to FTPS server - {e.Message}. {e.StackTrace}</p>",
                        BodyPlainText = $"Dear Admin, Failed to uploading file to FTPS server - {e.Message}. {e.StackTrace}",
                    }).Result;
                    if (resa != true.ToString())
                    {
                        _logger.Error("Error sending email when failed to retrieve files from FTPS.");
                    }
                }

                if (result)
                {
                    //update status for file as sent
                    fileReq.StatusId = (int)FileStatus.Sent;
                    //status.FirstOrDefault(x => x.Name == Data.Constants.Status.Sent && x.Type == StatusType.FileStatus).Id;
                    fileReq.ChangedDate = DateTime.UtcNow;
                    fileReq.Location = $"{partner.Name}-{partnerId}";
                    await _partnerTransactionService.UpdateAsync(fileReq);
                    return true.ToString();
                }
                //update status for file as Error
                fileReq.StatusId = (int)FileStatus.Error;
                //status.FirstOrDefault(x => x.Name == Data.Constants.Status.Error && x.Type == StatusType.FileStatus).Id;
                fileReq.ChangedDate = DateTime.UtcNow;
                await _partnerTransactionService.UpdateAsync(fileReq);
                //email sent for error file message
                var res = await _emailManager.SendEmailAsync(new dto.Email
                {
                    EmailTo = new List<string>() { adminEmail },
                    Subject = $"Partner Report send failure - {fileReq.Name}",
                    BodyHtml = $"Dear Admin,<br/><p>Failed to process partner report with fileName - {fileReq.Name}.</p><br/> Error occured during uploading file process.",
                    BodyPlainText = $"Dear Admin,Failed to process partner report with fileName - {fileReq.Name}. Error occured during uploading file process.",
                });
                if (res != true.ToString())
                {
                    _logger.Error("Error sending email when failed to process partner report upload.");
                }
                return "Error occured during uploading file process.";
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                //update status for file as Error
                fileReq.StatusId = (int)FileStatus.Error;
                //status.FirstOrDefault(x => x.Name == Data.Constants.Status.Error && x.Type == StatusType.FileStatus).Id;
                fileReq.ChangedDate = DateTime.UtcNow;
                await _partnerTransactionService.UpdateAsync(fileReq);
                //email sent for error file message
                var res = await _emailManager.SendEmailAsync(new dto.Email
                {
                    EmailTo = new List<string>() { adminEmail },
                    Subject = $"Partner Report send failure - {fileReq.Name}",
                    BodyHtml = $"Dear Admin,<br/><p>Failed to process partner report with fileName - {fileReq.Name}. {ex.StackTrace}",
                    BodyPlainText = $"Dear Admin,Failed to process partner report with fileName - {fileReq.Name}. {ex.StackTrace}",
                });
                if (res != true.ToString())
                {
                    _logger.Error("Error sending email when failed to process partner report upload.");
                }
                return ex.ToString();
            }
        }

        public async Task<string> SendPartnerWithdrawalReport(string adminEmail, string inFolder, string blobConnectionString, string container,
            string ftpUrl, string ftpUsername, string ftpPassword)
        {
            bool result = false;
            dto.Files fileReq = new dto.Files();
            try
            {
                var partner = await GetByNameAsync("TAM");
                if (partner == null || partner?.Id == 0)
                    return "TAM Partner not found.";

                var currentPath = Path.Combine(
                    Directory.GetCurrentDirectory(), TemporaryFilePath.TempFileIN);

                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
                //1. Get all pending Partner Reward Withdrawal
                List<dto.TamWithdrawalDataModel> tamWithdrawalDataModels = new List<dto.TamWithdrawalDataModel>();
                tamWithdrawalDataModels =
                    await _partnerRewardWithdrawalService.GetWithdrawalReport((int)WithdrawalStatus.Pending);
                //status.FirstOrDefault(x => x.IsActive && x.Name == Data.Constants.Status.Pending && x.Type == StatusType.WithdrawalStatus).Id);
                if (tamWithdrawalDataModels.Count == 0)
                    return "No record found in Partner Reward Withdrawal with pending status";
                //2. Generate File in TempIN
                DateTime current = DateTime.Now;
                string month = current.Month.ToString().Length == 1
                    ? $"0{current.Month.ToString()}"
                    : current.Month.ToString();
                string day = current.Day.ToString().Length == 1 ? $"0{current.Day.ToString()}" : current.Day.ToString();
                dto.ExternalFile externalFile = new dto.ExternalFile
                {
                    FileName = $"{FileName.PartnerWithdraw}{current.Year}-{month}-{day}-{current.Hour}-{current.Minute}-{current.Second}.txt"
                };
                //Generate file record
                fileReq = new dto.Files
                {
                    Name = externalFile.FileName,
                    PartnerId = partner.Id,
                    Type = FileType.PartnerWithdraw.ToString(),
                    StatusId = (int)FileStatus.Created,
                    //status.FirstOrDefault(x => x.IsActive && x.Name == Data.Constants.Status.Created && x.Type == StatusType.FileStatus).Id,
                    CreatedDate = DateTime.UtcNow,
                    ChangedDate = DateTime.UtcNow,
                    Location = $"/{inFolder}"
                };
                var filesAdded = await _partnerTransactionService.AddAsync(fileReq);
                if (filesAdded == null || filesAdded?.Id == 0)
                    return "Failed while creating file.";
                List<dto.TamDataModel> tamDataModels = new List<dto.TamDataModel>();
                tamDataModels = Services.ManualMappings.MapTamDataModels(tamWithdrawalDataModels);
                if (tamDataModels.Count == 0)
                    return "TAM dataModels found with no record";
                var fileStream = new StringBuilder();
                GenerateFileDataStream(fileStream, tamDataModels);
                if (fileStream.Length == 0)
                    return "Empty fileStream found.";
                using (FileStream
                    fs = new FileStream(Path.Combine(currentPath, externalFile.FileName), FileMode.Create))
                {
                    using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                    {
                        w.Write(fileStream.ToString());
                    }
                }
                //3. Update Status to Partner Reward Withdrawal (fileCreated status)
                List<dto.PartnerRewardWithdrawal> partnerRewardWithdrawals = new List<dto.PartnerRewardWithdrawal>();
                foreach (var withdrawal in tamWithdrawalDataModels)
                {
                    partnerRewardWithdrawals.Add(new dto.PartnerRewardWithdrawal
                    {
                        Id = withdrawal.PartnerRewardWithdrawalId,
                        PartnerRewardId = withdrawal.PartnerRewardId,
                        StatusId = (int)WithdrawalStatus.FileCreated,
                        //status.FirstOrDefault(x => x.IsActive && x.Name == Data.Constants.Status.FileCreated && x.Type == StatusType.WithdrawalStatus).Id,
                        RequestedAmount = withdrawal.Amount,
                        ConfirmedAmount = withdrawal.ConfirmedAmount,
                        BankDetailId = withdrawal.BankDetailId,
                        FileId = filesAdded.Id,
                        RequestedDate = withdrawal.RequestedDate
                    });
                }
                await _partnerRewardWithdrawalService
                    .BulkUpdateAsync(partnerRewardWithdrawals);
                //4. Uploading to SFTP IN Folder
                //using (var fs = new FileStream(Path.Combine(currentPath, externalFile.FileName), FileMode.Open, FileAccess.Read,
                //    FileShare.ReadWrite))
                //{
                //    externalFile.FileContent = fs;
                //Write file to blob
                await _azureStorageProvider.SaveFile(blobConnectionString, container, currentPath, $"{partner.Name}",
                    externalFile.FileName, true, _logger);
                try
                {
                    result = _sftpProvider.UploadFile(ftpUrl, ftpUsername, ftpPassword, $"{inFolder}", externalFile, adminEmail);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    //email sent for error in processing file
                    var resa = _emailManager.SendEmailAsync(new dto.Email
                    {
                        EmailTo = new List<string>() { adminEmail },
                        Subject = $"Partner Report processing failure - uploading file failed",
                        BodyHtml = $"Dear Admin,<br/><p>Failed to uploading file to FTPS server - {e.Message}. {e.StackTrace}</p>",
                        BodyPlainText = $"Dear Admin, Failed to uploading file to FTPS server - {e.Message}. {e.StackTrace}",
                    }).Result;
                    if (resa != true.ToString())
                    {
                        _logger.Error("Error sending email when failed to retrieve files from FTPS.");
                    }
                }
                //}
                if (result)
                {
                    //5. Update Status to Partner Reward Withdrawal (sent status)
                    foreach (var withdrawal in partnerRewardWithdrawals)
                    {
                        withdrawal.StatusId = (int)WithdrawalStatus.Sent;
                        //status.FirstOrDefault(x => x.IsActive && x.Name == Data.Constants.Status.Sent && x.Type == StatusType.WithdrawalStatus).Id;
                    }
                    await _partnerRewardWithdrawalService.BulkUpdateAsync(partnerRewardWithdrawals);
                }
                else
                {
                    //left with pending status if fails in uploading to sftp in folder.
                    foreach (var withdrawal in partnerRewardWithdrawals)
                    {
                        withdrawal.StatusId = (int)WithdrawalStatus.Pending;
                        //status.FirstOrDefault(x => x.IsActive && x.Name == Data.Constants.Status.Pending && x.Type == StatusType.WithdrawalStatus).Id;
                    }
                    await _partnerRewardWithdrawalService.BulkUpdateAsync(partnerRewardWithdrawals);
                    //Update file status as error
                    fileReq.Id = filesAdded.Id;
                    fileReq.Location = $"{partner.Name}";
                    fileReq.StatusId = (int)FileStatus.Error;
                    //status.FirstOrDefault(x => x.IsActive && x.Name == Data.Constants.Status.Error && x.Type == StatusType.FileStatus).Id;
                    await _partnerTransactionService.UpdateAsync(fileReq);
                    var res = await _emailManager.SendEmailAsync(new dto.Email
                    {
                        EmailTo = new List<string>() { adminEmail },
                        Subject = $"Partner Withdrawal Report send failure - {fileReq.Name}",
                        BodyHtml = $"Dear Admin,<br/><p>Failed to send partner withdrawal report with fileName - {fileReq.Name}.<br/> Error occurred while uploading partner withdrawal report to sftp in folder",
                        BodyPlainText = $"Dear Admin,Failed to send partner withdrawal report with fileName - {fileReq.Name}. Error occurred while uploading partner withdrawal report to sftp in folder",
                    });
                    if (res != true.ToString())
                    {
                        _logger.Error("Error sending email when failed to process partner report upload.");
                    }
                    return
                        $"Error occurred while uploading partner withdrawal report to sftp in folder of fileName :{externalFile.FileName}.";
                }
                return true.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                //email sent for error file message
                var res = await _emailManager.SendEmailAsync(new dto.Email
                {
                    EmailTo = new List<string>() { adminEmail },
                    Subject = $"Partner Withdrawal Report send failure - {fileReq.Name}",
                    BodyHtml = $"Dear Admin,<br/><p>Failed to send partner withdrawal report with fileName - {fileReq.Name}.<br/> StackTrace - {ex.StackTrace}",
                    BodyPlainText = $"Dear Admin,Failed to send partner withdrawal report with fileName - {fileReq.Name}. StackTrace - {ex.StackTrace}",
                });
                if (res != true.ToString())
                {
                    _logger.Error("Error sending email when failed to process partner report upload.");
                }
                return ex.ToString();
            }
        }

        public async Task<string> ProcessPartnerReport(int partnerId, string adminEmail, string tamFolderOut,
            string blobConnectionString, string partnerContainerName, string blobProcessing, string blobError,
            string blobProcessed, string serverUri, string username, string password)
        {
            try
            {
                if (partnerId == 0)
                    return "Partner Id not found.";
                //checking the partnerId exists
                var partner = await GetByIdAsync(partnerId);
                if (partner == null)
                    return "Invalid Partner Id, no record found.";

                //Create local folder if not exists
                var currentPath = Path.Combine(
                    Directory.GetCurrentDirectory(), TemporaryFilePath.TempFileOUT);

                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }

                //MoveFiles from SFTP out folder to Blob-partnerId/processing -> Pass filter fileName contains.
                List<dto.ExternalFile> files = null;
                try
                {
                    files = _sftpProvider.GetFilesFromFolder($"{serverUri}{tamFolderOut}", username, password, currentPath, adminEmail, partner.Name);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    //email sent for error in processing file
                    var res = _emailManager.SendEmailAsync(new dto.Email
                    {
                        EmailTo = new List<string>() { adminEmail },
                        Subject = $"No Balance file was found for partner {partner.Name}",
                        BodyHtml = $"Dear Admin,<br/><p>Failed to retrieve file from FTPS server - {e.Message}. {e.StackTrace}</p>",
                        BodyPlainText = $"Dear Admin, Failed to retrieve file from FTPS server - {e.Message}. {e.StackTrace}",
                    }).Result;
                    if (res != true.ToString())
                    {
                        _logger.Error("Error sending email when failed to retrieve files from FTPS.");
                    }
                }

                foreach (var file in files)
                {
                    bool res = false;
                    res = await _azureStorageProvider.SaveFile(blobConnectionString,
                        partnerContainerName, currentPath,
                        $"{partner.Name}-{partnerId}/{blobProcessing}", file.FileName, true, _logger);
                    if (res)
                    {
                        Uri uri = new Uri(serverUri);
                        try
                        {
                            _sftpProvider.DeleteFile(uri, username, password, tamFolderOut, file.FileName, adminEmail);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e);
                            //email sent for error in processing file
                            var resa = _emailManager.SendEmailAsync(new dto.Email
                            {
                                EmailTo = new List<string>() { adminEmail },
                                Subject = $"Partner Report processing failure - Deleting files failed",
                                BodyHtml = $"Dear Admin,<br/><p>Failed to delete file from FTPS server - {e.Message}. {e.StackTrace}</p>",
                                BodyPlainText = $"Dear Admin, Failed to delete file from FTPS server - {e.Message}. {e.StackTrace}",
                            }).Result;
                            if (resa != true.ToString())
                            {
                                _logger.Error("Error sending email when failed to retrieve files from FTPS.");
                            }
                        }
                    }
                }
                //GetFiles from Blob-partnerId/processing -> Pass filter fileName contains.
                var externalFiles = await _azureStorageProvider.GetBlobListAsync(blobConnectionString,
                    partnerContainerName, $"{partner.Name}-{partnerId}/{blobProcessing}",
                    FileName.PartnerTrans, _logger);
                foreach (dto.ExternalFile file in externalFiles)
                {
                    string destinationblob = string.Empty;
                    try
                    {
                        dto.ProcessResult processResult = null;
                        if (file.FileName.ToLower().Contains(FileName.PartnerTrans.ToLower()))
                        {
                            try
                            {
                                //Each partner file processing.
                                processResult =
                                    await _partnerTransactionService.ProcessPartnerFile(partnerId, file, blobError, blobProcessed, partner.Name);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex);
                                //email sent for error in processing file
                                var res = await _emailManager.SendEmailAsync(new dto.Email
                                {
                                    EmailTo = new List<string>() { adminEmail },
                                    Subject = $"Partner Report processing failure - {file.FileName}",
                                    BodyHtml = $"Dear Admin,<br/><p>Failed to processing partner report with fileName - {file.FileName} and description - {ex.Message}. {ex.StackTrace}",
                                    BodyPlainText = $"Dear Admin,Failed to processing partner report with fileName - {file.FileName} and description - {ex.Message}. {ex.StackTrace}",
                                });
                                if (res != true.ToString())
                                {
                                    _logger.Error("Error sending email when failed to process partner report file.");
                                }
                            }
                            finally
                            {
                                destinationblob = processResult.Status == Data.Constants.Status.Processed ? blobProcessed : blobError;

                                if (!string.IsNullOrEmpty(destinationblob))
                                {
                                    await _azureStorageProvider.MoveBlobFile(blobConnectionString,
                                        partnerContainerName, $"{partner.Name}-{partnerId}/{blobProcessing}",
                                        $"{partner.Name}-{partnerId}/{destinationblob}", file.FileName, _logger);
                                }
                            }
                        }
                        else if (file.FileName.ToLower().Contains(FileName.PartnerWithdraw.ToLower()))
                        {
                            try
                            {
                                //Process each partner withdrawal file
                                processResult =
                                    await _partnerTransactionService
                                        .ProcessWithdrawalFile(partnerId, file, blobError, blobProcessed, partner.Name);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex);
                                //email sent for error in processing file
                                var res = await _emailManager.SendEmailAsync(new dto.Email
                                {
                                    EmailTo = new List<string>() { adminEmail },
                                    Subject = $"Partner Report processing failure - {file.FileName}",
                                    BodyHtml = $"Dear Admin,<br/><p>Failed to processing partner withdrawal with fileName - {file.FileName} and description - {ex.Message}. {ex.StackTrace}",
                                    BodyPlainText = $"Dear Admin,Failed to processing partner withdrawal with fileName - {file.FileName} and description - {ex.Message}. {ex.StackTrace}",
                                });
                                if (res != true.ToString())
                                {
                                    _logger.Error("Error sending email when failed to process partner withdrawal file.");
                                }
                            }
                            finally
                            {
                                destinationblob = processResult.Status == Data.Constants.Status.Processed ? blobProcessed : blobError;
                                if (!string.IsNullOrEmpty(destinationblob))
                                {
                                    await _azureStorageProvider.MoveBlobFile(blobConnectionString,
                                        partnerContainerName, $"{partner.Name}-{partnerId}/{blobProcessing}",
                                        $"{partner.Name}-{partnerId}/{destinationblob}", file.FileName, _logger);
                                }
                            }
                        }
                        else
                        {
                            _logger.Error($"Filename expected to be either {FileName.PartnerTrans} or {FileName.PartnerWithdraw} but, it was {file.FileName}");
                            await _azureStorageProvider.MoveBlobFile(blobConnectionString,
                                partnerContainerName, $"{partner.Name}-{partnerId}/{blobProcessing}", $"{partner.Name}-{partnerId}/{blobError}", file.FileName, _logger);
                            var res = await _emailManager.SendEmailAsync(new dto.Email
                            {
                                EmailTo = new List<string>() { adminEmail },
                                Subject = $"Partner Report processing failure - {file.FileName}",
                                BodyHtml = $"Dear Admin,<br/><p>Failed to process partner report with fileName - {file.FileName}. Because filename expected to be either {FileName.PartnerTrans} or {FileName.PartnerWithdraw} but, it was {file.FileName}.</p>",
                                BodyPlainText = $"Dear Admin, Failed to process partner report with fileName - {file.FileName}. Because filename expected to be either {FileName.PartnerTrans} or {FileName.PartnerWithdraw} but, it was {file.FileName}.</p>",
                            });
                        }

                        if (processResult?.Status != null)
                        {
                            if (processResult.Status == Data.Constants.Status.Error || processResult.Status == Data.Constants.Status.TransactionError)
                            {
                                destinationblob = blobError;
                                throw new Exception(processResult.Description);
                            }
                            destinationblob = blobProcessed;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        //email sent for error in processing file
                        var res = await _emailManager.SendEmailAsync(new dto.Email
                        {
                            EmailTo = new List<string>() { adminEmail },
                            Subject = $"Partner Report processing failure - {file.FileName}",
                            BodyHtml = $"Dear Admin,<br/><p>Failed to processing partner report with fileName - {file.FileName} and description - {ex.Message}. {ex.StackTrace}",
                            BodyPlainText = $"Dear Admin, Failed to processing partner report with fileName - {file.FileName} and description - {ex.Message}. {ex.StackTrace}",
                        });
                        if (res != true.ToString())
                        {
                            _logger.Error("Error sending email when failed to process partner report upload.");
                        }
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(destinationblob))
                        {
                            await _azureStorageProvider.MoveBlobFile(blobConnectionString,
                                partnerContainerName, $"{partner.Name}-{partnerId}/{blobProcessing}", $"{partner.Name}-{partnerId}/{destinationblob}", file.FileName, _logger);
                        }
                    }
                }
                return true.ToString();
            }
            catch (Exception ex)
            {
                _logger?.Error(ex);
                return ex.Message;
            }
        }

        public async Task<string> ProcessPartnerPositionFile(string adminEmail, string blobConnectionString,
            string partnerContainerName, string blobProcessing, string tamFolderPosition, string blobProcessed,
            string blobError, string serverUri, string username, string password)
        {
            try
            {
                var partner = await GetByNameAsync("TAM");
                if (partner == null)
                    return "No partner record found with name - TAM.";

                StringBuilder fileStream = null;
                string destinationblob = String.Empty;
                dto.Files fileReq = new dto.Files();
                var currentPath = Path.Combine(
                        Directory.GetCurrentDirectory(), TemporaryFilePath.TempFilePosition);

                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }

                //1. Get all files in SFTP position folder [object]
                List<dto.ExternalFile> files = null;
                try
                {
                    files = _sftpProvider.GetFilesFromFolder($"{serverUri}{tamFolderPosition}", username, password, currentPath, adminEmail, partner.Name);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    //email sent for error in processing file
                    var res = _emailManager.SendEmailAsync(new dto.Email
                    {
                        EmailTo = new List<string>() { adminEmail },
                        Subject = $"No Balance file was found for partner {partner.Name}",
                        BodyHtml = $"Dear Admin,<br/><p>Failed to retrieve file from FTPS server - {e.Message}. {e.StackTrace}</p>",
                        BodyPlainText = $"Dear Admin, Failed to retrieve file from FTPS server - {e.Message}. {e.StackTrace}",
                    }).Result;
                    if (res != true.ToString())
                    {
                        _logger.Error("Error sending email when failed to retrieve files from FTPS.");
                    }
                }
                IList<dto.TamPositionDataModel> tampositionDM = null;
                if (files?.Count > 0)
                {
                    foreach (var file in files)
                    {
                        fileStream = new StringBuilder();
                        tampositionDM = new List<dto.TamPositionDataModel>();
                        using (StreamReader streamReader = new StreamReader($"{currentPath}{file.FileName}"))
                        {
                            using (var csvread = new CsvReader(streamReader))
                            {
                                csvread.Configuration.PrepareHeaderForMatch = (header, index) => header.Replace(" ", string.Empty);
                                csvread.Configuration.MissingFieldFound = null;
                                csvread.Configuration.IgnoreBlankLines = false;
                                csvread.Configuration.BadDataFound = context =>
                                {
                                };
                                csvread.Configuration.ReadingExceptionOccurred = (ex) =>
                                {
                                    _logger.Error(ex.ToString());
                                    return true;
                                };
                                tampositionDM = csvread.GetRecords<dto.TamPositionDataModel>().ToList();
                            }
                        }
                        if (tampositionDM.Count > 0)
                        {
                            var filterTamPosition = tampositionDM.Where(x => x.ASSET_NAME == "TOTAL").ToList();
                            if (filterTamPosition.Count > 0)
                            {
                                //2. Generate new text file then move to TempPosition Folder
                                GeneratePositionFileDataStream(fileStream, filterTamPosition, _logger);
                                using (FileStream
                                    fs = new FileStream(Path.Combine(currentPath, Path.GetFileName(file.FileName).Replace(".csv", ".txt")), FileMode.Create))
                                {
                                    using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                                    {
                                        w.Write(fileStream.ToString());
                                    }
                                }
                                //3. Create file record with status created
                                fileReq = new dto.Files
                                {
                                    Name = $"{Path.GetFileName(file.FileName).Replace(".csv", ".txt")}",
                                    PartnerId = partner.Id,
                                    Type = FileType.PartnerPosition.ToString(),
                                    StatusId = (int)FileStatus.Created, //status.FirstOrDefault(x => x.Name == Data.Constants.Status.Created && x.Type == StatusType.FileStatus).Id,
                                    CreatedDate = DateTime.UtcNow,
                                    ChangedDate = DateTime.UtcNow
                                };
                                var fileAdded = await _partnerTransactionService.AddAsync(fileReq);
                                //4. upload that file to blob processing and delete sftp position file.
                                var uploadfile = await _azureStorageProvider.SaveFile(blobConnectionString,
                                    partnerContainerName, TemporaryFilePath.TempFilePosition,
                                    $"{partner.Name}-{FileType.PartnerPosition.ToString()}/{blobProcessing}", fileAdded.Name,
                                    true, _logger);
                                if (uploadfile)
                                {
                                    Uri uri = new Uri(serverUri);
                                    try
                                    {
                                        _sftpProvider.DeleteFile(uri, username, password, tamFolderPosition, file.FileName, adminEmail);
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.Error(e);
                                        //email sent for error in processing file
                                        var resa = _emailManager.SendEmailAsync(new dto.Email
                                        {
                                            EmailTo = new List<string>() { adminEmail },
                                            Subject = $"Partner Report processing failure - Deleting files failed",
                                            BodyHtml = $"Dear Admin,<br/><p>Failed to delete file from FTPS server - {e.Message}. {e.StackTrace}</p>",
                                            BodyPlainText = $"Dear Admin, Failed to delete file from FTPS server - {e.Message}. {e.StackTrace}",
                                        }).Result;
                                        if (resa != true.ToString())
                                        {
                                            _logger.Error("Error sending email when failed to retrieve files from FTPS.");
                                        }
                                    }
                                    //_sftpProvider.DeleteFile($"/{tamFolderPosition}", Path.GetFileName(file));
                                }
                                //5. Get files in [PartnerPosition] Blob processing folder -> external files
                                var externalfiles = await _azureStorageProvider.GetBlobListAsync(blobConnectionString,
                                    partnerContainerName,
                                    $"{partner.Name}-{FileType.PartnerPosition.ToString()}/{blobProcessing}",
                                    FileName.PartnerPosition, _logger);
                                try
                                {
                                    foreach (var tamfile in externalfiles)
                                    {
                                        //6. Update PartnerReward while reading that each file and Update status
                                        destinationblob = string.Empty;
                                        var processResult = await _partnerTransactionService.ProcessTAMPositionFile(tamfile, _logger);
                                        if (!string.IsNullOrEmpty(processResult.Status))
                                        {
                                            if (processResult.Status == Data.Constants.Status.Processed)
                                            {
                                                destinationblob = blobProcessed;
                                            }
                                            else
                                            {
                                                destinationblob = blobError;
                                                throw new Exception(processResult.Description);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error(ex);
                                    var res = await _emailManager.SendEmailAsync(new dto.Email
                                    {
                                        EmailTo = new List<string>() { adminEmail },
                                        Subject = $"TAM position file processing failure - {file}",
                                        BodyHtml = $"Dear Admin,<br/><p>Failed to processing TAM position file with fileName - {file} and description - {ex.Message}. {ex.StackTrace}",
                                        BodyPlainText = $"Dear Admin,Failed to processing TAM position file with fileName - {file} and description - {ex.Message}. {ex.StackTrace}",
                                    });
                                    if (res != true.ToString())
                                    {
                                        _logger.Error("Error sending email when failed to process partner report upload.");
                                    }
                                }
                                finally
                                {
                                    //7. move to blob folder either processed or error
                                    if (!string.IsNullOrEmpty(destinationblob))
                                    {
                                        await _azureStorageProvider.MoveBlobFile(blobConnectionString,
                                            partnerContainerName, $"{partner.Name}-{FileType.PartnerPosition.ToString()}/{blobProcessing}", $"{partner.Name}-{FileType.PartnerPosition.ToString()}/{destinationblob}", fileAdded.Name, _logger);
                                    }
                                }
                                //update location for the file
                                fileReq.Id = fileAdded.Id;
                                fileReq.Location = $"{partner.Name}-{FileType.PartnerPosition.ToString()}/{destinationblob}";
                                await _partnerTransactionService.UpdateAsync(fileReq);
                            }
                        }
                    }
                }
                else
                {
                    return "No position file found on the server.";
                }

                return true.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return "Error occured while processing Tam position file.";
            }
        }

        #region Writes

        //Add Partner
        public async Task<dto.PartnerDto> Add(Models.DTOs.PartnerDto partner)
        {
            Partner req = _mapper.Map<Partner>(partner);
            return _mapper.Map<dto.PartnerDto>(
                await _partnerManager.Add(req));
        }

        //Update Partner
        public async Task<dto.PartnerDto> Update(Models.DTOs.PartnerDto partner)
        {
            Partner req = _mapper.Map<Partner>(partner);
            return _mapper.Map<dto.PartnerDto>(
                await _partnerManager.Update(req));
        }

        #endregion Writes

        #region Reads

        public async Task<dto.PartnerDto> GetByIdAsync(int id)
        {
            return _mapper.Map<dto.PartnerDto>(await _partnerManager.GetByIdAsync(id));
        }

        public async Task<dto.PartnerDto> GetByNameAsync(string name)
        {
            return _mapper.Map<dto.PartnerDto>(await _partnerManager.GetByNameAsync(name));
        }

        public async Task<List<dto.PartnerDto>> GetAllAsync(int? type)
        {
            return ManualMappings.MapPartners(await _partnerManager.GetAllAsync(type));
        }

        #endregion Reads

        private void GenerateFileDataStream(StringBuilder fileStream, List<dto.TamDataModel> data)
        {
            try
            {
                //Create Header
                var infos = typeof(dto.TamDataModel).GetProperties();

                for (var i = 1; i < infos.Length; i++)
                {
                    if (i == 1)
                    {
                        fileStream.Append(infos[i].Name);
                    }
                    else
                    {
                        fileStream.Append("\t");
                        fileStream.Append(infos[i].Name);
                    }

                    if (i == infos.Length - 1)
                    {
                        fileStream.Append("\r\n");
                    }
                }
                //build data line
                foreach (var rowData in data)
                {
                    fileStream.Append(
                        $"{rowData.TransType}\t{rowData.UniqueReference}\t{rowData.FundType}\t" +
                        $"{rowData.Title}\t{rowData.Forename}\t{rowData.Surname}\t{rowData.NINumber}\t" +
                        $"{rowData.Amount}\t{rowData.IntroducerCode}\t{rowData.ProcessState}\r\n");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void GeneratePositionFileDataStream(StringBuilder fileStream, List<dto.TamPositionDataModel> data, ILogger logger)
        {
            try
            {
                //Create Header
                var infos = typeof(dto.TamPositionDataModel).GetProperties();
                fileStream.Append(infos[2].Name); //PROVIDERREFERENCE
                fileStream.Append("\t");
                fileStream.Append(infos[8].Name); //ASSET_NAME
                fileStream.Append("\t");
                fileStream.Append(infos[13].Name); //ASSET_VALUE
                fileStream.Append("\r\n");
                //build data line
                foreach (var rowData in data)
                {
                    fileStream.Append(
                        $"{rowData.PROVIDERREFERENCE}\t{rowData.ASSET_NAME}\t{rowData.ASSET_VALUE}\r\n");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}