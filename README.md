# ðŸ”§ PAM_MS_Order

[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=flat-square&logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=flat-square)](LICENSE)

## ðŸ“‹ Sobre

MicroserviÃ§o para gerenciamento de pedidos e agendamentos. Controla criaÃ§Ã£o, status, histÃ³rico e lifecycle completo dos pedidos de manutenÃ§Ã£o na plataforma.

Este microserviÃ§o faz parte da **PAM (Plataforma de Agendamento de ManutenÃ§Ã£o)**, uma soluÃ§Ã£o completa para gerenciamento de serviÃ§os de manutenÃ§Ã£o.

## ðŸ—ï¸ Arquitetura

`
PAM_MS_Order/
â”œâ”€â”€ Model/           # Modelos de domÃ­nio e DTOs
â”œâ”€â”€ Repository/      # Camada de acesso a dados
â”œâ”€â”€ Service/         # LÃ³gica de negÃ³cio
â”œâ”€â”€ WebApi/          # Controllers e configuraÃ§Ãµes da API
â”œâ”€â”€ Dockerfile       # ConfiguraÃ§Ã£o do container
â””â”€â”€ README.md        # Este arquivo
`

## ðŸš€ Tecnologias

- **.NET 6.0** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **Swagger/OpenAPI** - DocumentaÃ§Ã£o da API
- **Serilog** - Logging estruturado
- **FluentValidation** - ValidaÃ§Ã£o de dados
- **JWT Bearer** - AutenticaÃ§Ã£o
- **Docker** - ContainerizaÃ§Ã£o

## ðŸ“¦ PrÃ©-requisitos

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/) (opcional)
- [SQL Server](https://www.microsoft.com/sql-server) ou [PostgreSQL](https://www.postgresql.org/)

## ðŸ”§ InstalaÃ§Ã£o e ExecuÃ§Ã£o

### Desenvolvimento Local

1. **Clone o repositÃ³rio**
   `ash
   git clone https://github.com/EmmanuelSMenezes/PAM_MS_Order.git
   cd PAM_MS_Order
   `

2. **Restaure as dependÃªncias**
   `ash
   dotnet restore
   `

3. **Configure a string de conexÃ£o**
   `ash
   # Edite o arquivo WebApi/appsettings.Development.json
   # Configure a ConnectionString para seu banco de dados
   `

4. **Execute as migraÃ§Ãµes** (se aplicÃ¡vel)
   `ash
   dotnet ef database update
   `

5. **Execute o projeto**
   `ash
   cd WebApi
   dotnet run
   `

6. **Acesse a API**
   - API: http://localhost:5005
   - Swagger: http://localhost:5005/swagger

### Docker

1. **Build da imagem**
   `ash
   docker build -t PAM_MS_Order.ToLower() .
   `

2. **Execute o container**
   `ash
   docker run -p 5005:5005 -e ConnectionStrings__DefaultConnection="sua-connection-string" PAM_MS_Order.ToLower()
   `

## ðŸ“š DocumentaÃ§Ã£o da API

A documentaÃ§Ã£o completa da API estÃ¡ disponÃ­vel via Swagger UI:
- **Desenvolvimento**: http://localhost:5005/swagger
- **ProduÃ§Ã£o**: https://api.pam.com/PAM_MS_Order.ToLower()/swagger

## ðŸ”’ AutenticaÃ§Ã£o

Este microserviÃ§o utiliza **JWT Bearer Token** para autenticaÃ§Ã£o. 

### Como usar:
1. Obtenha um token do serviÃ§o de autenticaÃ§Ã£o
2. Inclua o token no header das requisiÃ§Ãµes:
   `
   Authorization: Bearer {seu-token}
   `

## ðŸ§ª Testes

`ash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
`

## ðŸ“Š Monitoramento

- **Health Check**: /health
- **Metrics**: /metrics
- **Logs**: Estruturados via Serilog

## ðŸŒ VariÃ¡veis de Ambiente

| VariÃ¡vel | DescriÃ§Ã£o | PadrÃ£o |
|----------|-----------|---------|
| ConnectionStrings__DefaultConnection | String de conexÃ£o do banco | - |
| JwtSettings__SecretKey | Chave secreta JWT | - |
| JwtSettings__Issuer | Emissor do token | PAM |
| JwtSettings__Audience | AudiÃªncia do token | PAM-API |

## ðŸ¤ ContribuiÃ§Ã£o

1. Fork o projeto
2. Crie uma branch para sua feature (git checkout -b feature/AmazingFeature)
3. Commit suas mudanÃ§as (git commit -m 'Add some AmazingFeature')
4. Push para a branch (git push origin feature/AmazingFeature)
5. Abra um Pull Request

## ðŸ“„ LicenÃ§a

Este projeto estÃ¡ sob a licenÃ§a MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## ðŸ†˜ Suporte

- ðŸ“§ Email: suporte@pam.com
- ðŸ“± WhatsApp: +55 (11) 99999-9999
- ðŸ› Issues: [GitHub Issues](https://github.com/EmmanuelSMenezes/PAM_MS_Order/issues)

---

<div align="center">
  <strong>PAM - Plataforma de Agendamento de ManutenÃ§Ã£o</strong><br>
  Desenvolvido com â¤ï¸ pela equipe PAM
</div>
