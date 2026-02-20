# 📖 Backend Standards: Project "The Syndicate" (.NET 10)

Este documento define os padrões técnicos para o desenvolvimento do backend do "The Syndicate" utilizando **.NET 10** (última versão estável). O foco é manter o código simples, auditável e preparado para revisões de código (code review) eficientes.

---

## 1. Solution Structure (Arquitetura)

Adotamos a **Layered Architecture** simplificada. A dependência deve sempre apontar para o **Domain**.

* **`Syndicate.API`**: Porta de entrada. Contém Controllers, Middlewares, Swagger e configurações do `Program.cs`.
* **`Syndicate.Domain`**: O "Coração". Contém Entidades (Entities), Interfaces, Comandos/Consultas (MediatR), DTOs e as Regras de Negócio.
* **`Syndicate.Infrastructure`**: Detalhes técnicos. EF Core `DbContext`, Migrations, Repositórios e integrações (SignalR, Mail, etc).

---

## 2. Naming Conventions (Padrões de Nomenclatura)

**Idioma Obrigatório no Código: Inglês.**

* **Classes, Interfaces, Methods:** `PascalCase` (ex: `ProcessDebtCommand`, `IUserRepository`).
* **Local Variables & Parameters:** `camelCase` (ex: `blockedAmount`, `groupId`).
* **Private Fields:** `_camelCase` (ex: `_context`, `_mediator`).
* **Interfaces:** Prefixadas com **I** (ex: `IPollService`).
* **Database Tables (Postgres):** `snake_case` e no **plural** (ex: `users`, `group_members`).

---

## 3. Database & EF Core (Persistência)

### Sistema de Saldo (Em Discussão)

> ⚠️ **Nota:** A estratégia de saldo (Ledger vs Coluna Balance) ainda está em discussão — ver [ADR 004](ADR/004-ledger-ao-inves-de-coluna-balance.md). As tabelas abaixo representam a estrutura base, e a tabela `transactions` será ajustada conforme a decisão final.

| Table Name (English) | Description |
| --- | --- |
| `users` | Identity and basic data. |
| `groups` | Group name and metadata. |
| `members` | Pivot table (N:N) between User and Group. |
| `transactions` | **Ledger**: Record of all credits/debits. |
| `debts` | Escrow records (Creditor, Debtor, Amount, Status). |
| `polls` | Titles and rules for voting. |
| `votes` | Individual records with calculated weight. |

### Migration Rules

1. **Descriptive Names:** Ex: `20240215_AddDebtTable`.
2. **No Manual Changes:** Proibido alterar o banco sem Migration.
3. **Rollback Check:** O método `Down()` deve sempre ser capaz de reverter o `Up()`.

---

## 4. Coding Patterns & DI

### Dependency Injection (DI)

* **Constructor Only:** Proibido o uso de `serviceProvider.GetService`.
* **Lifetimes:** * `Scoped`: Contextos de banco e Repositórios (Padrão).
* `Singleton`: Configurações globais imutáveis.
* `Transient`: Cálculos sem estado.

### MediatR (Command/Query)

Controllers devem ser magras (Thin Controllers). Elas apenas repassam a tarefa.

```csharp
// Example in Controller
[HttpPost]
public async Task<IActionResult> AcceptDebt(AcceptDebtCommand command)
{
    var result = await _mediator.Send(command);
    return result.IsSuccess ? Ok() : BadRequest(result.Error);
}

```

---

## 5. File Organization

Dentro de `Syndicate.Domain`, usaremos **Feature Folders**:

* `/Entities`: `User.cs`, `Debt.cs`.
* `/Features/[FeatureName]`:
* `CreateDebtCommand.cs`
* `CreateDebtHandler.cs`
* `CreateDebtValidator.cs`

---

## 6. Golden Rules (Regras de Ouro)

1. **Async All The Way:** Todo método de IO (Banco/API) deve ser `async` e terminar com o sufixo `Async`.
2. **DTO Isolation:** Entidades de banco **nunca** saem na API. Use `Responses` ou `ViewModels`.
3. **Fail Fast:** Valide os dados com `FluentValidation` antes de qualquer lógica de negócio.
4. **Result Pattern:** Métodos de serviço/handlers devem retornar um objeto `Result` (Sucesso/Erro) em vez de lançar exceções para controle de fluxo.
5. **Transactional Integrity:** Operações de Escrow (Dívidas) devem rodar dentro de um `IDbContextTransaction`.
6. **No Try-Catch Walls:** Deixe o `GlobalExceptionMiddleware` capturar erros não tratados. Use log para erros críticos.

---

## 7. Security (JWT)

* **Default Policy:** Toda Controller deve ter o atributo `[Authorize]`.
* **Explicit Public:** Endpoints de Login/Register devem ser marcados com `[AllowAnonymous]`.

---

## 8. Code Review Checklist

Ao revisar um Pull Request, verifique:

* [ ] O nome de alguma variável ou classe está em Português? (Se sim, peça para mudar).
* [ ] Existe lógica de negócio dentro da Controller? (Deve ir para um Handler).
* [ ] A entidade do banco está sendo retornada no JSON da API? (Deve usar DTO).
* [ ] O código é assíncrono do início ao fim?
* [ ] Existe tratamento manual de saldo ou está usando o sistema de saldo definido pelo time (ver [ADR 004](ADR/004-ledger-ao-inves-de-coluna-balance.md))?