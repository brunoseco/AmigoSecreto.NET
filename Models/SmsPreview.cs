namespace AmigoSecreto.Models;

/// <summary>
/// Represents a preview of the SMS message for a specific recipient
/// </summary>
public class SmsPreview
{
    /// <summary>
    /// Temporary recipient ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Recipient's name
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Recipient's mobile phone
    /// </summary>
    public string Celular { get; set; } = string.Empty;

    /// <summary>
    /// The final message with all tags replaced
    /// </summary>
    public string MensagemFinal { get; set; } = string.Empty;

    /// <summary>
    /// Number of characters in the message
    /// </summary>
    public int CharacterCount { get; set; }

    /// <summary>
    /// Number of SMS parts (based on 160 chars per SMS)
    /// </summary>
    public int SmsCount => CharacterCount > 0 ? (int)Math.Ceiling(CharacterCount / 160.0) : 0;

    /// <summary>
    /// Whether this recipient will be ignored
    /// </summary>
    public bool WillBeIgnored { get; set; }
}
