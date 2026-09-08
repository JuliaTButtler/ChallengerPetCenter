# README — Challenger Pet Center API

API em C# do projeto PetCenter (Challenger).

## Integrantes

- Arthur dos Santos Cabral, 566515, 2TDSA
- Bruno Martins Bettio, 564939, 2TDSA
- José Diogo da Silva Neves, 562341, 2TDSA
- Júlia Tiziotto Buttler, 564975, 2TDSA
- Mariana Xavier Quispe, 566357, 2TDSA

**Repositório:** https://github.com/JuliaTButtler/ChallengerPetCenter.git

---

## Descrição do Projeto

API RESTful para acompanhamento contínuo da saúde animal, com foco em transformar o cuidado com pets em um processo preventivo, organizado e integrado ao veterinário.

A plataforma oferece um “diário inteligente do pet”, onde tutores registram informações diárias e veterinários acompanham o histórico para decisões mais precisas.

### Funcionalidades

- Cadastro e gerenciamento de usuários (tutores/veterinários)
- Cadastro de pets e associação com tutores
- Associação entre pets e veterinários
- Entradas diárias no diário do pet
- Registros de alimentação, sintomas, atividades e comportamento
- Comentários veterinários nas entradas do diário
- Solicitações de acompanhamento veterinário
- Camada de Application (Services + DTOs) com regras de negócio
- Monitoramento e observabilidade (Health Checks, Serilog, OpenTelemetry)
- Testes automatizados unitários e de integração (padrão AAA)

---

## Tecnologias

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core + Oracle
- Serilog (logging estruturado)
- OpenTelemetry (tracing + métricas Prometheus)
- Microsoft.Extensions.Diagnostics.HealthChecks
- xUnit + Moq + WebApplicationFactory
- Swagger / OpenAPI

---

## Estrutura do Projeto

```
ChallengerPetCenter/
├── Controllers/          # Endpoints HTTP (finos, delegam aos services)
├── Services/             # Regras de negócio / camada de aplicação
├── Domain/Rules/         # Validações de domínio reutilizáveis
├── DTOs/                 # Contratos de entrada e saída da API
├── Models/               # Entidades EF Core
├── Data/                 # AppDbContext
├── HealthChecks/         # Checks customizados (serviço externo)
├── Observability/        # Métricas de desempenho
├── Middleware/           # Autenticação por API Key
├── ChallengerPetCenter.UnitTests/
└── ChallengerPetCenter.IntegrationTests/
```

---

## Sprint 3 — Monitoramento, Observabilidade e Testes

### Health Checks

Endpoints disponíveis **sem API Key**:

| Endpoint | Descrição |
|---|---|
| `GET /health` | Status geral (API + banco + serviço externo) |
| `GET /health/live` | Liveness (processo no ar) |
| `GET /health/ready` | Readiness (dependências prontas) |

Checks implementados:

- **api** — saúde básica da aplicação
- **oracle-database** — conectividade com o banco via `AddDbContextCheck<AppDbContext>`
- **external-service** — disponibilidade de serviço externo (`ExternalServices:HealthCheckUrl`)
- **external-uri** — health check HTTP adicional (quando a URL estiver configurada)

Exemplo de resposta:

```json
{
  "status": "Healthy",
  "entries": [
    { "name": "api", "status": "Healthy" },
    { "name": "oracle-database", "status": "Healthy" },
    { "name": "external-service", "status": "Healthy" }
  ]
}
```

### Logging estruturado (Serilog)

- Níveis: Information, Warning, Error (e Debug em Development)
- Correlação de requisições via `TraceIdentifier` / Enrichers
- Saída para **console** e **arquivo** (`logs/petcenter-.log`, rolling diário)
- Request logging com host, scheme e correlation id

### Tracing e métricas (OpenTelemetry)

- Distributed tracing de requisições ASP.NET Core e HttpClient
- Exportador de traces para console
- Métricas Prometheus em `GET /metrics`
- Métricas customizadas: total de requests, erros e tempo de resposta (`petcenter.*`)

### Autenticação (API Key)

Quando `ApiSettings:ApiKey` estiver preenchido no `appsettings.json`, as rotas da API exigem o header:

```http
X-Api-Key: sua-chave
```

`/health`, `/metrics` e `/swagger` permanecem públicos. Se a chave estiver vazia, a validação é ignorada (compatível com sprints anteriores).

---

## Como executar a aplicação

### Pré-requisitos

- .NET 8 SDK
- Oracle Database (ou `UseInMemoryDatabase=true` para testes locais sem Oracle)

### Configuração

No `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=localhost:1521/XEPDB1;"
  },
  "ApiSettings": {
    "ApiKey": ""
  },
  "ExternalServices": {
    "HealthCheckUrl": "https://httpbin.org/status/200"
  }
}
```

### Rodar

```bash
dotnet restore
dotnet run --project ChallengerPetCenter.csproj
```

Swagger (Development): `https://localhost:5032/swagger` (porta conforme `launchSettings.json`)

Migrations (já existentes; só se precisar reaplicar o banco):

```bash
dotnet ef database update
```

---

## Como executar os testes

Na raiz da solution:

```bash
dotnet test ChallengerPetCenter.sln
```

Somente unitários:

```bash
dotnet test ChallengerPetCenter.UnitTests/ChallengerPetCenter.UnitTests.csproj
```

Somente integração:

```bash
dotnet test ChallengerPetCenter.IntegrationTests/ChallengerPetCenter.IntegrationTests.csproj
```

### Organização dos testes

| Projeto | Escopo |
|---|---|
| `ChallengerPetCenter.UnitTests` | Domínio (`Domain/Rules`) e Application (`Services`) com xUnit + Moq |
| `ChallengerPetCenter.IntegrationTests` | Endpoints HTTP com `WebApplicationFactory`, Collection Fixtures e InMemory DB |

Nomenclatura: `MetodoTestado_Cenario_ResultadoEsperado`  
Padrão: **Arrange / Act / Assert**

Os testes de integração cobrem:

- Fluxos de sucesso (201/200)
- Tratamento de erros (400/404)
- Autenticação por API Key (401 sem chave)
- Health Checks e métricas

---

## Principais rotas da API

### Usuários — `/api/users`

- `GET /api/users`
- `GET /api/users/{id}`
- `GET /api/users/email/{email}`
- `GET /api/users/tipo/{tipo}`
- `POST /api/users`
- `PUT /api/users/{id}`
- `DELETE /api/users/{id}`

### Veterinários — `/api/veterinarios`

- `GET /api/veterinarios`
- `GET /api/veterinarios/{id}`
- `GET /api/veterinarios/crmv/{crmv}`
- `GET /api/veterinarios/especialidade/{especialidade}`
- `GET /api/veterinarios/user/{userId}`
- `POST /api/veterinarios`
- `PUT /api/veterinarios/{id}`
- `DELETE /api/veterinarios/{id}`

### Pets — `/api/pets`

- `GET /api/pets`
- `GET /api/pets/{id}`
- `GET /api/pets/user/{userId}`
- `GET /api/pets/especie/{especie}`
- `GET /api/pets/nome/{nome}`
- `POST /api/pets`
- `PUT /api/pets/{id}`
- `DELETE /api/pets/{id}`

### Diário — `/api/diarioentradas`

- `GET /api/diarioentradas`
- `GET /api/diarioentradas/{id}`
- `GET /api/diarioentradas/pet/{petId}`
- `GET /api/diarioentradas/status/{status}`
- `GET /api/diarioentradas/humor/{humor}`
- `POST /api/diarioentradas`
- `PUT /api/diarioentradas/{id}`
- `DELETE /api/diarioentradas/{id}`

### Registros — `/api/registros`

- `GET /api/registros`
- `GET /api/registros/{id}`
- `GET /api/registros/diario/{diarioEntradaId}`
- `GET /api/registros/tipo/{tipo}`
- `GET /api/registros/subtipo/{subtipo}`
- `POST /api/registros`
- `PUT /api/registros/{id}`
- `DELETE /api/registros/{id}`

### Comentários — `/api/comentarios`

- `GET /api/comentarios`
- `GET /api/comentarios/{id}`
- `GET /api/comentarios/diario/{diarioEntradaId}`
- `GET /api/comentarios/user/{userId}`
- `GET /api/comentarios/texto/{texto}`
- `POST /api/comentarios`
- `PUT /api/comentarios/{id}`
- `DELETE /api/comentarios/{id}`

### Solicitações — `/api/solicitacoes`

- `GET /api/solicitacoes`
- `GET /api/solicitacoes/{id}`
- `GET /api/solicitacoes/pet/{petId}`
- `GET /api/solicitacoes/tutor/{tutorId}`
- `GET /api/solicitacoes/veterinario/{veterinarioId}`
- `GET /api/solicitacoes/status/{status}`
- `POST /api/solicitacoes`
- `PUT /api/solicitacoes/{id}`
- `DELETE /api/solicitacoes/{id}`

### Pet–Veterinário — `/api/petveterinarios`

- `GET /api/petveterinarios`
- `GET /api/petveterinarios/{id}`
- `GET /api/petveterinarios/pet/{petId}`
- `GET /api/petveterinarios/veterinario/{veterinarioId}`
- `GET /api/petveterinarios/ativos`
- `POST /api/petveterinarios`
- `PUT /api/petveterinarios/{id}`
- `DELETE /api/petveterinarios/{id}`

---

## Observações

- Migrations existentes foram mantidas; a Sprint 3 não altera o schema do banco.
- Controllers agora usam DTOs e Services; models de domínio/persistência permanecem em `Models/`.
- Funcionalidades de IA/insights continuam planejadas para sprints futuras.
