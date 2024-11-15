namespace passwordvault_domain.Entities;

public class LoginItem
{
    public int LoginItemId { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Notes { get; set; }
}