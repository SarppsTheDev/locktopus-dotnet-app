namespace passwordvault_presentation.Responses;

public record UserProfileResponse(
    string Email,
    string FirstName,
    string LastName,
    string Username);