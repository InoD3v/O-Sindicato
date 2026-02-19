# 🎨 Frontend Standards: Project "The Syndicate" (React + TS)

Este guia define os padrões para o desenvolvimento do ecossistema front-end do "The Syndicate". Foco em: **Desacoplamento, Performance e Tipagem Estrita.**

---

## 1. Naming & Language (Idioma e Nomenclatura)

**Regra de Ouro: Código em Inglês, Negócio em Inglês.**

* **Variables/Functions:** `camelCase` (ex: `const [isModalOpen, setIsModalOpen]`).
* **Components/Interfaces/Types:** `PascalCase` (ex: `DebtCard.tsx`, `UserPayload`).
* **Files:** Nome do componente em `PascalCase` ou `camelCase` para utilitários.
* **CSS/Tailwind:** Seguir o padrão de utilitários ou `kebab-case` se usar módulos.

---

## 2. Folder Structure (Arquitetura por Features)

Usaremos **Feature-Based Structure**. Se uma funcionalidade crescer, ela deve ser autossuficiente.

```text
src/
├── components/     # UI de uso geral (Button, Input, Modal, Badge)
├── config/         # Configurações de env, axios, rotas
├── features/       # Módulos de negócio
│   ├── debts/      # Ex: Feature de Dívidas
│   │   ├── components/ # Ex: DebtList.tsx, DebtActionButtons.tsx
│   │   ├── hooks/      # Ex: useDebtActions.ts
│   │   ├── services/   # Ex: debtService.ts
│   │   └── types/      # Ex: debt.types.ts
├── hooks/          # Hooks globais (useAuth, useLocalStorage)
├── pages/          # Componentes de rota (Views completas)
├── services/       # Instância da API e helpers globais
└── utils/          # Formatadores (currency, date-fns)

```

---

## 3. The Three Pillars (Camadas de Responsabilidade)

Para facilitar o Code Review, nenhum arquivo `.tsx` deve ter mais de 150 linhas.

1. **Service (Data):** Apenas chamadas `axios`. Sem lógica de tratamento, apenas retorno de tipos.
2. **Hook (Logic):** Onde o `useEffect`, `useState` e validações residem. É o "Cérebro".
3. **View (UI):** Onde o JSX e o Tailwind residem. É o "Corpo".

---

## 4. Styling: Tailwind CSS

Utilizaremos **Tailwind CSS** para evitar arquivos CSS gigantes e seletores globais.

* **Padrão:** Use classes utilitárias diretamente no JSX.
* **Complexidade:** Se uma lista de classes ficar muito grande, quebre o componente em partes menores em vez de criar variáveis de string para classes.
* **Icons:** Use apenas **Lucide React**. Ex: `<Users className="w-5 h-5" />`.

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

* **Instance:** Localizada em `src/services/api.ts`.
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

## 9. Code Review Checklist (Frontend)

* [ ] O componente está em inglês?
* [ ] Existe lógica de `fetch` ou `map` pesado dentro do `.tsx` da View? (Deveria estar no Hook).
* [ ] Os valores monetários estão sendo formatados via `utils`?
* [ ] O formulário possui validação visual de erro para o usuário?
* [ ] O componente é responsivo (usa classes `sm:`, `md:`, `lg:` do Tailwind)?
* [ ] Foram usados ícones do Lucide de forma consistente?

---