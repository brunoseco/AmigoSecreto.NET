# 🎁 AmigoSecreto.NET

<div align="center">

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green?style=for-the-badge)
![Status](https://img.shields.io/badge/status-active-success?style=for-the-badge)

**Sistema completo de sorteio de Amigo Secreto com envio automático de SMS**

Aplicação web moderna e intuitiva que realiza sorteio de Amigo Secreto e envia as atribuições via SMS usando a API da Comtele.

[Demonstração](#-demonstração) • [Instalação](#-instalação-e-execução) • [Como Usar](#-como-usar) • [Features](#-features) • [Comtele API](#-integração-comtele)

</div>

---

## ✨ Sobre o Projeto

O **AmigoSecreto.NET** é uma aplicação single-page desenvolvida em ASP.NET Razor Pages que automatiza completamente o processo de organização de Amigo Secreto. Com uma interface moderna e dark mode, o sistema:

- 🎲 **Realiza o sorteio automático** garantindo que ninguém tire a si mesmo
- 🚫 **Respeita restrições** personalizadas (ex: casais não tiram um ao outro)
- 📱 **Envia SMS automaticamente** com as atribuições para cada participante
- 🔒 **Não armazena dados** - tudo acontece em memória, garantindo privacidade total
- ⚡ **Interface responsiva** com tema dark glassmorphism inspirado no Google AI Studio

**Caso de uso ideal**: Amigos, famílias ou empresas que querem organizar Amigo Secreto sem complicação e com total sigilo.

## 🚀 Tecnologias

- **.NET 10.0**: Framework principal
- **ASP.NET Core Razor Pages**: Arquitetura single-page
- **Vanilla JavaScript**: Interatividade sem frameworks pesados
- **Bootstrap 5**: Framework CSS (altamente customizado)
- **Font Awesome 6.4.0**: Biblioteca de ícones
- **Comtele SMS API**: Integração para envio de SMS
- **HttpClient**: Cliente HTTP para API REST

## 📋 Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) ou superior
- Conta na [Comtele](https://sms.comtele.com.br) com créditos de SMS
- API Key da Comtele (instruções abaixo)

## 🔧 Instalação e Execução

### 1️⃣ Clone o repositório

```bash
git clone https://github.com/brunoseco/AmigoSecreto.NET.git
cd AmigoSecreto.NET
```

### 2️⃣ Restaure as dependências

```bash
dotnet restore
```

### 3️⃣ Configure a API da Comtele (opcional para desenvolvimento)

Edite o arquivo `appsettings.json` se desejar alterar a URL da API:

```json
{
  "Comtele": {
    "ApiUrl": "https://sms.comtele.com.br/api/v2/send",
    "DelayBetweenRequestsMs": 500
  }
}
```

### 4️⃣ Execute o projeto

```bash
dotnet run
```

ou para especificar a porta:

```bash
dotnet run --urls "http://localhost:5000"
```

### 5️⃣ Acesse no navegador

Abra seu navegador em: **http://localhost:5000** ou **https://localhost:5001**

## 📖 Como Usar

### 1. Obtenha sua API Key da Comtele

**Primeira vez usando a Comtele?** Siga estes passos:

1. **Crie sua conta**: Acesse [sms.comtele.com.br](https://sms.comtele.com.br) e crie uma conta gratuita
2. **Adquira créditos**: Faça uma recarga de créditos de SMS no painel
3. **Obtenha sua API Key**:
   - Acesse o menu lateral → **API** → **Chave de API**
   - Copie sua chave de API (formato: `XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX`)
4. **Cole no campo** de API Key do AmigoSecreto.NET

💡 **Dica**: A Comtele oferece créditos de teste para novos usuários!

### 2. Crie sua Mensagem Personalizada

Digite sua mensagem usando as **tags disponíveis** que serão substituídas automaticamente:

- `{NOME}` - Nome da pessoa que está recebendo o SMS (quem tirou)
- `{AMIGO}` - Nome da pessoa que foi sorteada (quem foi tirado)
- `{PRESENTE}` - Presente que a pessoa sorteada deseja

**Exemplo de mensagem:**
```
Olá {NOME}! 🎁

No sorteio de Amigo Secreto você tirou: {AMIGO}

Presente desejado: {PRESENTE}

Valor limite: R$ 50,00
Data da revelação: 24/12/2025
Local: Casa da Maria

Mantenha segredo! 🤫
```

💬 **Contador de caracteres**: O sistema mostra quantos SMS serão consumidos (cada SMS = 160 caracteres).

### 3. Cadastre os Participantes

Você tem **duas opções** para cadastrar os participantes:

#### 📄 Opção A - Upload de arquivo CSV

Prepare um arquivo CSV (Excel salvo como CSV) com 3 colunas separadas por **ponto e vírgula**:

```csv
nome;celular;presente
João Silva;11999999999;Livro de ficção
Maria Santos;11988888888;Fone Bluetooth
Pedro Costa;11977777777;Caneca personalizada
Ana Oliveira;11966666666;Planta suculenta
```

**Formato do telefone**: 
- ✅ Apenas números (DDD + número)
- ✅ Exemplo: `11999999999` (11 dígitos para celulares)
- ❌ Não use: `(11) 99999-9999` ou `+55 11 99999-9999`

#### 📋 Opção B - Colar texto diretamente

Cole no formato idêntico ao CSV:

```
João Silva;11999999999;Livro de ficção
Maria Santos;11988888888;Fone Bluetooth
Pedro Costa;11977777777;Caneca personalizada
```

### 4. Configure Restrições (Opcional)

Para cada participante, você pode **selecionar pessoas que ele NÃO PODE tirar** no sorteio:

1. Na tabela de participantes, clique no campo **"Restrições"**
2. Selecione uma ou mais pessoas (use **Ctrl+Click** para múltipla seleção)
3. O algoritmo garantirá que essa pessoa não tire nenhuma das selecionadas

**Casos de uso comuns:**
- 👫 **Casais**: João não tira Maria, Maria não tira João
- 👨‍👩‍👧 **Família**: Pai não tira filho
- 🏢 **Trabalho**: Chefe não tira subordinado direto

### 5. Edite os Dados (se necessário)

A tabela permite **edição inline** de todos os campos:
- Clique em qualquer campo para editar
- Adicione ou remova participantes
- Corrija números de telefone
- Atualize presentes desejados

### 6. Valide os Dados

Clique em **"Validar Dados"** para verificar:
- ✅ Todos os campos preenchidos
- ✅ Números de telefone válidos (10-15 dígitos)
- ✅ Nomes informados
- ✅ Presentes especificados

### 7. Visualize o Preview (Opcional)

Clique em **"Gerar Preview"** para ver:
- O sorteio simulado (quem tirou quem)
- As mensagens exatas que serão enviadas
- Verificação se as restrições foram respeitadas

**⚠️ Importante**: Cada vez que você gera o preview, um novo sorteio é realizado!

### 8. Envie os SMS! 🚀

1. Clique em **"Enviar SMS"**
2. O sistema irá:
   - Realizar o sorteio automático
   - Respeitar todas as restrições configuradas
   - Garantir que ninguém tire a si mesmo
   - Enviar SMS personalizado para cada participante
3. Acompanhe o progresso em tempo real
4. Veja o resultado de cada envio (Sucesso/Erro)

**💡 Custo**: Cada SMS consome 1 crédito da Comtele (mensagens longas consomem múltiplos créditos)

## 🎯 Features Principais

### 🎲 Sorteio Automático Inteligente
- **Algoritmo de shuffle** com até 1000 tentativas para encontrar combinação válida
- **Validação automática**: ninguém tira a si mesmo
- **Sistema de restrições**: configure quem não pode tirar quem (ex: casais)
- **Garantia de sorteio completo**: todos os participantes são incluídos

### 📱 Integração SMS via Comtele
- Envio automático via API REST da Comtele
- Taxa de envio configurável (padrão: 500ms entre cada SMS)
- Tratamento de erros e retry
- Feedback em tempo real de cada envio
- Suporte a mensagens longas (concatenação automática)

### ✉️ Sistema de Tags Personalizadas
Três tags disponíveis para personalização completa:
- **{NOME}**: Nome de quem recebe o SMS (quem tirou)
- **{AMIGO}**: Nome de quem foi sorteado (quem foi tirado)
- **{PRESENTE}**: Presente desejado pela pessoa sorteada

### 📊 Gerenciamento de Participantes
- **Upload CSV** ou **cola de texto** para cadastro em massa
- **Edição inline** de todos os campos na tabela
- **Sistema de restrições** com multi-seleção (Ctrl+Click)
- **Validação em tempo real** de telefones e dados obrigatórios
- **Exclusão individual** de participantes

### 🔍 Preview Inteligente
- Visualização exata das mensagens antes do envio
- Simulação do sorteio para conferência
- Verificação de restrições aplicadas
- Contador de caracteres e custos (quantidade de SMS)

### 🎨 Interface Moderna
- **Design dark glassmorphism** inspirado no Google AI Studio
- **Responsiva** para desktop, tablet e mobile
- **Animações suaves** e transições fluidas
- **Feedback visual** para todas as ações
- **Ícones Font Awesome** para melhor UX

### 🔒 Privacidade e Segurança
- **Zero persistência**: nenhum dado é salvo em banco ou arquivos
- **Stateless**: tudo acontece em memória durante a sessão
- **API Key não armazenada**: apenas em memória do navegador
- **Sem rastreamento**: não há analytics ou cookies de terceiros

### ⚡ Performance
- **Single Page Application**: sem recarregamentos de página
- **Rate limiting configurável**: evita sobrecarga da API
- **Processamento assíncrono**: interface não trava durante envios
- **Validações client-side**: feedback instantâneo

## 💬 Exemplos de Mensagens

### 📝 Exemplo 1: Amigo Secreto Tradicional

```
🎁 AMIGO SECRETO 2025 🎁

Olá {NOME}!

Você tirou: {AMIGO}
Presente desejado: {PRESENTE}

💰 Valor: até R$ 50,00
📅 Entrega: 24/12/2025
📍 Local: Casa da Ana

🤫 Mantenha segredo!
```

**Preview do SMS enviado** (exemplo):
```
🎁 AMIGO SECRETO 2025 🎁

Olá João!

Você tirou: Maria
Presente desejado: Livro de romance

💰 Valor: até R$ 50,00
📅 Entrega: 24/12/2025
📍 Local: Casa da Ana

🤫 Mantenha segredo!
```

### 🏢 Exemplo 2: Amigo Secreto Corporativo

```
AMIGO OCULTO EMPRESA XYZ

{NOME}, você foi sorteado(a)!

Seu amigo(a) oculto(a): {AMIGO}
Sugestão de presente: {PRESENTE}

Limite: R$ 100
Troca: 20/12 às 18h
Sala de reuniões

Dúvidas? Fale com o RH.
```

### 🎄 Exemplo 3: Natal em Família

```
🎅 Natal da Família Silva 🎄

Oi {NOME}!

No sorteio você tirou: {AMIGO} 🎁
Ele(a) deseja: {PRESENTE}

Não precisa gastar muito, o importante é o carinho!

Nos vemos dia 25/12 às 19h na casa da vovó ❤️

PS: Não conte pra ninguém! 🤐
```

### 🎉 Exemplo 4: Festa de Ano Novo

```
🎊 AMIGO SECRETO - RÉVEILLON 2025 🎊

E aí {NOME}! 🥳

Você pegou: {AMIGO}
Desejo: {PRESENTE}

💵 Valor sugerido: R$ 80
🗓️ Entrega: 31/12 à meia-noite
🏠 Reveillon na casa do Carlos

Vamos fazer dessa virada inesquecível! 🍾✨
```

### 💡 Dicas para Mensagens Eficazes

1. **Seja claro**: Informe valor limite, data e local
2. **Use emojis**: Tornam a mensagem mais alegre e visual
3. **Seja breve**: SMS tem limite de 160 caracteres por mensagem
4. **Inclua instruções**: Onde e quando será a revelação
5. **Reforce o sigilo**: Lembre de não contar o segredo!

**⚠️ Atenção ao tamanho**: Mensagens acima de 160 caracteres consomem créditos adicionais (1 crédito a cada 153 caracteres extras).

## 📁 Estrutura do Projeto

```
AmigoSecreto.NET/
├── 📂 Models/
│   ├── SmsRecipient.cs           # Modelo de participante
│   ├── SmsPreview.cs             # Modelo de preview
│   ├── SmsSendResult.cs          # Resultado do envio
│   ├── DrawResult.cs             # Resultado do sorteio
│   └── ComteleApiResponse.cs     # Resposta da API
│
├── 📂 Services/
│   └── ComteleSmsService.cs      # Integração com Comtele
│
├── 📂 Pages/
│   ├── Index.cshtml              # View principal
│   ├── Index.cshtml.cs           # Lógica de sorteio
│   └── _ViewImports.cshtml       # Imports globais
│
├── 📂 wwwroot/
│   ├── 📂 css/
│   │   └── dark-theme.css        # Tema dark personalizado
│   └── 📂 js/
│       └── site.js               # JavaScript client-side
│
├── 📄 Program.cs                 # Configuração da app
├── 📄 appsettings.json          # Configurações
└── 📄 README.md                  # Este arquivo
```

## 🔐 Segurança e Privacidade

- ✅ **API Key não persistida**: Armazenada apenas em memória do navegador
- ✅ **Zero banco de dados**: Nenhum dado é salvo permanentemente
- ✅ **Sistema stateless**: Tudo acontece em memória durante a sessão
- ✅ **Sem cookies de rastreamento**: Não há analytics ou tracking
- ✅ **Código aberto**: Auditável e transparente

**⚠️ Nota de Segurança para Produção:**

Esta é uma aplicação de demonstração. Para uso em produção corporativa, considere:
- ✨ Implementar autenticação de usuários (JWT, OAuth)
- 🛡️ Adicionar proteção CSRF adequada
- ⏱️ Implementar rate limiting robusto
- 🔍 Adicionar validação de entrada mais rigorosa
- 🔒 Usar HTTPS obrigatório com certificado válido
- 📊 Implementar logging de auditoria e monitoramento

## 🔌 Integração com Comtele API

### 📘 Como Obter sua API Key

1. **Acesse**: [https://sms.comtele.com.br](https://sms.comtele.com.br)
2. **Cadastre-se**: Preencha o formulário de registro (é rápido!)
3. **Valide o email**: Confirme seu cadastro pelo link enviado
4. **Faça uma recarga**: 
   - Acesse o menu **"Recarregar"**
   - Escolha o pacote de créditos
   - Pague por PIX, cartão ou boleto
5. **Obtenha sua API Key**:
   - Menu lateral → **"Configurações"** → **"API"** → **"Chave de API"**
   - Copie a chave no formato: `XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX`

💡 **Dica**: A Comtele frequentemente oferece créditos bônus para novos usuários!

### 📚 Documentação Técnica

- **📖 Docs oficial**: [https://docs.comtele.com.br](https://docs.comtele.com.br)
- **🌐 Endpoint**: `https://sms.comtele.com.br/api/v2/send`
- **🔧 Método HTTP**: `POST`
- **🔑 Autenticação**: Header `auth-key`

### 🛠️ Detalhes da Implementação

**Headers enviados:**
```http
POST https://sms.comtele.com.br/api/v2/send
Content-Type: application/json
auth-key: SUA-API-KEY-AQUI
```

**Body da requisição:**
```json
{
  "Receivers": "11999999999",
  "Content": "Sua mensagem personalizada aqui",
  "Sender": "AmigoSecreto"
}
```

**Resposta de sucesso (200 OK):**
```json
{
  "Success": true,
  "Object": {
    "requestUniqueId": "uuid-da-requisicao"
  },
  "Message": "A requisicao de envio foi encaminhada para processamento com sucesso."
}
```

**Códigos de erro comuns:**

| Código | Significado | Solução |
|--------|-------------|---------|
| `400` | Bad Request | Verifique o formato dos dados (telefone, mensagem) |
| `401` | Unauthorized | API Key inválida ou expirada - pegue uma nova |
| `404` | Not Found | Endpoint incorreto - verifique a URL |
| `429` | Too Many Requests | Rate limit excedido - aumente o delay |

### ⚙️ Configurações no Projeto

Arquivo `appsettings.json`:

```json
{
  "Comtele": {
    "ApiUrl": "https://sms.comtele.com.br/api/v2/send",
    "DelayBetweenRequestsMs": 500
  }
}
```

- **ApiUrl**: Endpoint da API da Comtele
- **DelayBetweenRequestsMs**: Intervalo entre envios (evita rate limiting)

### 💰 Custos e Créditos

| Tamanho da Mensagem | Créditos Consumidos |
|---------------------|---------------------|
| Até 160 caracteres | 1 crédito |
| 161-306 caracteres | 2 créditos |
| 307-459 caracteres | 3 créditos |
| 460-612 caracteres | 4 créditos |

💡 **Dica**: O sistema exibe automaticamente quantos SMS serão consumidos baseado no tamanho da mensagem.

## 🎯 Formato de Cadastro de Participantes

### 📄 Formato CSV/Texto

Três colunas separadas por **ponto e vírgula (;)**:

```
nome;celular;presente
João Silva;11999999999;Livro de ficção científica
Maria Santos;11988888888;Fone de ouvido Bluetooth
Pedro Costa;11977777777;Caneca personalizada
Ana Oliveira;11966666666;Planta suculenta
```

### ✅ Regras de Validação

| Campo | Obrigatório | Formato | Exemplo |
|-------|-------------|---------|---------|
| **Nome** | ✅ Sim | Texto livre | "João Silva" |
| **Celular** | ✅ Sim | 10-15 dígitos (somente números) | "11999999999" |
| **Presente** | ✅ Sim | Texto livre | "Livro de ficção" |

### 📱 Formato Correto do Telefone

| ✅ Válido | ❌ Inválido |
|-----------|-------------|
| `11999999999` | `(11) 99999-9999` |
| `5511999999999` | `11 9 9999-9999` |
| `21987654321` | `+55 21 98765-4321` |

**Dica**: Remova todos os caracteres especiais, deixe apenas dígitos!

### 💾 Template para Download

Copie e cole em um arquivo `.csv`:

```csv
nome;celular;presente
João Silva;11999999999;Livro de ficção
Maria Santos;11988888888;Fone Bluetooth  
Pedro Costa;11977777777;Caneca personalizada
Ana Oliveira;11966666666;Planta suculenta
Carlos Souza;11955555555;Kit de churrasco
Beatriz Lima;11944444444;Jogo de tabuleiro
Rafael Alves;11933333333;Garrafa térmica
Juliana Dias;11922222222;Almofada decorativa
```

### 📊 Como Preparar no Excel/Google Sheets

1. **Crie uma planilha** com 3 colunas: Nome | Celular | Presente
2. **Preencha os dados** normalmente
3. **Salve como CSV**:
   - Excel: "Arquivo" → "Salvar Como" → "CSV (separado por ponto e vírgula)"
   - Google Sheets: "Arquivo" → "Fazer download" → "CSV"
4. **Carregue no sistema** via upload ou copiar/colar

## 💡 Dicas de Uso

1. **Teste Primeiro**: Use a funcionalidade de preview antes de enviar
2. **Valide Números**: Certifique-se que os celulares estão no formato correto
3. **Mensagens Curtas**: SMS tem limite de 160 caracteres (o sistema avisa quando ultrapassar)
4. **Ignorar Contatos**: Marque contatos específicos para não enviar (útil para testes)
5. **Edição Inline**: Corrija dados diretamente na tabela sem recarregar o arquivo

## 🐛 Solução de Problemas

**API Key inválida:**
- Verifique se a chave foi copiada corretamente
- Confirme que a chave está ativa na Comtele

**Erro no envio:**
- Verifique se os números estão no formato correto
- Confirme que há créditos disponíveis na conta Comtele
- Veja os logs no console da aplicação

**CSV não carrega:**
- Verifique o formato (separador `;`)
- Certifique-se que o arquivo está codificado em UTF-8
- Verifique se não há linhas vazias no meio do arquivo

## � Dicas de Uso

1. 🧪 **Teste primeiro**: Use o preview antes de enviar para todos
2. 📱 **Valide os números**: Certifique-se que os celulares estão corretos
3. 💬 **Mensagens curtas**: SMS tem limite de 160 caracteres (sistema avisa quando ultrapassar)
4. 🎯 **Use restrições**: Configure casais ou familiares que não devem se tirar
5. ✏️ **Edição inline**: Corrija dados diretamente na tabela sem recarregar
6. 🔄 **Sorteio automático**: Deixe o algoritmo garantir um sorteio justo
7. 💾 **Sem backup**: Sistema não salva dados - organize bem antes de fechar!

## 🐛 Solução de Problemas

### ❌ "API Key inválida"
- ✅ Verifique se copiou a chave completa (formato UUID)
- ✅ Confirme que a chave está ativa no painel da Comtele
- ✅ Tente gerar uma nova chave de API

### ❌ "Erro no envio de SMS"
- ✅ Confirme que há **créditos disponíveis** na conta Comtele
- ✅ Verifique se os números estão no **formato correto** (apenas dígitos)
- ✅ Confira se a mensagem não tem caracteres especiais incompatíveis
- ✅ Veja os logs no console do navegador (F12) para detalhes

### ❌ "CSV não carrega"
- ✅ Verifique o **separador** (deve ser ponto e vírgula `;`)
- ✅ Certifique-se que o arquivo está em **UTF-8**
- ✅ Remova linhas vazias no meio do arquivo
- ✅ Teste com um arquivo pequeno primeiro (2-3 linhas)

### ❌ "Sorteio não encontra solução"
- ✅ Revise as **restrições** - podem estar muito restritivas
- ✅ Certifique-se que há **pelo menos 3 participantes**
- ✅ Verifique se não criou restrições impossíveis (ex: todos restringem todos)

### ❌ "Mensagem não personaliza"
- ✅ Use as tags corretas: `{NOME}`, `{AMIGO}`, `{PRESENTE}`
- ✅ As tags são case-sensitive (letras maiúsculas)
- ✅ Não use espaços dentro das chaves: `{ NOME }` ❌

## 🔄 Melhorias Futuras Sugeridas

Contribua com o projeto! Ideias para próximas versões:

### 🎯 Features Planejadas
- [ ] **Agendamento**: Agendar envios para data/hora específica
- [ ] **Templates**: Salvar/carregar templates de mensagens (localStorage)
- [ ] **Histórico**: Histórico de envios realizados com filtros
- [ ] **Importação Excel**: Suporte nativo a arquivos `.xlsx`
- [ ] **Grupos**: Organizar participantes em grupos/categorias
- [ ] **Webhooks**: Receber confirmações de entrega da Comtele
- [ ] **Multi-idioma**: Suporte a inglês, espanhol e português
- [ ] **Dark/Light Toggle**: Alternar entre tema claro e escuro
- [ ] **Exportar resultados**: Baixar lista de quem tirou quem (PDF/CSV)
- [ ] **Validação de entrega**: Verificar status de entrega dos SMS

### 🎨 Melhorias de UI/UX
- [ ] **Tour guiado**: Tutorial interativo para primeiros usos
- [ ] **Atalhos de teclado**: Produtividade com shortcuts
- [ ] **Drag & drop**: Upload de CSV por arrastar e soltar
- [ ] **Preview responsivo**: Visualização mobile das mensagens
- [ ] **Temas personalizáveis**: Mais opções de cores e estilos

### 🔧 Melhorias Técnicas
- [ ] **Testes unitários**: Cobertura com xUnit
- [ ] **Docker**: Containerização da aplicação
- [ ] **CI/CD**: Pipeline automatizado (GitHub Actions)
- [ ] **PWA**: Transformar em Progressive Web App
- [ ] **Logs estruturados**: Logging com Serilog

## 📝 Licença

Este projeto está licenciado sob a **Licença MIT** - veja o arquivo [LICENSE](LICENSE) para detalhes.

```
MIT License - Copyright (c) 2025

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software...
```

**Em resumo**: Você pode usar, modificar e distribuir livremente! 🎉

## 🤝 Contribuindo

Contribuições são **muito bem-vindas**! 

### Como Contribuir

1. **Fork** o projeto
2. Crie uma **branch** para sua feature (`git checkout -b feature/MinhaFeature`)
3. **Commit** suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. **Push** para a branch (`git push origin feature/MinhaFeature`)
5. Abra um **Pull Request**

### Diretrizes

- ✅ Mantenha o código limpo e documentado
- ✅ Adicione comentários em lógicas complexas
- ✅ Teste suas alterações antes de submeter
- ✅ Siga o padrão de código existente
- ✅ Atualize a documentação se necessário

### Reportar Bugs

Encontrou um bug? Abra uma [issue](https://github.com/brunoseco/AmigoSecreto.NET/issues) com:
- 🐛 Descrição clara do problema
- 📋 Passos para reproduzir
- 🖼️ Screenshots se aplicável
- 💻 Ambiente (SO, navegador, versão do .NET)

## 👨‍💻 Autor

**Bruno Seco**
- 🌐 GitHub: [@brunoseco](https://github.com/brunoseco)

Desenvolvido com o objetivo de simplificar a organização de Amigo Secreto e demonstrar as capacidades da integração SMS via API.

## 🤖 Desenvolvido com GitHub Copilot

<div align="center">

### ⚡ Este projeto foi construído utilizando IA!

![GitHub Copilot](https://img.shields.io/badge/Powered%20by-GitHub%20Copilot-blue?style=for-the-badge&logo=github)
![VS Code](https://img.shields.io/badge/Built%20with-VS%20Code-007ACC?style=for-the-badge&logo=visualstudiocode)
![Time](https://img.shields.io/badge/Built%20in-~30%20minutes-success?style=for-the-badge&logo=clockify)

</div>

**🚀 Velocidade de desenvolvimento:**
- ⏱️ Tempo total: **~30 minutos** (da ideia ao projeto funcional)
- 🤖 **GitHub Copilot** para geração de código
- 💻 **VS Code** como IDE
- 🎯 Foco em produtividade e qualidade

### 💡 Como o Copilot Ajudou

O **GitHub Copilot** foi fundamental em todas as etapas do desenvolvimento:

1. **🏗️ Arquitetura**: Sugestões de estrutura de projeto ASP.NET Core
2. **💻 Código Backend**: Geração de models, services e handlers
3. **🎨 Frontend**: HTML, CSS e JavaScript com sugestões contextuais
4. **🔌 Integração API**: Implementação completa do cliente HTTP para Comtele
5. **📝 Documentação**: Geração deste README detalhado
6. **🐛 Debugging**: Identificação e correção de bugs rapidamente
7. **♻️ Refatoração**: Melhorias de código e otimizações

### 🎯 Benefícios da IA no Desenvolvimento

- ✨ **Produtividade 10x**: O que levaria horas foi feito em minutos
- 🎓 **Aprendizado**: Sugestões ensinam boas práticas e padrões
- 🔧 **Menos erros**: Code review em tempo real
- 📚 **Documentação automática**: Comentários e docs gerados
- 🚀 **Foco no problema**: Menos tempo com sintaxe, mais com lógica

### 🤔 Quer experimentar?

1. Instale o [GitHub Copilot](https://github.com/features/copilot) no VS Code
2. Clone este projeto e explore o código
3. Veja as sugestões do Copilot ao editar
4. Experimente pedir para ele criar novas features!

**Dica**: Use comentários descritivos e o Copilot gerará código de qualidade automaticamente.

---

<div align="center">

**⭐ Se este projeto foi útil, deixe uma estrela no GitHub!**

**🎁 Boas festas e bons sorteios! 🎄**

</div>