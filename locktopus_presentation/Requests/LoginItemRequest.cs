using System.ComponentModel.DataAnnotations;

namespace locktopus_presentation.Requests;

//TODO: Refactor URL validation into domain service
public class LoginItemRequest
{
    /// <summary>
    /// Title of the login item i.e. name of website it is for
    /// </summary>
    [Required]
    public string Title { get; set; }

    /// <summary>
    /// URL of website associated with login item
    /// </summary>
    [Required]
    [Url(ErrorMessage = "The Website URL must be a valid URL.")]
    public string WebsiteUrl { get; set; }

    /// <summary>
    /// Username for website login
    /// </summary>
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// Password for website login
    /// </summary>
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// Notes/description that is attached to log in item
    /// </summary>
    public string Notes { get; set; }
}