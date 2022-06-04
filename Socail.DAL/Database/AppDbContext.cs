using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Socail.DAL.Entity;
using Socail.DAL.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.DAL.Database
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)      {       }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>(user =>
            {
                user.Property(x=>x.FullName)
                .HasComputedColumnSql("[FirstName] + ' ' + [LastName]");
            });
            base.OnModelCreating(builder);
        }

        public DbSet<Photo> Photos { get; set; }
    }
}
