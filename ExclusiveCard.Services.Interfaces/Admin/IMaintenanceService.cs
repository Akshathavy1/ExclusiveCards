using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IMaintenanceService
    {
        #region writes
        Task ExecuteSPRunDailyMaintenanceTasks();
        #endregion

    }
}
