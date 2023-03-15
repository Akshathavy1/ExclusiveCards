using ExclusiveCard.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using st = ExclusiveCard.Data.StagingModels;
using dto = ExclusiveCard.Services.Models.DTOs;
using System.Threading.Tasks;

namespace ExclusiveCard.Managers
{    
    public class StagingCashbackManager : IStagingCashbackManager
    {
        #region Private Field and Constructors

        private readonly IRepository<st.CashbackTransaction> _txnRepo;
        private readonly IRepository<st.TransactionFile> _txnFileRepo;
        private readonly IRepository<st.CashbackTransactionError> _errorRepo;
        private readonly IMapper _mapper;

        public StagingCashbackManager(IRepository<st.TransactionFile> txnFileRepo, IRepository<st.CashbackTransaction> txnRepo,
                                    IRepository<st.CashbackTransactionError> errorRepo,  IMapper mapper)
        {
            _txnRepo = txnRepo;
            _txnFileRepo = txnFileRepo;
            _errorRepo = errorRepo;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        #region TransactionFiles

        public int CreateTransactionFile(dto.StagingModels.TransactionFile file)
        {
            var dbFile = _mapper.Map<st.TransactionFile>(file);
            _txnFileRepo.Create(dbFile);
            _txnFileRepo.SaveChanges();

            return dbFile.Id;
        }

        public async Task<List<dto.StagingModels.TransactionFile>> GetTransactionFilesAsync(Enums.StagingTransactionFiles fileStatus)
        {
            int statusId = (int)fileStatus;
            var dbFiles = _txnFileRepo.FilterNoTrack(x => x.StatusId == statusId);

            var files = _mapper.Map<List<dto.StagingModels.TransactionFile>>(dbFiles);

            await Task.CompletedTask;

            return files;
        }

        public void SetTransactionFileStatus(int fileId, Enums.StagingTransactionFiles fileStatus)
        {
            var dbFile = _txnFileRepo.GetById(fileId);
            if (dbFile != null)
            {
                dbFile.StatusId = (int)fileStatus;
                _txnFileRepo.Update(dbFile);
                _txnFileRepo.SaveChanges();
            }
        }

        #endregion

        #region Transactions

        public int CreateTransaction(dto.StagingModels.CashbackTransaction transaction)
        {
            var dbTransaction = _mapper.Map<st.CashbackTransaction>(transaction);

            _txnRepo.Create(dbTransaction);
            _txnRepo.SaveChanges();

            return dbTransaction.Id;
        }

        public async Task<List<dto.StagingModels.CashbackTransaction>> GetTransactionsAsync(Enums.StagingCashbackTransactions status)
        {
            int statusId = (int)status;
            var dbTransactions = await _txnRepo.FilterNoTrackAsync(x => x.RecordStatusId == statusId);

            var transactions = _mapper.Map<List<dto.StagingModels.CashbackTransaction>>(dbTransactions);
            return transactions;
        }

        public void SetTransactionStatus(int transactionId, Enums.StagingCashbackTransactions status)
        {
            var dbTransaction = _txnRepo.GetById(transactionId);
            if (dbTransaction != null)
            {
                dbTransaction.RecordStatusId = (int)status;
                _txnRepo.Update(dbTransaction);
                _txnRepo.SaveChanges();
            }
        }

        public void UpdateTransaction(dto.StagingModels.CashbackTransaction transaction)
        {
            var dbTransaction = _txnRepo.GetById(transaction.Id);

            if (dbTransaction != null)
            {
                _mapper.Map(transaction, dbTransaction);
                _txnRepo.Update(dbTransaction);
                _txnRepo.SaveChanges();
            }
        }
        #endregion

        #region Transaction Errors

        public void CreateError(dto.StagingModels.CashbackTransactionError error)
        {
            var dbError = _mapper.Map<st.CashbackTransactionError>(error);
            _errorRepo.Create(dbError);
            _errorRepo.SaveChanges();
        }

        #endregion

        #endregion
    }
}
