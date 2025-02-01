namespace passwordvault_presentation.Responses;

public record LoginItemResponse(
    int Id,
    string Title,
    string WebsiteUrl,
    string Username,
    string Password,
    string Notes);