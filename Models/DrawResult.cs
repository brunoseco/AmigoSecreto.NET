namespace AmigoSecreto.Models;

/// <summary>
/// Represents the result of a secret santa draw
/// </summary>
public class DrawResult
{
    /// <summary>
    /// The participant who will give the gift (receives the SMS)
    /// </summary>
    public SmsRecipient Giver { get; set; } = new();

    /// <summary>
    /// The participant who will receive the gift (the "friend" drawn)
    /// </summary>
    public SmsRecipient Receiver { get; set; } = new();
}
