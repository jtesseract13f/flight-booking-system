using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BonusServiceApi.DAL;

public class BonusDbContext(DbContextOptions<BonusDbContext> options) : DbContext(options)
{
    DbSet<Privilege> Privileges { get; set; }
    DbSet<PrivilegeHistory>  PrivilegeHistories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<PrivilegeHistory>()
            .Property(t => t.OperationType)
            .HasConversion(new EnumToStringConverter<OperationType>());
    }
}

