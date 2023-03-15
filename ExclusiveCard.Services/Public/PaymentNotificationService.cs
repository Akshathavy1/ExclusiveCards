using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;
using System.Threading.Tasks;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Public
{
    public class PaymentNotificationService : IPaymentNotificationService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IPaymentNotificationManager _paymentNotificationManager;

        #endregion

        #region Constructor

        public PaymentNotificationService(IMapper mapper, IPaymentNotificationManager paymentNotificationManager)
        {
            _mapper = mapper;
            _paymentNotificationManager = paymentNotificationManager;
        }

        #endregion

        #region Writes

        //Add PaymentNotification

        public async Task<DTOs.PaymentNotification> Add(
            DTOs.PaymentNotification paymentNotification)
        {
            PaymentNotification req = _mapper.Map<PaymentNotification>(paymentNotification);
            var resp = await _paymentNotificationManager.Add(req);
            return MapToDTo(resp);
        }

        //Update PaymentNotification

        public async Task<DTOs.PaymentNotification> Update(DTOs.PaymentNotification paymentNotification)
        {
            PaymentNotification req = _mapper.Map<PaymentNotification>(paymentNotification);
            return MapToDTo(await _paymentNotificationManager.Update(req));
        }

        public async Task DeleteAsync(DTOs.PaymentNotification paymentNotification)
        {
            PaymentNotification req = _mapper.Map<PaymentNotification>(paymentNotification);
            await _paymentNotificationManager.DeleteAsync(req);
        }

        #endregion

        #region Reads

        public async Task<DTOs.PaymentNotification> GetByCustomerProviderId(string customerProviderId, int paymentProviderId, string trnType = null)
        {
            //return _mapper.Map<DTOs.PaymentNotification>(
            //    await _paymentNotificationManager.GetByCustomerProviderId(customerProviderId, paymentProviderId));
            return MapToDTo(await _paymentNotificationManager.GetByCustomerProviderId(customerProviderId, paymentProviderId, trnType));
        }

        #endregion

        #region Private Members

        private DTOs.PaymentNotification MapToDTo(PaymentNotification payment)
        {
            if (payment == null)
                return null;
            DTOs.PaymentNotification paymentNotification = new DTOs.PaymentNotification
            {
               PaymentProviderId = payment.PaymentProviderId,
               CustomerPaymentProviderId = payment.CustomerPaymentProviderId,
               TransactionType = payment.TransactionType,
               DateReceived = payment.DateReceived,
               FullMessage = payment.FullMessage
            };
            if (payment.Id > 0)
            {
                paymentNotification.Id = payment.Id;
            }

            return paymentNotification;
        }

        #endregion

    }
}
