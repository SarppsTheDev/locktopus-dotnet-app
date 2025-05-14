namespace passwordvault_presentation.Requests;

public record UserUpdatePersonalInfoRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}