using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Public;
using DTOs = ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.Services.Public
{
    [Obsolete]
    public class CustomerService : ICustomerService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly ICustomerManager _customerManager;

        #endregion

        #region Constructor

        public CustomerService(IMapper mapper, ICustomerManager customerManager)
        {
            _mapper = mapper;
            _customerManager = customerManager;
        }

        #endregion

        #region Writes

        //TODO:  Remove the Customer CRUD in the old customer "service"
        //Add Customer
        //public async Task<Models.DTOs.Customer> Add(Models.DTOs.Customer customer)
        //{
        //    Customer req = _mapper.Map<Customer>(customer);
        //    return _mapper.Map<Models.DTOs.Customer>(
        //        await _customerManager.Add(req));
        //}
        public async Task<Models.DTOs.LoginUserToken> AddLoginToken(DTOs.LoginUserToken userToken)
        {
            LoginUserToken req = _mapper.Map<LoginUserToken>(userToken);
            var data = await _customerManager.AddLoginToken(req);
            return _mapper.Map<Models.DTOs.LoginUserToken>(data);
        }

        //TODO: Check if updating customer DTO mapping to Customer, to add Identity user breaks update and delete.
        //Update Customer
        public async Task<Models.DTOs.Customer> Update(Models.DTOs.Customer customer)
        {
            Customer req = _mapper.Map<Customer>(customer);
            return _mapper.Map<Models.DTOs.Customer>(
                await _customerManager.Update(req));
        }

        public async Task<Models.DTOs.Customer> DeleteAsync(Models.DTOs.Customer customer)
        {
            Customer req = _mapper.Map<Customer>(customer);
            return _mapper.Map<Models.DTOs.Customer>(
                await _customerManager.DeleteAsync(req));
        }

        #endregion

        #region Reads
        //Get LoginUserToken with AspNetUserId
        public Models.DTOs.LoginUserToken GetUserTokenByUserId(string AspNetUserId)
        {
            return _mapper.Map<Models.DTOs.LoginUserToken>(_customerManager.GetUserTokenByUserId(AspNetUserId));
        }
        //Get LoginUserToken with tokenvalue
        public Models.DTOs.LoginUserToken GetUserTokenByTokenValue(Guid tokenvalue)
        {
            return _mapper.Map<Models.DTOs.LoginUserToken>(_customerManager.GetUserTokenByTokenValue(tokenvalue));
        }
        //Get LoginUserToken with token
        public Models.DTOs.LoginUserToken GetUserTokenByToken(string token)
        {
            return _mapper.Map<Models.DTOs.LoginUserToken>(_customerManager.GetUserTokenByToken(token));
        }
        //Get Customer with AspNetUserId
        public async Task<Models.DTOs.Customer> Get(string aspNetUserId)
        {
            return Map(await _customerManager.GetAsync(aspNetUserId));
        }

        //Get Customer Personal Details, bank details and membership cards
        public Models.DTOs.Customer GetDetails(int id)
        {
            return MapToCustomerDto(_customerManager.GetDetails(id));
        }
        //Get Customer Personal Details, bank details and membership cards
        public Models.DTOs.Customer GetCustomerByUserName(string userName)
        {
            return _mapper.Map<Models.DTOs.Customer>(_customerManager.GetCustomerByUserName(userName));
        }
        #endregion

        #region Private Methods

        private Models.DTOs.Customer MapToCustomerDto(Customer cust)
        {
            if (cust == null)
                return null;
            Models.DTOs.Customer customer = new Models.DTOs.Customer
            {
                Id = cust.Id,
                AspNetUserId = cust.AspNetUserId,
                ContactDetailId = cust.ContactDetailId,
                Title = cust.Title,
                Forename = cust.Forename,
                Surname = cust.Surname,
                DateOfBirth = cust.DateOfBirth,
                IsActive = cust.IsActive,
                IsDeleted = cust.IsDeleted,
                DateAdded = cust.DateAdded,
                MarketingNewsLetter = cust.MarketingNewsLetter,
                MarketingThirdParty = cust.MarketingThirdParty,
                NINumber = cust.NINumber
            };

            if (cust.ContactDetail != null)
            {
                customer.ContactDetail = new Models.DTOs.ContactDetail
                {
                    Id = cust.ContactDetail.Id,
                    Address1 = cust.ContactDetail.Address1,
                    Address2 = cust.ContactDetail.Address2,
                    Address3 = cust.ContactDetail.Address3,
                    Town = cust.ContactDetail.Town,
                    District = cust.ContactDetail.District,
                    PostCode = cust.ContactDetail.PostCode,
                    CountryCode = cust.ContactDetail.CountryCode,
                    Latitude = cust.ContactDetail.Latitude,
                    Longitude = cust.ContactDetail.Longitude,
                    LandlinePhone = cust.ContactDetail.LandlinePhone,
                    MobilePhone = cust.ContactDetail.MobilePhone,
                    EmailAddress = cust.ContactDetail.EmailAddress,
                    IsDeleted = cust.ContactDetail.IsDeleted
                };
            }

            if (cust.CustomerBankDetails != null && cust.CustomerBankDetails.Count > 0)
            {
                customer.CustomerBankDetails = new List<Models.DTOs.CustomerBankDetail>();
                foreach (var detail in cust.CustomerBankDetails)
                {
                    customer.CustomerBankDetails.Add(MapBankDetailDto(detail));
                }
            }

            if (cust.MembershipCards != null && cust.MembershipCards.Count > 0)
            {
                customer.MembershipCards = new List<Models.DTOs.MembershipCard>();
                foreach (var card in cust.MembershipCards)
                {
                    customer.MembershipCards.Add(MapToCardDto(card));
                }
            }

            return customer;
        }

        private Models.DTOs.CustomerBankDetail MapBankDetailDto(CustomerBankDetail bank)
        {
            if (bank == null)
                return null;

            Models.DTOs.CustomerBankDetail detail = new Models.DTOs.CustomerBankDetail
            {
                CustomerId = bank.CustomerId,
                BankDetailsId = bank.BankDetailsId,
                MandateAccepted = bank.MandateAccepted,
                DateMandateAccepted = bank.DateMandateAccepted,
                IsActive = bank.IsActive,
                IsDeleted = bank.IsDeleted
            };

            if (detail.BankDetail != null)
            {
                detail.BankDetail = new Models.DTOs.BankDetail
                {
                    Id = bank.BankDetail.Id,
                    BankName = bank.BankDetail.BankName,
                    ContactDetailId = bank.BankDetail.ContactDetailId,
                    SortCode = bank.BankDetail.SortCode,
                    AccountName = bank.BankDetail.AccountName,
                    AccountNumber = bank.BankDetail.AccountNumber,
                    IsDeleted = bank.BankDetail.IsDeleted
                };
            }

            return detail;
        }

        private Models.DTOs.MembershipCard MapToCardDto(MembershipCard card)
        {
            if (card == null)
                return null;
            Models.DTOs.MembershipCard member = new Models.DTOs.MembershipCard
            {
                Id = card.Id,
                CustomerId = card.CustomerId,
                MembershipPlanId = card.MembershipPlanId,
                CardNumber = card.CardNumber,
                ValidFrom = card.ValidFrom,
                ValidTo = card.ValidTo,
                DateIssued = card.DateIssued,
                StatusId = card.StatusId,
                PhysicalCardRequested = card.PhysicalCardRequested,
                CustomerPaymentProviderId = card.CustomerPaymentProviderId,
                IsActive = card.IsActive,
                IsDeleted = card.IsDeleted,
                PhysicalCardStatusId = card.PhysicalCardStatusId,
                RegistrationCode = card.RegistrationCode,
                PartnerRewardId = card.PartnerRewardId,
                TermsConditionsId = card.TermsConditionsId
            };

            return member;
        }
        
        private Models.DTOs.Customer Map(Customer customer)
        {
            if (customer == null)
                return null;

            Models.DTOs.Customer cust = new Models.DTOs.Customer
            {
                Id = customer.Id,
                AspNetUserId = customer.AspNetUserId,
                ContactDetailId = customer.ContactDetailId,
                Title = customer.Title,
                Forename = customer.Forename,
                Surname = customer.Surname,
                DateOfBirth = customer.DateOfBirth,
                IsActive = customer.IsActive,
                IsDeleted = customer.IsDeleted,
                DateAdded = customer.DateAdded,
                MarketingNewsLetter = customer.MarketingNewsLetter,
                MarketingThirdParty = customer.MarketingThirdParty,
                NINumber = customer.NINumber,
                ContactDetail = new Models.DTOs.ContactDetail
                {
                    Id = customer.ContactDetail.Id,
                    Address1 = customer.ContactDetail.Address1,
                    Address2 = customer.ContactDetail.Address2,
                    Address3 = customer.ContactDetail.Address3,
                    Town = customer.ContactDetail.Town,
                    District = customer.ContactDetail.District,
                    PostCode = customer.ContactDetail.PostCode,
                    CountryCode = customer.ContactDetail.CountryCode,
                    Latitude = customer.ContactDetail.Latitude,
                    Longitude = customer.ContactDetail.Longitude,
                    LandlinePhone = customer.ContactDetail.LandlinePhone,
                    MobilePhone = customer.ContactDetail.MobilePhone,
                    EmailAddress = customer.ContactDetail.EmailAddress,
                    IsDeleted = customer.ContactDetail.IsDeleted
                }
            };

            if (customer.CustomerBankDetails != null && customer.CustomerBankDetails.Count > 0)
            {
                cust.CustomerBankDetails = new List<Models.DTOs.CustomerBankDetail>();
                foreach (CustomerBankDetail bank in customer.CustomerBankDetails)
                {
                    Models.DTOs.CustomerBankDetail bankDetail = new Models.DTOs.CustomerBankDetail
                    {
                        CustomerId = bank.CustomerId,
                        BankDetailsId = bank.BankDetailsId,
                        MandateAccepted = bank.MandateAccepted,
                        DateMandateAccepted = bank.DateMandateAccepted,
                        IsActive = bank.IsActive,
                        IsDeleted = bank.IsDeleted,
                        
                    };
                    if (bank.BankDetail != null)
                    {
                        bankDetail.BankDetail = new Models.DTOs.BankDetail
                        {
                            Id = bank.BankDetail.Id,
                            BankName = bank.BankDetail.BankName,
                            ContactDetailId = bank.BankDetail.ContactDetailId,
                            SortCode = bank.BankDetail.SortCode,
                            AccountNumber = bank.BankDetail.AccountNumber,
                            AccountName = bank.BankDetail.AccountName,
                            IsDeleted = bank.BankDetail.IsDeleted
                        };
                    }

                    cust.CustomerBankDetails.Add(bankDetail);
                }
            }
            if (customer.MembershipCards != null && customer.MembershipCards.Count > 0)
            {
                foreach (var card in customer.MembershipCards)
                {
                    cust.MembershipCards = new List<Models.DTOs.MembershipCard>
                    {
                        MapCard(card)
                    };
                }
            }
            return cust;
        }

        private Models.DTOs.MembershipCard MapCard(MembershipCard card)
        {
            if (card == null)
                return null;
            return new Models.DTOs.MembershipCard
            {
                Id = card.Id,
                CustomerId = card.CustomerId,
                MembershipPlanId = card.MembershipPlanId,
                CardNumber = card.CardNumber,
                ValidFrom = card.ValidFrom,
                ValidTo = card.ValidTo,
                DateIssued = card.DateIssued,
                StatusId = card.StatusId,
                PhysicalCardRequested = card.PhysicalCardRequested,
                CustomerPaymentProviderId = card.CustomerPaymentProviderId,
                IsActive = card.IsActive,
                IsDeleted = card.IsDeleted,
                PhysicalCardStatusId = card.PhysicalCardStatusId,
                RegistrationCode = card.RegistrationCode,
                PartnerRewardId = card.PartnerRewardId,
                TermsConditionsId = card.TermsConditionsId
            };
        }

        #endregion
    }
}
