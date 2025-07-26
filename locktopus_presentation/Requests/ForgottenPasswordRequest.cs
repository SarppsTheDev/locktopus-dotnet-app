using System.ComponentModel.DataAnnotations;

namespace locktopus_presentation.Requests;

public record ForgottenPasswordRequest([Required, EmailAddress] string Email);