using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IContactsManager
    {
        Task<ContactDetail> Add(ContactDetail contactDetail);
        Task<ContactDetail> Update(ContactDetail contactDetail);
        Task<ContactDetail> Get(int id);
    }
}