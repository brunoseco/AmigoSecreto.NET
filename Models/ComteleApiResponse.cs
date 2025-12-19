namespace AmigoSecreto.Models;

/// <summary>
/// Response from Comtele SMS API
/// </summary>
public class ComteleApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? MessageId { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Result of a single SMS send operation
/// </summary>
public class SmsSendResult
{
    public string RecipientId { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
