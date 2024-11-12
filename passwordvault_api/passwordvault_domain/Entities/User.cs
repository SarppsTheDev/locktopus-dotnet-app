using Microsoft.AspNetCore.Identity;

namespace passwordvault_domain.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}