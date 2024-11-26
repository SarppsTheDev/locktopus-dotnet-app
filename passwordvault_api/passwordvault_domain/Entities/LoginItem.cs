using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace passwordvault_domain.Entities;

public class LoginItem
{
    /// <summary>
    /// ID for Login Item
    /// </summary>
    [Key]
    public int LoginItemId { get; set; }
    
    /// <summary>
    /// Title of the login item i.e. name of website it is for
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// URL of website associated with login item
    /// </summary>
    public string WebsiteUrl { get; set; }
    
    /// <summary>
    /// Username for website login
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// Password for website login, this is the raw value and will not be stored
    /// </summary>
    [NotMapped]
    public string Password { get; set; }
    
    /// <summary>
    /// Encrypted value of password, which will be stored
    /// </summary>
    public string EncryptedPassword { get; set; }
    
    /// <summary>
    /// Notes/description that is attached to log in item
    /// </summary>
    public string? Notes { get; set; }
    
    public string UserId { get; set; }
    public User User { get; set; }
}