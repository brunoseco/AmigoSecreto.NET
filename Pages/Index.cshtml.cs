using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AmigoSecreto.Models;
using AmigoSecreto.Services;

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
    /// Performs the secret santa draw ensuring no one draws themselves
    /// </summary>
    private List<DrawResult> PerformDraw(List<SmsRecipient> participants)
    {
        var activeParticipants = participants.Where(p => !p.IgnorarAmigo).ToList();

        if (activeParticipants.Count < 2)
        {
            throw new InvalidOperationException("É necessário pelo menos 2 participantes ativos para o sorteio");
        }

        var givers = activeParticipants.ToList();
        var receivers = activeParticipants.ToList();
        var results = new List<DrawResult>();
        var random = new Random();
        var maxAttempts = 100;
        var attempt = 0;

        // Try to create a valid draw where no one draws themselves
        while (attempt < maxAttempts)
        {
            results.Clear();
            var availableReceivers = new List<SmsRecipient>(receivers);
            var isValidDraw = true;

            foreach (var giver in givers)
            {
                // Remove giver from available receivers to avoid self-draw
                var possibleReceivers = availableReceivers.Where(r => r.Id != giver.Id).ToList();

                if (possibleReceivers.Count == 0)
                {
                    isValidDraw = false;
                    break;
                }

                // Pick random receiver
                var receiverIndex = random.Next(possibleReceivers.Count);
                var receiver = possibleReceivers[receiverIndex];

                results.Add(new DrawResult
                {
                    Giver = giver,
                    Receiver = receiver
                });

                availableReceivers.Remove(receiver);
            }

            if (isValidDraw)
            {
                _logger.LogInformation("Secret santa draw completed successfully on attempt {Attempt}", attempt + 1);
                return results;
            }

            attempt++;
        }

        throw new InvalidOperationException("Não foi possível realizar o sorteio após várias tentativas");
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
    /// Generates preview of messages with secret santa draw
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

            // Perform secret santa draw
            List<DrawResult> drawResults;
            try
            {
                drawResults = PerformDraw(recipients);
            }
            catch (InvalidOperationException ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }

            var previews = new List<SmsPreview>();

            foreach (var draw in drawResults)
            {
                var personalizedMessage = request.MessageTemplate
                    .Replace("{NOME}", draw.Giver.Nome)
                    .Replace("{AMIGO}", draw.Receiver.Nome)
                    .Replace("{PRESENTE}", draw.Receiver.Presente);

                previews.Add(new SmsPreview
                {
                    Id = draw.Giver.Id,
                    Nome = draw.Giver.Nome,
                    Celular = draw.Giver.Celular,
                    MensagemFinal = personalizedMessage,
                    CharacterCount = personalizedMessage.Length,
                    WillBeIgnored = false
                });
            }

            // Add ignored participants as previews
            foreach (var ignored in recipients.Where(r => r.IgnorarAmigo))
            {
                previews.Add(new SmsPreview
                {
                    Id = ignored.Id,
                    Nome = ignored.Nome,
                    Celular = ignored.Celular,
                    MensagemFinal = "Participante ignorado - não receberá SMS",
                    CharacterCount = 0,
                    WillBeIgnored = true
                });
            }

            _logger.LogInformation("Generated {Count} previews with secret santa draw", previews.Count);
            return new JsonResult(new { success = true, previews = previews });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating preview");
            return new JsonResult(new { success = false, message = $"Erro ao gerar preview: {ex.Message}" });
        }
    }

    /// <summary>
    /// Sends SMS to all recipients with secret santa assignments
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

            // Perform secret santa draw
            List<DrawResult> drawResults;
            try
            {
                drawResults = PerformDraw(recipients);
            }
            catch (InvalidOperationException ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }

            var results = new List<SmsSendResult>();

            // Send SMS to each participant with their secret santa assignment
            foreach (var draw in drawResults)
            {
                var personalizedMessage = request.MessageTemplate
                    .Replace("{NOME}", draw.Giver.Nome)
                    .Replace("{AMIGO}", draw.Receiver.Nome)
                    .Replace("{PRESENTE}", draw.Receiver.Presente);

                var result = await _smsService.SendSmsAsync(
                    request.ApiKey,
                    draw.Giver.Celular,
                    personalizedMessage
                );

                result.RecipientId = draw.Giver.Id;
                result.RecipientName = draw.Giver.Nome;

                results.Add(result);

                // Add delay to avoid rate limiting
                var delayMs = 500; // Could be moved to configuration
                await Task.Delay(delayMs);
            }

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
