namespace passwordvault_presentation.Requests;

public record UpdateProfileRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}