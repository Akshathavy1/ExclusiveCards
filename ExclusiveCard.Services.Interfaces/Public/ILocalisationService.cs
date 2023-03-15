using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface ILocalisationService
    {
        #region Writes

        Task<Localisation> Add(Localisation locale);

        Task<Localisation> Update(Localisation locale);

        #endregion

        #region Reads

        Localisation Get(int id);
        Task<List<Localisation>> GetAll(string localisationCode);
        string GetByContext(string context, string localisationCode);

        #endregion
    }
}
