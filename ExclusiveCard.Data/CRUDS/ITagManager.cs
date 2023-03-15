using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface ITagManager
    {
        Task<Tag> Add(Tag tag);
        Task<Tag> Update(Tag tag);
        Task<List<Tag>> GetAll();
    }
}