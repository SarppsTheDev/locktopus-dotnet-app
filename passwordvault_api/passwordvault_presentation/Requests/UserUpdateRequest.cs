namespace passwordvault_presentation.Requests;

public record UserUpdateRequest : UserRegistrationRequest
{
    public string OldPassword { get; set; }
}