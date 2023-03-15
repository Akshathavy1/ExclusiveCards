using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface ITagService
    {
        #region Reads

        Task<List<Models.DTOs.Tag>> GetAll();

        #endregion
    }
}