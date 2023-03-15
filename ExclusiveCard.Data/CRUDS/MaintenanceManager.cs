using ExclusiveCard.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExclusiveCard.Data.CRUDS
{
    public class MaintenanceManager : IMaintenanceManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public MaintenanceManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        public async Task ExecuteSPRunDailyMaintenanceTasks()
        {
            var sqlstr = "Execute [Exclusive].[SP_RunDailyMaintenanceTasks]";
            await _ctx.Database.ExecuteSqlCommandAsync(sqlstr);
        }

    }
}
