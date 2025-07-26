using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using locktopus_domain.Entities;

namespace locktopus_dataaccess;

public class AppDbContext : IdentityDbContext<User>
{
    DbSet<User> Users { get; set; }
    DbSet<LoginItem> LoginItems { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=localhost,1433;Database=LocktopusDB;User Id=sa;Password=Ars3nal1996?!;TrustServerCertificate=true;");
    }
}