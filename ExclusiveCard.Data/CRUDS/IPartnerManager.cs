using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IPartnerManager
    {
        Task<Partner> Add(Partner partner);
        Task<Partner> Update(Partner partner);
        Task<Partner> GetByIdAsync(int id);
        Task<Partner> GetByNameAsync(string name);
        Task<List<Partner>> GetAllAsync(int? type);
    }
}