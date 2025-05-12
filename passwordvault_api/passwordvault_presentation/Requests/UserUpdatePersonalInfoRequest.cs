namespace passwordvault_presentation.Requests;

public record UserUpdatePersonalInfoRequest
{
    public string UserId { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}