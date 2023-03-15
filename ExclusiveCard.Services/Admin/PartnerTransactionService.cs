using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Services.Interfaces.Admin;
using Microsoft.EntityFrameworkCore.Internal;
using NLog;
using IPublic = ExclusiveCard.Services.Interfaces.Public;
using dto = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Managers;

namespace ExclusiveCard.Services.Admin
{
    public class PartnerTransactionService : IPartnerTransactionService
    {
        #region Private Members and Constructor

        private readonly IPartnerTransactionManager _partnerTransactionManager;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;
        private readonly IPublic.IPartnerRewardService _partnerRewardService;
        private readonly IStatusServices _statusServices;
        private readonly ICashbackManager _cashbackManager;
        private readonly IOLD_MembershipPlanService _membershipPlanService;
        private readonly IPartnerRewardWithdrawalService _partnerRewardWithdrawalService;
        private readonly IPublic.IMembershipCardService _cardService;
        private readonly ILogger _logger;

        public PartnerTransactionService(IMapper mapper, IPublic.IPartnerRewardService partnerRewardService,
            IStatusServices statusServices,
            ICashbackManager cashbackManager,
            IOLD_MembershipPlanService membershipPlanService,
            IPartnerRewardWithdrawalService partnerRewardWithdrawalService,
            IPartnerTransactionManager partnerTransactionManager, IFileManager fileManager,
            IPublic.IMembershipCardService cardService)
        {
            _mapper = mapper;
            _partnerTransactionManager = partnerTransactionManager;
            _fileManager = fileManager;
            _partnerRewardService = partnerRewardService;
            _statusServices = statusServices;
            _cashbackManager = cashbackManager;
            _membershipPlanService = membershipPlanService;
            _partnerRewardWithdrawalService = partnerRewardWithdrawalService;
            _cardService = cardService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Writes

        public async Task<dto.Files> AddAsync(dto.Files file)
        {
            var req = MapToReq(file);
            return MapToDto(await _fileManager.AddAsync(req));
        }

        public async Task<dto.Files> UpdateAsync(dto.Files file)
        {
            var req = MapToReq(file);
            return MapToDto(await _fileManager.UpdateAsync(req));
        }

        #endregion

        #region Reads

        public async Task<List<dto.TamDataModel>> GetTransactionReport(int partnerId)
        {
            var resp = await _partnerTransactionManager.GetTransactionReport(partnerId);
            var dtoResult = _mapper.Map<List<dto.TamDataModel>>(resp);
            return dtoResult;
        }

        public async Task<dto.Files> GetByIdAsync(int id)
        {
            return MapToDto(await _fileManager.GetByIdAsync(id));
        }

        public async Task<dto.Files> GetByNameAsync(string name)
        {
            return MapToDto(await _fileManager.GetByNameAsync(name));
        }

        public async Task<List<dto.Files>> GetAllAsync()
        {
            return MapToList(await _fileManager.GetAllAsync());
        }

        public bool CheckIfFileExistsWithProcessingState(int partnerId)
        {
            return _fileManager.CheckIfFileExistsWithProcessingState(partnerId);
        }

        public async Task<dto.ProcessResult> ProcessPartnerFile(int partnerId,
            dto.ExternalFile externalFile, string errorBlob, string processedBlob, string partnerName)
        {
            try
            {
                dto.ProcessResult processResult = new dto.ProcessResult();
                List<dto.TamDataModel> tamDataModels = new List<dto.TamDataModel>();
                string contents = string.Empty;
                decimal fileAmount = decimal.Zero;
                var file = await GetByNameAsync(externalFile.FileName);
                if (file == null)
                    throw new Exception($"File not found, fileName : {externalFile.FileName}");
                List<dto.Status> status = await _statusServices.GetAll();
                using (var sr = new StreamReader(externalFile.FileMemoryContent))
                {
                    externalFile.FileMemoryContent.Seek(0, SeekOrigin.Begin);
                    contents = await sr.ReadToEndAsync();
                }

                if (!string.IsNullOrEmpty(contents))
                {
                    string[] lines = contents.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );

                    var headers = lines[0].Split(new[] { "\t" }, StringSplitOptions.None);
                    var passwordIndex = headers.IndexOf("Password");
                    lines = lines.Skip(1).ToArray();
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] content = line.Split(new[] { "\t" }, StringSplitOptions.None);
                            dto.TamDataModel tamDataModel = new dto.TamDataModel
                            {
                                TransType = content[0],
                                UniqueReference = content[1],
                                FundType = content[2],
                                Title = content[3],
                                Forename = content[4],
                                Surname = content[5],
                                NINumber = content[6],
                                Amount = decimal.Parse(content[7]),
                                IntroducerCode = content[8],
                                ProcessState = content[9]
                            };
                            if (passwordIndex > -1 && passwordIndex <= content.Length - 1)
                            {
                                tamDataModel.Password = content[passwordIndex];
                            }

                            tamDataModels.Add(tamDataModel);
                        }
                    }
                }

                if (tamDataModels.Count == 0)
                    throw new Exception($"Empty file content found, fileName : {externalFile.FileName}");
                int errorCount = tamDataModels
                    .Where(x => x.ProcessState.ToLower() == Data.Constants.Status.Error.ToLower()).ToList().Count;
                int processedCount = tamDataModels
                    .Where(x => x.ProcessState.ToLower() == Data.Constants.Status.PostedCash.ToLower()).ToList().Count;
                if (errorCount + processedCount != tamDataModels.Count)
                    throw new Exception(
                        $"Invalid ProcessState found in file and the fileName : {externalFile.FileName}");
                //all transaction in file process state is errored -> set file status - error.
                if (errorCount == tamDataModels.Count)
                {
                    processResult = new dto.ProcessResult
                    {
                        Status = Data.Constants.Status.Error,
                        Description = $"{errorCount} of {tamDataModels.Count} transaction errored."
                    };
                }

                var filtertamDataModels = tamDataModels?.Where(x =>
                    x.ProcessState.ToLower() == Data.Constants.Status.PostedCash.ToLower() ||
                    x.ProcessState.ToLower() == Data.Constants.Status.Error.ToLower()).ToList();

                //case when Processed records are empty
                if (filtertamDataModels.Count == 0)
                {
                    processResult = new dto.ProcessResult
                    {
                        Status = Data.Constants.Status.TransactionError,
                        Description = $"{filtertamDataModels.Count} of {tamDataModels.Count} transaction found."
                    };
                }

                string error = string.Empty;
                foreach (var item in filtertamDataModels)
                {
                    //1. get partner reward with membership card
                    var partnerReward = await _partnerRewardService.GetByRewardKey(item.UniqueReference);
                    if (partnerReward != null && partnerReward.MembershipCards.Count > 0)
                    {
                        //Update PartnerReward with password
                        if (!string.IsNullOrEmpty(item.Password))
                        {
                            var rewardReq = MapPartnerRewardReq(partnerReward);
                            rewardReq.Password = item.Password;
                            await _partnerRewardService.UpdateAsync(rewardReq);
                        }

                        dto.MembershipCard membershipcard = null;
                        //always only one membership card in PartnerReward record.
                        if (partnerReward.MembershipCards?.FirstOrDefault()?.Customer != null)
                        {
                            //AL: should be membershipmanager.GetActiveMembershipCard
                            membershipcard = _cardService.GetActiveMembershipCard(partnerReward.MembershipCards
                                ?.FirstOrDefault()?.Customer.AspNetUserId);
                        }
                        else
                        {
                            var plan = _membershipPlanService.GetDiamondPlan((int)partnerReward.PartnerId);
                            if (plan != null)
                            {
                                membershipcard =
                                    partnerReward.MembershipCards.FirstOrDefault(x => x.MembershipPlanId == plan.Id && x.ValidTo >= DateTime.UtcNow) ??
                                    partnerReward.MembershipCards.FirstOrDefault();
                            }
                        }

                        if (item.ProcessState.ToLower() == Data.Constants.Status.Error.ToLower())
                        {
                            //update cashbacktransaction status with membership card Id and file Id
                            await _partnerTransactionManager.UpdatePartnerTransactions(partnerReward.Id, file.Id,
                                (int)FilePayment.PartnerError);
                            //status.FirstOrDefault(x => x.IsActive && x.Name == Data.Constants.Status.PartnerError && x.Type == Data.Constants.StatusType.FilePayment).Id);
                        }
                        else if (item.ProcessState.ToLower() == Data.Constants.Status.PostedCash.ToLower())
                        {
                            fileAmount += item.Amount;
                            //update cashbacktransaction status with membership card Id and file Id
                            await _partnerTransactionManager.UpdatePartnerTransactions(partnerReward.Id, file.Id,
                                (int)FilePayment.Completed);
                            //status.FirstOrDefault(x => x.IsActive && x.Name == Data.Constants.Status.Completed && x.Type == Data.Constants.StatusType.FilePayment).Id);
                            var membershipPlan = await _membershipPlanService.Get(membershipcard.MembershipPlanId);
                            //---- Commented by Winston on 20 Sept 2019. As it was mentioned not to create an entry

                        }
                    }
                    else
                    {
                        errorCount += 1;
                        var errorMsg = item.UniqueReference;
                        error = string.IsNullOrEmpty(error) ? errorMsg : $"{error}, {errorMsg}";
                    }
                }

                if (!string.IsNullOrEmpty(error))
                {
                    error = $"Partner reward not found for {error}";
                }

                if (processedCount > 0)
                {
                    //any of the transaction processState error then file status set to Transaction Error.
                    if (errorCount > 0)
                    {
                        processResult = new dto.ProcessResult
                        {
                            Status = Data.Constants.Status.TransactionError,
                            Description = string.IsNullOrEmpty(error) ? $"{errorCount} of {tamDataModels.Count} transaction errored."
                                : $"{errorCount} of {tamDataModels.Count} transaction errored. {error} in {externalFile.FileName}"
                        };
                    }
                    else
                    {
                        processResult = new dto.ProcessResult
                        {
                            Status = Data.Constants.Status.Processed,
                            Description = $"{errorCount} of {tamDataModels.Count} transaction errored."
                        };
                    }
                }

                if (!string.IsNullOrEmpty(processResult.Status))
                {
                    //update file with status
                    file.ConfirmedAmount = fileAmount;
                    file.StatusId = status.FirstOrDefault(x =>
                            x.IsActive && x.Name == processResult.Status &&
                            x.Type == Data.Constants.StatusType.FileStatus)
                        .Id;
                    file.Location = processResult.Status == Data.Constants.Status.Processed ? $"{partnerName}-{partnerId}/{processedBlob}" : $"{partnerName}-{partnerId}/{errorBlob}";

                    await UpdateAsync(file);
                }

                return processResult;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                dto.ProcessResult result = new dto.ProcessResult
                {
                    Status = Data.Constants.Status.Error,
                    Description = ex.Message
                };
                return result;
            }
        }

        public async Task<dto.ProcessResult> ProcessTAMPositionFile(dto.ExternalFile externalFile,
            ILogger logger = null)
        {
            dto.ProcessResult processResult = new dto.ProcessResult();
            try
            {
                int successcount = 0, errorcount = 0;
                List<dto.TamPositionDataModel> tamPositionDataModels =
                    new List<dto.TamPositionDataModel>();
                string contents = string.Empty;
                var file = await GetByNameAsync(externalFile.FileName);
                if (file == null)
                    throw new Exception($"File not found, fileName : {externalFile.FileName}");
                List<dto.Status> status = await _statusServices.GetAll();
                using (var sr = new StreamReader(externalFile.FileMemoryContent))
                {
                    externalFile.FileMemoryContent.Seek(0, SeekOrigin.Begin);
                    contents = await sr.ReadToEndAsync();
                }

                if (!string.IsNullOrEmpty(contents))
                {
                    string[] lines = contents.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );
                    lines = lines.Skip(1).ToArray();
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] content = line.Split(new[] { "\t" }, StringSplitOptions.None);
                            dto.TamPositionDataModel tamPositionDM =
                                new dto.TamPositionDataModel
                                {
                                    PROVIDERREFERENCE = content[0],
                                    ASSET_NAME = content[1],
                                    ASSET_VALUE = decimal.Parse(content[2]),
                                };
                            tamPositionDataModels.Add(tamPositionDM);
                        }
                    }
                }

                if (tamPositionDataModels.Count == 0)
                    throw new Exception($"Empty file content found, fileName : {externalFile.FileName}");

                decimal fileAmount = decimal.Zero;
                foreach (var tamPosition in tamPositionDataModels)
                {
                    try
                    {
                        var partnerReward = await _partnerRewardService.GetByRewardKey(tamPosition.PROVIDERREFERENCE);
                        if (partnerReward != null)
                        {
                            dto.PartnerRewards reqPartnerRewards =
                                new dto.PartnerRewards
                                {
                                    Id = partnerReward.Id,
                                    CreatedDate = partnerReward.CreatedDate,
                                    PartnerId = partnerReward.PartnerId,
                                    RewardKey = partnerReward.RewardKey
                                };
                            reqPartnerRewards.LatestValue = tamPosition.ASSET_VALUE;
                            reqPartnerRewards.ValueDate = DateTime.UtcNow;

                            fileAmount += reqPartnerRewards.LatestValue;

                            await _partnerRewardService.UpdateAsync(reqPartnerRewards);
                            if (string.IsNullOrEmpty(processResult.Status))
                            {
                                processResult.Status = Data.Constants.Status.Processed;
                            }

                            successcount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        processResult.Status = Data.Constants.Status.Error;
                        processResult.Description = ex.ToString();
                        errorcount++;
                    }
                }

                processResult.Description = processResult.Description +
                                            $"{errorcount} of {tamPositionDataModels.Count} position error";
                //if (!string.IsNullOrEmpty(processResult.Status))
                //{
                //update file with status
                file.StatusId = status.FirstOrDefault(x =>
                        x.IsActive && x.Name == processResult.Status &&
                        x.Type == Data.Constants.StatusType.FileStatus)
                    .Id;
                file.TotalAmount = fileAmount;
                await UpdateAsync(_mapper.Map<dto.Files>(file));
                //}
            }
            catch (Exception ex)
            {
                logger?.Error(ex.ToString());
            }

            return processResult;
        }

        public async Task<dto.PagedResult<dto.Files>> GetTransactionsAsync(int statusId, int partnerId,
            int page,
            int pageSize, TransactionSortOrder sortOrder)
        {
            var result =
                await _partnerTransactionManager.GetTransactionsAsync(statusId, partnerId, page, pageSize, sortOrder);

            return ManualMappings.MapPartnerTransactionFiles(result);
        }

        public async Task<dto.PagedResult<dto.Files>> GetTransactionsAsync(int partnerId, DateTime startDate,
            DateTime endDate, int page, int pageSize, TransactionSortOrder sortOrder)
        {
            return ManualMappings.MapPartnerTransactionFiles(await _fileManager.GetTransactionsAsync(partnerId, startDate, endDate, page, pageSize, sortOrder));
        }

        public async Task<List<dto.Files>> GetTransactionsAsync(int partnerId, DateTime startDate,
            DateTime endDate, TransactionSortOrder sortOrder)
        {
            return ManualMappings.MapFiles(await _fileManager.GetTransactionsAsync(partnerId, startDate, endDate, sortOrder));
        }

        public async Task<dto.PagedResult<dto.CustomerWithdrawal>>GetPagedCustomerRewardWithdrawalsAsync(                DateTime fromDate, DateTime toDate, int page, int pageSize)
        {
            var resp = await _partnerTransactionManager.GetPagedCustomerRewardWithdrawalsAsync(fromDate, toDate, page, pageSize);
            var dtoResult = _mapper.Map<dto.PagedResult<dto.CustomerWithdrawal>>(resp);
            return dtoResult;
        }

        public async Task<List<dto.CustomerWithdrawal>> GetCustomerRewardWithdrawalsAsync(
            int partnerId, DateTime fromDate, DateTime toDate)
        {
            var resp = await _partnerTransactionManager.GetCustomerRewardWithdrawalsAsync(partnerId, fromDate, toDate);
            var dtoResult = _mapper.Map<List<dto.CustomerWithdrawal>>(resp);
            return dtoResult;
        }

        public async Task<dto.ProcessResult> ProcessWithdrawalFile(int partnerId,
            dto.ExternalFile externalFile, string errorBlob, string processedBlob, string partnerName)
        {
            try
            {
                dto.ProcessResult processResult = new dto.ProcessResult();
                List<dto.TamWithdrawalDataModel> tamDataModels = new List<dto.TamWithdrawalDataModel>();
                string contents = string.Empty;
                var file = await GetByNameAsync(externalFile.FileName);
                if (file == null)
                    throw new Exception($"File not found, fileName : {externalFile.FileName}");
                decimal fileAmount = decimal.Zero;
                //Get statuses
                List<dto.Status> status = await _statusServices.GetAll();
                using (var sr = new StreamReader(externalFile.FileMemoryContent))
                {
                    externalFile.FileMemoryContent.Seek(0, SeekOrigin.Begin);
                    contents = await sr.ReadToEndAsync();
                }

                if (!string.IsNullOrEmpty(contents))
                {
                    string[] lines = contents.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );
                    lines = lines.Skip(1).ToArray();
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] content = line.Split(new[] { "\t" }, StringSplitOptions.None);
                            try
                            {
                                dto.TamWithdrawalDataModel tamDataModel =
                                    new dto.TamWithdrawalDataModel
                                    {
                                        TransType = content[0],
                                        UniqueReference = content[1],
                                        FundType = content[2],
                                        Title = content[3],
                                        Forename = content[4],
                                        Surname = content[5],
                                        NINumber = content[6],
                                        Amount = decimal.Parse(content[7]),
                                        IntroducerCode = content[8],
                                        ProcessState = content[9]
                                    };
                                tamDataModels.Add(tamDataModel);
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                if (tamDataModels.Count == 0)
                    throw new Exception($"Empty file content found, fileName : {externalFile.FileName}");
                int errorCount = tamDataModels
                    .Where(x => x.ProcessState.ToLower() == Data.Constants.Status.Error.ToLower()).ToList().Count;
                int processedCount = tamDataModels
                    .Where(x => x.ProcessState.ToLower() == Data.Constants.Status.Confirmed.ToLower()).ToList().Count;
                if (errorCount + processedCount != tamDataModels.Count)
                    throw new Exception(
                        $"Invalid ProcessState found in file and the fileName : {externalFile.FileName}");

                //all transaction in file process state is errored -> set file status - error.
                if (errorCount == tamDataModels.Count)
                {
                    processResult = new dto.ProcessResult
                    {
                        Status = Data.Constants.Status.Error,
                        Description = $"{errorCount} of {tamDataModels.Count} transaction errored."
                    };
                }

                var filtertamDataModels = tamDataModels.Where(x =>
                    x.ProcessState.ToLower() == Data.Constants.Status.Confirmed.ToLower() ||
                    x.ProcessState.ToLower() == Data.Constants.Status.Error.ToLower()).ToList();


                //Case when Conformed are null
                if (filtertamDataModels.Count == 0)
                {
                    processResult = new dto.ProcessResult
                    {
                        Status = Data.Constants.Status.TransactionError,
                        Description = $"{filtertamDataModels.Count} of {tamDataModels.Count} transaction found in confirmed."
                    };
                }

                foreach (var item in filtertamDataModels)
                {
                    //1. get partnerReward
                    var partnerReward = await _partnerRewardService.GetByRewardKey(item.UniqueReference);
                    if (partnerReward != null && partnerReward.MembershipCards.Count > 0)
                    {
                        if (item.ProcessState.ToLower() == Data.Constants.Status.Error.ToLower())
                        {
                            //update PartnerRewardWithdrawal status with membership card Id and file Id
                            //TODO: NOT specified the status to be updated
                        }
                        else if (item.ProcessState.ToLower() == Data.Constants.Status.Confirmed.ToLower())
                        {
                            fileAmount += item.Amount;
                            //update PartnerRewardWithdrawal with amount, status and date
                            await _partnerRewardWithdrawalService.UpdateConfirmationAsync(partnerReward.Id,
                                (int)WithdrawalStatus.Confirmed, item.Amount);
                            //status.FirstOrDefault(x => x.Name == Data.Constants.Status.Confirmed && x.Type == Data.Constants.StatusType.WithdrawalStatus).Id, item.Amount);

                            //Update partnerReward with confirmed amount
                            var reqPartnerReward = MapPartnerRewardReq(partnerReward);
                            reqPartnerReward.TotalConfirmedWithdrawn += item.Amount;
                            await _partnerRewardService.UpdateAsync(reqPartnerReward);
                        }
                    }
                }

                if (processedCount > 0)
                {
                    //any of the transaction processState error then file status set to Transaction Error.
                    if (errorCount > 0)
                    {
                        processResult = new dto.ProcessResult
                        {
                            Status = Data.Constants.Status.TransactionError,
                            Description = $"{errorCount} of {tamDataModels.Count} transaction errored."
                        };
                    }
                    else
                    {
                        processResult = new dto.ProcessResult
                        {
                            Status = Data.Constants.Status.Processed,
                            Description = $"{errorCount} of {tamDataModels.Count} transaction errored."
                        };
                    }
                }

                if (!string.IsNullOrEmpty(processResult.Status))
                {
                    //update file with status
                    file.ConfirmedAmount = fileAmount;
                    file.StatusId = status.FirstOrDefault(x =>
                            x.IsActive && x.Name == processResult.Status &&
                            x.Type == Data.Constants.StatusType.FileStatus)
                        .Id;
                    file.Location = processResult.Status == Data.Constants.Status.Processed
                        ? $"{partnerName}-{partnerId}/{processedBlob}"
                        : $"{partnerName}-{partnerId}/{errorBlob}";
                    await UpdateAsync(file);
                }

                return processResult;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new dto.ProcessResult
                {
                    Description = ex.Message,
                    Status = FileStatus.Error.ToString()
                };
            }
        }

        public async Task<dto.PagedResult<dto.Files>> GetPagedFileResults(int page, int pageSize,
            int? state, string type, DateTime? createdFrom, DateTime? createdTo)
        {
            var resp = await _fileManager.GetPagedFileResults(page, pageSize, state, type, createdFrom, createdTo);
            var dtoResult = _mapper.Map<dto.PagedResult<dto.Files>>(resp);

            return dtoResult;
        }

        #endregion

        #region Private Members

        //TODO:  Rewrite manual mapping in PartnerTransactionSerivce the grown up way using Automapper.
        // Manual mappings are for Kindergarten programming school
        public dto.PartnerRewards MapPartnerRewardReq(dto.PartnerRewards data)
        {
            if (data == null)
            {
                return null;
            }
            return new dto.PartnerRewards
            {
                Id = data.Id,
                CreatedDate = data.CreatedDate,
                PartnerId = data.PartnerId,
                RewardKey = data.RewardKey,
                LatestValue = data.LatestValue,
                ValueDate = data.ValueDate,
                TotalConfirmedWithdrawn = data.TotalConfirmedWithdrawn,
                Password = data.Password
            };
        }



        private List<dto.Files> MapToList(List<Data.Models.Files> data)
        {
            if (data == null || data?.Count == 0)
                return null;

            List<dto.Files> dtos = new List<dto.Files>();

            dtos.AddRange(data.Select(MapToDto));

            return dtos;
        }

        private Data.Models.Files MapToReq(dto.Files req)
        {
            if (req == null)
                return null;
            return new Data.Models.Files
            {
                Id = req.Id,
                Name = req.Name,
                PartnerId = req.PartnerId,
                Type = req.Type,
                StatusId = req.StatusId,
                PaymentStatusId = req.PaymentStatusId,
                TotalAmount = req.TotalAmount,
                CreatedDate = req.CreatedDate,
                ChangedDate = req.ChangedDate,
                PaidDate = req.PaidDate,
                UpdatedBy = req.UpdatedBy,
                ConfirmedAmount = req.ConfirmedAmount,
                Location = req.Location
            };
        }

        private dto.Files MapToDto(Data.Models.Files req)
        {
            if (req == null)
                return null;
            var dt = new dto.Files
            {
                Id = req.Id,
                Name = req.Name,
                PartnerId = req.PartnerId,
                Type = req.Type,
                StatusId = req.StatusId,
                PaymentStatusId = req.PaymentStatusId,
                TotalAmount = req.TotalAmount,
                CreatedDate = req.CreatedDate,
                ChangedDate = req.ChangedDate,
                PaidDate = req.PaidDate,
                UpdatedBy = req.UpdatedBy,
                ConfirmedAmount = req.ConfirmedAmount,
                Location = req.Location
            };

            if (req.Status != null)
            {
                dt.Status = new dto.Status
                {
                    Id = req.Status.Id,
                    Name = req.Status.Name,
                    Type = req.Status.Type,
                    IsActive = req.Status.IsActive
                };
            }

            return dt;
        }

        

        #endregion
    }
}