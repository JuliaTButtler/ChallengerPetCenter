# README — Challenger Pet Center API
API de C# do projeto PetCenter para o Challenger2

Integrantes: 

   Arthur dos Santos Cabral, 566515, 2TDSA

   Bruno Martins Bettio, 564939, 2TDSA

   José Diogo da Silva Neves, 562341, 2TDSA 

   Júlia Tiziotto Buttler, 564975, 2TDSA 

   Mariana Xavier Quispe, 566357, 2TDSA 

# LINK DO REPOSITÓRIO: https://github.com/JuliaTButtler/ChallengerPetCenter.git

## Descrição do Projeto

O projeto consiste no desenvolvimento de uma API RESTful para uma plataforma de acompanhamento contínuo da saúde animal, com foco em transformar o cuidado com pets em um processo preventivo, organizado e integrado ao acompanhamento veterinário.

A proposta surgiu a partir da identificação de um problema recorrente: muitos tutores não possuem um histórico estruturado sobre a saúde do animal e frequentemente recorrem ao veterinário apenas em situações emergenciais. Isso dificulta a identificação de padrões comportamentais, reduz a precisão das análises clínicas e torna o cuidado animal majoritariamente reativo.

Como solução, a plataforma propõe a criação de um “diário inteligente do pet”, onde tutores podem registrar informações diárias sobre seus animais, permitindo um acompanhamento contínuo da rotina e da saúde do pet.

O sistema permite:

* Cadastro e gerenciamento de usuários (tutores e veterinários)
* Autenticação JWT mínima (login e endpoints protegidos)
* Cadastro de pets e vínculo pet–veterinário
* Diário do pet com registros diários (alimentação, sintomas, comportamento)
* Comentários e solicitações de acompanhamento
* Health Checks, logging estruturado (Serilog) e tracing/métricas (OpenTelemetry)
* Testes automatizados (unitários e de integração)

O conceito principal da plataforma é transformar registros cotidianos em informações organizadas e úteis para acompanhamento clínico, criando um histórico contínuo da vida do animal.

Nesta primeira sprint, o foco do projeto foi o desenvolvimento da API RESTful, integração com banco de dados Oracle utilizando Entity Framework Core, documentação via Swagger/OpenAPI e implementação completa dos CRUDs das principais entidades do sistema.

Nas sprints seguintes a API evoluiu com:

* Separação em **Controllers** (HTTP), **Services** (regras de negócio) e **DTOs** (contratos de entrada/saída)
* **Health Checks** (API, Oracle e serviço externo)
* **Serilog** (console + arquivo, CorrelationId / RequestId)
* **OpenTelemetry** (tracing e métricas no console, sem cloud)
* **JWT** mínimo (`/api/auth/login`, `/api/auth/me`, delete de usuário protegido)
* Projetos **UnitTests** e **IntegrationTests** (xUnit, Moq, WebApplicationFactory)

As funcionalidades relacionadas à inteligência artificial, geração automática de insights e integração com dispositivos IoT foram planejadas para futuras sprints do projeto.

---

## Tecnologias Utilizadas

* ASP.NET Core Web API
* Entity Framework Core
* Oracle Database
* Oracle SQL Developer
* Oracle Entity Framework Core Provider
* Swagger / OpenAPI
* Microsoft.Extensions.Diagnostics.HealthChecks
* Serilog (logging estruturado)
* OpenTelemetry (tracing e métricas — export console)
* JWT Bearer (autenticação mínima)
* xUnit / Moq / WebApplicationFactory (testes)
* C#
* .NET 8

---

## Health Checks (Monitoramento)

A API expõe endpoints de saúde com `Microsoft.Extensions.Diagnostics.HealthChecks`:

| Endpoint | O que verifica |
|----------|----------------|
| `GET /health` ou `GET /healthz` | Todos os checks (visão geral) |
| `GET /health/live` | Processo da API (liveness / self) |
| `GET /health/ready` | Oracle + serviço externo (readiness) |

Checks registrados:

* **self** — API em execução
* **oracle** — conectividade com o banco via `AddDbContextCheck<AppDbContext>`
* **external_service** — HTTP GET em URL configurável (`HealthChecks:ExternalServiceUrl` no `appsettings.json`). Se a URL estiver vazia, o check reporta Healthy com mensagem explicativa.

Exemplo de resposta JSON (status Healthy):

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.12",
  "entries": {
    "self": { "status": "Healthy", "description": "API ChallengerPetCenter está em execução." },
    "oracle": { "status": "Healthy" },
    "external_service": { "status": "Healthy", "description": "Serviço externo respondendo: https://www.google.com (200)." }
  }
}
```

Como testar: com a API rodando (`dotnet run`), abra no navegador ou via curl:

```text
http://localhost:<porta>/health
http://localhost:<porta>/health/live
http://localhost:<porta>/health/ready
```

---

## Logging Estruturado (Serilog)

A API usa **Serilog** no lugar do logging padrão do ASP.NET Core.

### Saídas
* **Console** — durante `dotnet run`
* **Arquivo** — `logs/challenger-pet-center-YYYYMMDD.log` (rolling diário, retém 14 dias)

### Níveis
* Padrão da aplicação: **Information**
* Warnings/Erros de request HTTP: status `4xx` → Warning; `5xx` ou exception → Error
* Framework (`Microsoft.*`, EF Core): **Warning** (para reduzir ruído)

### Correlação de requisições
* Middleware `CorrelationIdMiddleware` gera ou propaga o header `X-Correlation-ID`
* Cada log inclui `CorrelationId` e `RequestId` (TraceIdentifier)
* `UseSerilogRequestLogging` registra método, path, status e tempo de resposta

### Como observar
1. Rode `dotnet run` e faça uma chamada (ex.: `/health` ou `/api/users`)
2. No console, veja linhas no formato: `[HH:mm:ss INF] (correlationId/requestId) HTTP GET /health respondeu 200 em ... ms`
3. Confira o mesmo conteúdo em `logs/challenger-pet-center-*.log`
4. Opcional: envie o header `X-Correlation-ID: meu-id-teste` e confira se ele aparece nos logs e na resposta

---

## OpenTelemetry (Tracing e Métricas)

Observabilidade **local**, sem Application Insights / cloud.

### O que está instrumentado
* **AspNetCore** — cada request HTTP gera um span (ex.: `GET /api/users`)
* **HttpClient** — chamadas externas (ex.: health check do Google) entram no mesmo fluxo de tracing

Rotas `/health*` e `/swagger*` são filtradas do tracing para reduzir ruído.

### Métricas
* Instrumentation AspNetCore/HttpClient (duração de request, contagem, erros)
* Exportadas no console a cada ~15 segundos

### Configuração (`appsettings.json`)
```json
"OpenTelemetry": {
  "ServiceName": "ChallengerPetCenter",
  "EnableConsoleExporters": true
}
```

### Como observar
1. `dotnet run`
2. Chame `GET /api/users` (ou outro endpoint de negócio)
3. No terminal, além do Serilog, aparecem blocos do exporter OpenTelemetry (Activity/spans e, periodicamente, métricas)

Não há dependência de collector OTLP nem de Azure nesta entrega.

---

## Autenticação JWT (mínima)

* `POST /api/auth/login` — email + senha → retorna Bearer token
* `GET /api/auth/me` — requer JWT (**401** sem token)
* `DELETE /api/users/{id}` — requer JWT

Configuração em `appsettings.json` (`Jwt:Key`, `Issuer`, `Audience`, `ExpirationMinutes`).

Cada login gera um token ligado àquele usuário (claims: id, email, tipo). O token vale até expirar (padrão 120 minutos); não há blacklist — apagar o usuário não invalida o token imediatamente.

> Observação: a senha ainda é comparada em texto puro (mínimo acadêmico). Hash seria evolução futura.

Veja o **Guia de testes manuais no Swagger** mais abaixo para exemplos completos de login e Authorize.

---

## Testes Automatizados

A solution inclui projetos separados por camada:

* `ChallengerPetCenter.UnitTests` — Domain (regras puras, sem EF/Moq) + Application (Moq dos repos) + Controllers
* `ChallengerPetCenter.IntegrationTests` — `WebApplicationFactory` + EF InMemory + Collection Fixture

Nomenclatura: `Metodo_Cenario_Resultado` (ex.: `CreateAsync_QuandoEmailJaExiste_RetornaFailure`). AAA visível em cada teste.

### O que é coberto

**Domain (unitário):** email inválido, diário duplicado no mesmo dia, pet sem tutor, usuário inativo.

**Application (Moq):** por agregado — create ok, create com regra, get not found, delete; Auth — ok / senha inválida / inativo. Inclui **Comentario** e **Registro**.

**Integração:** Users + Auth + Pets (HTTP com JWT); `401` sem token; `/health/live` e `/health/ready`.

```bash
dotnet test ChallengerPetCenter.sln
```

### Cobertura (Domain + Application)

Coletor apontado só para Domain/Application via `coverlet.runsettings` (Controllers/Program ficam de fora do alvo).

```bash
dotnet test ChallengerPetCenter.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings --results-directory ./TestResults
```

| Assembly | Line coverage |
|----------|---------------|
| `ChallengerPetCenter.Domain` | **94,9%** |
| `ChallengerPetCenter.Application` | **91,5%** |
| Combinado (Domain + Application) | **92,0%** |

Relatório: `TestResults/**/coverage.cobertura.xml`.

Para testes manuais com exemplos de body, use a seção **Guia de testes manuais no Swagger**.

---

## Funcionalidades Implementadas

### Usuários

* Cadastro de usuários
* Consulta de usuários
* Atualização de usuários
* Remoção de usuários (requer JWT)

### Autenticação

* Login com JWT (`POST /api/auth/login`)
* Perfil autenticado (`GET /api/auth/me`)

### Veterinários

* Cadastro de veterinários
* Busca por CRMV
* Busca por especialidade
* Associação com usuários

### Pets

* Cadastro de pets
* Associação com tutores
* Consulta de pets

### Diário do Pet

* Criação de entradas diárias
* Organização cronológica
* Controle de uma entrada por pet por dia

### Registros

* Registros de alimentação
* Sintomas
* Comportamentos
* Atividades
* Observações complementares

### Comentários

* Comentários em entradas do diário
* Associação entre usuários e entradas

### Solicitações

* Solicitações entre tutores e veterinários
* Controle de status

### Relacionamento Pet-Veterinário

* Associação entre pets e veterinários
* Histórico de acompanhamento

### Monitoramento

* Health Checks (`/health`, `/health/live`, `/health/ready`)
* Verificação de Oracle e serviço externo
* Logging estruturado com Serilog (console + arquivo, CorrelationId)
* OpenTelemetry (tracing + métricas no console)
* JWT mínimo (login + endpoints protegidos)

---

## Objetivo da API

A API foi desenvolvida seguindo o padrão REST, utilizando boas práticas de:

* Organização de rotas e retornos HTTP corretos
* Controllers finos + Services com regras de negócio + DTOs
* Integração com Oracle (EF Core)
* Documentação com Swagger/OpenAPI
* Observabilidade (Health Checks, Serilog, OpenTelemetry)
* Autenticação JWT mínima
* Testes automatizados (unitários e integração)

### Como subir a API

```bash
dotnet run
```

Swagger (perfil http): http://localhost:5032/swagger

### Como rodar os testes automatizados

```bash
dotnet test ChallengerPetCenter.sln
```

---

## Guia de testes manuais no Swagger (POST)

Use este fluxo na ordem. Em cada POST: abra o endpoint → **Try it out** → cole o JSON → **Execute**.  
Os IDs (`userId`, `petId`, etc.) devem ser os retornados nas respostas anteriores (não copie IDs inventados se o banco já tiver outros registros).

### Passo 1 — Criar tutor

`POST /api/users`

```json
{
  "nome": "Camila Ferreira",
  "email": "camila.ferreira@email.com",
  "senha": "Camila@2024",
  "telefone": "11987654321",
  "tipoUsuario": "TUTOR"
}
```

Espere **201**. Anote o `id` do tutor (ex.: `26`).

### Passo 2 — Criar usuário veterinário

`POST /api/users`

```json
{
  "nome": "Ricardo Almeida",
  "email": "ricardo.almeida@email.com",
  "senha": "Ricardo@2024",
  "telefone": "11976543210",
  "tipoUsuario": "VETERINARIO"
}
```

Anote o `id` do usuário veterinário (ex.: `27`).

### Passo 3 — Login (JWT)

`POST /api/auth/login`

```json
{
  "email": "camila.ferreira@email.com",
  "senha": "Camila@2024"
}
```

Copie o `token`. No Swagger, clique em **Authorize**, cole o token e confirme.  
Opcional: `GET /api/auth/me` deve retornar **200** com o email da Camila.

### Passo 4 — Perfil de veterinário

`POST /api/veterinarios`  
(substitua `userId` pelo id do Ricardo)

```json
{
  "crmv": "SP-45872",
  "especialidade": "Clínica Geral",
  "descricao": "Atendimento de cães e gatos, com foco em acompanhamento preventivo.",
  "userId": 27
}
```

Anote o `id` do veterinário (ex.: `5`).

### Passo 5 — Cadastrar o pet

`POST /api/pets`  
(substitua `userId` pelo id da Camila)

```json
{
  "nome": "Luna",
  "especie": "Gato",
  "raca": "Siamês",
  "dataNascimento": "2022-03-15T00:00:00",
  "observacoes": "Alimentação controlada e rotina de brincadeiras à noite.",
  "userId": 26
}
```

Anote o `id` do pet (ex.: `8`).

### Passo 6 — Vincular pet e veterinário

`POST /api/petveterinarios`

```json
{
  "petId": 8,
  "veterinarioId": 5,
  "dataInicio": "2026-09-01T10:00:00",
  "observacoes": "Acompanhamento mensal de rotina."
}
```

### Passo 7 — Entrada no diário

`POST /api/diarioentradas`

```json
{
  "data": "2026-09-11T00:00:00",
  "resumo": "Dia calmo em casa, brincou após o almoço.",
  "humorGeral": "TRANQUILO",
  "status": "ATIVO",
  "petId": 8
}
```

Anote o `id` da entrada (ex.: `12`).  
Regra: só **uma entrada por pet por dia**.

### Passo 8 — Registro do dia (alimentação)

`POST /api/registros`

```json
{
  "tipo": "ALIMENTACAO",
  "subtipo": "RACAO",
  "valor": 80,
  "unidade": "g",
  "nota": "Comeu bem a porção da manhã.",
  "horario": "2026-09-11T08:30:00",
  "diarioEntradaId": 12
}
```

Outro exemplo (sintoma):

```json
{
  "tipo": "SINTOMA",
  "subtipo": "APETITE",
  "valor": null,
  "unidade": null,
  "nota": "Apetite levemente reduzido no final da tarde.",
  "horario": "2026-09-11T17:00:00",
  "diarioEntradaId": 12
}
```

### Passo 9 — Comentário no diário

`POST /api/comentarios`  
(`userId` pode ser o da Camila ou do Ricardo)

```json
{
  "textoComentario": "Manter a rotina alimentar e observar o apetite pelos próximos dois dias.",
  "diarioEntradaId": 12,
  "userId": 27
}
```

### Passo 10 — Solicitação de acompanhamento

`POST /api/solicitacoes`  
(`tutorId` deve ser o dono do pet)

```json
{
  "status": "PENDENTE",
  "mensagem": "Gostaria de uma orientação sobre a mudança de apetite da Luna.",
  "petId": 8,
  "tutorId": 26,
  "veterinarioId": 5
}
```

Se `status` for omitido, a API assume `PENDENTE`.

### Cenários de erro úteis

| Ação | Body / situação | Esperado |
|------|-----------------|----------|
| Email duplicado | mesmo email da Camila em `POST /api/users` | **400** |
| Login errado | senha incorreta no login | **400** |
| `/api/auth/me` sem Authorize | — | **401** |
| Segunda entrada no mesmo dia | mesmo `petId` + mesma `data` | **400** |
| Solicitação com tutor errado | `tutorId` diferente do dono do pet | **400** |
| `DELETE /api/users/{id}` sem token | — | **401** |

### Campos dos POST (referência rápida)

| Endpoint | Campos |
|----------|--------|
| `POST /api/users` | `nome*`, `email*`, `senha*`, `telefone`, `tipoUsuario*` (`TUTOR` ou `VETERINARIO`) |
| `POST /api/auth/login` | `email*`, `senha*` |
| `POST /api/veterinarios` | `crmv*`, `especialidade`, `descricao`, `userId*` |
| `POST /api/pets` | `nome*`, `especie*`, `raca`, `dataNascimento`, `observacoes`, `userId*` |
| `POST /api/petveterinarios` | `petId*`, `veterinarioId*`, `dataInicio`, `observacoes` |
| `POST /api/diarioentradas` | `data*`, `resumo`, `humorGeral`, `status`, `petId*` |
| `POST /api/registros` | `tipo*`, `subtipo`, `valor`, `unidade`, `nota`, `horario`, `diarioEntradaId*` |
| `POST /api/comentarios` | `textoComentario*`, `diarioEntradaId*`, `userId*` |
| `POST /api/solicitacoes` | `status`, `mensagem`, `petId*`, `tutorId*`, `veterinarioId*` |

\* obrigatório

---

# Documentação das Rotas

---

# Usuários

### GET
`/api/users`
- Lista todos os usuários

### GET
`/api/users/{id}`
- Busca usuário por ID

### GET
`/api/users/email/{email}`
- Busca usuário por email

### POST
`/api/users`
- Cria um novo usuário

### PUT
`/api/users/{id}`
- Atualiza um usuário

### DELETE
`/api/users/{id}`
- Remove um usuário (**requer JWT**)

---

# Auth

### POST
`/api/auth/login`
- Autentica e retorna JWT

### GET
`/api/auth/me`
- Dados do usuário autenticado (**requer JWT**)

---

# Veterinários

### GET
`/api/veterinarios`
- Lista todos os veterinários

### GET
`/api/veterinarios/{id}`
- Busca veterinário por ID

### GET
`/api/veterinarios/crmv/{crmv}`
- Busca veterinário por CRMV

### GET
`/api/veterinarios/especialidade/{especialidade}`
- Busca veterinários por especialidade

### GET
`/api/veterinarios/user/{userId}`
- Busca veterinário pelo ID do usuário

### POST
`/api/veterinarios`
- Cria um veterinário

### PUT
`/api/veterinarios/{id}`
- Atualiza um veterinário

### DELETE
`/api/veterinarios/{id}`
- Remove um veterinário

---

# Pets

### GET
`/api/pets`
- Lista todos os pets

### GET
`/api/pets/{id}`
- Busca pet por ID

### GET
`/api/pets/user/{userId}`
- Lista pets de um tutor

### POST
`/api/pets`
- Cria um novo pet

### PUT
`/api/pets/{id}`
- Atualiza um pet

### DELETE
`/api/pets/{id}`
- Remove um pet

---

# Diário de Entradas

### GET
`/api/diarioentradas`
- Lista todas as entradas do diário

### GET
`/api/diarioentradas/{id}`
- Busca entrada por ID

### GET
`/api/diarioentradas/pet/{petId}`
- Lista entradas de um pet

### GET
`/api/diarioentradas/status/{status}`
- Busca entradas por status

### POST
`/api/diarioentradas`
- Cria uma entrada no diário

### PUT
`/api/diarioentradas/{id}`
- Atualiza uma entrada

### DELETE
`/api/diarioentradas/{id}`
- Remove uma entrada

---

# Comentários

### GET
`/api/comentarios`
- Lista comentários

### GET
`/api/comentarios/{id}`
- Busca comentário por ID

### GET
`/api/comentarios/diario/{diarioEntradaId}`
- Lista comentários da entrada do diário

### GET
`/api/comentarios/user/{userId}`
- Lista comentários de um usuário

### POST
`/api/comentarios`
- Cria um comentário

### PUT
`/api/comentarios/{id}`
- Atualiza um comentário

### DELETE
`/api/comentarios/{id}`
- Remove um comentário

---

# Registros

### GET
`/api/registros`
- Lista registros

### GET
`/api/registros/{id}`
- Busca registro por ID

### GET
`/api/registros/diario/{diarioEntradaId}`
- Lista registros da entrada do diário

### GET
`/api/registros/tipo/{tipo}`
- Busca registros por tipo

### POST
`/api/registros`
- Cria um registro

### PUT
`/api/registros/{id}`
- Atualiza um registro

### DELETE
`/api/registros/{id}`
- Remove um registro

---

# Solicitações

### GET
`/api/solicitacoes`
- Lista solicitações

### GET
`/api/solicitacoes/{id}`
- Busca solicitação por ID

### GET
`/api/solicitacoes/tutor/{tutorId}`
- Busca solicitações de um tutor

### GET
`/api/solicitacoes/veterinario/{veterinarioId}`
- Busca solicitações de um veterinário

### POST
`/api/solicitacoes`
- Cria uma solicitação

### PUT
`/api/solicitacoes/{id}`
- Atualiza uma solicitação

### DELETE
`/api/solicitacoes/{id}`
- Remove uma solicitação

---

# Relacionamento Pet-Veterinário

### GET
`/api/petveterinarios`
- Lista vínculos entre pets e veterinários

### GET
`/api/petveterinarios/{id}`
- Busca vínculo por ID

### GET
`/api/petveterinarios/pet/{petId}`
- Busca veterinários vinculados a um pet

### GET
`/api/petveterinarios/veterinario/{veterinarioId}`
- Busca pets vinculados a um veterinário

### POST
`/api/petveterinarios`
- Cria vínculo entre pet e veterinário

### PUT
`/api/petveterinarios/{id}`
- Atualiza vínculo

### DELETE
`/api/petveterinarios/{id}`
- Remove vínculo

---

# Swagger

A documentação completa da API pode ser acessada via Swagger após executar o projeto:

https://localhost:5032/swagger



# Instalação e Execução

## Pré-requisitos

Antes de executar o projeto, é necessário possuir instalado:

- .NET 8 SDK
- Visual Studio 2022
- Oracle Database
- Oracle SQL Developer

---

# Clonar o repositório

git clone https://github.com/JuliaTButtler/ChallengerPetCenter.git

---

# Abrir o projeto

Abra o arquivo no Visual Studio 2022.

Os pacotes NuGet utilizados no projeto serão restaurados automaticamente ao abrir a solução.

---

# Configurar conexão com o banco Oracle

No arquivo:

appsettings.json

configure sua string de conexão Oracle:

"ConnectionStrings": {
  "DefaultConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=localhost:1521/XEPDB1;"
}

---

# Executar as migrations

As tabelas e migrations já foram criadas previamente durante o desenvolvimento do projeto, caso queria recriar manualmente digite no terminal:

dotnet ef migrations add InitialCreate

Caso deseje recriar o banco manualmente, utilize:

dotnet ef database update

no terminal.
---

# Executar a aplicação

Via Visual Studio:

F5

Ou via terminal:

dotnet run

---

# Acessar Swagger

Após executar o projeto, a documentação da API estará disponível em:

https://localhost:5032/swagger

---

# Estrutura do Projeto

O projeto segue arquitetura baseada em:

* Controllers
* Models
* Entity Framework Core
* DbContext
* Oracle Database

---

# Observações

* O projeto utiliza ASP.NET Core Web API seguindo padrão REST.
* O banco de dados utilizado é Oracle.
* O Swagger foi configurado para documentação e testes dos endpoints.
* As funcionalidades relacionadas à inteligência artificial e insights automáticos serão implementadas em futuras sprints.


