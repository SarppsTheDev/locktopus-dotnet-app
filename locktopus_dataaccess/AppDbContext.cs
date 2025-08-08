using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using locktopus_domain.Entities;
using Microsoft.Extensions.Configuration;

namespace locktopus_dataaccess;

public class AppDbContext(IConfiguration config) : IdentityDbContext<User>
{
    DbSet<User> Users { get; set; }
    DbSet<LoginItem> LoginItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(config["SQLServerDB:ConnectionString"]);
    }
}