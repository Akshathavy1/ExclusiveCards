using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IContactDetailService
    {
        #region Writes

        Task<ContactDetail> Add(ContactDetail contactDetail);

        Task<ContactDetail> Update(ContactDetail contactDetail);

        #endregion

        #region Reads

        Task<ContactDetail> Get(int id);

        #endregion
    }
}