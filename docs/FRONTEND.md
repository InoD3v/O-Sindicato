# 🎨 Frontend Standards: Project "The Syndicate" (React 19 + TS)

Este guia define os padrões para o desenvolvimento do ecossistema front-end do "The Syndicate" utilizando **React 19** (última versão estável). Foco em: **Desacoplamento, Performance e Tipagem Estrita.**

---

## 1. Naming & Language (Idioma e Nomenclatura)

**Regra de Ouro: Código em Inglês, Negócio em Inglês.**

* **Variables/Functions:** `camelCase` (ex: `const [isModalOpen, setIsModalOpen]`).
* **Components/Interfaces/Types:** `PascalCase` (ex: `DebtCard.tsx`, `UserPayload`).
* **Files:** Nome do componente em `PascalCase` ou `camelCase` para utilitários.
* **CSS/Styled Components:** Componentes estilizados em `PascalCase` (ex: `StatusBadge`, `Container`). Props de estilo com prefixo `$` (ex: `$healthy`). Ver [ADR 002](ADR/002-styled-components-como-default-de-design.md).

---

## 2. Folder Structure (Arquitetura por Features + Atomic Design)

Usaremos **Feature-Based Structure** combinada com **Atomic Design** na camada de componentes globais.

```text
src/
├── components/           # Componentes BURROS globais (Atomic Design)
│   ├── atoms/            # Elementos indivisíveis (Button, Input, Badge, Text)
│   │   └── Badge/
│   │       ├── Badge.tsx
│   │       └── Badge.styles.ts
│   ├── molecules/        # Combinações de Atoms (FormField, SearchBar, NavItem)
│   │   └── FormField/
│   │       ├── FormField.tsx
│   │       └── FormField.styles.ts
│   └── organisms/        # Seções complexas de UI (Header, Sidebar, DebtTable)
│       └── Header/
│           ├── Header.tsx
│           └── Header.styles.ts
├── config/               # Configurações de env, axios, rotas
├── features/             # Módulos de negócio
│   ├── home/
│   │   ├── pages/        # Páginas da feature
│   │   │   └── Home/
│   │   │       ├── Home.tsx          # View — monta a tela
│   │   │       ├── Home.styles.ts    # Styled Components da View
│   │   │       ├── Home.test.tsx     # Teste da View
│   │   │       └── useHome.ts        # Hook da página (lógica local)
│   │   ├── services/     # Ex: healthService.ts (chamadas API)
│   │   └── types/        # Ex: health.types.ts
│   ├── debts/
│   │   ├── components/   # Componentes BURROS da feature (qualquer nível atômico)
│   │   │   └── DebtCard/
│   │   │       ├── DebtCard.tsx
│   │   │       └── DebtCard.styles.ts
│   │   ├── hooks/        # Ex: useDebtActions.ts (lógica)
│   │   ├── pages/        # Ex: DebtsPage/DebtsPage.tsx
│   │   ├── services/     # Ex: debtService.ts (chamadas API)
│   │   └── types/        # Ex: debt.types.ts
├── hooks/                # Hooks globais (useAuth, useLocalStorage)
├── pages/                # ⚠️ Barrel — apenas re-exporta páginas das features
│   └── index.ts            # export { Home } from '@/features/home/pages/Home/Home'
├── routes/               # Configuração de rotas
│   ├── index.ts            # Re-exporta AppRoutes e PrivateRoute
│   ├── AppRoutes.tsx       # Todas as <Route> da aplicação
│   └── PrivateRoute.tsx    # Wrapper para rotas autenticadas
├── services/             # Instância da API e helpers globais
└── utils/                # Formatadores (currency, date-fns)
```

### Regra de Pages

Páginas vivem **dentro de sua feature** em `features/<feature>/pages/`. A pasta `src/pages/` é apenas um **barrel** (arquivo `index.ts`) que re-exporta as páginas. As rotas (`App.tsx`) importam **somente** do barrel.

```typescript
// src/pages/index.ts  — barrel (NEVER put components here)
export { default as Home } from '@/features/home/pages/Home/Home';
export { default as DebtsPage } from '@/features/debts/pages/DebtsPage/DebtsPage';
// ... demais features
```

```tsx
// App.tsx — delega para AppRoutes
import { AppRoutes } from '@/routes';

function App() {
  return <AppRoutes />;
}
```

### Rotas Privadas

Use `PrivateRoute` para proteger rotas que exigem autenticação:

```tsx
// routes/AppRoutes.tsx
import { Routes, Route } from 'react-router-dom';
import { PrivateRoute } from './PrivateRoute';
import { Home, DebtsPage } from '@/pages';

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/debts" element={
        <PrivateRoute>
          <DebtsPage />
        </PrivateRoute>
      } />
    </Routes>
  );
}
```

> **Regra:** `App.tsx` apenas renderiza `<AppRoutes />`. Toda lógica de rotas fica em `src/routes/`.

### Atomic Design — Níveis

Os componentes globais (`src/components/`) seguem a classificação do **Atomic Design**:

| Nível | O que é | Exemplos | Regra |
| --- | --- | --- | --- |
| **Atom** | Menor unidade de UI, indivisível | `Button`, `Input`, `Badge`, `Text`, `Spinner`, `Icon` | Sem dependência de outro componente do projeto |
| **Molecule** | Combinação de 2+ Atoms com propósito único | `FormField` (Label + Input + Error), `SearchBar` (Input + Button) | Composta apenas de Atoms |
| **Organism** | Seção complexa da UI, combina Molecules e/ou Atoms | `Header`, `Sidebar`, `DataTable`, `PollCard` | Pode importar Atoms e Molecules |

> **Regra de Importação:** Atoms não importam nada. Molecules importam Atoms. Organisms importam Molecules e/ou Atoms. **Nunca** importe de cima para baixo.

> **Dentro de Features:** Os componentes em `features/<feature>/components/` podem ser de qualquer nível atômico, mas seguem as mesmas regras de "burro" (zero lógica, zero state). A classificação atoms/molecules/organisms **fica apenas na pasta global** `src/components/` para evitar over-engineering nas features.

### Regra de Pasta

Quando um componente ou página tem **mais de um arquivo** (`.tsx` + `.styles.ts` + `.test.tsx`), ele deve viver **dentro de uma pasta com seu nome**.

```text
# ❌ Errado — arquivos soltos
features/home/pages/Home.tsx
features/home/pages/Home.styles.ts
features/home/pages/Home.test.tsx

# ✅ Correto — pasta própria
features/home/pages/Home/Home.tsx
features/home/pages/Home/Home.styles.ts
features/home/pages/Home/Home.test.tsx
features/home/pages/Home/useHome.ts
```

---

## 3. The Three Pillars (Camadas de Responsabilidade)

Para facilitar o Code Review, nenhum arquivo `.tsx` deve ter mais de 150 linhas.

### 3.1 Service (Data) — "O Mensageiro"

Apenas chamadas `axios`. Sem lógica de tratamento, apenas retorno de tipos.

```typescript
// features/home/services/healthService.ts
export async function getHealth(): Promise<HealthStatus> {
  const { data } = await api.get<HealthStatus>('/api/health');
  return data;
}
```

> **CQRS no Service:** Separe **sempre** as funções de **Leitura** (Queries) das de **Escrita** (Commands/Mutations) no mesmo Service. Isso organiza o código por intenção e facilita manutenção. Hoje usamos `useState` + `useEffect` nos hooks; quando adotarmos uma lib de cache (React Query/SWR), a separação já estará pronta.

```typescript
// features/debts/services/debtService.ts

// ─── Queries (Leitura) ──────────────────────────────
export async function getDebts(): Promise<Debt[]> { ... }
export async function getDebtById(id: string): Promise<Debt> { ... }

// ─── Commands (Escrita) ─────────────────────────────
export async function createDebt(data: CreateDebtPayload): Promise<Debt> { ... }
export async function settleDebt(id: string): Promise<void> { ... }
```

### 3.2 Hook (Logic) — "O Cérebro"

Onde `useEffect`, `useState`, validações e chamadas ao Service residem. **Toda a inteligência fica aqui.**
O hook retorna apenas dados prontos e callbacks — a View nunca sabe como os dados são obtidos.

```typescript
// pages/Home/useHome.ts
export function useHome() {
  const [health, setHealth] = useState<HealthStatus | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getHealth()
      .then(setHealth)
      .catch((err) => setError(err.message));
  }, []);

  return { health, error, isLoading: !health && !error };
}
```

### 3.3 Component (Dumb) — "O Tijolo"

Componentes **burros**: recebem props, renderizam UI. **Zero lógica, zero estado, zero side-effects.**
São reutilizáveis e testáveis em isolamento.

Em `src/components/` seguem a classificação **Atomic Design** (ver Seção 2): `atoms/` → `molecules/` → `organisms/`.
Dentro de features (`features/<feature>/components/`) ficam sem sub-pastas atômicas.

```tsx
// components/atoms/Badge/Badge.tsx
import { BadgeWrapper } from './Badge.styles';

interface BadgeProps {
  $variant: 'success' | 'danger';
  children: React.ReactNode;
}

export function Badge({ $variant, children }: BadgeProps) {
  return <BadgeWrapper $variant={$variant}>{children}</BadgeWrapper>;
}
```

#### Regra de Composição: Prefira Slots

Prefira props que recebem `ReactNode` (slots) em vez de passar múltiplas props de dados. Isso mantém o componente genérico e evita acoplamento.

```tsx
// ❌ Errado — componente acoplado a dados e callbacks específicos
<UserCard name={user.name} onEdit={handleEdit} onDelete={handleDelete} />

// ✅ Correto — componente recebe slots genéricos
<UserCard actions={<UserActions onEdit={handleEdit} onDelete={handleDelete} />} />
```

### 3.4 View (Page) — "O Maestro"

Orquestra componentes burros e injeta hooks. A View **monta a tela**, mas não contém lógica de negócio nem styled components inline.

```tsx
// features/home/pages/Home/Home.tsx
import { useHome } from './useHome';
import { Container, Title, Info } from './Home.styles';
import { Badge } from '@/components/atoms/Badge/Badge';

export default function Home() {
  const { health, error, isLoading } = useHome();

  return (
    <Container>
      <Title>O Sindicato</Title>
      {error && <Badge $variant="danger">Disconnected: {error}</Badge>}
      {health && (
        <>
          <Badge $variant={health.database ? 'success' : 'danger'}>
            {health.status === 'healthy' ? 'Connected' : 'Disconnected'}
          </Badge>
          <Info>Database: {health.database ? 'Online' : 'Offline'}</Info>
        </>
      )}
      {isLoading && <Info>Checking connection...</Info>}
    </Container>
  );
}
```

### Resumo Visual

```
Service  →  Hook  →  View  ←  Component (burro)
 (API)     (lógica)  (monta)    (renderiza)
```

> **Regra de Ouro:** Se você está escrevendo `useState`, `useEffect` ou `try/catch` dentro de um `.tsx` de View ou Component, **pare e mova para um Hook.**

---

## 4. Styling (Styled Components)

Usamos **Styled Components v6+** como estratégia de estilização — ver [ADR 002](ADR/002-styled-components-como-default-de-design.md).

### Regra Principal: Arquivo Separado

Styled Components **sempre** ficam em um arquivo `.styles.ts` separado, nunca dentro do `.tsx`.

```text
# ❌ Errado — styled component dentro do .tsx
Home.tsx  (contém const Container = styled.div`...`)

# ✅ Correto — arquivo separado
Home.tsx         (importa de Home.styles.ts)
Home.styles.ts   (exporta Container, Title, Info, etc.)
```

Exemplo de arquivo `.styles.ts`:

```typescript
// pages/Home/Home.styles.ts
import styled from 'styled-components';

export const Container = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  min-height: 100vh;
`;

export const Title = styled.h1`
  font-size: 2.5rem;
  margin-bottom: 1rem;
`;
```

### Transient Props

Use o prefixo `$` para props que só servem para estilização e não devem ser repassadas ao DOM.

```typescript
export const Badge = styled.span<{ $healthy: boolean }>`
  background-color: ${({ $healthy }) => ($healthy ? '#16a34a' : '#dc2626')};
`;
```

### Outras Regras

* **Naming:** Componentes estilizados seguem `PascalCase` (ex: `StatusBadge`, `Container`, `Title`).
* **Icons:** Use apenas **Lucide React**. Estilize tamanho via props do Lucide (`size`) ou styled-components.
* **Responsividade:** Garantir que todos os componentes funcionem em diferentes tamanhos de tela. Use media queries dentro dos styled components.
* **Componentes pequenos:** Se a estilização de um componente ficar muito extensa, quebre em componentes menores.

---

## 5. Forms & Validation (React Hook Form + Zod)

Para alinhar com a **FluentValidation** do C#, usaremos **Zod** no front-end.

* **Regra:** Todo formulário deve ter um `schema` de validação definido com Zod.
* **Biblioteca:** `react-hook-form` com `@hookform/resolvers/zod`.
* **Benefício:** Tipagem automática dos campos do formulário baseada no schema.

```typescript
const debtSchema = z.object({
  amount: z.number().min(1, "Value must be positive"),
  description: z.string().min(5, "Description too short")
});

```

---

## 6. State Management (Context API)

Não usaremos Redux por enquanto para manter o MVP simples.

* **Global:** `AuthContext` (usuário logado) e `ThemeContext`.
* **Local:** Estados específicos de cada Feature devem ser resolvidos com `useState` dentro dos hooks da feature ou, se necessário, um Contexto local à pasta da feature.

---

## 7. API Communication (Axios)

* **Instance:** Localizada em `src/services/api.ts`. Usamos **Bun** como runtime e package manager.
* **Interceptors:**
* `onRequest`: Anexa o `Bearer Token` do localStorage.
* `onResponse`: Captura erros `401` para deslogar o usuário e `403` para avisar sobre falta de permissão.

---

## 8. TypeScript Best Practices

* **No Any:** O uso de `any` causará reprovação imediata no Code Review.
* **Interfaces vs Types:**
  * Use `interface` para definições de objetos globais e entidades.
  * Use `type` para uniões (ex: `type Status = 'open' | 'closed'`).
* **No Enums — Use `as const`:** Enums do TypeScript são proibidos. Use objetos `as const` + tipo derivado. O ESLint bloqueia `enum` automaticamente.

```typescript
// ❌ Proibido — enum gera código JS desnecessário e não é tree-shakeable
enum DebtStatus {
  Pending = 'PENDING',
  Active = 'ACTIVE',
  Settled = 'SETTLED',
}

// ✅ Correto — "as const" object + derived type
const DEBT_STATUS = {
  Pending: 'PENDING',
  Active: 'ACTIVE',
  Settled: 'SETTLED',
} as const;

type DebtStatus = (typeof DEBT_STATUS)[keyof typeof DEBT_STATUS];
// Result type: 'PENDING' | 'ACTIVE' | 'SETTLED'
```

* **Generics:** Use em componentes de lista ou inputs genéricos para manter a flexibilidade.

---

## 9. Lint (ESLint)

Usamos **ESLint 10** com **flat config** (`eslint.config.js`) e suporte nativo a TypeScript via `typescript-eslint`.

### Plugins Ativos

| Plugin | Papel |
| --- | --- |
| `@eslint/js` | Regras base recomendadas do ESLint |
| `typescript-eslint` | Regras TypeScript (extends `recommended`) |
| `eslint-plugin-react-hooks` | Garante regras dos Hooks (deps de `useEffect`, etc.) |
| `eslint-plugin-react-refresh` | Valida que apenas componentes são exportados para HMR funcionar |

### Regras Importantes

| Regra | Nível | Motivo |
| --- | --- | --- |
| `@typescript-eslint/no-explicit-any` | **error** | Zero `any` — reprovação imediata |
| `@typescript-eslint/consistent-type-imports` | warn | Prefira `import type { X }` quando possível |
| `@typescript-eslint/no-unused-vars` | warn | Ignora variáveis com prefixo `_` |
| `no-console` | warn | Permite apenas `console.warn` e `console.error` |
| `eqeqeq` | error | Sempre `===`, nunca `==` |
| `no-duplicate-imports` | error | Imports duplicados do mesmo módulo |
| `prefer-const` | warn | Use `const` quando variável não é reatribuída |
| `no-restricted-syntax` (TSEnumDeclaration) | **error** | Enums proibidos — use `as const` objects |
| `@typescript-eslint/naming-convention` | warn | camelCase vars, PascalCase types, sem prefixo `I` em interfaces |
| `max-lines` | warn | Máximo 150 linhas por arquivo (ignora comentários/linhas em branco) |
| `no-warning-comments` | warn | Avisa sobre TODO/HACK/FIXME esquecidos |
| `no-restricted-imports` (../../) | error | Proíbe imports relativos profundos — use `@/` alias |
| `no-var` | error | Proíbe `var` — use `const`/`let` |
| `react-hooks/exhaustive-deps` | error | Deps faltando em useEffect são bugs |

### Executar

```bash
cd frontend

# Verificar (retorna erros no terminal)
bun run lint

# Corrigir automaticamente
bun run lint:fix
```

> **PR Rule:** Todo Pull Request deve passar `bun run lint` sem erros antes do merge.

---

## 10. Testing (Testes)

Usamos **Vitest** (integração nativa com Vite) + **React Testing Library** + **jest-dom** matchers.

### Stack

| Pacote | Papel |
| --- | --- |
| `vitest` | Test runner (configurado em `vite.config.ts`) |
| `@testing-library/react` | Renderizar e interagir com componentes |
| `@testing-library/jest-dom` | Matchers extras (`toBeInTheDocument`, etc.) |
| `@testing-library/user-event` | Simular interações reais do usuário |
| `jsdom` | Ambiente DOM para rodar testes fora do browser |

### Convenções

1. **Co-localização:** Testes ficam junto ao componente —`Home.tsx` → `Home.test.tsx`.
2. **Globals:** `describe`, `it`, `expect`, `vi` estão disponíveis globalmente (config `globals: true`).
3. **Mocks:** Use `vi.mock()` para mockar módulos (ex: api). Use `vi.fn()` para funções individuais.
4. **Naming:** `describe('ComponentName', () => { it('describes behavior', ...) })`.
5. **Sem detalhes de implementação:** Teste **o que o usuário vê**, não estado interno ou hooks.

### Executar

```bash
bun run test        # watch mode (desenvolvimento)
bun run test:run    # single run (CI)
bun run test:coverage  # com cobertura
```

---

## 11. Code Review Checklist (Frontend)

* [ ] O componente está em inglês?
* [ ] Existe `useState`, `useEffect` ou lógica dentro de um `.tsx` de View ou Component? (Deve estar num Hook).
* [ ] Existem styled components definidos dentro de um `.tsx`? (Devem estar em `.styles.ts`).
* [ ] O componente recebe dados via props (burro) ou busca dados sozinho? (Componentes devem ser burros).
* [ ] Componentes globais estão na pasta atômica correta (`atoms/`, `molecules/`, `organisms/`)?
* [ ] Services separam Queries de Commands (CQRS)?
* [ ] Páginas estão dentro de `features/<feature>/pages/` e re-exportadas no barrel `src/pages/index.ts`?
* [ ] Os valores monetários estão sendo formatados via `utils`?
* [ ] O formulário possui validação visual de erro para o usuário?
* [ ] O componente é responsivo?
* [ ] Foram usados ícones do Lucide de forma consistente?
* [ ] O código passa `bun run lint` sem erros?

---