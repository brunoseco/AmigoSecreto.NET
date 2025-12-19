# AmigoSecreto.NET - AI Coding Agent Instructions

## Project Overview

**AmigoSecreto.NET** is a single-page ASP.NET Razor Pages application for sending personalized bulk SMS messages via the Comtele API. Primary use case: sending "Secret Santa" (Amigo Secreto) assignments via SMS with personalized message templates.

**Key Design Principle**: No data persistence - all sensitive contact information exists only in client-side memory and server request lifecycle.

## Architecture

### Technology Stack

- **Framework**: .NET 10.0 (net10.0) with Razor Pages
- **Frontend**: Vanilla JavaScript + Bootstrap 5 (heavily customized with dark glassmorphism theme)
- **API Integration**: HttpClient with Comtele SMS REST API
- **State Management**: Client-side only (no database, no session storage)

### Project Structure

```
Models/              - Data models (SmsRecipient, SmsPreview, SmsSendResult, ComteleApiResponse)
Services/            - ComteleSmsService (HTTP client wrapper for Comtele API)
Pages/               - Index.cshtml + Index.cshtml.cs (single-page app)
wwwroot/js/site.js   - All client-side logic (~650 lines)
wwwroot/css/dark-theme.css - Custom dark theme with glassmorphism effects
```

### Data Flow

1. **Client input** → CSV/text parsing in JavaScript → Recipients array in browser memory
2. **Validation** → POST to `/Index?handler=Validate` → Server-side validation → JSON response
3. **Preview** → POST to `/Index?handler=GeneratePreview` → Tag replacement → JSON with previews
4. **Send SMS** → POST to `/Index?handler=SendSms` → Bulk send via ComteleSmsService → Status results

## Critical Patterns

### Message Template System

Messages use two template tags replaced at send time:

- `{NOME}` - Recipient name
- `{PRESENTE}` - Gift/assignment name

**Example**: `"Olá {NOME}! Você tirou o presente: {PRESENTE}"`

Tag replacement happens in TWO places:

1. **Preview**: [Index.cshtml.cs](Pages/Index.cshtml.cs) OnPostGeneratePreview (lines 92-94)
2. **Send**: [ComteleSmsService.cs](Services/ComteleSmsService.cs) SendBulkSmsAsync (lines 162-164)

Always maintain consistency between both implementations.

### API Integration with Comtele

- **Service**: [ComteleSmsService.cs](Services/ComteleSmsService.cs)
- **Authentication**: Bearer token in Authorization header (per-request, not global)
- **Rate Limiting**: Configurable delay via `Comtele:DelayBetweenRequestsMs` (default 500ms)
- **Phone Format**: Strip non-digits, keep 10-15 digits including country code

⚠️ **Important**: HttpClient headers MUST be set per-request (using HttpRequestMessage) to avoid concurrency issues when sending bulk SMS. See lines 100-103 in ComteleSmsService.cs.

### Validation Logic

Phone validation in [SmsRecipient.cs](Models/SmsRecipient.cs) (lines 55-62):

- Strip non-digits
- Require 10-15 digits (supports international formats)
- All fields (Nome, Celular, Presente) are required

### Security Note

`[IgnoreAntiforgeryToken]` is applied to IndexModel for simplicity. Production deployments with authentication should implement proper CSRF protection.

## Development Workflows

### Build & Run

```powershell
dotnet restore
dotnet build
dotnet run
```

App runs on https://localhost:5001 or http://localhost:5000

### Project Targets .NET 10

The .csproj targets `net10.0`. Ensure SDK compatibility when suggesting NuGet packages or language features.

### Configuration

Settings in [appsettings.json](appsettings.json):

- `Comtele:ApiUrl` - Base API endpoint
- `Comtele:DelayBetweenRequestsMs` - Throttling between SMS sends

### Client-Side Development

Main JavaScript file: [wwwroot/js/site.js](wwwroot/js/site.js)

- Global state in `recipients` array (line 6)
- Validation flag `isValidated` controls send button enable/disable
- All fetch calls use POST with JSON body to Razor Page handlers

## Component-Specific Guidance

### When Modifying Models

All models in `Models/` folder have XML doc comments. Maintain this pattern for IDE IntelliSense.

### When Modifying ComteleSmsService

- Respect delay configuration for rate limiting
- Always use per-request HttpRequestMessage for headers (not default headers)
- Log all API interactions (ILogger injected)
- Return SmsSendResult objects with detailed status

### When Modifying UI

- Custom CSS in [dark-theme.css](wwwroot/css/dark-theme.css) - glassmorphism effects
- Bootstrap 5 classes used but heavily overridden
- Font Awesome 6.4.0 icons
- Character counting updates on input (160 chars/SMS threshold)

### When Adding API Endpoints

Follow the existing handler pattern in Index.cshtml.cs:

```csharp
public IActionResult OnPost<HandlerName>([FromBody] <RequestType> request)
{
    // Deserialize recipients from JSON string
    var recipients = JsonSerializer.Deserialize<List<SmsRecipient>>(request.Recipients);
    // Process and return JsonResult
}
```

## Common Tasks

### Adding New Message Template Tags

1. Update tag replacement in OnPostGeneratePreview handler
2. Update tag replacement in ComteleSmsService.SendBulkSmsAsync
3. Add tag documentation to UI (message section in Index.cshtml)
4. Update README.md usage examples

### Modifying SMS Validation

Validation logic centralized in SmsRecipient.Validate() method. Update there for consistent validation across all endpoints.

### Changing API Provider

Encapsulation in ComteleSmsService allows swapping SMS providers. Key interface:

- `SendSmsAsync(apiKey, phone, message)` - Single SMS
- `SendBulkSmsAsync(apiKey, recipients, template)` - Bulk with template

## Testing Strategy

No automated tests exist. Manual testing workflow:

1. Use CSV format: `nome;celular;presente` (semicolon-separated)
2. Test with Comtele sandbox/test API key
3. Verify rate limiting works (check logs for delays)
4. Test validation with invalid phones/missing fields

## External Dependencies

- **Comtele API**: https://docs.comtele.com.br/ (SMS provider)
- No other external services or databases
