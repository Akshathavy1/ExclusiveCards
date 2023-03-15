using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IStatusManager
    {
        Task<Models.Status> Add(Models.Status status);
        Task<List<Models.Status>> AddRange(List<Models.Status> status);
        Task<List<Models.Status>> GetAll(string type);
        Task DeleteRangeAsync();
    }
}