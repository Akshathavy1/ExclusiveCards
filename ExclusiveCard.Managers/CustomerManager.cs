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
    /// <summary>
    /// CustomerManager is in charge of the Customer data.
    /// This includes creating and maintaining data specific to Customers.
    /// Will include Customer table and the Customer Security questions, along with contact details and bank details.
    /// Note - even though contact & bank details are actually shared by other entities (partner / merchant),  
    /// they are shared at repository level.  All interactions for customers are through the customer manager. 
    /// Partners and merchants will use their own managers to handle their contact and bank details. 
    /// Login and account validation are not customer functions but are managed by a separate userManager.
    /// </summary>
    public class CustomerManager : ICustomerManager
    {
        private IMapper _mapper = null;
        private IRepository<db.Customer> _customerRepo = null;
        private readonly IMarketingManager _marketingManager;


        public CustomerManager(IMapper mapper, IRepository<db.Customer> customerRepo, IMarketingManager marketingManager)
        {
            _mapper = mapper;
            _customerRepo = customerRepo;
            _marketingManager = marketingManager;
        }
        
        public Customer Create(Customer customer)
        {
            var dbCustomer = _mapper.Map<db.Customer>(customer);

            dbCustomer.DateAdded = DateTime.UtcNow;             // Set the date added property to current timestamp
            dbCustomer.IsActive = true;  // Create customer as active. Seems reasonable assumption. 
            _customerRepo.Create(dbCustomer);
            _customerRepo.SaveChanges();

            customer = _mapper.Map<Customer>(dbCustomer);
            
            // NOTE - if customer not returned as a return value from method, the identityUser property never gets set in the client's customer object. Weird.
            return customer;
        }

        /// <summary>
        /// Finds a customer  from the associated customer Id
        /// </summary>
        /// <param name="customerId">Id value to match Customer.Id in Db</param>
        /// <returns>Customer DtO  or null if not found</returns>
        public Customer Get(int customerId)
        {
            Customer customer = null;

            var dbCustomer = _customerRepo.GetById(customerId);
            if (dbCustomer != null)
                customer = _mapper.Map<Customer>(dbCustomer);

            return customer;
        }

        /// <summary>
        /// Finds a customer from the associated ASPNetUserId
        /// </summary>
        /// <param name="aspNetUserId">The aspnetUserId, as defined in Microsoft Indentity aspNetUsers table</param>
        /// <returns>Customer DtO  or null if not found</returns>
        public Customer Get (string aspNetUserId)
        {
            Customer customer = null;

            var dbCustomer = _customerRepo.Include(c => c.ContactDetail).FirstOrDefault(x => x.AspNetUserId == aspNetUserId);
            if (dbCustomer != null)
                customer = _mapper.Map<Customer>(dbCustomer);

            return customer;

        }

        /// <summary>
        /// Finds a customer Id from the associated ASPNetUserId
        /// </summary>
        /// <param name="aspNetUserId"></param>
        /// <returns></returns>
        public int? FindCustomerId(string aspNetUserId)
        {
            var customer =_customerRepo.Get(x => x.AspNetUserId == aspNetUserId);
            return customer?.Id;
        }

        public Customer UpdateCustomerSettings(Customer customer)
        {
            var dbCustomer = _customerRepo.Include(x => x.ContactDetail).Where(x => x.Id == customer.Id).FirstOrDefault();

            // update contact details first
            var dbContact = dbCustomer.ContactDetail;
            var contact = customer.ContactDetail;

            // If it doesn't exist, create a record
            if (dbContact == null && contact != null)
            {
                var newDbContact = _mapper.Map<db.ContactDetail>(contact);
                dbCustomer.ContactDetail = newDbContact;
            }
            else if (contact != null)
            {
                if( !string.IsNullOrWhiteSpace(dbContact.EmailAddress) && 
                    !string.IsNullOrWhiteSpace(contact.EmailAddress) && 
                    dbContact.EmailAddress.ToLower() != contact.EmailAddress.ToLower())
                {
                    //try to remove the old email address from our marketing provider
                  _marketingManager.RemoveMarketingContact(dbContact.EmailAddress).Wait();
                }

                dbContact.EmailAddress = contact.EmailAddress;
                dbContact.Address1 = contact.Address1;
                dbContact.Address2 = contact.Address2;
                dbContact.Address3 = contact.Address3;
                dbContact.Town = contact.Town;
                dbContact.District = contact.District;
                dbContact.PostCode = contact.PostCode;
            }

            // Now the customer
            // Won't use automapper as there are too many child properties and mapping get scary. 
            // We will just update a few fields here
            //dbCustomer.Title = customer.Title;
            dbCustomer.Forename = customer.Forename;
            dbCustomer.Surname = customer.Surname;
            dbCustomer.DateOfBirth = customer.DateOfBirth;
            dbCustomer.MarketingNewsLetter = customer.MarketingNewsLetter;
            dbCustomer.MarketingThirdParty = customer.MarketingThirdParty;
            dbCustomer.NINumber = customer.NINumber;

            _customerRepo.Update(dbCustomer);
            _customerRepo.SaveChanges();

            var updatedCustomer = _mapper.Map<Customer>(dbCustomer);
            return updatedCustomer;
        }
    }
}
