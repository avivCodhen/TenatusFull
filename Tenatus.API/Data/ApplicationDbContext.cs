using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders;

namespace Tenatus.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserOrder> UserOrders { get; set; }
        public DbSet<StockData> StocksData { get; set; }
        public DbSet<RangeStrategy> RangeStrategies { get; set; }
        public DbSet<PercentStrategy> PercentStrategies { get; set; }
        public DbSet<Strategy> Strategies { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasMany(x => x.UserOrders).WithOne(x => x.ApplicationUser);
            builder.Entity<ApplicationUser>().HasMany(x => x.Strategies).WithOne(x => x.User);

            builder.Entity<UserOrder>()
                .Property(o => o.BuyingPrice)
                .HasColumnType("decimal(18,6)");
            builder.Entity<ApplicationUser>()
                .Property(o => o.MinimumFee)
                .HasColumnType("decimal(18,6)");
            builder.Entity<StockData>()
                .Property(o => o.CurrentPrice)
                .HasColumnType("decimal(18,6)");
            builder.Entity<StockData>()
                .Property(o => o.Open)
                .HasColumnType("decimal(18,6)");
            builder.Entity<StockData>()
                .Property(o => o.Close)
                .HasColumnType("decimal(18,6)");
            builder.Entity<StockData>()
                .Property(o => o.High)
                .HasColumnType("decimal(18,6)");
            builder.Entity<StockData>()
                .Property(o => o.Low)
                .HasColumnType("decimal(18,6)");
            builder.Entity<PercentStrategy>()
                .Property(o => o.Percent)
                .HasColumnType("decimal(18,6)");
            builder.Entity<RangeStrategy>()
                .Property(o => o.Minimum)
                .HasColumnType("decimal(18,6)");
            builder.Entity<RangeStrategy>()
                .Property(o => o.Maximum)
                .HasColumnType("decimal(18,6)");
            builder.Entity<Strategy>()
                .Property(o => o.Budget)
                .HasColumnType("decimal(18,6)");

            base.OnModelCreating(builder);
        }
    }
}