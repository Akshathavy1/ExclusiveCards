using ExclusiveCard.Providers.Marketing.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Managers
{
    public interface IContactManager
    {
        void Add(MarketingContact sendGridContact);

        Task Add(List<MarketingContact> sendGridContact);

        Task Update(MarketingContact sendGridContact);

        Task Delete(MarketingContact detail);

        Task Delete(List<MarketingContact> sendGridContact);

        Task<MarketingContact> GetByCustomerId(int customerId);

        Task<List<MarketingContact>> GetAll();

    }
}
