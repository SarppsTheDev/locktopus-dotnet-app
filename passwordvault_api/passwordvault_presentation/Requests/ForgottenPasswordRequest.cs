using System.ComponentModel.DataAnnotations;

namespace passwordvault_presentation.Requests;

public record ForgottenPasswordRequest([Required, EmailAddress] string Email);