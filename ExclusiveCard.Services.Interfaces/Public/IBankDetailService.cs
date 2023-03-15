using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IBankDetailService
    {
        #region Write  

        Task<dto.BankDetail> Add(dto.BankDetail bankDetail);

        Task<dto.BankDetail> Update(dto.BankDetail bankDetail);

        #endregion

        #region Reads

        Task<dto.BankDetail> Get(int id);

        Task<List<dto.BankDetail>> GetAll();

        #endregion
    }
}
