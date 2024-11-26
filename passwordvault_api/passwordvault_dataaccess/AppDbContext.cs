using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using passwordvault_domain.Entities;

namespace passwordvault_dataaccess;

public class AppDbContext : IdentityDbContext<User>
{
    DbSet<User> Users { get; set; }
    DbSet<LoginItem> LoginItems { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=localhost,1433;Database=PasswordVaultDB;User Id=sa;Password=Arsenal1996;TrustServerCertificate=true;");
    }
}