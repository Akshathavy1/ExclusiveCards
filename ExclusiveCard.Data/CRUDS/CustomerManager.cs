using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    [Obsolete("DO NOT WRITE ANY NEW CODE IN THIS OLD MANAGER, use Exclusivecard.Managers.Customermanager instead")]
    public class CustomerManager : ICustomerManager
    {
        #region Private Member

        private readonly ExclusiveContext _ctx;

        #endregion

        #region Constructor

        public CustomerManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        public async Task<Customer> Add(Customer customer)
        {
            DbSet<Customer> customers = _ctx.Set<Customer>();
            customers.Add(customer);
            await _ctx.SaveChangesAsync();

            return customer;
        }
        public async Task<LoginUserToken> AddLoginToken(LoginUserToken loginUserToken)
        {
            DbSet<LoginUserToken> loginUserTokens = _ctx.Set<LoginUserToken>();
            loginUserTokens.Add(loginUserToken);
            await _ctx.SaveChangesAsync();

            return loginUserToken;
        }
        public LoginUserToken GetUserTokenByToken(string token)
        {
            return _ctx.LoginUserToken
                .FirstOrDefault(x => x.Token == token);
        }

        public LoginUserToken GetUserTokenByUserId(string aspNetUserId)
        {
            return _ctx.LoginUserToken
                .FirstOrDefault(x => x.AspNetUserId == aspNetUserId);
        }

        public LoginUserToken GetUserTokenByTokenValue(Guid tokenValue)
        {
            return _ctx.LoginUserToken
                .FirstOrDefault(x => x.TokenValue == tokenValue);
        }

        public async Task<Customer> Update(Customer customer)
        {
            DbSet<Customer> customers = _ctx.Set<Customer>();
            customers.Update(customer);
            await _ctx.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> DeleteAsync(Customer customer)
        {
            DbSet<Customer> customers = _ctx.Set<Customer>();
            customers.Remove(customer);
            await _ctx.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer> GetAsync(string aspNetUserId)
        {
            return await _ctx.Customer
                .Include(x => x.ContactDetail)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.AspNetUserId == aspNetUserId);
        }

        public Customer GetDetails(int id)
        {
            return _ctx.Customer
                .Include(x => x.ContactDetail)
                .Include(x => x.CustomerBankDetails)
                    .ThenInclude(b => b.BankDetail)
                .Include(x => x.MembershipCards).AsNoTracking()
                .FirstOrDefault(x => !x.IsDeleted && x.Id == id);
        }

        public Customer GetCustomerByUserName(string userName)
        {
            return _ctx.Customer.FirstOrDefault(x => x.IdentityUser.UserName == userName);
        }

        public List<SPCustomerSearch> GetPagedSearch(string userName, string foreName, string surName,
            string cardNumber, string postCode, int? cardStatus,string registrationCode, DateTime? dob, DateTime? dateOfIssue, int page, int pageSize)
        {
            List<SPCustomerSearch> spCustomerSearches = new List<SPCustomerSearch>();


            DbCommand cmd = LoadStoredProc(_ctx, "[Exclusive].[SP_CustomerSearch]");
            cmd = WithSqlParam(cmd, "@PageNumber", page);
            cmd = WithSqlParam(cmd, "@PageSize", pageSize);
            if (!string.IsNullOrEmpty(userName))
            {
                cmd = WithSqlParam(cmd, "@Username", userName);
            }
            if (!string.IsNullOrEmpty(foreName))
            {
                cmd = WithSqlParam(cmd, "@Forename", foreName);
            }
            if (!string.IsNullOrEmpty(surName))
            {
                cmd = WithSqlParam(cmd, "@Surname", surName);
            }
            if (!string.IsNullOrEmpty(cardNumber))
            {
                cmd = WithSqlParam(cmd, "@Cardnumber", cardNumber);
            }
            if (!string.IsNullOrEmpty(postCode))
            {
                cmd = WithSqlParam(cmd, "@Postcode", postCode);
            }
            if (cardStatus > 0)
            {
                cmd = WithSqlParam(cmd, "@CardStatus", cardStatus);
            }
            if (!string.IsNullOrEmpty(registrationCode))
            {
                cmd = WithSqlParam(cmd, "@RegistrationCode", registrationCode);
            }
            if (dob.HasValue)
            {
                cmd = WithSqlParam(cmd, "@DateOfBrith", dob);
            }
            if (dateOfIssue.HasValue)
            {
                cmd = WithSqlParam(cmd, "@DateOfIssue", dateOfIssue);
            }
            spCustomerSearches = ExecuteStoredProc<SPCustomerSearch>(cmd).ToList();


            return spCustomerSearches;
        }

        public List<SPCustomerSearch> GetAllPagedSearch(int page, int pageSize)
        {
            List<SPCustomerSearch> spCustomerSearches = new List<SPCustomerSearch>();

            DbCommand cmd = LoadStoredProc(_ctx, "[Exclusive].[SP_CustomerSearch]");
            cmd = WithSqlParam(cmd, "@PageNumber", page);
            cmd = WithSqlParam(cmd, "@PageSize", pageSize);
            spCustomerSearches = ExecuteStoredProc<SPCustomerSearch>(cmd).ToList();

            return spCustomerSearches;
        }

        public async Task<Customer> GetCustomerAsync(int id)
        {
            return await _ctx.Customer
                .Include(x => x.ContactDetail)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
        }

        public async Task<List<Customer>> GetCustomersToBeAddedToSendGridAsync()
        {
            return await _ctx.Customer.AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive && x.MarketingNewsLetter && x.SendGridContact == null && x.MembershipCards.Count > 0 
                        && x.MembershipCards.Any(c=>c.IsActive && !c.IsDeleted && c.ValidTo >= DateTime.UtcNow && c.MembershipStatus.Name == Data.Constants.Status.Active)
                        && x.ContactDetail.EmailAddress != null && x.ContactDetail.EmailAddress != "")
                .Include(x => x.ContactDetail)
                .Include(x => x.MembershipCards).ThenInclude(y => y.MembershipPlan)                 //.ThenInclude(y => y.Partner).ThenInclude(z => z.WhiteLabelSettings)
                .ToListAsync();
        }

        public async Task<List<Customer>> GetCustomersToBeRemovedFromSendGridAsync()
        {
            return await _ctx.Customer.AsNoTracking()
                .Where(x => !x.MarketingNewsLetter && x.SendGridContact != null)
                .Include(x => x.SendGridContact)
                .ToListAsync();
        }

        #region Store Procedure

        public static DbCommand LoadStoredProc(DbContext context, string storedProcName)
        {
            var cmd = context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = storedProcName;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return cmd;
        }

        public static DbCommand WithSqlParam(DbCommand cmd, string paramName, object paramValue)
        {
            if (string.IsNullOrEmpty(cmd.CommandText))
                throw new InvalidOperationException("Call LoadStoredProc before using this method");

            var param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue;
            cmd.Parameters.Add(param);
            return cmd;
        }

        private static IList<T> MapToList<T>(DbDataReader dr)
        {
            var objList = new List<T>();
            var props = typeof(T).GetRuntimeProperties();

            var colMapping = dr.GetColumnSchema()
                .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                .ToDictionary(key => key.ColumnName.ToLower());

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    T obj = Activator.CreateInstance<T>();
                    foreach (var prop in props)
                    {
                        var val = dr.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                        prop.SetValue(obj, val == DBNull.Value ? null : val);
                    }
                    objList.Add(obj);
                }
            }
            return objList;
        }

        public static IList<T> ExecuteStoredProc<T>(DbCommand command)
        {
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    var reader = command.ExecuteReader();
                    using (reader)
                    {
                        return MapToList<T>(reader);
                    }
                }
                
                finally
                {
                    command.Connection.Close();
                }
                return default(IList<T>);
            }
        }

        #endregion
    }
}
