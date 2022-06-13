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


            builder.Entity<Like>()
                .HasKey(a => new { a.LikerId,a.LikeeId});

            builder.Entity<Like>()
                .HasOne(l=>l.Likee)
                .WithMany(u=>u.Likers)
                .HasForeignKey(l=>l.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(l => l.Liker)
                .WithMany(u => u.Likees)
                .HasForeignKey(l => l.LikerId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(builder);


        }

        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
    }
}
