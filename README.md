# Comtele SMS Sender - Amigo Secreto

Sistema moderno de envio de SMS personalizado via API da Comtele, desenvolvido com .NET 8 e Razor Pages.

## 🎨 Características

- **Interface Dark Futurista**: Inspirada no Google AI Studio com efeitos glassmorphism
- **Envio Personalizado**: Suporte a tags dinâmicas ({NOME}, {PRESENTE})
- **Múltiplas Entradas**: Upload CSV ou cola de texto manual
- **Tabela Editável**: Edição inline de todos os campos
- **Validação Completa**: Validação de dados antes do envio
- **Preview de Mensagens**: Visualização exata do que será enviado
- **Controle de Envio**: Opção de ignorar contatos específicos
- **Sem Persistência**: Dados sensíveis apenas em memória

## 🚀 Tecnologias

- **.NET 8**: Framework principal
- **Razor Pages**: Arquitetura de páginas
- **Bootstrap 5**: Framework CSS (customizado)
- **Font Awesome**: Ícones
- **JavaScript**: Interatividade (sem frameworks pesados)
- **HttpClient**: Integração com API REST

## 📋 Pré-requisitos

- .NET 8 SDK ou superior
- API Key da Comtele (obtida em https://comtele.com.br/)

## 🔧 Instalação e Execução

1. Clone o repositório:
```bash
git clone https://github.com/brunoseco/AmigoSecreto.NET.git
cd AmigoSecreto.NET
```

2. Restaure as dependências:
```bash
dotnet restore
```

3. Execute o projeto:
```bash
dotnet run
```

4. Abra o navegador em: `https://localhost:5001` ou `http://localhost:5000`

## 📖 Como Usar

### 1. Configure a API Key
- Insira sua API Key da Comtele no campo apropriado
- A chave não é armazenada, apenas mantida em memória durante a sessão

### 2. Crie a Mensagem
- Digite sua mensagem no campo de texto
- Use as tags disponíveis:
  - `{NOME}` - Será substituído pelo nome do destinatário
  - `{PRESENTE}` - Será substituído pelo nome do presente
- Exemplo: "Olá {NOME}! Você tirou o presente: {PRESENTE}"

### 3. Carregue os Contatos

**Opção A - Upload CSV:**
- Formato esperado: `nome;celular;presente`
- Exemplo de arquivo:
```
João Silva;5511999999999;Livro
Maria Santos;5511888888888;Boneca
```

**Opção B - Colar Texto:**
- Cole os dados no mesmo formato, uma linha por contato
- Exemplo:
```
João Silva;5511999999999;Livro
Maria Santos;5511888888888;Boneca
```

### 4. Edite e Configure
- Edite qualquer campo diretamente na tabela
- Marque "Ignorar amigo" para não enviar SMS a um contato específico
- Exclua contatos indesejados com o botão de lixeira

### 5. Valide e Visualize
- **Validar Apenas**: Verifica se todos os dados estão corretos
- **Validar + Preview**: Mostra exatamente como cada mensagem será enviada

### 6. Envie os SMS
- Clique em "Enviar SMS" (habilitado apenas após validação)
- Acompanhe o progresso em tempo real
- Visualize o resultado de cada envio

## 📁 Estrutura do Projeto

```
AmigoSecreto/
├── Models/
│   ├── SmsRecipient.cs          # Modelo de destinatário
│   ├── SmsPreview.cs            # Modelo de preview
│   └── ComteleApiResponse.cs    # Modelos de resposta da API
├── Services/
│   └── ComteleSmsService.cs     # Serviço de integração com Comtele
├── Pages/
│   ├── Index.cshtml             # Página principal
│   ├── Index.cshtml.cs          # Code-behind da página
│   └── _ViewImports.cshtml      # Imports globais
├── wwwroot/
│   ├── css/
│   │   └── dark-theme.css       # Tema dark futurista
│   └── js/
│       └── site.js              # JavaScript principal
├── Program.cs                   # Configuração da aplicação
└── appsettings.json            # Configurações

```

## 🔐 Segurança

- API Key não é persistida (apenas em memória)
- Nenhum dado sensível é armazenado
- Sistema stateless (sem banco de dados)
- IDs temporários apenas para controle de UI

## 🔌 API da Comtele

Este projeto integra com a API da Comtele para envio de SMS.

**Documentação oficial**: https://docs.comtele.com.br/

**Configuração**:
- O endpoint da API pode ser ajustado em `appsettings.json`
- Por padrão: `https://api.comtele.com.br/v1/sms`

## 🎯 Formato do CSV

O arquivo CSV deve seguir este formato:

```csv
nome;celular;presente
João Silva;5511999999999;Livro
Maria Santos;5511888888888;Boneca
Pedro Costa;5511777777777;Caneca
```

**Observações**:
- Separador: ponto e vírgula (`;`)
- Celular: incluir código do país e DDD (ex: 5511999999999)
- Primeira linha pode ser cabeçalho (será ignorada)

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

## 🔄 Melhorias Futuras Sugeridas

1. **Agendamento**: Agendar envios para data/hora específica
2. **Templates**: Salvar templates de mensagens (localStorage)
3. **Histórico**: Histórico de envios (localStorage)
4. **Importação Excel**: Suporte a arquivos .xlsx
5. **Grupos**: Organizar contatos em grupos
6. **Webhooks**: Receber confirmações de entrega
7. **Multi-idioma**: Suporte a outros idiomas
8. **Dark/Light Toggle**: Alternar entre temas

## 📝 Licença

Este projeto é de código aberto e está disponível sob a licença MIT.

## 👨‍💻 Autor

Desenvolvido para facilitar o envio de SMS personalizados para eventos de Amigo Secreto e outras ocasiões especiais.

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou pull requests.

---

**Nota**: Este sistema é uma demonstração/exemplo. Para uso em produção, considere adicionar:
- Autenticação de usuários
- Rate limiting
- Logging mais robusto
- Tratamento de erros mais detalhado
- Testes automatizados