using ExclusiveCard.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface IOfferBranchServices
    {
        Task<List<int>> GetofferBranch(int offerId);
    }
}