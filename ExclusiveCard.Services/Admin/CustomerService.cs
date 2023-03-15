using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Services.Interfaces.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;


namespace ExclusiveCard.Services.Admin
{
    public class CustomerService : ICustomerService
    {
        #region Private members

        private readonly IMapper _mapper;
        private readonly ICustomerManager _customerManager;
        private readonly ILogger _logger;

        #endregion

        #region Constuctor

        public CustomerService(IMapper mapper, ICustomerManager customerManager)
        {
            _customerManager = customerManager;
            _mapper = mapper;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Write
        public async Task<dto.Customer> Add(dto.Customer customer)
        {
            Customer creq = _mapper.Map<Customer>(customer);
            return _mapper.Map<dto.Customer>(await _customerManager.Add(creq));
        }

        public async Task<dto.Customer> Update(dto.Customer customer)
        {
            Customer creq = _mapper.Map<Customer>(customer);
            return _mapper.Map<dto.Customer>(await _customerManager.Update(creq));
        }

        #endregion

        #region Reads

        public dto.Customer GetDetails(int id)
        {
            var cust = _customerManager.GetDetails(id);
            var result = _mapper.Map<dto.Customer>(cust);
            return result;

            //Manual Mappings causes crash!!
            //return ManualMappings.MapCustomer(_customerManager.GetDetails(id));
        }

        public dto.PagedResult<dto.CustomerSummary> GetAllPagedSearch(int page, int pageSize)
        {
            var list = _customerManager.GetAllPagedSearch(page, pageSize);
            return Map(list, page, pageSize);
        }

        public dto.PagedResult<dto.CustomerSummary> GetPagedSearch(string userName, string foreName,
            string surName, string cardNumber, string postCode, int? cardStatus,string registrationCode, DateTime? dob, DateTime? dateOfIssue, int page, int pageSize)
        {
            var list = _customerManager.GetPagedSearch(userName, foreName, surName, cardNumber, postCode, cardStatus, registrationCode,
                dob, dateOfIssue, page, pageSize);
            return Map(list, page, pageSize);
        }

        #endregion

        #region Private Method

        private dto.PagedResult<dto.CustomerSummary> Map(List<SPCustomerSearch> spCustomer, int page, int pageSize)
        {
            dto.PagedResult<dto.CustomerSummary> result = new dto.PagedResult<dto.CustomerSummary>();
            foreach (var item in spCustomer)
            {
                var customer = new dto.CustomerSummary()
                {
                    Id = item.Id,
                    Forename = item.Forename,
                    Surname = item.Surname,
                    Postcode = item.PostCode,
                    CardNumber = item.CardNumber,
                    Username = item.UserName
                };
                if (item.DateOfBirth.HasValue)
                {
                    customer.Dob = item.DateOfBirth;
                }
                result.Results.Add(customer);
            }

            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = Convert.ToInt32(spCustomer?.FirstOrDefault()?.TotalRecord);
            double pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            return result;
        }

        #endregion
    }
}
