using System;
using ExclusiveCard.Data.Context;
using ExclusiveCard.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ExclusiveCard.Data.Repositories;

namespace ExclusiveCard.Data.CRUDS
{
    public class TagManager : ITagManager
    {
        private readonly ExclusiveContext _ctx;


        public TagManager(ExclusiveContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Tag> Add(Tag tag)
        {
            DbSet<Tag> tags = _ctx.Set<Tag>();
            tags.Add(tag);
            await _ctx.SaveChangesAsync();

            return tag;
        }

        public async Task<Tag> Update(Tag tag)
        {
            DbSet<Tag> tags = _ctx.Set<Tag>();
            tags.Update(tag);
            await _ctx.SaveChangesAsync();

            return tag;
        }

        public async Task<List<Tag>> GetAll()
        {
            return await _ctx.Tag.Where(x => x.IsActive).ToListAsync();
        }
       
    }
}
