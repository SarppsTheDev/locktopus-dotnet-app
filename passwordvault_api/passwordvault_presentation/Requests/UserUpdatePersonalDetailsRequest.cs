namespace passwordvault_presentation.Requests;

public record UserUpdatePersonalDetailsRequest
{
    public string Email { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}