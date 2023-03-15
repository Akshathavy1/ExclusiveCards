using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IClickTrackingService
    {
        #region Writes

        Task<ClickTracking> Add(ClickTracking membershipCard);

        Task<ClickTracking> Update(ClickTracking membershipCard);

        #endregion

        #region Reads

        ClickTracking Get(int id);

        #endregion
    }
}