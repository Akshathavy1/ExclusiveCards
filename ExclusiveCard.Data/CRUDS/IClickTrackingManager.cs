using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IClickTrackingManager
    {
        Task<ClickTracking> Add(ClickTracking clickTracking);
        Task<ClickTracking> Update(ClickTracking clickTracking);
        ClickTracking Get(int id);
    }
}