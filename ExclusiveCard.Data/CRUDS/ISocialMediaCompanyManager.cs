using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface ISocialMediaCompanyManager
    {
        Task<SocialMediaCompany> Add(SocialMediaCompany company);
        Task<List<SocialMediaCompany>> GetAll();
    }
}