using NUnit.Framework;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.IntegrationTests.Admin
{
    class AdminDeleteMercahnt
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task DeleteMerchant()
        {
            dto.Merchant merchant = Common.Common.BuildMerchantModel();
            dto.Merchant merchantAdded = await ServiceHelper.Instance.MerchantService.Add(merchant);

        }
    }
}
