using ContactApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactApi.Data
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options)
        {
            
        }

        public DbSet<Contact> Contacts { get; set; }
    }
}
