using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IBankDetailManager
    {
        Task<BankDetail> Add(BankDetail bankDetail);
        Task<BankDetail> Update(BankDetail bankDetail);
        Task<BankDetail> Get(int id);
        Task<List<BankDetail>> GetAll();
    }
}