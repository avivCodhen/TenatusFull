using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Tenatus.API.Data
{
    public class Context : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public Context(DbContextOptions options) : base(options)
        {
        }
    }
}