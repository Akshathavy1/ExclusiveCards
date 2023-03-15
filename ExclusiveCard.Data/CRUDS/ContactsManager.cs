using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ExclusiveCard.Data.CRUDS
{
    public class ContactsManager : IContactsManager
    {
        private readonly ExclusiveContext _ctx;

        public ContactsManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ContactDetail> Add(ContactDetail contactDetail)
        {
            DbSet<ContactDetail> contactDetails = _ctx.Set<ContactDetail>();
            contactDetails.Add(contactDetail);
            await _ctx.SaveChangesAsync();
            return contactDetail;
        }

        public async Task<ContactDetail> Update(ContactDetail contactDetail)
        {
            DbSet<ContactDetail> contactDetails = _ctx.Set<ContactDetail>();
            contactDetails.Update(contactDetail);
            await _ctx.SaveChangesAsync();

            return contactDetail;
        }

        public async Task<ContactDetail> Get(int id)
        {
            return await _ctx.ContactDetail.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }
    }
}
