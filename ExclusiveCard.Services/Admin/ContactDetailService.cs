using System.Threading.Tasks;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
    public class ContactDetailService : IContactDetailService
    {
        #region Private members

        private readonly IContactsManager _contactsManager;

        #endregion

        #region Constuctor

        public ContactDetailService(IContactsManager contactsManager)
        {
            _contactsManager = contactsManager;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.ContactDetail> Add(Models.DTOs.ContactDetail contactDetail)
        {
            ContactDetail req = MapContactDetailReq(contactDetail);
            return MapContactDetail(
                await _contactsManager.Add(req));
        }

        public async Task<Models.DTOs.ContactDetail> Update(Models.DTOs.ContactDetail contactDetail)
        {
            ContactDetail req = MapContactDetailReq(contactDetail);
            return MapContactDetail(
                await _contactsManager.Update(req));
        }

        #endregion

        #region Reads

        public async Task<Models.DTOs.ContactDetail> Get(int id)
        {
            return MapContactDetail(await _contactsManager.Get(id));
        }

        #endregion

        #region Private Methods

        private Models.DTOs.ContactDetail MapContactDetail(ContactDetail con)
        {
            if (con == null)
                return null;
            Models.DTOs.ContactDetail dto = new Models.DTOs.ContactDetail
            {
                Id = con.Id,
                Address1 = con.Address1,
                Address2 = con.Address2,
                Address3 = con.Address3,
                Town = con.Town,
                District = con.District,
                PostCode = con.PostCode,
                CountryCode = con.CountryCode,
                Latitude = con.Latitude,
                Longitude = con.Longitude,
                LandlinePhone = con.LandlinePhone,
                MobilePhone = con.MobilePhone,
                EmailAddress = con.EmailAddress,
                IsDeleted = con.IsDeleted
            };
            return dto;
        }

        private ContactDetail MapContactDetailReq(Models.DTOs.ContactDetail con)
        {
            if (con == null)
                return null;
            ContactDetail dto = new ContactDetail
            {
                Id = con.Id,
                Address1 = con.Address1,
                Address2 = con.Address2,
                Address3 = con.Address3,
                Town = con.Town,
                District = con.District,
                PostCode = con.PostCode,
                CountryCode = con.CountryCode,
                Latitude = con.Latitude,
                Longitude = con.Longitude,
                LandlinePhone = con.LandlinePhone,
                MobilePhone = con.MobilePhone,
                EmailAddress = con.EmailAddress,
                IsDeleted = con.IsDeleted
            };
            return dto;
        }

        #endregion
    }
}
