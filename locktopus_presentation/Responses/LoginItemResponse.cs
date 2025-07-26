namespace locktopus_presentation.Responses;

public record LoginItemResponse(
    long Id,
    string Title,
    string WebsiteUrl,
    string Username,
    string Password,
    string? Notes,
    DateTime CreatedAt,
    DateTime LastUpdatedAt);