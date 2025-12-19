using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AmigoSecreto.Models;
using AmigoSecreto.Services;
using System.Text.Json;

namespace AmigoSecreto.Pages;

/// <summary>
/// Main page for SMS sending functionality.
/// Note: [IgnoreAntiforgeryToken] is used for simplicity in this demo.
/// For production use with authentication, implement proper CSRF protection.
/// </summary>
[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    private readonly ComteleSmsService _smsService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ComteleSmsService smsService, ILogger<IndexModel> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    /// <summary>
    /// Validates recipients data
    /// </summary>
    public IActionResult OnPostValidate([FromBody] ValidateRequest request)
    {
        try
        {
            var recipients = request.Recipients;

            if (recipients == null || !recipients.Any())
            {
                return new JsonResult(new { success = false, message = "Nenhum contato para validar" });
            }

            // Validate each recipient
            foreach (var recipient in recipients)
            {
                recipient.Validate();
            }

            var invalidCount = recipients.Count(r => !r.IsValid);
            var validCount = recipients.Count(r => r.IsValid);

            _logger.LogInformation("Validation complete: {ValidCount} valid, {InvalidCount} invalid", validCount, invalidCount);

            return new JsonResult(new
            {
                success = invalidCount == 0,
                message = invalidCount == 0
                    ? $"Todos os {validCount} contatos são válidos!"
                    : $"{validCount} válidos, {invalidCount} com erros",
                recipients = recipients
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating recipients");
            return new JsonResult(new { success = false, message = $"Erro ao validar: {ex.Message}" });
        }
    }

    /// <summary>
    /// Generates preview of messages
    /// </summary>
    public IActionResult OnPostGeneratePreview([FromBody] PreviewRequest request)
    {
        try
        {
            var recipients = request.Recipients;

            if (recipients == null || !recipients.Any())
            {
                return new JsonResult(new { success = false, message = "Nenhum contato para preview" });
            }

            if (string.IsNullOrWhiteSpace(request.MessageTemplate))
            {
                return new JsonResult(new { success = false, message = "Mensagem não pode estar vazia" });
            }

            var previews = new List<SmsPreview>();

            foreach (var recipient in recipients)
            {
                var personalizedMessage = request.MessageTemplate
                    .Replace("{NOME}", recipient.Nome)
                    .Replace("{PRESENTE}", recipient.Presente);

                previews.Add(new SmsPreview
                {
                    Id = recipient.Id,
                    Nome = recipient.Nome,
                    Celular = recipient.Celular,
                    MensagemFinal = personalizedMessage,
                    CharacterCount = personalizedMessage.Length,
                    WillBeIgnored = recipient.IgnorarAmigo
                });
            }

            _logger.LogInformation("Generated {Count} previews", previews.Count);
            return new JsonResult(new { success = true, previews = previews });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating preview");
            return new JsonResult(new { success = false, message = $"Erro ao gerar preview: {ex.Message}" });
        }
    }

    /// <summary>
    /// Sends SMS to all recipients
    /// </summary>
    public async Task<IActionResult> OnPostSendSms([FromBody] SendSmsRequest request)
    {
        try
        {
            var recipients = request.Recipients;

            if (recipients == null || !recipients.Any())
            {
                return new JsonResult(new { success = false, message = "Nenhum contato para enviar" });
            }

            if (string.IsNullOrWhiteSpace(request.ApiKey))
            {
                return new JsonResult(new { success = false, message = "API Key não fornecida" });
            }

            if (string.IsNullOrWhiteSpace(request.MessageTemplate))
            {
                return new JsonResult(new { success = false, message = "Mensagem não pode estar vazia" });
            }

            // Send SMS
            var results = await _smsService.SendBulkSmsAsync(
                request.ApiKey,
                recipients,
                request.MessageTemplate
            );

            var successCount = results.Count(r => r.Success);
            var errorCount = results.Count(r => !r.Success);
            var ignoredCount = recipients.Count(r => r.IgnorarAmigo);

            return new JsonResult(new
            {
                success = true,
                results = results,
                summary = new
                {
                    total = recipients.Count,
                    sent = successCount,
                    errors = errorCount,
                    ignored = ignoredCount
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS");
            return new JsonResult(new { success = false, message = $"Erro ao enviar SMS: {ex.Message}" });
        }
    }
}

// Request models for API endpoints
public class ValidateRequest
{
    public List<SmsRecipient> Recipients { get; set; } = new();
}

public class PreviewRequest
{
    public List<SmsRecipient> Recipients { get; set; } = new();
    public string MessageTemplate { get; set; } = string.Empty;
}

public class SendSmsRequest
{
    public string ApiKey { get; set; } = string.Empty;
    public List<SmsRecipient> Recipients { get; set; } = new();
    public string MessageTemplate { get; set; } = string.Empty;
}
