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

## 2. Folder Structure (Arquitetura por Features)

Usaremos **Feature-Based Structure**. Se uma funcionalidade crescer, ela deve ser autossuficiente.

```text
src/
├── components/         # Componentes BURROS de uso geral (Button, Input, Modal, Badge)
│   ├── StatusBadge/
│   │   ├── StatusBadge.tsx       # Componente puro (recebe props, renderiza)
│   │   └── StatusBadge.styles.ts # Styled Components do componente
├── config/             # Configurações de env, axios, rotas
├── features/           # Módulos de negócio
│   ├── debts/
│   │   ├── components/     # Componentes BURROS da feature
│   │   │   ├── DebtCard.tsx
│   │   │   └── DebtCard.styles.ts
│   │   ├── hooks/          # Ex: useDebtActions.ts (lógica)
│   │   ├── services/       # Ex: debtService.ts (chamadas API)
│   │   └── types/          # Ex: debt.types.ts
├── hooks/              # Hooks globais (useAuth, useLocalStorage)
├── pages/              # Views (orquestram componentes + hooks)
│   ├── Home/
│   │   ├── Home.tsx            # View — monta a tela com componentes + hooks
│   │   ├── Home.styles.ts      # Styled Components da View
│   │   ├── Home.test.tsx       # Teste da View
│   │   └── useHome.ts          # Hook da página (lógica local)
├── services/           # Instância da API e helpers globais
└── utils/              # Formatadores (currency, date-fns)
```

### Regra de Pasta

Quando um componente ou página tem **mais de um arquivo** (`.tsx` + `.styles.ts` + `.test.tsx`), ele deve viver **dentro de uma pasta com seu nome**.

```text
# ❌ Errado — arquivos soltos
pages/Home.tsx
pages/Home.styles.ts
pages/Home.test.tsx

# ✅ Correto — pasta própria
pages/Home/Home.tsx
pages/Home/Home.styles.ts
pages/Home/Home.test.tsx
pages/Home/useHome.ts
```

---

## 3. The Three Pillars (Camadas de Responsabilidade)

Para facilitar o Code Review, nenhum arquivo `.tsx` deve ter mais de 150 linhas.

### 3.1 Service (Data) — "O Mensageiro"

Apenas chamadas `axios`. Sem lógica de tratamento, apenas retorno de tipos.

```typescript
// services/healthService.ts
export async function getHealth(): Promise<HealthStatus> {
  const { data } = await api.get<HealthStatus>('/api/health');
  return data;
}
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

```tsx
// components/StatusBadge/StatusBadge.tsx
import { Badge } from './StatusBadge.styles';

interface StatusBadgeProps {
  $healthy: boolean;
  children: React.ReactNode;
}

export function StatusBadge({ $healthy, children }: StatusBadgeProps) {
  return <Badge $healthy={$healthy}>{children}</Badge>;
}
```

> **📖 Padrões de Composição:** Para detalhes sobre como estruturar componentes (compound components, slots, wrappers), veja [ADR 007 — Arquitetura de Componentes e Padrões de Composição](ADR/007-arquitetura-componentes-composicao.md).

### 3.4 View (Page) — "O Maestro"

Orquestra componentes burros e injeta hooks. A View **monta a tela**, mas não contém lógica de negócio nem styled components inline.

```tsx
// pages/Home/Home.tsx
import { useHome } from './useHome';
import { Container, Title, Info } from './Home.styles';
import { StatusBadge } from '../../components/StatusBadge/StatusBadge';

export default function Home() {
  const { health, error, isLoading } = useHome();

  return (
    <Container>
      <Title>O Sindicato</Title>
      {error && <StatusBadge $healthy={false}>Disconnected: {error}</StatusBadge>}
      {health && (
        <>
          <StatusBadge $healthy={health.database}>
            {health.status === 'healthy' ? 'Connected' : 'Disconnected'}
          </StatusBadge>
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

### 🚨 Mobile First

**IMPORTANTE:** Este projeto adota a estratégia **Mobile First**.

* **Estilos Base:** Todos os estilos devem ser escritos primeiro para dispositivos móveis (mobile).
* **Media Queries:** Use media queries para **adicionar** estilos para telas maiores (tablet e desktop), nunca o contrário.
* **Breakpoints:** Use min-width nas media queries, não max-width.

```typescript
// ✅ Correto — Mobile First
export const Container = styled.div`
  padding: 1rem;           // Mobile (base)
  font-size: 0.875rem;     // Mobile (base)

  @media (min-width: 768px) {
    padding: 2rem;         // Tablet
    font-size: 1rem;
  }

  @media (min-width: 1024px) {
    padding: 3rem;         // Desktop
    font-size: 1.125rem;
  }
`;

// ❌ Errado — Desktop First
export const Container = styled.div`
  padding: 3rem;           // Desktop
  
  @media (max-width: 1024px) {
    padding: 2rem;         // Tablet
  }
  
  @media (max-width: 768px) {
    padding: 1rem;         // Mobile
  }
`;
```

**Por que Mobile First?**
- Performance: Dispositivos móveis carregam apenas o CSS necessário.
- Progressivo: Você constrói do mais simples para o mais complexo.
- Tendência moderna: A maioria dos usuários acessa via mobile.

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
* [ ] Os valores monetários estão sendo formatados via `utils`?
* [ ] O formulário possui validação visual de erro para o usuário?
* [ ] O componente é responsivo e segue a estratégia **Mobile First** (estilos base para mobile + media queries com `min-width`)?
* [ ] Foram usados ícones do Lucide de forma consistente?
* [ ] O código passa `bun run lint` sem erros?

---