// AmigoSecreto .NET - Main JavaScript
// Handles all client-side interactions

// Global state
let recipients = [];
let isValidated = false;

// Initialize on DOM load
document.addEventListener('DOMContentLoaded', function () {
    initializeEventListeners();
    updateCharCount();
});

// Initialize all event listeners
function initializeEventListeners() {
    // API Key
    document.getElementById('apiKey').addEventListener('input', validateApiKey);
    document.getElementById('toggleApiKey').addEventListener('click', toggleApiKeyVisibility);

    // Message Template
    document.getElementById('messageTemplate').addEventListener('input', updateCharCount);

    // Contact Input
    document.getElementById('loadCsvBtn').addEventListener('click', loadFromCsv);
    document.getElementById('loadTextBtn').addEventListener('click', loadFromText);
    document.getElementById('csvFile').addEventListener('change', handleFileSelect);

    // Table Actions
    document.getElementById('clearTableBtn').addEventListener('click', clearTable);

    // Validation and Preview
    document.getElementById('validateBtn').addEventListener('click', validateOnly);
    document.getElementById('previewBtn').addEventListener('click', validateAndPreview);

    // Send SMS
    document.getElementById('sendSmsBtn').addEventListener('click', sendSms);
}

// API Key Management
function validateApiKey() {
    const apiKey = document.getElementById('apiKey').value.trim();
    const statusDiv = document.getElementById('apiKeyStatus');

    if (apiKey.length === 0) {
        statusDiv.innerHTML = '';
        return false;
    }

    if (apiKey.length < 10) {
        statusDiv.innerHTML = '<div class="alert alert-warning mt-2"><i class="fas fa-exclamation-triangle"></i> API Key parece muito curta</div>';
        return false;
    }

    statusDiv.innerHTML = '<div class="alert alert-success mt-2"><i class="fas fa-check-circle"></i> API Key configurada</div>';
    return true;
}

function toggleApiKeyVisibility() {
    const apiKeyInput = document.getElementById('apiKey');
    const toggleBtn = document.getElementById('toggleApiKey');

    if (apiKeyInput.type === 'password') {
        apiKeyInput.type = 'text';
        toggleBtn.innerHTML = '<i class="fas fa-eye-slash"></i>';
    } else {
        apiKeyInput.type = 'password';
        toggleBtn.innerHTML = '<i class="fas fa-eye"></i>';
    }
}

// Character Counter
function updateCharCount() {
    const messageTemplate = document.getElementById('messageTemplate').value;
    const charCount = messageTemplate.length;
    const smsCount = charCount > 0 ? Math.ceil(charCount / 160) : 0;

    document.getElementById('charCount').textContent = `${charCount} caracteres`;
    document.getElementById('smsCount').textContent = `${smsCount} SMS`;

    const charWarning = document.getElementById('charWarning');
    if (charCount > 160) {
        charWarning.classList.remove('d-none');
        document.getElementById('charCount').classList.add('warning');
    } else {
        charWarning.classList.add('d-none');
        document.getElementById('charCount').classList.remove('warning');
    }
}

// File Selection
function handleFileSelect(event) {
    const file = event.target.files[0];
    if (file) {
        document.getElementById('loadCsvBtn').innerHTML =
            `<i class="fas fa-file-check me-2"></i>${file.name}`;
    }
}

// Load contacts from CSV
function loadFromCsv() {
    const fileInput = document.getElementById('csvFile');
    const file = fileInput.files[0];

    if (!file) {
        showAlert('Por favor, selecione um arquivo CSV', 'warning');
        return;
    }

    const reader = new FileReader();
    reader.onload = function (e) {
        const text = e.target.result;
        parseAndLoadContacts(text);
    };
    reader.readAsText(file);
}

// Load contacts from text input
function loadFromText() {
    const text = document.getElementById('textInput').value.trim();

    if (!text) {
        showAlert('Por favor, cole os contatos no formato correto', 'warning');
        return;
    }

    parseAndLoadContacts(text);
}

// Parse and load contacts
function parseAndLoadContacts(text) {
    const lines = text.split('\n').filter(line => line.trim() !== '');
    const newRecipients = [];
    let errors = 0;

    lines.forEach((line, index) => {
        // Skip header line if it exists
        if (index === 0 && line.toLowerCase().includes('nome') && line.toLowerCase().includes('celular')) {
            return;
        }

        const parts = line.split(';').map(p => p.trim());

        if (parts.length >= 3) {
            newRecipients.push({
                id: generateTempId(),
                nome: parts[0],
                celular: parts[1],
                presente: parts[2],
                restrictions: [],
                isValid: true,
                validationMessage: null
            });
        } else {
            errors++;
        }
    });

    if (newRecipients.length > 0) {
        recipients = [...recipients, ...newRecipients];
        renderRecipientsTable();
        showAlert(`${newRecipients.length} contatos carregados com sucesso!${errors > 0 ? ` (${errors} linhas ignoradas)` : ''}`, 'success');

        // Enable validation buttons
        document.getElementById('validateBtn').disabled = false;
        document.getElementById('previewBtn').disabled = false;

        // Clear inputs
        document.getElementById('csvFile').value = '';
        document.getElementById('textInput').value = '';
        document.getElementById('loadCsvBtn').innerHTML = '<i class="fas fa-upload me-2"></i>Carregar CSV';
    } else {
        showAlert('Nenhum contato válido encontrado', 'danger');
    }
}

// Generate temporary ID
function generateTempId() {
    return 'r_' + Math.random().toString(36).substring(2, 11);
}

// Render recipients table
function renderRecipientsTable() {
    const container = document.getElementById('recipientsTableContainer');
    console.log('Rendering table with recipients:', recipients);

    if (recipients.length === 0) {
        container.innerHTML = `
            <p class="text-muted text-center py-4">
                <i class="fas fa-inbox fa-3x mb-3 d-block"></i>
                Nenhum contato carregado ainda
            </p>
        `;
        return;
    }

    let tableHtml = `
        <div class="table-responsive">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <th style="width: 45%;">Contato</th>
                        <th style="width: 40%;">Não pode tirar</th>
                        <th style="width: 15%;" class="text-center">Ações</th>
                    </tr>
                </thead>
                <tbody>
    `;

    recipients.forEach((recipient, index) => {
        const rowClass = !recipient.isValid ? 'table-warning' : '';

        // Build restrictions dropdown
        const restrictionsOptions = recipients
            .filter(r => r.id !== recipient.id)
            .map(r => {
                const isRestricted = recipient.restrictions && recipient.restrictions.includes(r.id);
                return `<option value="${r.id}" ${isRestricted ? 'selected' : ''}>${escapeHtml(r.nome)}</option>`;
            })
            .join('');

        tableHtml += `
            <tr class="${rowClass}" data-id="${recipient.id}">
                <td>
                    <div class="contact-cell">
                        <input type="text" class="form-control contact-input" 
                            value="${escapeHtml(recipient.nome)}" 
                            placeholder="Nome completo"
                            onchange="updateRecipient('${recipient.id}', 'nome', this.value); renderRecipientsTable();">
                        <input type="text" class="form-control contact-input" 
                            value="${escapeHtml(recipient.celular)}" 
                            placeholder="(11) 99999-9999"
                            onchange="updateRecipient('${recipient.id}', 'celular', this.value)">
                        <input type="text" class="form-control contact-input" 
                            value="${escapeHtml(recipient.presente)}" 
                            placeholder="Presente desejado"
                            onchange="updateRecipient('${recipient.id}', 'presente', this.value)">
                    </div>
                </td>
                <td>
                    <select multiple class="form-control restrictions-select" 
                        onchange="updateRestrictions('${recipient.id}', this)">
                        ${restrictionsOptions}
                    </select>
                </td>
                <td class="text-center">
                    <button type="button" class="btn btn-outline-danger btn-delete" 
                        onclick="deleteRecipient('${recipient.id}')"
                        title="Remover contato">
                        <i class="fas fa-trash-alt"></i>
                    </button>
                </td>
            </tr>
        `;

        if (!recipient.isValid && recipient.validationMessage) {
            tableHtml += `
                <tr class="${rowClass}">
                    <td colspan="3">
                        <div class="alert alert-warning mb-0">
                            <i class="fas fa-exclamation-triangle"></i> ${recipient.validationMessage}
                        </div>
                    </td>
                </tr>
            `;
        }
    });

    tableHtml += `
                </tbody>
            </table>
        </div>
        <div class="mt-3">
            <span class="badge bg-primary me-2"><i class="fas fa-users me-1"></i> Total: ${recipients.length}</span>
            <small class="text-muted">Ctrl+Click para selecionar múltiplas restrições</small>
        </div>
    `;

    container.innerHTML = tableHtml;
}

// Update restrictions for a recipient
function updateRestrictions(id, selectElement) {
    const recipient = recipients.find(r => r.id === id);
    if (recipient) {
        const selectedOptions = Array.from(selectElement.selectedOptions);
        recipient.restrictions = selectedOptions.map(opt => opt.value);
        isValidated = false;
    }
}

// Update recipient field
function updateRecipient(id, field, value) {
    const recipient = recipients.find(r => r.id === id);
    if (recipient) {
        recipient[field] = value;
        isValidated = false; // Reset validation status
    }
}

// Delete recipient
function deleteRecipient(id) {
    if (confirm('Tem certeza que deseja remover este contato?')) {
        recipients = recipients.filter(r => r.id !== id);
        renderRecipientsTable();
        isValidated = false;

        if (recipients.length === 0) {
            document.getElementById('validateBtn').disabled = true;
            document.getElementById('previewBtn').disabled = true;
            document.getElementById('sendSmsBtn').disabled = true;
        }
    }
}

// Clear table
function clearTable() {
    if (recipients.length > 0 && confirm('Tem certeza que deseja limpar todos os contatos?')) {
        recipients = [];
        renderRecipientsTable();
        isValidated = false;
        document.getElementById('validateBtn').disabled = true;
        document.getElementById('previewBtn').disabled = true;
        document.getElementById('sendSmsBtn').disabled = true;
        document.getElementById('validationResult').innerHTML = '';
        document.getElementById('previewContainer').innerHTML = '';
    }
}

// Validate only
async function validateOnly() {
    if (recipients.length === 0) {
        showAlert('Nenhum contato para validar', 'warning');
        return;
    }

    showLoader('validateBtn', true);

    try {
        const response = await fetch('/Index?handler=Validate', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify({
                recipients: recipients
            })
        });

        const result = await response.json();
        console.log('Validation result:', result);

        if (result.success) {
            // Update recipients with validation results
            recipients = result.recipients;
            console.log('Updated recipients:', recipients);
            renderRecipientsTable();

            document.getElementById('validationResult').innerHTML = `
                <div class="alert alert-success">
                    <i class="fas fa-check-circle me-2"></i>${result.message}
                </div>
            `;

            isValidated = true;
            document.getElementById('sendSmsBtn').disabled = false;
        } else {
            recipients = result.recipients || recipients;
            renderRecipientsTable();

            document.getElementById('validationResult').innerHTML = `
                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle me-2"></i>${result.message}
                </div>
            `;

            isValidated = false;
            document.getElementById('sendSmsBtn').disabled = true;
        }
    } catch (error) {
        console.error('Validation error:', error);
        showAlert('Erro ao validar contatos', 'danger');
    } finally {
        showLoader('validateBtn', false);
    }
}

// Validate and preview
async function validateAndPreview() {
    if (recipients.length === 0) {
        showAlert('Nenhum contato para preview', 'warning');
        return;
    }

    const messageTemplate = document.getElementById('messageTemplate').value.trim();
    if (!messageTemplate) {
        showAlert('Por favor, insira uma mensagem', 'warning');
        return;
    }

    showLoader('previewBtn', true);

    try {
        const response = await fetch('/Index?handler=GeneratePreview', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify({
                recipients: recipients,
                messageTemplate: messageTemplate
            })
        });

        const result = await response.json();

        if (result.success) {
            console.log('Preview result:', result.previews);
            displayPreview(result.previews);
            isValidated = true;
            document.getElementById('sendSmsBtn').disabled = false;
        } else {
            showAlert(result.message, 'danger');
        }
    } catch (error) {
        console.error('Preview error:', error);
        showAlert('Erro ao gerar preview', 'danger');
    } finally {
        showLoader('previewBtn', false);
    }
}

// Display preview
function displayPreview(previews) {
    const container = document.getElementById('previewContainer');

    let html = '<div class="accordion mt-3" id="previewAccordion">';

    previews.forEach((preview, index) => {
        const status = preview.willBeIgnored ?
            '<span class="badge bg-secondary">Será Ignorado</span>' :
            '<span class="badge bg-success">Será Enviado</span>';

        html += `
            <div class="accordion-item">
                <h2 class="accordion-header" id="heading${index}">
                    <button class="accordion-button ${index === 0 ? '' : 'collapsed'}" type="button" 
                        data-bs-toggle="collapse" data-bs-target="#collapse${index}">
                        <strong>${preview.nome}</strong>
                        <span class="ms-2">${preview.celular}</span>
                        <span class="ms-auto me-2">${status}</span>
                        <span class="badge bg-info">${preview.smsCount} SMS</span>
                    </button>
                </h2>
                <div id="collapse${index}" class="accordion-collapse collapse ${index === 0 ? 'show' : ''}" 
                    data-bs-parent="#previewAccordion">
                    <div class="accordion-body">
                        <p><strong>Mensagem Final:</strong></p>
                        <div class="alert alert-info">
                            ${escapeHtml(preview.mensagemFinal)}
                        </div>
                        <small class="text-muted">
                            ${preview.characterCount} caracteres | ${preview.smsCount} SMS
                        </small>
                    </div>
                </div>
            </div>
        `;
    });

    html += '</div>';
    container.innerHTML = html;
}

// Send SMS
async function sendSms() {
    if (!isValidated) {
        showAlert('Por favor, valide os contatos antes de enviar', 'warning');
        return;
    }

    const apiKey = document.getElementById('apiKey').value.trim();
    if (!apiKey) {
        showAlert('Por favor, insira a API Key', 'warning');
        return;
    }

    const messageTemplate = document.getElementById('messageTemplate').value.trim();
    if (!messageTemplate) {
        showAlert('Por favor, insira uma mensagem', 'warning');
        return;
    }

    if (recipients.length < 2) {
        showAlert('É necessário pelo menos 2 participantes para o sorteio', 'warning');
        return;
    }

    if (!confirm(`Deseja enviar SMS para ${recipients.length} contato(s)?`)) {
        return;
    }

    // Show progress
    document.getElementById('sendProgress').classList.remove('d-none');
    document.getElementById('sendSmsBtn').disabled = true;

    try {
        const response = await fetch('/Index?handler=SendSms', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify({
                apiKey: apiKey,
                recipients: recipients,
                messageTemplate: messageTemplate
            })
        });

        const result = await response.json();

        if (result.success) {
            displaySendResults(result.results, result.summary);
        } else {
            showAlert(result.message, 'danger');
            document.getElementById('sendSmsBtn').disabled = false;
        }
    } catch (error) {
        console.error('Send error:', error);
        showAlert('Erro ao enviar SMS', 'danger');
        document.getElementById('sendSmsBtn').disabled = false;
    } finally {
        document.getElementById('sendProgress').classList.add('d-none');
    }
}

// Display send results
function displaySendResults(results, summary) {
    const container = document.getElementById('sendResults');

    let html = `
        <div class="alert alert-info mt-3">
            <h5><i class="fas fa-chart-bar me-2"></i>Resumo do Envio</h5>
            <p class="mb-0">
                <strong>Total:</strong> ${summary.total} | 
                <strong class="text-success">Enviados:</strong> ${summary.sent} | 
                <strong class="text-danger">Erros:</strong> ${summary.errors}
            </p>
        </div>
        
        <div class="table-responsive mt-3">
            <table class="table table-sm">
                <thead>
                    <tr>
                        <th>Nome</th>
                        <th>Celular</th>
                        <th>Status</th>
                        <th>Detalhes</th>
                    </tr>
                </thead>
                <tbody>
    `;

    results.forEach(result => {
        const statusClass = result.success ? 'status-enviado' : 'status-erro';
        const statusIcon = result.success ? 'fa-check-circle' : 'fa-times-circle';

        html += `
            <tr>
                <td>${escapeHtml(result.recipientName)}</td>
                <td>${escapeHtml(result.phoneNumber)}</td>
                <td>
                    <span class="status-badge ${statusClass}">
                        <i class="fas ${statusIcon}"></i>${result.status}
                    </span>
                </td>
                <td>
                    ${result.errorMessage ?
                `<small class="text-danger">${escapeHtml(result.errorMessage)}</small>` :
                '<small class="text-success">Enviado com sucesso</small>'}
                </td>
            </tr>
        `;
    });

    html += `
                </tbody>
            </table>
        </div>
    `;

    container.innerHTML = html;
}

// Utility functions
function showAlert(message, type) {
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'warning' ? 'exclamation-triangle' : 'times-circle'} me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

    // Display in validation result area
    const resultDiv = document.getElementById('validationResult');
    resultDiv.innerHTML = alertHtml;

    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        const alert = resultDiv.querySelector('.alert');
        if (alert) {
            alert.classList.remove('show');
            setTimeout(() => alert.remove(), 150);
        }
    }, 5000);
}

function showLoader(buttonId, show) {
    const button = document.getElementById(buttonId);
    const originalHtml = button.innerHTML;

    if (show) {
        button.disabled = true;
        button.setAttribute('data-original-html', originalHtml);
        button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Processando...';
    } else {
        button.disabled = false;
        const original = button.getAttribute('data-original-html');
        if (original) {
            button.innerHTML = original;
        }
    }
}

function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, m => map[m]);
}

function getAntiForgeryToken() {
    // Try to get the token from a hidden input or meta tag
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    if (tokenInput) {
        return tokenInput.value;
    }

    const tokenMeta = document.querySelector('meta[name="__RequestVerificationToken"]');
    if (tokenMeta) {
        return tokenMeta.content;
    }

    // If no token found, return empty string (for development)
    return '';
}
