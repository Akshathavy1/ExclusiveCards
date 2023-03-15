using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface ILocalisationManager
    {
        Task<Localisation> Add(Localisation locale);
        Task<Localisation> Update(Localisation locale);
        Localisation Get(int id);
        Task<List<Localisation>> GetAll(string localisationCode);
        string GetByContext(string context, string localisationCode);
    }
}