# PAM_MS_Order

<div align="center">

[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker)](https://www.docker.com/)
[![Swagger](https://img.shields.io/badge/Swagger-API%20Docs-85EA2D?style=for-the-badge&logo=swagger)](https://swagger.io/)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](LICENSE)

**Microservico de alta performance para a Plataforma PAM**

[Demo](#demo) â€¢ [Documentacao](#documentacao) â€¢ [Instalacao](#instalacao) â€¢ [Contribuicao](#contribuicao)

</div>

---

## Sobre o Projeto

Microservico para gerenciamento de pedidos e agendamentos. Controla criacao, status, historico, workflow completo, cancelamentos, reagendamentos, SLA, notificacoes e lifecycle completo dos pedidos de manutencao na plataforma.

### Principais Funcionalidades

- **Gestao de Pedidos**: CRUD e workflow completo
- **Agendamentos**: Sistema de scheduling avancado
- **Status**: Rastreamento em tempo real
- **Cancelamentos**: Politicas e reembolsos
- **Reagendamentos**: Flexibilidade para clientes
- **SLA**: Monitoramento de prazos
- **Relatorios**: Analytics de pedidos
- **Notificacoes**: Alertas automaticos

### Arquitetura Clean Architecture

`
PAM_MS_Order/
â”œâ”€â”€ Model/              # Dominio e Entidades
â”‚   â”œâ”€â”€ Entities/       # Entidades de dominio
â”‚   â”œâ”€â”€ DTOs/           # Data Transfer Objects
â”‚   â”œâ”€â”€ Enums/          # Enumeracoes
â”‚   â””â”€â”€ Validators/     # Validacoes de dominio
â”œâ”€â”€ Repository/         # Camada de Dados
â”‚   â”œâ”€â”€ Interfaces/     # Contratos de repositorio
â”‚   â”œâ”€â”€ Implementations/# Implementacoes concretas
â”‚   â”œâ”€â”€ Context/        # Contexto do Entity Framework
â”‚   â””â”€â”€ Migrations/     # Migracoes do banco
â”œâ”€â”€ Service/            # Logica de Negocio
â”‚   â”œâ”€â”€ Interfaces/     # Contratos de servicos
â”‚   â”œâ”€â”€ Implementations/# Implementacoes de servicos
â”‚   â”œâ”€â”€ Mappers/        # Mapeamento de objetos
â”‚   â””â”€â”€ Validators/     # Validacoes de negocio
â”œâ”€â”€ WebApi/             # Camada de Apresentacao
â”‚   â”œâ”€â”€ Controllers/    # Controladores da API
â”‚   â”œâ”€â”€ Middlewares/    # Middlewares customizados
â”‚   â”œâ”€â”€ Filters/        # Filtros de acao
â”‚   â””â”€â”€ Configuration/  # Configuracoes da API
â”œâ”€â”€ Dockerfile          # Containerizacao
â”œâ”€â”€ docker-compose.yml  # Orquestracao local
â””â”€â”€ README.md           # Este arquivo
`

## Tecnologias e Ferramentas

### Core Framework
- **[.NET 6.0](https://dotnet.microsoft.com/)** - Framework principal
- **[ASP.NET Core](https://docs.microsoft.com/aspnet/core/)** - Web API framework
- **[Entity Framework Core](https://docs.microsoft.com/ef/core/)** - ORM para acesso a dados

### Documentacao e Testes
- **[Swagger/OpenAPI](https://swagger.io/)** - Documentacao interativa da API
- **[FluentValidation](https://fluentvalidation.net/)** - Validacao de dados
- **[AutoMapper](https://automapper.org/)** - Mapeamento de objetos
- **[xUnit](https://xunit.net/)** - Framework de testes

### Seguranca e Monitoramento
- **[JWT Bearer](https://jwt.io/)** - Autenticacao e autorizacao
- **[Serilog](https://serilog.net/)** - Logging estruturado
- **[HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)** - Monitoramento de saude

### DevOps e Infraestrutura
- **[Docker](https://www.docker.com/)** - Containerizacao
- **[Azure DevOps](https://azure.microsoft.com/services/devops/)** - CI/CD Pipeline
- **[SQL Server](https://www.microsoft.com/sql-server)** - Banco de dados principal

## Pre-requisitos

Antes de comecar, certifique-se de ter instalado:

- **[.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)** (versao 6.0 ou superior)
- **[Docker Desktop](https://www.docker.com/products/docker-desktop)** (opcional, para containerizacao)
- **[SQL Server](https://www.microsoft.com/sql-server)** ou **[PostgreSQL](https://www.postgresql.org/)** (banco de dados)
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** ou **[VS Code](https://code.visualstudio.com/)** (IDE recomendada)

## Instalacao e Configuracao

### 1. Clone o Repositorio

`ash
git clone https://github.com/EmmanuelSMenezes/PAM_MS_Order.git
cd PAM_MS_Order
`

### 2. Configuracao do Ambiente

`ash
# Copie o arquivo de configuracao
cp WebApi/appsettings.example.json WebApi/appsettings.Development.json

# Configure suas variaveis de ambiente
# Edite o arquivo WebApi/appsettings.Development.json
`

### 3. Configuracao do Banco de Dados

`json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PAM_PAM_MS_Order;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
`

### 4. Restaurar Dependencias

`ash
dotnet restore
`

### 5. Executar Migracoes

`ash
# Navegar para o projeto WebApi
cd WebApi

# Executar migracoes
dotnet ef database update

# Voltar para a raiz
cd ..
`

### 6. Executar o Projeto

`ash
# Desenvolvimento
cd WebApi
dotnet run

# Ou usando o Visual Studio
# Abra o arquivo .sln e pressione F5
`

### 7. Verificar Instalacao

Acesse os seguintes endpoints para verificar se tudo esta funcionando:

- **API**: http://localhost:5005
- **Swagger UI**: http://localhost:5005/swagger
- **Health Check**: http://localhost:5005/health

## Docker

### Executar com Docker

`ash
# Build da imagem
docker build -t pam_ms_order .

# Executar container
docker run -p 5005:5005 \
  -e ConnectionStrings__DefaultConnection="sua-connection-string" \
  -e JwtSettings__SecretKey="sua-chave-secreta" \
  pam_ms_order
`

### Docker Compose (Desenvolvimento)

`ash
# Subir todos os servicos
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar servicos
docker-compose down
`

## Documentacao da API

### Swagger UI
A documentacao completa e interativa esta disponivel em:
- **Local**: http://localhost:5005/swagger
- **Producao**: https://api.pam.com/pam_ms_order/swagger

### Principais Endpoints

| Metodo | Endpoint | Descricao | Auth |
|--------|----------|-----------|------|
| GET | /health | Status de saude do servico | Nao |
| GET | /api/v1/... | Listar recursos | Sim |
| POST | /api/v1/... | Criar novo recurso | Sim |
| PUT | /api/v1/.../{{id}} | Atualizar recurso | Sim |
| DELETE | /api/v1/.../{{id}} | Remover recurso | Sim |

## Autenticacao e Seguranca

### JWT Bearer Token
Este microservico utiliza **JWT (JSON Web Token)** para autenticacao:

1. **Obter Token**: Faca login no endpoint /api/v1/auth/login
2. **Usar Token**: Inclua o token no header Authorization: Bearer {token}
3. **Renovar Token**: Use o endpoint /api/v1/auth/refresh

### Configuracao de Seguranca

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

## Testes

### Executar Testes

`ash
# Todos os testes
dotnet test

# Testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Testes especificos
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
`

### Estrutura de Testes

`
Tests/
â”œâ”€â”€ Unit/              # Testes unitarios
â”œâ”€â”€ Integration/       # Testes de integracao
â”œâ”€â”€ Fixtures/          # Dados de teste
â””â”€â”€ Helpers/           # Utilitarios de teste
`

## Contribuicao

Contribuicoes sao sempre bem-vindas! Siga estas etapas:

### 1. Fork o Projeto
`ash
git clone https://github.com/EmmanuelSMenezes/PAM_MS_Order.git
`

### 2. Criar Branch
`ash
git checkout -b feature/nova-funcionalidade
`

### 3. Commit das Mudancas
`ash
git commit -m "feat: adiciona nova funcionalidade incrivel"
`

### 4. Push para Branch
`ash
git push origin feature/nova-funcionalidade
`

### 5. Abrir Pull Request
Abra um PR descrevendo suas mudancas detalhadamente.

### Padroes de Commit
Seguimos o padrao [Conventional Commits](https://www.conventionalcommits.org/):

- eat: Nova funcionalidade
- ix: Correcao de bug
- docs: Documentacao
- style: Formatacao
- efactor: Refatoracao
- 	est: Testes
- chore: Manutencao

## Licenca

Este projeto esta sob a licenca **MIT**. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## Suporte e Contato

### Canais de Suporte
- **Email**: suporte@pam.com
- **WhatsApp**: +55 (11) 99999-9999
- **Issues**: [GitHub Issues](https://github.com/EmmanuelSMenezes/PAM_MS_Order/issues)
- **Wiki**: [Documentacao Completa](https://github.com/EmmanuelSMenezes/PAM_MS_Order/wiki)

### Equipe de Desenvolvimento
- **Tech Lead**: Emmanuel Menezes
- **Backend**: Equipe PAM
- **DevOps**: Equipe PAM

---

<div align="center">

**PAM - Plataforma de Agendamento de Manutencao**  
*Desenvolvido com amor pela equipe PAM*

[![GitHub](https://img.shields.io/badge/GitHub-PAM-181717?style=for-the-badge&logo=github)](https://github.com/EmmanuelSMenezes)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-PAM-0077B5?style=for-the-badge&logo=linkedin)](https://linkedin.com/company/pam)

</div>