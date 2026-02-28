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

1. **Rich Domain Model:** Entidades **não são sacolas de dados**. Devem conter métodos que representam comportamentos de negócio com guards/validações internas. Setters são `private set` e mutações acontecem apenas através de métodos públicos que garantem consistência. Entidades imutáveis por design (ex: `Transaction`, `Vote`) são aceitáveis apenas com factory `Create()`.
2. **Async All The Way:** Todo método de IO (Banco/API) deve ser `async` e terminar com o sufixo `Async`.
3. **DTO Isolation:** Entidades de banco **nunca** saem na API. Use `Responses` ou `ViewModels`.
4. **Fail Fast:** Valide os dados com `FluentValidation` antes de qualquer lógica de negócio.
5. **Result Pattern:** Métodos de serviço/handlers devem retornar um objeto `Result` (Sucesso/Erro) em vez de lançar exceções para controle de fluxo.
6. **Transactional Integrity:** Operações de Escrow (Dívidas) devem rodar dentro de um `IDbContextTransaction`.
7. **No Try-Catch Walls:** Deixe o `GlobalExceptionMiddleware` capturar erros não tratados. Use log para erros críticos.

### Rich vs Anemic

```csharp
// ❌ Anêmico — sacola de dados, lógica mora no Handler
public class Debt {
    public DebtStatus Status { get; set; }
}

// Handler faz tudo:
if (debt.Status != DebtStatus.Pending)
    return Result.Failure("...");
debt.Status = DebtStatus.Active;  // setter público — qualquer um muda

// ✅ Rico — entidade protege seu próprio estado
public class Debt {
    public DebtStatus Status { get; private set; }

    public Result Accept() {
        if (Status != DebtStatus.Pending)
            return Result.Failure("Debt can only be accepted when pending.");
        Status = DebtStatus.Active;
        return Result.Success();
    }
}

// Handler apenas delega:
var result = debt.Accept();
```

> **Regra:** Toda vez que um código fora da entidade fizer `entity.Property = valor`, pare e crie um método na entidade.

---

## 7. Security (JWT)

* **Default Policy:** Toda Controller deve ter o atributo `[Authorize]`.
* **Explicit Public:** Endpoints de Login/Register devem ser marcados com `[AllowAnonymous]`.

---

## 8. Lint & Formatting

O backend utiliza **Roslyn Analyzers** + **`.editorconfig`** para garantir estilo e qualidade de código. Não usamos ferramentas externas — tudo roda via SDK do .NET.

### Configuração

| Arquivo | Papel |
| --- | --- |
| `.editorconfig` (raiz do repo) | Regras de estilo C# (var, namespaces, formatting, diagnostics) |
| `backend/Directory.Build.props` | Habilita analyzers em todos os projetos (`EnableNETAnalyzers`, `AnalysisLevel=latest-recommended`, `EnforceCodeStyleInBuild`) |

### Regras Importantes

* **File-scoped namespaces** — obrigatório (`namespace X;` em vez de bloco `namespace X { }`).
* **Remoção de usings desnecessários** (`IDE0005`) — warning em build.
* **Readonly fields** (`IDE0044`) — warning se um campo privado pode ser `readonly`.
* **var preferences** — usar `var` quando o tipo é aparente; tipo explícito caso contrário.
* **CA1707 suprimido no projeto de testes** — permite underscores em nomes de métodos de teste (`Metodo_Cenario_Resultado`).

### Executar

```bash
cd backend

# Verificar (não altera arquivos) — use no CI
dotnet format --verify-no-changes

# Corrigir automaticamente
dotnet format
```

> **PR Rule:** Todo Pull Request deve passar `dotnet format --verify-no-changes` antes do merge.

---

## 9. Testing (Testes)

O projeto `Syndicate.Tests` usa **xUnit** como framework, **NSubstitute** para mocks e **FluentAssertions** (v7.x — Apache 2.0) para assertions fluentes.

### Estrutura

Os testes espelham a estrutura do Domain:

```
Syndicate.Tests/
  Domain/
    Common/          → ResultTests
    Entities/        → DebtTests, UserTests, GroupTests, PollTests, MemberTests, TransactionTests
    Features/
      Auth/          → RegisterHandlerTests, LoginHandlerTests, RegisterValidatorTests, LoginValidatorTests
```

### Convenções

1. **Naming:** `MetodoTestado_CenarioOuEstado_ResultadoEsperado` (ex: `Accept_WhenPending_ShouldTransitionToActive`).
2. **AAA:** Arrange → Act → Assert.
3. **Mocks com NSubstitute:** Substitua interfaces nos testes de Handler — nunca dependa de infraestrutura real.
4. **Validators:** Use `TestValidate()` do FluentValidation para testar validators isoladamente.
5. **Novos Handlers:** Sempre criar testes de handler + validator ao implementar uma nova feature.

### Executar

```bash
cd backend
dotnet test
```

---

## 10. Code Review Checklist

Ao revisar um Pull Request, verifique:

* [ ] O nome de alguma variável ou classe está em Português? (Se sim, peça para mudar).
* [ ] Existe lógica de negócio dentro da Controller? (Deve ir para um Handler).
* [ ] Existe mutação direta de propriedade fora da entidade? (Ex: `debt.Status = ...` — deve ser `debt.Accept()`).
* [ ] A entidade do banco está sendo retornada no JSON da API? (Deve usar DTO).
* [ ] O código é assíncrono do início ao fim?
* [ ] Existe tratamento manual de saldo ou está usando o sistema de saldo definido pelo time (ver [ADR 004](ADR/004-ledger-ao-inves-de-coluna-balance.md))?
* [ ] O código passa `dotnet format --verify-no-changes` sem erros?