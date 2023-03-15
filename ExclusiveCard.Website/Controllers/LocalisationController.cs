using ExclusiveCard.Services.Interfaces.Public;
using Microsoft.Extensions.Caching.Memory;

namespace ExclusiveCard.Website.Controllers
{
    public class LocalisationController : BaseController
    {
        private readonly IMemoryCache _cache;
        private readonly ILocalisationService _localisationService;

        public LocalisationController(IMemoryCache cache, ILocalisationService localisationService)
        {
            _cache = cache;
            _localisationService = localisationService;
        }
    }
}