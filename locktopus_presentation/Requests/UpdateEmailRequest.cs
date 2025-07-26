namespace passwordvault_presentation.Requests;

public record UpdateEmailRequest
{
    public string NewEmail { get; init; }
    public string CurrentEmail { get; init; }
    public string CurrentPassword { get; init; }
}