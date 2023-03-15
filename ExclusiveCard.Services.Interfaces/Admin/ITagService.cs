using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface ITagService
    {
        #region Writes

        Task<Tag> Add(Tag tag);

        Task<Tag> Update(Tag tag);

        #endregion

        #region Reads

        Task<List<Tag>> GetAll();

        #endregion
    }
}
