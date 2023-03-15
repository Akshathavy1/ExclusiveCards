using AutoMapper;
using ExclusiveCard.Services.Models.DTOs;
using db = ExclusiveCard.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ExclusiveCard.Data.Repositories;
using System.Linq;

namespace ExclusiveCard.Managers
{
    public class BankDetailManager : IBankDetailManager
    {
        private IMapper _mapper = null;
        private IRepository<db.BankDetail> _bankDetailRepo = null;
        private IRepository<db.CustomerBankDetail> _customerbankDetailRepo = null;

        public BankDetailManager(IMapper mapper, IRepository<db.BankDetail> bankDetailRepo, IRepository<db.CustomerBankDetail> customerbankDetailRepo)
        {
            _mapper = mapper;
            _bankDetailRepo = bankDetailRepo;
            _customerbankDetailRepo = customerbankDetailRepo;
        }

        public BankDetail Get(int bankDetailId)
        {
            BankDetail bankDetail = null;

            var dbbankDetail = _bankDetailRepo.GetById(bankDetailId);
            if (dbbankDetail != null)
                bankDetail = _mapper.Map<BankDetail>(dbbankDetail);

            return bankDetail;
        }
        public BankDetail Create(BankDetail bankDetail)
        {
            var dbbankDetail = _mapper.Map<db.BankDetail>(bankDetail);
            _bankDetailRepo.Create(dbbankDetail);
            _bankDetailRepo.SaveChanges();

            bankDetail = _mapper.Map<BankDetail>(dbbankDetail);

            return bankDetail;
        }
        public BankDetail Update(BankDetail bankDetail)
        {
            var dbbankDetail = _bankDetailRepo.GetById(bankDetail.Id);
            _mapper.Map(bankDetail, dbbankDetail);
            _bankDetailRepo.Update(dbbankDetail);
            _bankDetailRepo.SaveChanges();

            bankDetail = _mapper.Map<BankDetail>(dbbankDetail);

            return bankDetail;
        }

        public CustomerBankDetail GetCustomerBankDetail(int customerId, int bankDetailId = 0)
        {
            CustomerBankDetail customerBankDetail = null;

           var dbResult = _customerbankDetailRepo.GetNoTrack(x => x.CustomerId == customerId && (x.BankDetailsId == bankDetailId || bankDetailId < 1) && x.IsActive == true && x.IsDeleted == false);
            if (dbResult != null)
                customerBankDetail = _mapper.Map<CustomerBankDetail>(dbResult);

            return customerBankDetail;
        }
        public CustomerBankDetail CreateCustomerBankDetail(CustomerBankDetail customerBankDetail)
        {
            if (customerBankDetail == null || customerBankDetail.CustomerId < 1 || (customerBankDetail.BankDetail == null && customerBankDetail.BankDetailsId < 1))
            {
                throw new ArgumentException("Configuration invalid", "CustomerBankDetail");
            }

            var dbCustomerBankDetail = _mapper.Map<db.CustomerBankDetail>(customerBankDetail);
            _customerbankDetailRepo.Create(dbCustomerBankDetail);
            _customerbankDetailRepo.SaveChanges();
            customerBankDetail = _mapper.Map<CustomerBankDetail>(dbCustomerBankDetail);

            return customerBankDetail;
        }
        public CustomerBankDetail UpdateCustomerBankDetail(CustomerBankDetail customerBankDetail)
        {
            if (customerBankDetail == null || customerBankDetail.CustomerId < 1 || customerBankDetail.BankDetailsId < 1)
            {
                throw new ArgumentException("Configuration invalid", "CustomerBankDetail");
            }

            var dbOriginalCBD = _customerbankDetailRepo.Get(x => x.CustomerId == customerBankDetail.CustomerId && x.BankDetailsId == customerBankDetail.BankDetailsId);
            //Create customer bank detail if needed
            if (dbOriginalCBD != null &&
                      (customerBankDetail.DateMandateAccepted != dbOriginalCBD.DateMandateAccepted ||
                        customerBankDetail.IsActive != dbOriginalCBD.IsActive ||
                        customerBankDetail.IsDeleted != dbOriginalCBD.IsDeleted ||
                        customerBankDetail.MandateAccepted != dbOriginalCBD.MandateAccepted ||
                        customerBankDetail.BankDetailsId != dbOriginalCBD.BankDetailsId))
            {

                _mapper.Map(customerBankDetail, dbOriginalCBD);

                _customerbankDetailRepo.Update(dbOriginalCBD);

                customerBankDetail = _mapper.Map<CustomerBankDetail>(dbOriginalCBD);
            }

            return customerBankDetail;
        }

    }
}
