using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IPartnerRewardManager
    {
        Task<PartnerRewards> AddAsync(PartnerRewards reward);
        Task<PartnerRewards> UpdateAsync(PartnerRewards reward);
        Task<PartnerRewards> GetByIdAsync(int id);
        Task<PartnerRewards> GetByRewardKey(string key);
        Task<List<PartnerRewards>> GetAllAsync();
    }
}