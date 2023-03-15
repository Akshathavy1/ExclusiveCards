using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IMaintenanceManager
    {
        Task ExecuteSPRunDailyMaintenanceTasks();
    }
}