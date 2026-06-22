# users-api

Serviço responsável pelo gerenciamento de usuários e autenticação JWT da plataforma FIAP Cloud Games (FCG).

## Finalidade

- Cadastro, consulta, atualização e remoção de usuários.
- Autenticação via JWT (login retorna Bearer token).
- Promoção de usuários ao papel de administrador.
- Publica o evento `UserCreatedEvent` no RabbitMQ ao registrar um novo usuário.

## Eventos

| Direção  | Evento             | Descrição                                    |
|----------|--------------------|----------------------------------------------|
| Publica  | `UserCreatedEvent` | Disparado após criação de usuário com sucesso |

## Variáveis de Ambiente

| Variável                                | Obrigatório | Valor padrão (dev local)                          | Descrição                          |
|-----------------------------------------|-------------|----------------------------------------------------|------------------------------------|
| `ConnectionStrings__DefaultConnection` | Sim         | `(localdb)\MSSQLLocalDB;Database=FcgUsers;...`    | Connection string SQL Server       |
| `JwtSettings__SecretKey`               | Sim         | `fiap-cloud-games-secret-key-minimo-32-chars!`    | Chave secreta do JWT (≥32 chars)   |
| `JwtSettings__Issuer`                  | Sim         | `Users.API`                                       | Issuer do token JWT                |
| `JwtSettings__Audience`               | Sim         | `FiapCloudGames.Client`                           | Audience do token JWT              |
| `RabbitMq__Host`                       | Sim         | `localhost`                                       | Host do RabbitMQ                   |
| `RabbitMq__User`                       | Não         | `guest`                                           | Usuário do RabbitMQ                |
| `RabbitMq__Pass`                       | Não         | `guest`                                           | Senha do RabbitMQ                  |

> No Docker Compose e K8s, `JwtSettings__Issuer` = `FiapCloudGames` e `JwtSettings__Audience` = `FiapCloudGames.Client`.

## Endpoints

| Método   | Rota                          | Auth           | Descrição                                    |
|----------|-------------------------------|----------------|----------------------------------------------|
| `POST`   | `/api/user`                   | Anônimo        | Cadastra novo usuário                        |
| `POST`   | `/api/auth/login`             | Anônimo        | Realiza login e retorna token JWT            |
| `GET`    | `/api/user/{id}`              | Bearer (user)  | Retorna dados de um usuário                  |
| `GET`    | `/api/user`                   | Bearer (Admin) | Lista todos os usuários                      |
| `PUT`    | `/api/user/{id}`              | Bearer (user)  | Atualiza dados de um usuário                 |
| `DELETE` | `/api/user/{id}`              | Bearer (Admin) | Remove um usuário                            |
| `PATCH`  | `/api/user/{id}/promote`      | Bearer (Admin) | Promove usuário para administrador           |

### Exemplos de uso

```bash
# Cadastrar usuário
curl -X POST http://localhost:5001/api/user \
  -H "Content-Type: application/json" \
  -d '{"name":"João","email":"joao@example.com","password":"Senha@123"}'

# Login
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"joao@example.com","password":"Senha@123"}'
```

## Como rodar localmente

**Pré-requisitos:** .NET 8 SDK, SQL Server (LocalDB ou instância local), RabbitMQ.

```bash
cd services/users-api/src/Users.API
dotnet run
# Swagger disponível em http://localhost:<porta>/ (modo Development)
```

Ou com variáveis customizadas:

```bash
dotnet run --ConnectionStrings__DefaultConnection="Server=...;Database=users;..." \
           --JwtSettings__SecretKey="minha-chave-secreta-32-chars!!" \
           --RabbitMq__Host="localhost"
```

## Como rodar via Docker

A partir da raiz do repositório:

```bash
docker compose -f orchestration/docker-compose.yml up --build users-api users-db rabbitmq
```

O serviço ficará disponível em **http://localhost:5001**.

## Repositório

> Placeholder: `<users-api repo>` (preencher após split de repositórios)
