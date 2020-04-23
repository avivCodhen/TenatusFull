using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders;

namespace Tenatus.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserOrder> UserOrders { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TraderSetting> TraderSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasOne(x => x.TraderSetting).WithOne(x => x.User)
                .HasForeignKey<TraderSetting>(x => x.UserId);

            builder.Entity<ApplicationUser>().HasMany(x => x.UserOrders).WithOne(x => x.ApplicationUser);

            builder.Entity<TraderSetting>().HasMany(x => x.Stocks).WithOne(x => x.TraderSetting)
                .HasForeignKey(x => x.TradeSettingId);

            builder.Entity<TraderSetting>()
                .Property(o => o.BuyingValue)
                .HasColumnType("decimal(18,6)");
            builder.Entity<UserOrder>()
                .Property(o => o.BuyingPrice)
                .HasColumnType("decimal(18,6)");
            builder.Entity<TraderSetting>()
                .Property(o => o.SellingValue)
                .HasColumnType("decimal(18,6)");

            base.OnModelCreating(builder);
        }
    }
}