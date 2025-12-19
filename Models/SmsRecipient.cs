namespace AmigoSecreto.Models;

/// <summary>
/// Represents a SMS recipient with contact information and gift assignment
/// </summary>
public class SmsRecipient
{
    /// <summary>
    /// Temporary unique identifier for UI control
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>
    /// Recipient's name - used for {NOME} tag replacement
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Recipient's mobile phone number (with country code)
    /// </summary>
    public string Celular { get; set; } = string.Empty;

    /// <summary>
    /// Gift assignment - used for {PRESENTE} tag replacement
    /// </summary>
    public string Presente { get; set; } = string.Empty;

    /// <summary>
    /// List of recipient IDs that this person cannot draw in the secret santa
    /// </summary>
    public List<string> Restrictions { get; set; } = new();

    /// <summary>
    /// Validation status after validation check
    /// </summary>
    public bool IsValid { get; set; } = true;

    /// <summary>
    /// Validation error message if any
    /// </summary>
    public string? ValidationMessage { get; set; }

    /// <summary>
    /// Validates the recipient data
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Nome))
        {
            IsValid = false;
            ValidationMessage = "Nome é obrigatório";
            return;
        }

        if (string.IsNullOrWhiteSpace(Celular))
        {
            IsValid = false;
            ValidationMessage = "Celular é obrigatório";
            return;
        }

        // Basic phone validation - should contain only digits and be between 10-15 chars
        var phoneDigits = new string(Celular.Where(char.IsDigit).ToArray());
        if (phoneDigits.Length < 10 || phoneDigits.Length > 15)
        {
            IsValid = false;
            ValidationMessage = "Celular inválido (10-15 dígitos necessários)";
            return;
        }

        if (string.IsNullOrWhiteSpace(Presente))
        {
            IsValid = false;
            ValidationMessage = "Presente é obrigatório";
            return;
        }

        IsValid = true;
        ValidationMessage = null;
    }
}
