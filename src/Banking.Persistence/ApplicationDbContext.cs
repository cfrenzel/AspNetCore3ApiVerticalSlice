using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Banking.Core.Entities;

namespace Banking.Persistence.EFCore
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<Institution> Institutions { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Account> Accounts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Institution>()
                .HasIndex(u => u.Name)
                .IsUnique();           
        }

        public override int SaveChanges()
        {
            _preSaveChanges();
            var res = base.SaveChanges();
            return res;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _preSaveChanges();
            var res = await base.SaveChangesAsync(cancellationToken);
            return res;
        }

        private void _preSaveChanges()
        {
            _addDateTimeStamps();
        }


        private void _addDateTimeStamps()
        {
            foreach (var item in ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                var now = DateTime.UtcNow;

                //if (item.State == EntityState.Added && item.Metadata.FindProperty("CreatedAt") != null)
                //{
                //    var prop = item.Property("CreatedAt");
                //    if(prop.CurrentValue == null && (DateTime)prop.CurrentValue == default(DateTime))
                //        prop.CurrentValue = now;
                //}
                //else 
                if (item.State == EntityState.Added && item.Metadata.FindProperty("CreatedAtUtc") != null)
                {
                    var prop = item.Property("CreatedAtUtc");
                    if (prop.CurrentValue == null && (DateTime)prop.CurrentValue == default(DateTime))
                        prop.CurrentValue = now;
                }

                //if (item.Metadata.FindProperty("UpdatedAt") != null)
                //    item.Property("UpdatedAt").CurrentValue = now;
                //else 
                if (item.Metadata.FindProperty("UpdatedAtUtc") != null)
                    item.Property("UpdatedAtUtc").CurrentValue = now;
            }
        }




    }
}
