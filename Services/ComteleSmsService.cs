using System.Text;
using System.Text.Json;
using AmigoSecreto.Models;

namespace AmigoSecreto.Services;

/// <summary>
/// Service for sending SMS via Comtele API
/// Documentation: https://docs.comtele.com.br/
/// </summary>
public class ComteleSmsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ComteleSmsService> _logger;

    public ComteleSmsService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ComteleSmsService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Sends a single SMS via Comtele API
    /// </summary>
    /// <param name="apiKey">Comtele API Key</param>
    /// <param name="phoneNumber">Recipient phone number</param>
    /// <param name="message">Message content</param>
    /// <returns>Result of the SMS send operation</returns>
    public async Task<SmsSendResult> SendSmsAsync(string apiKey, string phoneNumber, string message)
    {
        var result = new SmsSendResult
        {
            PhoneNumber = phoneNumber,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                result.Success = false;
                result.Status = "Erro";
                result.ErrorMessage = "API Key não fornecida";
                _logger.LogWarning("API Key not provided for SMS to {PhoneNumber}", phoneNumber);
                return result;
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                result.Success = false;
                result.Status = "Erro";
                result.ErrorMessage = "Número de telefone inválido";
                _logger.LogWarning("Invalid phone number provided");
                return result;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                result.Success = false;
                result.Status = "Erro";
                result.ErrorMessage = "Mensagem vazia";
                _logger.LogWarning("Empty message for SMS to {PhoneNumber}", phoneNumber);
                return result;
            }

            // Clean phone number (remove non-digit characters)
            var cleanPhone = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Prepare API request based on Comtele documentation
            var apiUrl = _configuration["Comtele:ApiUrl"] ?? "https://api.comtele.com.br/v1/sms";

            var requestBody = new
            {
                Receiver = cleanPhone,
                Message = message,
                // Add other required fields based on Comtele API docs
                MessageType = "text"
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending SMS to {PhoneNumber}", cleanPhone);

            // Create request message with per-request headers to avoid concurrency issues
            using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Content = content;
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Headers.Add("Accept", "application/json");

            // Send request
            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                result.Success = true;
                result.Status = "Enviado";
                _logger.LogInformation("SMS sent successfully to {PhoneNumber}", cleanPhone);
            }
            else
            {
                result.Success = false;
                result.Status = "Erro";
                result.ErrorMessage = $"API Error: {response.StatusCode} - {responseContent}";
                _logger.LogError("Failed to send SMS to {PhoneNumber}. Status: {StatusCode}, Response: {Response}",
                    cleanPhone, response.StatusCode, responseContent);
            }
        }
        catch (HttpRequestException ex)
        {
            result.Success = false;
            result.Status = "Erro";
            result.ErrorMessage = $"Network error: {ex.Message}";
            _logger.LogError(ex, "Network error while sending SMS to {PhoneNumber}", phoneNumber);
        }
        catch (TaskCanceledException ex)
        {
            result.Success = false;
            result.Status = "Erro";
            result.ErrorMessage = "Request timeout";
            _logger.LogError(ex, "Timeout while sending SMS to {PhoneNumber}", phoneNumber);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Status = "Erro";
            result.ErrorMessage = $"Unexpected error: {ex.Message}";
            _logger.LogError(ex, "Unexpected error while sending SMS to {PhoneNumber}", phoneNumber);
        }

        return result;
    }

    /// <summary>
    /// Sends SMS to multiple recipients
    /// </summary>
    /// <param name="apiKey">Comtele API Key</param>
    /// <param name="recipients">List of recipients with their personalized messages</param>
    /// <param name="messageTemplate">Message template with tags</param>
    /// <returns>List of results for each recipient</returns>
    public async Task<List<SmsSendResult>> SendBulkSmsAsync(
        string apiKey,
        List<SmsRecipient> recipients,
        string messageTemplate)
    {
        var results = new List<SmsSendResult>();

        // Get delay configuration from settings
        var delayMs = _configuration.GetValue<int>("Comtele:DelayBetweenRequestsMs", 500);

        foreach (var recipient in recipients)
        {
            // Replace tags in message
            var personalizedMessage = messageTemplate
                .Replace("{NOME}", recipient.Nome)
                .Replace("{PRESENTE}", recipient.Presente);

            var result = await SendSmsAsync(apiKey, recipient.Celular, personalizedMessage);
            result.RecipientId = recipient.Id;
            result.RecipientName = recipient.Nome;

            results.Add(result);

            // Add delay to avoid rate limiting (configurable via appsettings)
            await Task.Delay(delayMs);
        }

        return results;
    }
}
