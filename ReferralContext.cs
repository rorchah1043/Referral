using Microsoft.EntityFrameworkCore;
using Referral.Models;

namespace Referral
{
    public class ReferralContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ReferralData> ReferralData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=test.db");
        }
    }
}