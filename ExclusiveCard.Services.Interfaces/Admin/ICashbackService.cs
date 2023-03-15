using System;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface ICashbackService
    {
        int GetTransactionReport(DateTime? dateFrom, DateTime? dateTo, string apiId, string apiKey);

        Task<string> MigrateCashbackTransactions(string adminEmail, string cashbackConfirmedInDays);
    }
}