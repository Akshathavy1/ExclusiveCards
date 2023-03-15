using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IFileManager
    {
        Task<Files> AddAsync(Files file);
        Task<Files> UpdateAsync(Files file);
        Task<Files> GetByIdAsync(int id);
        Task<Files> GetByNameAsync(string name);
        Task<List<Files>> GetAllAsync();
        Task<PagedResult<Files>> GetPagedFileResults(int page, int pageSize, int? state, string type, DateTime? createdFrom, DateTime? createdTo);
        bool CheckIfFileExistsWithProcessingState(int partnerId);

        Task<List<Files>> GetTransactionsAsync(int partnerId, DateTime startDate,
            DateTime endDate, TransactionSortOrder sortOrder);

        Task<PagedResult<Files>> GetTransactionsAsync(int partnerId, DateTime startDate,
            DateTime endDate, int page, int pageSize, TransactionSortOrder sortOrder);
    }
}