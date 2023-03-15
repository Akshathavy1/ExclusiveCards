using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;
using System.Threading.Tasks;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Public
{
    public class PaymentProviderService : IPaymentProviderService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IPaymentProviderManager _paymentProviderManager;

        #endregion

        #region Constructor

        public PaymentProviderService(IMapper mapper, IPaymentProviderManager paymentProviderManager)
        {
            _mapper = mapper;
            _paymentProviderManager = paymentProviderManager;
        }

        #endregion

        #region Writes

        //Add PaymentProvider

        public async Task<Models.DTOs.PaymentProvider> Add(DTOs.PaymentProvider paymentProvider)
        {
            PaymentProvider req = _mapper.Map<PaymentProvider>(paymentProvider);
            return _mapper.Map<Models.DTOs.PaymentProvider>(
                await _paymentProviderManager.Add(req));
        }

        //Update PaymentProvider

        public async Task<Models.DTOs.PaymentProvider> Update(DTOs.PaymentProvider paymentProvider)
        {
            PaymentProvider req = _mapper.Map<PaymentProvider>(paymentProvider);
            return _mapper.Map<Models.DTOs.PaymentProvider>(
                await _paymentProviderManager.Update(req));
        }

        #endregion

        #region Reads

        //GetByName PaymentProvider

        public async Task<Models.DTOs.PaymentProvider> GetByName (string name)
        {
            return MapToDto(await _paymentProviderManager.GetByName(name));
        }

        #endregion

        #region Private Member 

        private Models.DTOs.PaymentProvider MapToDto(PaymentProvider payment)
        {
            if(payment == null)
                return  new Models.DTOs.PaymentProvider();
            Models.DTOs.PaymentProvider paymentprovider = new Models.DTOs.PaymentProvider
            {
                Name =  payment.Name,
                IsActive = payment.IsActive
            };
            if (payment.Id > 0)
            {
                paymentprovider.Id = payment.Id;
            }

            return paymentprovider;
        }

        #endregion
    }
}
