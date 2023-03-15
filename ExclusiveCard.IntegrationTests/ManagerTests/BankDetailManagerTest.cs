using db = ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Data.Repositories;
using ExclusiveCard.Managers;
using ExclusiveCard.Enums;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using AutoMapper;
using Microsoft.SqlServer.Server;

namespace ExclusiveCard.IntegrationTests.ManagerTests
{
    class BankDetailManagerTest
    {
        private IMapper _mapper;

        private IBankDetailManager _bankDetailManager;
        private IRepository<db.Customer> _customerRepo = null;

        [SetUp]
        public void Setup()
        {
            _mapper = Configuration.ServiceProvider.GetService<IMapper>();
            _bankDetailManager = Configuration.ServiceProvider.GetService<IBankDetailManager>();
            _customerRepo = Configuration.ServiceProvider.GetService<IRepository<db.Customer>>();
        }

        [Test]
        public void CreateBankDetailTest()
        {
            var dtoBankdetail = CreateDtoBankDetail();
            Assert.IsNotNull(dtoBankdetail, "Couldn't create BankDetail DTO");
            var result = _bankDetailManager.Create(dtoBankdetail);
            Assert.IsTrue(result.Id > 0, "Couldn't create BankDetail record");
        }

        [Test]
        public void GetBankDetailTest()
        {
            var dtoBankdetail = CreateDtoBankDetail();
            Assert.IsNotNull(dtoBankdetail, "Couldn't create BankDetail DTO");
            dtoBankdetail = _bankDetailManager.Create(dtoBankdetail);
            Assert.IsTrue(dtoBankdetail.Id > 0, "Couldn't create BankDetail record");

            var result = _bankDetailManager.Get(dtoBankdetail.Id);
            Assert.IsNotNull(result, "Could not find BankDetail record");
            Assert.IsTrue(result.Id > 0, "BankDetail record has no id");
        }

        [Test]
        public void UpdateBankDetailTest()
        {

            BankDetail dtoBankdetail = CreateDtoBankDetail();
            Assert.IsNotNull(dtoBankdetail, "Couldn't create BankDetail DTO");

            dtoBankdetail = _bankDetailManager.Create(dtoBankdetail);
            Assert.IsTrue(dtoBankdetail.Id > 0, "Couldn't create BankDetail record");

            dtoBankdetail.AccountName = "Updated";
            try
            {
                dtoBankdetail = _bankDetailManager.Update(dtoBankdetail);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            Assert.IsNotNull(dtoBankdetail, "Could not find BankDetail record");
            Assert.IsTrue(dtoBankdetail.Id > 0, "BankDetail record has no id");
        }

        [Test]
        public void CreateCustomerBankDetailTest()
        {
            var custId = GetACustomerId();
            Assert.IsTrue(custId > 0, "Couldn't find a customer Id");

            var dtoBankdetail = CreateDtoBankDetail();
            Assert.IsNotNull(dtoBankdetail, "Couldn't create BankDetail DTO");
            dtoBankdetail = _bankDetailManager.Create(dtoBankdetail);
            Assert.IsTrue(dtoBankdetail.Id > 0, "Couldn't create BankDetail record");

            var dtocustbankDetail = CreateDtoCustomerBankDetail(custId, dtoBankdetail.Id);
            Assert.IsNotNull(dtocustbankDetail, "Couldn't create CustomerBankDetail DTO");

            dtocustbankDetail = _bankDetailManager.CreateCustomerBankDetail(dtocustbankDetail);
            Assert.IsNotNull(dtocustbankDetail, "Couldn't create CustomerBankDetail record");
        }

        [Test]
        public void GetCustomerBankDetailTest()
        {
            var custId = GetACustomerId();
            Assert.IsTrue(custId > 0, "Couldn't find a customer Id");

            var dtoBankdetail = CreateDtoBankDetail();
            Assert.IsNotNull(dtoBankdetail, "Couldn't create BankDetail DTO");
            dtoBankdetail = _bankDetailManager.Create(dtoBankdetail);
            Assert.IsTrue(dtoBankdetail.Id > 0, "Couldn't create BankDetail record");

            var dtocustbankDetail = CreateDtoCustomerBankDetail(custId, dtoBankdetail.Id);
            Assert.IsNotNull(dtocustbankDetail, "Couldn't create CustomerBankDetail DTO");

            dtocustbankDetail = _bankDetailManager.CreateCustomerBankDetail(dtocustbankDetail);
            Assert.IsNotNull(dtocustbankDetail, "Couldn't create CustomerBankDetail record");

            var result = _bankDetailManager.GetCustomerBankDetail(custId, dtoBankdetail.Id);
            Assert.IsNotNull(result, "Could not find CustomerBankDetail record");
            Assert.IsTrue(result.CustomerId > 0 && result.BankDetailsId > 0, "BankDetail record has no id");

        }

        [Test]
        public void UpdateCustomerBankDetailTest()
        {
            var custId = GetACustomerId();
            Assert.IsTrue(custId > 0, "Couldn't find a customer Id");

            var dtoBankdetail = CreateDtoBankDetail();
            Assert.IsNotNull(dtoBankdetail, "Couldn't create BankDetail DTO");
            dtoBankdetail = _bankDetailManager.Create(dtoBankdetail);
            Assert.IsTrue(dtoBankdetail.Id > 0, "Couldn't create BankDetail record");

            var dtocustbankDetail = CreateDtoCustomerBankDetail(custId, dtoBankdetail.Id);
            Assert.IsNotNull(dtocustbankDetail, "Couldn't create CustomerBankDetail DTO");

            dtocustbankDetail = _bankDetailManager.CreateCustomerBankDetail(dtocustbankDetail);
            Assert.IsNotNull(dtocustbankDetail, "Couldn't create CustomerBankDetail record");

            dtocustbankDetail.DateMandateAccepted = DateTime.Now;
            dtocustbankDetail.MandateAccepted = true;

            try
            {
                dtocustbankDetail = _bankDetailManager.UpdateCustomerBankDetail(dtocustbankDetail);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            Assert.IsNotNull(dtocustbankDetail, "Could not update CustomerBankDetail record");
            Assert.IsTrue(dtocustbankDetail.MandateAccepted == true, "CustomerBankDetail record has not been changed");

        }

        private int GetACustomerId()
        {
            //Just get any active customer id
            var dbCust = _customerRepo.FilterNoTrack(x => x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
            var result = (dbCust != null) ? dbCust.Id : 0;
            return result;
        }

        private CustomerBankDetail CreateDtoCustomerBankDetail(int customerId, int bankDetailId)
        {
            var customerBankDetail = new CustomerBankDetail()
            {
                CustomerId = customerId,
                BankDetailsId = bankDetailId,
                IsActive = true,
                IsDeleted = false
            };
            return customerBankDetail;
        }

        private BankDetail CreateDtoBankDetail()
        {
            string detail = DateTime.UtcNow.Ticks.ToString();

            var bankdetail = new BankDetail()
            {
                AccountName = $"AC{detail}",
                BankName = $"BN{detail}",
                AccountNumber = detail.PadRight(11, '0').Substring(0, 11),
                SortCode = detail.PadRight(6, '0').Substring(0, 6),
                IsDeleted = false
            };
            return bankdetail;
        }

    }
}
