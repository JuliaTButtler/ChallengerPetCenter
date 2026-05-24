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

* Cadastro e gerenciamento de usuários
* Cadastro de pets
* Associação entre pets e veterinários
* Criação de entradas diárias no diário do pet
* Registro de alimentação, sintomas, atividades e comportamento
* Comentários veterinários nas entradas do diário
* Solicitações de acompanhamento veterinário

O conceito principal da plataforma é transformar registros cotidianos em informações organizadas e úteis para acompanhamento clínico, criando um histórico contínuo da vida do animal.

Nesta primeira sprint, o foco do projeto foi o desenvolvimento da API RESTful, integração com banco de dados Oracle utilizando Entity Framework Core, documentação via Swagger/OpenAPI e implementação completa dos CRUDs das principais entidades do sistema.

As funcionalidades relacionadas à inteligência artificial, geração automática de insights e integração com dispositivos IoT foram planejadas para futuras sprints do projeto.

---

## Tecnologias Utilizadas

* ASP.NET Core Web API
* Entity Framework Core
* Oracle Database
* Oracle SQL Developer
* Oracle Entity Framework Core Provider
* Swagger / OpenAPI
* C#
* .NET 8

---

## Funcionalidades Implementadas

### Usuários

* Cadastro de usuários
* Consulta de usuários
* Atualização de usuários
* Remoção de usuários

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

---

## Objetivo da API

A API foi desenvolvida seguindo o padrão REST, utilizando boas práticas de:

* Organização de rotas
* Retornos HTTP corretos
* Estruturação em Controllers
* Integração com banco Oracle
* Documentação profissional com Swagger
* Separação entre Models, Controllers e DbContext

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
- Remove um usuário

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
O projeto já está configurado com as credenciais do banco Oracle utilizadas durante o desenvolvimento.

Credenciais utilizadas:
Login: RM564975
Senha: 050607
Local Host: oracle.fiap.com.br
Porta: 1521
Service: ORCL

Caso queira usar outras credenciais, siga as instruções:

No arquivo:

appsettings.json

configure sua string de conexão Oracle:

"ConnectionStrings": {
  "DefaultConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=localhost:1521/XEPDB1;"
}

---

# Executar as migrations

As tabelas e migrations já foram criadas previamente durante o desenvolvimento do projeto, portanto não é necessário executar as migrations novamente para utilização da aplicação.

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


