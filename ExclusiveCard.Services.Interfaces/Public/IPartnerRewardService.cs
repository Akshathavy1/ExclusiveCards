using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IPartnerRewardService
    {
        Task<Models.DTOs.PartnerRewards> AddAsync(PartnerRewards reward);
        Task<Models.DTOs.PartnerRewards> UpdateAsync(PartnerRewards reward);

        Task<Models.DTOs.PartnerRewards> GetByIdAsync(int id);
        Task<Models.DTOs.PartnerRewards> GetByRewardKey(string key);
        Task<List<Models.DTOs.PartnerRewards>> GetAllAsync();
    }
}