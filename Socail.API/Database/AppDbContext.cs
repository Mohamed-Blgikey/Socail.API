using Microsoft.EntityFrameworkCore;

namespace Socail.API.Database
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options){ }


    }
}
