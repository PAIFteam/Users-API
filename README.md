# Users API

Microsserviço responsável pela **gestão de usuários** da plataforma PAIF Games,
projetado para atuar como **publisher de eventos** em uma arquitetura orientada a eventos.

---

## 🧱 Arquitetura e Tecnologias

- .NET 8
- Minimal APIs
- Carter
- CQRS (Commands / Queries)
- MediatR
- Dapper
- MassTransit
- RabbitMQ
- Docker (multi-stage build)
- Pronto para Kubernetes

Arquitetura em camadas:
- API
- Core (Domain + Application)
- Infra (Data + Messaging)

---

## 🧠 Design Decisions

- CQRS adotado para separar leitura e escrita
- Dapper utilizado por performance e controle de SQL
- RabbitMQ usado para desacoplamento entre domínios
- Serviço atua apenas como publisher por design


## 📦 Responsabilidades do Serviço

- Criar, atualizar, listar e remover usuários
- Validar regras de negócio (e-mail, senha e duplicidade)
- Persistir dados em banco relacional
- Publicar evento de boas-vindas via RabbitMQ
- Não consome eventos (atua apenas como Publisher)

---

## 📡 Mensageria (RabbitMQ)

Evento publicado:
- WelcomeCustomerMessage

Quando publica:
- Após sucesso no PutUserUseCase

Fila:
- welcome_customer_queue

Papel do serviço:
- Publisher apenas
- Consumer desativado por design
- StartConsumer = false

---

## ⚙️ Configuração

### appsettings.json (exemplo)

```json
{
  "ConnectionStrings": {
    "Database": "Server=localhost;Port=5432;Database=UsersDb;User Id=postgres;Password=***;",
    "DB_SQL_PAIF_GAMES": "Server=localhost;Database=PAIF_GAMES;User Id=***;Password=***;"
  },
  "RabbitSettings": {
    "HostName": "localhost",
    "Username": "fcg",
    "Password": "***",
    "QueueName": "welcome_customer_queue",
    "RedeliveryInSeconds": [],
    "RetryInSeconds": [],
    "StartConsumer": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
"AllowedHosts": "*"
}
```
## 🔐 Variáveis de Ambiente (Docker / Deploy / Kubernetes)
```text
ConnectionStrings__Database
ConnectionStrings__DB_SQL_PAIF_GAMES
RabbitSettings__HostName
RabbitSettings__Username
RabbitSettings__Password
RabbitSettings__QueueName
RabbitSettings__StartConsumer
```
## 🔌 Endpoints (Carter / CQRS)

| Método | Rota | Descrição |
|------|------|----------|
| POST | /users | Criar usuário |
| GET | /users | Listar usuários |
| GET | /users/{id} | Buscar usuário por ID |
| PUT | /users | Atualizar usuário |
| DELETE | /users/{id} | Remover usuário |

## 🔄 Fluxo de Integração

- Cliente chama PUT /PutUser  
- Usuário é validado e persistido no banco  
- Evento WelcomeCustomerMessage é publicado no RabbitMQ  
- Notifications API consome a mensagem e envia notificação  

## ▶️ Executando Localmente

Pré-requisitos:

.NET SDK 8
PostgreSQL
SQL Server
RabbitMQ

Run:
```bash
dotnet restore
dotnet run --project Service/Users/Users.API/Users.API.csproj
```
Swagger disponível automaticamente em ambiente Development.

## 🐳 Docker

Build:
```bash
docker build -t users-api -f Service/Users/Users.API/Dockerfile .
```

Run:
```bash
docker run -p 8080:8080 \
  -e ASPNETCORE_URLS=http://+:8080 \
  users-api
```

## 📄 Licença
Projeto para fins educacionais e demonstrativos.
