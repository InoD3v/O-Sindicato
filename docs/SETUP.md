# 🔧 Guia de Configuração do Ambiente: Projeto "O Sindicato"

Este documento ensina como preparar sua máquina do **zero** para rodar o projeto localmente. Siga cada seção na ordem.

---

## 1. Ferramentas Necessárias

Instale tudo antes de clonar o repositório.

| Ferramenta | Versão Mínima | Link | Verificar Instalação |
| --- | --- | --- | --- |
| **Git** | 2.40+ | [git-scm.com](https://git-scm.com/) | `git --version` |
| **.NET SDK** | 10.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) | `dotnet --version` |
| **Bun** | Última | [bun.sh](https://bun.sh/) | `bun --version` |
| **Docker** | 24+ | [docker.com](https://www.docker.com/get-started/) | `docker --version` |
| **Docker Compose** | 2.20+ (incluso no Docker Desktop) | — | `docker compose version` |
| **PostgreSQL** | 16+ | [postgresql.org](https://www.postgresql.org/download/) | `psql --version` (ou via Docker) |
| **VS Code** | Última | [code.visualstudio.com](https://code.visualstudio.com/) | — |
| **Visual Studio** | 2022+ (opcional, para .NET) | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) | — |

### Ferramentas Globais do .NET

Após instalar o .NET SDK:

```bash
dotnet tool install --global dotnet-ef
```

Verificar:

```bash
dotnet ef --version
```

---

## 2. Extensões Recomendadas (VS Code)

Instale estas extensões para ter a melhor experiência no projeto:

### Obrigatórias

| Extensão | ID | Motivo |
| --- | --- | --- |
| **C# Dev Kit** | `ms-dotnettools.csdevkit` | IntelliSense e debug para .NET |
| **ESLint** | `dbaeumer.vscode-eslint` | Lint no frontend |
| **Prettier** | `esbenp.prettier-vscode` | Formatação automática |
| **Docker** | `ms-azuretools.vscode-docker` | Gerenciar containers |
| **EditorConfig** | `editorconfig.editorconfig` | Padronização de indentação |

### Recomendadas

| Extensão | ID | Motivo |
| --- | --- | --- |
| **GitLens** | `eamodio.gitlens` | Histórico de Git inline |
| **Thunder Client** | `rangav.vscode-thunder-client` | Testar APIs sem sair do VS Code |
| **Error Lens** | `usernamehw.errorlens` | Mostra erros inline no editor |

### Configuração do VS Code

Ative o **Format on Save** adicionando ao seu `settings.json`:

```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "[csharp]": {
    "editor.defaultFormatter": "ms-dotnettools.csharp"
  }
}
```

---

## 3. Clonar o Repositório

```bash
git clone https://github.com/InoD3v/O-Sindicato.git
cd O-Sindicato
```

Verificar as branches:

```bash
git branch -a
```

Trocar para a branch de desenvolvimento:

```bash
git checkout dev
git pull origin dev
```

---

## 4. Configurar o PostgreSQL (via Docker)

> **Recomendado:** Usar Docker para subir o PostgreSQL. Isso evita instalação local e garante que todos usem a mesma versão.

### 4.1 Subir o PostgreSQL via Docker

```bash
docker run -d \
  --name syndicate-postgres \
  -e POSTGRES_DB=syndicate_dev \
  -e POSTGRES_USER=syndicate_user \
  -e POSTGRES_PASSWORD=sua_senha_aqui \
  -p 5432:5432 \
  postgres:16
```

Verificar se está rodando:
```bash
docker ps
```

### 4.1 (Alternativa) Criar o Banco Manualmente

Se preferir instalar o PostgreSQL localmente, abra o terminal do PostgreSQL:

```sql
CREATE DATABASE syndicate_dev;
CREATE USER syndicate_user WITH PASSWORD 'sua_senha_aqui';
GRANT ALL PRIVILEGES ON DATABASE syndicate_dev TO syndicate_user;
```

### 4.2 Variáveis de Conexão

A string de conexão será usada no backend. Formato padrão:

```
Host=localhost;Port=5432;Database=syndicate_dev;Username=syndicate_user;Password=sua_senha_aqui
```

> **Nunca** suba credenciais reais para o Git. Veja as regras em [GERAL.md](GERAL.md).

---

## 5. Configurar o Backend (.NET)

```bash
cd backend
```

### 5.1 Arquivo de Configuração Local

Copie o arquivo de exemplo e preencha com seus dados locais:

```bash
cp Syndicate.API/appsettings.Example.json Syndicate.API/appsettings.Development.json
```

Edite o `Syndicate.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=syndicate_dev;Username=syndicate_user;Password=sua_senha_aqui"
  },
  "Jwt": {
    "Secret": "um-segredo-local-qualquer-com-pelo-menos-32-caracteres",
    "Issuer": "syndicate-dev",
    "Audience": "syndicate-dev"
  }
}
```

> Este arquivo está no `.gitignore`. Ele é **seu** e nunca será comitado.

### 5.2 Restaurar Dependências

```bash
dotnet restore
```

### 5.3 Rodar Migrations

> **Nota:** Em modo `Development`, a API aplica migrations automaticamente ao iniciar. Este passo só é necessário se você quiser aplicar previamente ou se o auto-migrate falhar.

```bash
dotnet ef database update --project Syndicate.Infrastructure --startup-project Syndicate.API
```

> Se houver erros de conexão, verifique se o PostgreSQL está rodando e se as credenciais estão corretas no `Syndicate.API/appsettings.Development.json` (ou `appsettings.json`).

### 5.4 Rodar a API

```bash
dotnet run --project Syndicate.API
```

A API estará disponível em:
- **HTTP:** `http://localhost:5000`
- **Swagger:** `http://localhost:5000/swagger`

### 5.5 Verificar Lint / Formatação

```bash
dotnet format --verify-no-changes   # verificar sem alterar
dotnet format                       # corrigir automaticamente
```

O backend usa `.editorconfig` + Roslyn analyzers (`Directory.Build.props`) para garantir:
- Code style (file-scoped namespaces, var preferences, formatting)
- Code quality (CA rules: tipos em namespaces, readonly, usings desnecessários)

### 5.6 Rodar os Testes

```bash
dotnet test
```

O projeto `Syndicate.Tests` usa **xUnit**, **NSubstitute** (mocks) e **FluentAssertions** (assertions fluentes). Os testes estão organizados espelhando a estrutura do Domain:

```
Syndicate.Tests/
  Domain/
    Common/       → ResultTests
    Entities/     → DebtTests, UserTests, GroupTests, PollTests, MemberTests, TransactionTests
    Features/
      Auth/        → RegisterHandlerTests, LoginHandlerTests, RegisterValidatorTests, LoginValidatorTests
```

---

## 6. Configurar o Frontend (React 19 + TS + Bun)

Em outro terminal:

```bash
cd frontend
```

### 6.1 Arquivo de Ambiente

Copie o exemplo:

```bash
cp .env.example .env
```

Edite o `.env`:

```env
VITE_API_URL=http://localhost:5000
```

> Este arquivo também está no `.gitignore`.

### 6.2 Instalar Dependências

```bash
bun install
```

### 6.3 Rodar o Dev Server

```bash
bun run dev
```

A aplicação estará disponível em: `http://localhost:5173`

### 6.4 Rodar os Testes

```bash
bun run test        # watch mode (desenvolvimento)
bun run test:run    # single run (CI)
```

O frontend usa **Vitest** + **React Testing Library** + **jest-dom**. O ambiente jsdom simula o browser. Testes ficam co-localizados com os componentes (`*.test.tsx`).

### 6.5 Verificar Lint

```bash
bun run lint        # verificar erros
bun run lint:fix    # corrigir automaticamente o que for possível
```

O frontend usa **ESLint 10** com flat config (`eslint.config.js`). Plugins ativos:
- `typescript-eslint` — regras TypeScript (no `any`, imports tipados)
- `react-hooks` — regras de hooks (deps de useEffect, etc.)
- `react-refresh` — garante que componentes são compatíveis com HMR

---

## 7. Verificação Final (Checklist)

Antes de começar a codar, confirme que tudo funciona:

- [ ] `git --version` retorna 2.40+
- [ ] `dotnet --version` retorna 10.0+
- [ ] `bun --version` retorna versão instalada
- [ ] `docker --version` retorna 24+
- [ ] `dotnet ef --version` retorna versão instalada
- [ ] PostgreSQL está rodando e o banco `syndicate_dev` existe
- [ ] Backend: `dotnet run` sobe sem erros e Swagger abre no browser
- [ ] Frontend: `bun run dev` sobe sem erros e a tela inicial carrega
- [ ] `dotnet test` passa sem falhas
- [ ] `dotnet format --verify-no-changes` não reporta diferenças (backend lint)
- [ ] `bun run test:run` passa sem falhas
- [ ] `bun run lint` passa sem erros

---

## 8. Problemas Comuns

### "dotnet ef não é reconhecido como comando"

```bash
dotnet tool install --global dotnet-ef
```

Se já instalou, verifique se o PATH inclui `~/.dotnet/tools`.

### "Erro de conexão com o banco"

1. Verifique se o PostgreSQL está rodando: `pg_isready`
2. Confirme que o banco existe: `psql -l | grep syndicate_dev`
3. Verifique usuário/senha no `appsettings.Development.json`

### "CORS error no frontend"

Verifique se o `VITE_API_URL` no `.env` aponta para a porta correta do backend e se o backend tem CORS configurado para `http://localhost:5173`.

### "node_modules não está no .gitignore" (Bun também usa node_modules)

Ele deveria estar. Verifique o `.gitignore` na raiz. Se não estiver, **não commite** e avise o time.

### "Port already in use"

Outra instância do backend ou frontend está rodando. Mate o processo:

```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5000
kill -9 <PID>
```

---

## 9. Dia a Dia de Desenvolvimento

### Início do dia

```bash
# 1. Atualize a dev
git checkout dev
git pull origin dev

# 2. Crie sua branch
git checkout -b feat/S1-MODULO-descricao

# 3. Suba o backend
cd backend && dotnet run

# 4. Suba o frontend (outro terminal)
cd frontend && bun run dev
```

### Antes de abrir o PR

```bash
# 1. Rode os testes
cd backend && dotnet test
cd ../frontend && bun run test:run

# 2. Verifique o lint
cd ../backend && dotnet format --verify-no-changes
cd ../frontend && bun run lint

# 3. Atualize com a dev (evitar conflitos)
git checkout dev
git pull origin dev
git checkout feat/sua-branch
git merge dev

# 4. Push e abra o PR
git push origin feat/sua-branch
```

---

> **Documentos relacionados:**
> - [GERAL.md](GERAL.md) — Regras de segurança e variáveis de ambiente
> - [WORKFLOW.md](WORKFLOW.md) — Fluxo de Git e branches
> - [BACKEND.md](BACKEND.md) — Padrões técnicos do .NET
> - [FRONTEND.md](FRONTEND.md) — Padrões técnicos do React/TS
