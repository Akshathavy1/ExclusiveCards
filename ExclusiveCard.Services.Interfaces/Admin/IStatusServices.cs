using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
   public interface IStatusServices
   {
        #region writes

        Task<Status> Add(Status status);

        Task<List<Status>> AddRange(List<Status> statuses);

        Task DeleteRangeAsync();

        #endregion


        #region Reads  

        Task<List<Status>> GetAll(string type = null);

        #endregion
    }
}
