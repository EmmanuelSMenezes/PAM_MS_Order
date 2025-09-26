# 🔧 PAM_MS_Order

<div align="center">

[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker)](https://www.docker.com/)
[![Swagger](https://img.shields.io/badge/Swagger-API%20Docs-85EA2D?style=for-the-badge&logo=swagger)](https://swagger.io/)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](LICENSE)

**Microserviço de alta performance para a Plataforma PAM**

[🚀 Demo](#demo) • [📖 Documentação](#documentacao) • [🛠️ Instalação](#instalacao) • [🤝 Contribuição](#contribuicao)

</div>

---

## 📋 Sobre o Projeto

Microserviço para **gerenciamento de pedidos e agendamentos**. Controla criação, status, histórico, workflow completo, cancelamentos, reagendamentos, SLA, notificações e lifecycle completo dos pedidos de manutenção na plataforma.

### 🎯 Principais Funcionalidades

- 📋 **Gestão de Pedidos**: CRUD e workflow completo
- 📅 **Agendamentos**: Sistema de scheduling avançado
- 🔄 **Status**: Rastreamento em tempo real
- ❌ **Cancelamentos**: Políticas e reembolsos
- 🔄 **Reagendamentos**: Flexibilidade para clientes
- ⏱️ **SLA**: Monitoramento de prazos
- 📊 **Relatórios**: Analytics de pedidos
- 🔔 **Notificações**: Alertas automáticos

### 🏗️ Arquitetura Clean Architecture

```
PAM_MS_Order/
├── 📁 Model/              # 🎯 Domínio e Entidades
│   ├── Entities/          # Entidades de domínio
│   ├── DTOs/              # Data Transfer Objects
│   ├── Enums/             # Enumerações
│   └── Validators/        # Validações de domínio
├── 📁 Repository/         # 🗄️ Camada de Dados
│   ├── Interfaces/        # Contratos de repositório
│   ├── Implementations/   # Implementações concretas
│   ├── Context/           # Contexto do Entity Framework
│   └── Migrations/        # Migrações do banco
├── 📁 Service/            # 🔧 Lógica de Negócio
│   ├── Interfaces/        # Contratos de serviços
│   ├── Implementations/   # Implementações de serviços
│   ├── Mappers/           # Mapeamento de objetos
│   └── Validators/        # Validações de negócio
├── 📁 WebApi/             # 🌐 Camada de Apresentação
│   ├── Controllers/       # Controladores da API
│   ├── Middlewares/       # Middlewares customizados
│   ├── Filters/           # Filtros de ação
│   └── Configuration/     # Configurações da API
├── 📄 Dockerfile          # 🐳 Containerização
├── 📄 docker-compose.yml  # 🐳 Orquestração local
└── 📄 README.md           # 📖 Este arquivo
```

## 🚀 Tecnologias e Ferramentas

### Core Framework
- **[.NET 6.0](https://dotnet.microsoft.com/)** - Framework principal
- **[ASP.NET Core](https://docs.microsoft.com/aspnet/core/)** - Web API framework
- **[Entity Framework Core](https://docs.microsoft.com/ef/core/)** - ORM para acesso a dados

### Documentação e Testes
- **[Swagger/OpenAPI](https://swagger.io/)** - Documentação interativa da API
- **[FluentValidation](https://fluentvalidation.net/)** - Validação de dados
- **[AutoMapper](https://automapper.org/)** - Mapeamento de objetos
- **[xUnit](https://xunit.net/)** - Framework de testes

### Segurança e Monitoramento
- **[JWT Bearer](https://jwt.io/)** - Autenticação e autorização
- **[Serilog](https://serilog.net/)** - Logging estruturado
- **[HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)** - Monitoramento de saúde

### DevOps e Infraestrutura
- **[Docker](https://www.docker.com/)** - Containerização
- **[Azure DevOps](https://azure.microsoft.com/services/devops/)** - CI/CD Pipeline
- **[SQL Server](https://www.microsoft.com/sql-server)** - Banco de dados principal

## 📦 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- **[.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)** (versão 6.0 ou superior)
- **[Docker Desktop](https://www.docker.com/products/docker-desktop)** (opcional, para containerização)
- **[SQL Server](https://www.microsoft.com/sql-server)** ou **[PostgreSQL](https://www.postgresql.org/)** (banco de dados)
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** ou **[VS Code](https://code.visualstudio.com/)** (IDE recomendada)

## 🛠️ Instalação e Configuração

### 1️⃣ Clone o Repositório

```bash
git clone https://github.com/EmmanuelSMenezes/PAM_MS_Order.git
cd PAM_MS_Order
```

### 2️⃣ Configuração do Ambiente

```bash
# Copie o arquivo de configuração
cp WebApi/appsettings.example.json WebApi/appsettings.Development.json

# Configure suas variáveis de ambiente
# Edite o arquivo WebApi/appsettings.Development.json
```

### 3️⃣ Configuração do Banco de Dados

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PAM_PAM_MS_Order;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 4️⃣ Restaurar Dependências

```bash
dotnet restore
```

### 5️⃣ Executar Migrações

```bash
# Navegar para o projeto WebApi
cd WebApi

# Executar migrações
dotnet ef database update

# Voltar para a raiz
cd ..
```

### 6️⃣ Executar o Projeto

```bash
# Desenvolvimento
cd WebApi
dotnet run

# Ou usando o Visual Studio
# Abra o arquivo .sln e pressione F5
```

### 7️⃣ Verificar Instalação

Acesse os seguintes endpoints para verificar se tudo está funcionando:

- **API**: `http://localhost:5005`
- **Swagger UI**: `http://localhost:5005/swagger`
- **Health Check**: `http://localhost:5005/health`

## 🐳 Docker

### Executar com Docker

```bash
# Build da imagem
docker build -t pam_ms_order .

# Executar container
docker run -p 5005:5005 \
  -e ConnectionStrings__DefaultConnection="sua-connection-string" \
  -e JwtSettings__SecretKey="sua-chave-secreta" \
  pam_ms_order
```

### Docker Compose (Desenvolvimento)

```bash
# Subir todos os serviços
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down
```

## 📚 Documentação da API

### Swagger UI
A documentação completa e interativa está disponível em:
- **Local**: `http://localhost:5005/swagger`
- **Produção**: `https://api.pam.com/pam_ms_order/swagger`

### Principais Endpoints

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/health` | Status de saúde do serviço | ❌ |
| GET | `/api/v1/...` | Listar recursos | ✅ |
| POST | `/api/v1/...` | Criar novo recurso | ✅ |
| PUT | `/api/v1/.../{{id}}` | Atualizar recurso | ✅ |
| DELETE | `/api/v1/.../{{id}}` | Remover recurso | ✅ |

## 🔒 Autenticação e Segurança

### JWT Bearer Token
Este microserviço utiliza **JWT (JSON Web Token)** para autenticação:

1. **Obter Token**: Faça login no endpoint `/api/v1/auth/login`
2. **Usar Token**: Inclua o token no header `Authorization: Bearer {token}`
3. **Renovar Token**: Use o endpoint `/api/v1/auth/refresh`

## 🧪 Testes

### Executar Testes

```bash
# Todos os testes
dotnet test

# Testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Testes específicos
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
```

## 🤝 Contribuição

Contribuições são sempre bem-vindas! Siga estas etapas:

### 1️⃣ Fork o Projeto
```bash
git clone https://github.com/EmmanuelSMenezes/PAM_MS_Order.git
```

### 2️⃣ Criar Branch
```bash
git checkout -b feature/nova-funcionalidade
```

### 3️⃣ Commit das Mudanças
```bash
git commit -m "feat: adiciona nova funcionalidade incrível"
```

### 4️⃣ Push para Branch
```bash
git push origin feature/nova-funcionalidade
```

### 5️⃣ Abrir Pull Request
Abra um PR descrevendo suas mudanças detalhadamente.

## 📄 Licença

Este projeto está sob a licença **MIT**. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 🆘 Suporte e Contato

### 📞 Canais de Suporte
- **📧 Email**: suporte@pam.com
- **💬 WhatsApp**: +55 (11) 99999-9999
- **🐛 Issues**: [GitHub Issues](https://github.com/EmmanuelSMenezes/PAM_MS_Order/issues)
- **📖 Wiki**: [Documentação Completa](https://github.com/EmmanuelSMenezes/PAM_MS_Order/wiki)

### 👥 Equipe de Desenvolvimento
- **Tech Lead**: Emmanuel Menezes
- **Backend**: Equipe PAM
- **DevOps**: Equipe PAM

---

<div align="center">

**[⬆ Voltar ao Topo](#-PAM_MS_Order)**

**PAM - Plataforma de Agendamento de Manutenção**  
*Desenvolvido com ❤️ pela equipe PAM*

[![GitHub](https://img.shields.io/badge/GitHub-PAM-181717?style=for-the-badge&logo=github)](https://github.com/EmmanuelSMenezes)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-PAM-0077B5?style=for-the-badge&logo=linkedin)](https://linkedin.com/company/pam)

</div>
