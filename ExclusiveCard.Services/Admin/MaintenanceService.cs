using System.Threading.Tasks;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
    public class MaintenanceService:IMaintenanceService
    {
        private readonly IMaintenanceManager _maintenanceManager;

        #region Constructor

        public MaintenanceService(IMaintenanceManager maintenanceManager)
        {
            _maintenanceManager = maintenanceManager;
        }
        #endregion

        public async Task ExecuteSPRunDailyMaintenanceTasks()
        {
            await _maintenanceManager.ExecuteSPRunDailyMaintenanceTasks();
        }
    }
}
