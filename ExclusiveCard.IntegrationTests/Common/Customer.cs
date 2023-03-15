using ExclusiveCard.Enums;
using ExclusiveCard.Data.Models;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.IntegrationTests.Common
{
    public static class Customer
    {
        public static async Task<ExclusiveUser> CreateUser(ExclusiveUser iUser, Roles role)
        {
            //Create User
            string password = "Abcd@1234";

            //Add user
            bool result =
                await ServiceHelper.Instance.UserService.CreateAsync(iUser, password, role.ToString());

            //Get user back
            if (result)
                return await ServiceHelper.Instance.UserService.FindByEmailAsync(iUser.Email);
            return null;
        }

        //TODO: Refactor CreateCustomer in Integration tests to use new service
        public static async Task<dto.Customer> CreateCustomer(string iUserId, dto.Customer customer)
        {
            await Task.CompletedTask;

            if (string.IsNullOrEmpty(iUserId)) return null;

            customer.AspNetUserId = iUserId;
            //Add Customer
            return null; // await ServiceHelper.Instance.CustomerService.Add(customer);
        }

        public static async Task<dto.MembershipCard> CreatMembershipCard(int customerId, int planId, string paymentProviderId, dto.MembershipCard iCard)
        {
            if (iCard == null) return null;

            iCard.CustomerId = customerId;
            iCard.MembershipPlanId = planId;
            iCard.CustomerPaymentProviderId = paymentProviderId;

            return await ServiceHelper.Instance.PMembershipCardService.Add(iCard, null, null, null);
        }

        public static async Task DeleteUser(ExclusiveUser iUser)
        {
            await ServiceHelper.Instance.UserService.DeleteAsync(iUser);
        }

        public static async Task DeleteCustomer(int id, int? contactDetailId, dto.Customer iCustomer)
        {
            iCustomer.Id = id;
            iCustomer.ContactDetailId = contactDetailId;
            if (contactDetailId.HasValue)
            {
                iCustomer.ContactDetail.Id = contactDetailId.Value;
            }
            await ServiceHelper.Instance.CustomerService.DeleteAsync(iCustomer);
        }

        public static async Task DeleteMembershipCard(int id, dto.MembershipCard iCard)
        {
            iCard.Id = id;
            await ServiceHelper.Instance.MembershipCardService.DeleteAsync(iCard);
        }
    }
}
