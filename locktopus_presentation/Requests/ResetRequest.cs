using System.ComponentModel.DataAnnotations;

namespace locktopus_presentation.Requests;

public record ResetRequest(
    [Required, EmailAddress] string Email,
    [Required] string Token,
    [Required, MinLength(6)] string NewPassword);