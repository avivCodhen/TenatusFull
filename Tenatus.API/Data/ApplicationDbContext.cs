using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders;

namespace Tenatus.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TraderSetting> TraderSettings { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasOne<TraderSetting>(x => x.TraderSetting).WithOne(x => x.User)
                .HasForeignKey<TraderSetting>(x => x.UserId);
            base.OnModelCreating(builder);
        }
    }
}