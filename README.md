# ðŸ”§ PAM_MS_Order

<div align="center">

[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker)](https://www.docker.com/)
[![Swagger](https://img.shields.io/badge/Swagger-API%20Docs-85EA2D?style=for-the-badge&logo=swagger)](https://swagger.io/)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](LICENSE)

**MicroserviÃ§o de alta performance para a Plataforma PAM**

[ðŸš€ Demo](#-demo) â€¢ [ðŸ“– DocumentaÃ§Ã£o](#-documentaÃ§Ã£o) â€¢ [ðŸ› ï¸ InstalaÃ§Ã£o](#ï¸-instalaÃ§Ã£o) â€¢ [ðŸ¤ ContribuiÃ§Ã£o](#-contribuiÃ§Ã£o)

</div>

---

## ðŸ“‹ Sobre o Projeto

MicroserviÃ§o para **gerenciamento de pedidos e agendamentos**. Controla criaÃ§Ã£o, status, histÃ³rico, workflow completo, cancelamentos, reagendamentos, SLA, notificaÃ§Ãµes e lifecycle completo dos pedidos de manutenÃ§Ã£o na plataforma.

### ðŸŽ¯ Principais Funcionalidades

- ðŸ“‹ **GestÃ£o de Pedidos**: CRUD e workflow completo
- ðŸ“… **Agendamentos**: Sistema de scheduling avanÃ§ado
- ðŸ”„ **Status**: Rastreamento em tempo real
- âŒ **Cancelamentos**: PolÃ­ticas e reembolsos
- ðŸ”„ **Reagendamentos**: Flexibilidade para clientes
- â±ï¸ **SLA**: Monitoramento de prazos
- ðŸ“Š **RelatÃ³rios**: Analytics de pedidos
- ðŸ”” **NotificaÃ§Ãµes**: Alertas automÃ¡ticos

### ðŸ—ï¸ Arquitetura Clean Architecture

`
PAM_MS_Order/
â”œâ”€â”€ ðŸ“ Model/              # ðŸŽ¯ DomÃ­nio e Entidades
â”‚   â”œâ”€â”€ Entities/          # Entidades de domÃ­nio
â”‚   â”œâ”€â”€ DTOs/              # Data Transfer Objects
â”‚   â”œâ”€â”€ Enums/             # EnumeraÃ§Ãµes
â”‚   â””â”€â”€ Validators/        # ValidaÃ§Ãµes de domÃ­nio
â”œâ”€â”€ ðŸ“ Repository/         # ðŸ—„ï¸ Camada de Dados
â”‚   â”œâ”€â”€ Interfaces/        # Contratos de repositÃ³rio
â”‚   â”œâ”€â”€ Implementations/   # ImplementaÃ§Ãµes concretas
â”‚   â”œâ”€â”€ Context/           # Contexto do Entity Framework
â”‚   â””â”€â”€ Migrations/        # MigraÃ§Ãµes do banco
â”œâ”€â”€ ðŸ“ Service/            # ðŸ”§ LÃ³gica de NegÃ³cio
â”‚   â”œâ”€â”€ Interfaces/        # Contratos de serviÃ§os
â”‚   â”œâ”€â”€ Implementations/   # ImplementaÃ§Ãµes de serviÃ§os
â”‚   â”œâ”€â”€ Mappers/           # Mapeamento de objetos
â”‚   â””â”€â”€ Validators/        # ValidaÃ§Ãµes de negÃ³cio
â”œâ”€â”€ ðŸ“ WebApi/             # ðŸŒ Camada de ApresentaÃ§Ã£o
â”‚   â”œâ”€â”€ Controllers/       # Controladores da API
â”‚   â”œâ”€â”€ Middlewares/       # Middlewares customizados
â”‚   â”œâ”€â”€ Filters/           # Filtros de aÃ§Ã£o
â”‚   â””â”€â”€ Configuration/     # ConfiguraÃ§Ãµes da API
â”œâ”€â”€ ðŸ“„ Dockerfile          # ðŸ³ ContainerizaÃ§Ã£o
â”œâ”€â”€ ðŸ“„ docker-compose.yml  # ðŸ³ OrquestraÃ§Ã£o local
â””â”€â”€ ðŸ“„ README.md           # ðŸ“– Este arquivo
`

## ðŸš€ Tecnologias e Ferramentas

### Core Framework
- **[.NET 6.0](https://dotnet.microsoft.com/)** - Framework principal
- **[ASP.NET Core](https://docs.microsoft.com/aspnet/core/)** - Web API framework
- **[Entity Framework Core](https://docs.microsoft.com/ef/core/)** - ORM para acesso a dados

### DocumentaÃ§Ã£o e Testes
- **[Swagger/OpenAPI](https://swagger.io/)** - DocumentaÃ§Ã£o interativa da API
- **[FluentValidation](https://fluentvalidation.net/)** - ValidaÃ§Ã£o de dados
- **[AutoMapper](https://automapper.org/)** - Mapeamento de objetos
- **[xUnit](https://xunit.net/)** - Framework de testes

### SeguranÃ§a e Monitoramento
- **[JWT Bearer](https://jwt.io/)** - AutenticaÃ§Ã£o e autorizaÃ§Ã£o
- **[Serilog](https://serilog.net/)** - Logging estruturado
- **[HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)** - Monitoramento de saÃºde

### DevOps e Infraestrutura
- **[Docker](https://www.docker.com/)** - ContainerizaÃ§Ã£o
- **[Azure DevOps](https://azure.microsoft.com/services/devops/)** - CI/CD Pipeline
- **[SQL Server](https://www.microsoft.com/sql-server)** - Banco de dados principal

## ðŸ“¦ PrÃ©-requisitos

Antes de comeÃ§ar, certifique-se de ter instalado:

- **[.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)** (versÃ£o 6.0 ou superior)
- **[Docker Desktop](https://www.docker.com/products/docker-desktop)** (opcional, para containerizaÃ§Ã£o)
- **[SQL Server](https://www.microsoft.com/sql-server)** ou **[PostgreSQL](https://www.postgresql.org/)** (banco de dados)
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** ou **[VS Code](https://code.visualstudio.com/)** (IDE recomendada)

## ðŸ› ï¸ InstalaÃ§Ã£o e ConfiguraÃ§Ã£o

### 1ï¸âƒ£ Clone o RepositÃ³rio

`ash
git clone https://github.com/EmmanuelSMenezes/PAM_MS_Order.git
cd PAM_MS_Order
`

### 2ï¸âƒ£ ConfiguraÃ§Ã£o do Ambiente

`ash
# Copie o arquivo de configuraÃ§Ã£o
cp WebApi/appsettings.example.json WebApi/appsettings.Development.json

# Configure suas variÃ¡veis de ambiente
# Edite o arquivo WebApi/appsettings.Development.json
`

### 3ï¸âƒ£ ConfiguraÃ§Ã£o do Banco de Dados

`json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PAM_PAM_MS_Order;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
`

### 4ï¸âƒ£ Restaurar DependÃªncias

`ash
dotnet restore
`

### 5ï¸âƒ£ Executar MigraÃ§Ãµes

`ash
# Navegar para o projeto WebApi
cd WebApi

# Executar migraÃ§Ãµes
dotnet ef database update

# Voltar para a raiz
cd ..
`

### 6ï¸âƒ£ Executar o Projeto

`ash
# Desenvolvimento
cd WebApi
dotnet run

# Ou usando o Visual Studio
# Abra o arquivo .sln e pressione F5
`

### 7ï¸âƒ£ Verificar InstalaÃ§Ã£o

Acesse os seguintes endpoints para verificar se tudo estÃ¡ funcionando:

- **API**: http://localhost:5005
- **Swagger UI**: http://localhost:5005/swagger
- **Health Check**: http://localhost:5005/health

## ðŸ³ Docker

### Executar com Docker

`ash
# Build da imagem
docker build -t PAM_MS_Order.ToLower() .

# Executar container
docker run -p 5005:5005 \
  -e ConnectionStrings__DefaultConnection="sua-connection-string" \
  -e JwtSettings__SecretKey="sua-chave-secreta" \
  PAM_MS_Order.ToLower()
`

### Docker Compose (Desenvolvimento)

`ash
# Subir todos os serviÃ§os
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar serviÃ§os
docker-compose down
`

## ðŸ“š DocumentaÃ§Ã£o da API

### Swagger UI
A documentaÃ§Ã£o completa e interativa estÃ¡ disponÃ­vel em:
- **Local**: http://localhost:5005/swagger
- **ProduÃ§Ã£o**: https://api.pam.com/PAM_MS_Order.ToLower()/swagger

### Principais Endpoints

| MÃ©todo | Endpoint | DescriÃ§Ã£o | Auth |
|--------|----------|-----------|------|
| GET | /health | Status de saÃºde do serviÃ§o | âŒ |
| GET | /api/v1/... | Listar recursos | âœ… |
| POST | /api/v1/... | Criar novo recurso | âœ… |
| PUT | /api/v1/.../{{id}} | Atualizar recurso | âœ… |
| DELETE | /api/v1/.../{{id}} | Remover recurso | âœ… |

### Exemplos de Uso

`ash
# Obter token de autenticaÃ§Ã£o
curl -X POST "http://localhost:5005/api/v1/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "password123"
  }'

# Usar token nas requisiÃ§Ãµes
curl -X GET "http://localhost:5005/api/v1/..." \
  -H "Authorization: Bearer {seu-token-jwt}"
`

## ðŸ”’ AutenticaÃ§Ã£o e SeguranÃ§a

### JWT Bearer Token
Este microserviÃ§o utiliza **JWT (JSON Web Token)** para autenticaÃ§Ã£o:

1. **Obter Token**: FaÃ§a login no endpoint /api/v1/auth/login
2. **Usar Token**: Inclua o token no header Authorization: Bearer {token}
3. **Renovar Token**: Use o endpoint /api/v1/auth/refresh

### ConfiguraÃ§Ã£o de SeguranÃ§a

`json
{
  "JwtSettings": {
    "SecretKey": "sua-chave-super-secreta-com-pelo-menos-32-caracteres",
    "Issuer": "PAM-API",
    "Audience": "PAM-Client",
    "ExpirationInMinutes": 60
  }
}
`

## ðŸ§ª Testes

### Executar Testes

`ash
# Todos os testes
dotnet test

# Testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Testes especÃ­ficos
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
`

### Estrutura de Testes

`
Tests/
â”œâ”€â”€ Unit/              # Testes unitÃ¡rios
â”œâ”€â”€ Integration/       # Testes de integraÃ§Ã£o
â”œâ”€â”€ Fixtures/          # Dados de teste
â””â”€â”€ Helpers/           # UtilitÃ¡rios de teste
`

## ðŸ“Š Monitoramento e Observabilidade

### Health Checks
- **Endpoint**: /health
- **Detailed**: /health/detailed

### Logging
Logs estruturados com **Serilog**:
- **Console**: Desenvolvimento
- **File**: ProduÃ§Ã£o
- **SQL Server**: Logs de auditoria

### MÃ©tricas
- **Performance**: Tempo de resposta
- **Disponibilidade**: Uptime
- **Erros**: Taxa de erro

## ðŸŒ VariÃ¡veis de Ambiente

| VariÃ¡vel | DescriÃ§Ã£o | Exemplo | ObrigatÃ³rio |
|----------|-----------|---------|-------------|
| ConnectionStrings__DefaultConnection | String de conexÃ£o do banco | Server=... | âœ… |
| JwtSettings__SecretKey | Chave secreta JWT | sua-chave-secreta | âœ… |
| JwtSettings__Issuer | Emissor do token | PAM-API | âœ… |
| JwtSettings__Audience | AudiÃªncia do token | PAM-Client | âœ… |
| Serilog__MinimumLevel | NÃ­vel mÃ­nimo de log | Information | âŒ |

## ðŸš€ Deploy

### Azure App Service

`ash
# Publicar para Azure
dotnet publish -c Release -o ./publish
az webapp deploy --resource-group PAM --name PAM_MS_Order --src-path ./publish
`

### Docker Registry

`ash
# Tag da imagem
docker tag PAM_MS_Order.ToLower() registry.azurecr.io/pam/PAM_MS_Order.ToLower():latest

# Push para registry
docker push registry.azurecr.io/pam/PAM_MS_Order.ToLower():latest
`

## ðŸ¤ ContribuiÃ§Ã£o

ContribuiÃ§Ãµes sÃ£o sempre bem-vindas! Siga estas etapas:

### 1ï¸âƒ£ Fork o Projeto
`ash
# Fork via GitHub ou
git clone https://github.com/EmmanuelSMenezes/PAM_MS_Order.git
`

### 2ï¸âƒ£ Criar Branch
`ash
git checkout -b feature/nova-funcionalidade
`

### 3ï¸âƒ£ Commit das MudanÃ§as
`ash
git commit -m "feat: adiciona nova funcionalidade incrÃ­vel"
`

### 4ï¸âƒ£ Push para Branch
`ash
git push origin feature/nova-funcionalidade
`

### 5ï¸âƒ£ Abrir Pull Request
Abra um PR descrevendo suas mudanÃ§as detalhadamente.

### ðŸ“ PadrÃµes de Commit
Seguimos o padrÃ£o [Conventional Commits](https://www.conventionalcommits.org/):

- eat: Nova funcionalidade
- ix: CorreÃ§Ã£o de bug
- docs: DocumentaÃ§Ã£o
- style: FormataÃ§Ã£o
- efactor: RefatoraÃ§Ã£o
- 	est: Testes
- chore: ManutenÃ§Ã£o

## ðŸ“„ LicenÃ§a

Este projeto estÃ¡ sob a licenÃ§a **MIT**. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## ðŸ†˜ Suporte e Contato

### ðŸ“ž Canais de Suporte
- **ðŸ“§ Email**: suporte@pam.com
- **ðŸ’¬ WhatsApp**: +55 (11) 99999-9999
- **ðŸ› Issues**: [GitHub Issues](https://github.com/EmmanuelSMenezes/PAM_MS_Order/issues)
- **ðŸ“– Wiki**: [DocumentaÃ§Ã£o Completa](https://github.com/EmmanuelSMenezes/PAM_MS_Order/wiki)

### ðŸ‘¥ Equipe de Desenvolvimento
- **Tech Lead**: Emmanuel Menezes
- **Backend**: Equipe PAM
- **DevOps**: Equipe PAM

---

<div align="center">

**[â¬† Voltar ao Topo](#-PAM_MS_Order)**

**PAM - Plataforma de Agendamento de ManutenÃ§Ã£o**  
*Desenvolvido com â¤ï¸ pela equipe PAM*

[![GitHub](https://img.shields.io/badge/GitHub-PAM-181717?style=for-the-badge&logo=github)](https://github.com/EmmanuelSMenezes)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-PAM-0077B5?style=for-the-badge&logo=linkedin)](https://linkedin.com/company/pam)

</div>
