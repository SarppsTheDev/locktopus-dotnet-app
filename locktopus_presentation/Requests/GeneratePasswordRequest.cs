namespace locktopus_presentation.Requests;

public class GeneratePasswordRequest
{
    /// <summary>
    /// Length of the password string
    /// </summary>
    public int PasswordLength { get; set; } = 20;
    
    /// <summary>
    /// Option to use letter characters in password string
    /// </summary>
    public bool UseLetters { get; set; } = true;
    
    /// <summary>
    /// Option to use mixed casing for letter characters
    /// </summary>
    public bool UseMixedCase { get; set; } = true;
    
    /// <summary>
    /// Option to use numbers in password string
    /// </summary>
    public bool UseNumbers { get; set; } = true;
    
    /// <summary>
    /// Option to use special characters in password string
    /// </summary>
    public bool UseSpecialCharacters { get; set; } = true;
}