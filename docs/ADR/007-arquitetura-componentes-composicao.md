# ADR 007 — Arquitetura de Componentes e Padrões de Composição

## Status

`Aceita`

## Data

2026-02-24

## Contexto

O frontend do "O Sindicato" usa React 19 + TypeScript + Styled Components. Precisamos definir padrões claros de como os componentes devem ser estruturados, compostos e reutilizados para manter consistência e facilitar manutenção.

Questões a responder:
- Como estruturar componentes para máxima reutilização?
- Usar Design Atômico formal ou uma abordagem mais pragmática?
- Como compor componentes: `children`, `slots`, compound components, ou render props?
- Como lidar com variações de componentes (variants)?

## Decisão

Adotamos uma **arquitetura pragmática baseada em "Componentes Burros"** (Dumb Components) com composição via `children` e props tipadas, sem a rigidez formal do Design Atômico.

### Princípios Fundamentais

1. **Componentes Burros (Presentational Components)**
   - Componentes em `src/components/` são **stateless** e **logic-free**
   - Recebem tudo via props
   - Não fazem chamadas de API, não têm `useEffect`, não fazem cálculos complexos
   - São testáveis em isolamento via Storybook ou testes de snapshot

2. **Composição via `children` + Props**
   - Padrão primário: aceitar `children: React.ReactNode` para conteúdo flexível
   - Props específicas para comportamento (ex.: `variant`, `size`, `disabled`)
   - Evitar "god components" com 20+ props — se tiver muitas variações, considere quebrar em sub-componentes
   - **Componentes reutilizáveis devem ter estados abrangentes** (ex: Button com `isLoading`, `disabled`, `startIcon`, `endIcon`)

3. **Slots para Customização Pontual**
   - Use slots (`footer?: React.ReactNode`) para partes customizáveis de componentes estruturais
   - Quando um caso específico se repete, crie um wrapper ao invés de poluir o componente base com variantes

4. **Separação Visual vs Lógica**
   - Componentes visuais (`src/components/`) não sabem de contextos de negócio
   - Lógica de negócio fica em hooks (`src/hooks/`, `src/features/{feature}/hooks/`)
   - Componentes de feature (`src/features/{feature}/components/`) podem ser levemente acoplados ao domínio, mas ainda sem lógica interna

5. **Pragmatismo sobre Purismo**
   - Componentes base = máxima reutilização + estados completos
   - Componentes estruturais = slots para customização
   - Casos específicos repetidos = criar wrappers
   - Foco em DX (Developer Experience) e manutenibilidade

## Padrões de Composição

### Pattern 1: Children (Padrão Default)

Usar para conteúdo textual ou elementos simples.

```tsx
// components/Button/Button.tsx
interface ButtonProps {
  variant?: 'primary' | 'secondary' | 'danger';
  size?: 'sm' | 'md' | 'lg';
  disabled?: boolean;
  onClick?: () => void;
  children: React.ReactNode;
}

export function Button({ variant = 'primary', size = 'md', children, ...props }: ButtonProps) {
  return (
    <StyledButton $variant={variant} $size={size} {...props}>
      {children}
    </StyledButton>
  );
}

// Uso
<Button variant="primary">Salvar</Button>
```

### Pattern 2: Compound Components (Para componentes complexos)

Usar quando um componente tem sub-partes com semântica clara (ex: Modal, Card, Tabs).

```tsx
// components/Card/Card.tsx
export const Card = ({ children }: { children: React.ReactNode }) => (
  <CardContainer>{children}</CardContainer>
);

Card.Header = ({ children }: { children: React.ReactNode }) => (
  <CardHeader>{children}</CardHeader>
);

Card.Body = ({ children }: { children: React.ReactNode }) => (
  <CardBody>{children}</CardBody>
);

Card.Footer = ({ children }: { children: React.ReactNode }) => (
  <CardFooter>{children}</CardFooter>
);

// Uso
<Card>
  <Card.Header>Título da Dívida</Card.Header>
  <Card.Body>50 Pikas - Trazer coca-cola</Card.Body>
  <Card.Footer>
    <Button>Aceitar</Button>
  </Card.Footer>
</Card>
```

### Pattern 3: Render Props (Casos Específicos)

Apenas quando precisar customizar rendering interno baseado em estado.

```tsx
interface DataListProps<T> {
  data: T[];
  renderItem: (item: T) => React.ReactNode;
}

export function DataList<T>({ data, renderItem }: DataListProps<T>) {
  return <List>{data.map(renderItem)}</List>;
}

// Uso
<DataList data={debts} renderItem={(debt) => <DebtCard key={debt.id} debt={debt} />} />
```

### Pattern 4: Slot Props (Para Customização Específica de Partes)

Usar `slotProps` quando um componente tem partes fixas que podem precisar de customização específica em alguns contextos. Ideal para componentes como Modal, Dialog, Drawer onde há estrutura fixa mas conteúdo variável.

**Regra de Ouro:** Se o componente é **altamente reutilizável** (ex: Button, Input), use props diretas e estados abrangentes. Se é **estrutural com partes customizáveis** (ex: Modal, Dialog), slotProps é aceitável.

```tsx
// components/Modal/Modal.tsx
interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title?: string;
  children: React.ReactNode;
  footer?: React.ReactNode; // Slot para footer customizável
  footerVariant?: 'default' | 'actions' | 'centered'; // Variantes pré-definidas
}

export function Modal({ 
  isOpen, 
  onClose, 
  title, 
  children, 
  footer,
  footerVariant = 'default'
}: ModalProps) {
  if (!isOpen) return null;
  
  return (
    <ModalOverlay onClick={onClose}>
      <ModalContainer onClick={(e) => e.stopPropagation()}>
        {title && <ModalHeader>{title}</ModalHeader>}
        <ModalBody>{children}</ModalBody>
        {footer && (
          <ModalFooter $variant={footerVariant}>
            {footer}
          </ModalFooter>
        )}
      </ModalContainer>
    </ModalOverlay>
  );
}

// Uso 1: Footer customizado via slot
<Modal 
  isOpen={true} 
  onClose={handleClose}
  title="Confirmar Dívida"
  footer={
    <>
      <Button variant="ghost" onClick={handleClose}>Cancelar</Button>
      <Button variant="danger" onClick={handleDelete}>Excluir</Button>
    </>
  }
  footerVariant="actions"
>
  <p>Tem certeza que deseja excluir esta dívida?</p>
</Modal>

// Uso 2: Wrapper específico para casos repetidos
function ConfirmDeleteModal({ isOpen, onClose, onConfirm, itemName }: ConfirmDeleteModalProps) {
  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Confirmar Exclusão"
      footer={
        <>
          <Button variant="ghost" onClick={onClose}>Cancelar</Button>
          <Button variant="danger" onClick={onConfirm}>Excluir</Button>
        </>
      }
      footerVariant="actions"
    >
      <p>Tem certeza que deseja excluir <strong>{itemName}</strong>?</p>
    </Modal>
  );
}
```

**Quando usar slotProps:**
- ✅ Componentes estruturais (Modal, Dialog, Drawer, Popover)
- ✅ Quando há uma parte fixa que pode ter conteúdo variável (header, footer, actions)
- ✅ Quando criar variantes para todos os casos seria inviável

**Quando NÃO usar slotProps:**
- ❌ Componentes primitivos/base (Button, Input, Badge)
- ❌ Quando props diretas são suficientes
- ❌ Para configurar sub-partes com muitos detalhes (estilo Material-UI `slotProps={{ icon: { color, size, variant } }}`)

## Variantes e Temas

### Variantes via Props

Usar union types para variantes finitas e conhecidas.

```tsx
interface BadgeProps {
  variant: 'success' | 'error' | 'warning' | 'info';
  children: React.ReactNode;
}

const variantColors = {
  success: '#16a34a',
  error: '#dc2626',
  warning: '#f59e0b',
  info: '#3b82f6'
};

export const StyledBadge = styled.span<{ $variant: BadgeProps['variant'] }>`
  background-color: ${({ $variant }) => variantColors[$variant]};
`;
```

### Tema Global via ThemeProvider

Para tokens de design (cores, spacing, tipografia) que devem ser consistentes.

```tsx
// config/theme.ts
export const theme = {
  colors: {
    primary: '#6366f1',
    secondary: '#8b5cf6',
    success: '#16a34a',
    error: '#dc2626',
    // ...
  },
  spacing: (factor: number) => `${factor * 0.25}rem`, // 4px base
  breakpoints: {
    mobile: '640px',
    tablet: '768px',
    desktop: '1024px'
  }
};

// main.tsx
<ThemeProvider theme={theme}>
  <App />
</ThemeProvider>

// Uso em styled components
const Container = styled.div`
  color: ${({ theme }) => theme.colors.primary};
  padding: ${({ theme }) => theme.spacing(4)}; // 1rem = 16px
`;
```

## Organização e Nomenclatura

### Níveis de Componentes (Inspiração no Design Atômico, mas SEM rigidez)

Não vamos forçar categorização em Atoms/Molecules/Organisms, mas usaremos essa intuição:

| Nível | Descrição | Exemplos | Localização |
|-------|-----------|----------|-------------|
| **Base** | Componentes primitivos, sem sub-partes | `Button`, `Input`, `Badge`, `Icon` | `src/components/` |
| **Compostos** | Componentes com sub-partes | `Card`, `Modal`, `Dropdown`, `Tabs` | `src/components/` |
| **Feature** | Componentes específicos de domínio | `DebtCard`, `PollVoteForm`, `MemberBalance` | `src/features/{feature}/components/` |
| **Pages** | Orquestradores de tela | `Home`, `DebtsList`, `PollDetails` | `src/pages/` |

### Estrutura de Pasta

```
src/
├── components/
│   ├── Button/
│   │   ├── Button.tsx         # Componente
│   │   ├── Button.styles.ts   # Styled components
│   │   ├── Button.test.tsx    # Testes
│   │   └── index.ts           # Re-export limpo
│   ├── Card/
│   │   ├── Card.tsx           # Compound component
│   │   ├── Card.styles.ts
│   │   ├── Card.test.tsx
│   │   └── index.ts
├── features/
│   ├── debts/
│   │   ├── components/
│   │   │   ├── DebtCard/
│   │   │   │   ├── DebtCard.tsx
│   │   │   │   ├── DebtCard.styles.ts
│   │   │   │   └── DebtCard.test.tsx
```

## Alternativas Consideradas

| Alternativa | Prós | Contras | Por que não? |
|-------------|------|---------|--------------|
| **Design Atômico Rígido** | Estrutura clara, bem documentada | Overhead de categorização, debates sobre "isso é átomo ou molécula?" | Burocracia desnecessária para MVP |
| **Headless UI + Radix** | Acessibilidade out-of-the-box, lógica abstraída | Curva de aprendizado, dependência externa | Styled Components já resolve nosso caso, React 19 melhora a11y nativamente |
| **Slots Complexos (MUI style)** | Flexibilidade extrema | Complexidade de API, DX ruim com TypeScript | Slots simples (React.ReactNode) cobrem 90% dos casos sem essa complexidade |
| **Render Props para tudo** | Máxima customização | JSX verboso, difícil de ler | Usar apenas quando necessário |

## Consequências

### Positivas
- Componentes fáceis de entender e testar
- Composição natural via `children` — idiomática no React
- Escalável: adicionar novos componentes não quebra arquitetura
- Compound components dão flexibilidade sem explodir props
- Slots permitem customização pontual sem criar 50 variantes
- ThemeProvider centraliza design tokens
- Pragmático: adapta-se ao nível de reutilização necessário

### Negativas / Trade-offs
- Sem "caminho único" — devs precisam escolher entre children, compound components, render props, slots
- Falta rigidez do Design Atômico pode gerar inconsistência se não houver code review
- Slots podem ser mal usados (ex: em componentes primitivos) se não houver guidelines claras
- ThemeProvider tem leve overhead de runtime (aceitável)

### Mitigações
- Code Review valida padrões de composição
- Documentar exemplos de cada padrão no Storybook (ADR futura)
- Regra: "Se tiver dúvida, use `children` primeiro"
- Guideline clara: slots apenas para componentes estruturais, não primitivos
- Quando um caso específico se repete 3+ vezes, criar wrapper ao invés de mais props

## Convenções Obrigatórias

1. **Props de Estilo:** Prefixo `$` para transient props no styled-components (ex: `$variant`, `$size`)
2. **Children sempre tipado:** `children: React.ReactNode` ou mais específico se necessário
3. **Variants finitas:** Use union types (`'primary' | 'secondary'`), nunca string solto
4. **Export limpo:** Todo componente deve ter `index.ts` que exporta apenas o componente, não os styled components internos
5. **Mobile First:** Estilos base para mobile, `@media (min-width: ...)` para telas maiores

## Exemplo Completo: Button Component

```tsx
// components/Button/Button.tsx
import { ButtonStyled, IconWrapper } from './Button.styles';
import type { ReactNode } from 'react';

export interface ButtonProps {
  variant?: 'primary' | 'secondary' | 'danger' | 'ghost';
  size?: 'sm' | 'md' | 'lg';
  disabled?: boolean;
  isLoading?: boolean;
  startIcon?: ReactNode;
  endIcon?: ReactNode;
  onClick?: () => void;
  children: ReactNode;
  type?: 'button' | 'submit' | 'reset';
}

export function Button({
  variant = 'primary',
  size = 'md',
  disabled = false,
  isLoading = false,
  startIcon,
  endIcon,
  children,
  type = 'button',
  ...rest
}: ButtonProps) {
  return (
    <ButtonStyled
      $variant={variant}
      $size={size}
      disabled={disabled || isLoading}
      type={type}
      {...rest}
    >
      {startIcon && <IconWrapper $position="start">{startIcon}</IconWrapper>}
      {isLoading ? 'Loading...' : children}
      {endIcon && <IconWrapper $position="end">{endIcon}</IconWrapper>}
    </ButtonStyled>
  );
}
```

```tsx
// components/Button/Button.styles.ts
import styled from 'styled-components';

const variantStyles = {
  primary: `
    background-color: #6366f1;
    color: #ffffff;
    &:hover:not(:disabled) { background-color: #4f46e5; }
  `,
  secondary: `
    background-color: #8b5cf6;
    color: #ffffff;
    &:hover:not(:disabled) { background-color: #7c3aed; }
  `,
  danger: `
    background-color: #dc2626;
    color: #ffffff;
    &:hover:not(:disabled) { background-color: #b91c1c; }
  `,
  ghost: `
    background-color: transparent;
    color: #6366f1;
    border: 1px solid #6366f1;
    &:hover:not(:disabled) { background-color: rgba(99, 102, 241, 0.1); }
  `
};

const sizeStyles = {
  sm: `
    padding: 0.5rem 1rem;
    font-size: 0.875rem;
  `,
  md: `
    padding: 0.75rem 1.5rem;
    font-size: 1rem;
  `,
  lg: `
    padding: 1rem 2rem;
    font-size: 1.125rem;
  `
};

export const ButtonStyled = styled.button<{
  $variant: 'primary' | 'secondary' | 'danger' | 'ghost';
  $size: 'sm' | 'md' | 'lg';
}>`
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  border: none;
  border-radius: 0.375rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;

  ${({ $variant }) => variantStyles[$variant]}
  ${({ $size }) => sizeStyles[$size]}

  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  /* Mobile First */
  @media (min-width: 768px) {
    /* Ajustes para tablet se necessário */
  }
`;

export const IconWrapper = styled.span<{ $position: 'start' | 'end' }>`
  display: inline-flex;
  align-items: center;
`;
```

```tsx
// components/Button/index.ts
export { Button } from './Button';
export type { ButtonProps } from './Button';
```

## Checklist de Code Review para Componentes

### Componentes Base/Primitivos (Button, Input, Badge, etc.)
- [ ] Componente é stateless (sem `useState`, `useEffect`)?
- [ ] Props são tipadas com interface exportada?
- [ ] Variantes usam union types, não strings soltas?
- [ ] Tem estados abrangentes necessários (loading, disabled, icons, etc.)?
- [ ] **NÃO** usa slots — apenas props diretas?
- [ ] Transient props no styled-components têm prefixo `$`?
- [ ] Componente aceita `children` quando faz sentido?
- [ ] Estilos estão em arquivo `.styles.ts` separado?
- [ ] Componente tem `index.ts` com export limpo?
- [ ] Estilos seguem Mobile First?
- [ ] Não há lógica de negócio no componente?

### Componentes Estruturais (Modal, Dialog, Drawer, etc.)
- [ ] Usa slots (`footer?: React.ReactNode`) para partes customizáveis?
- [ ] Tem variantes pré-definidas para casos comuns (`footerVariant`)?
- [ ] Slots são opcionais e têm fallback razoável?
- [ ] Documentação clara de quando criar wrapper vs usar slot?

### Wrappers Específicos
- [ ] Wrapper resolve caso que se repete 3+ vezes?
- [ ] Wrapper usa componente base/estrutural internamente?
- [ ] Props do wrapper são específicas ao caso de uso?

## Referências

- [FRONTEND.md](../FRONTEND.md) — The Three Pillars (Service, Hook, Component, View)
- [ADR 002](002-styled-components-como-default-de-design.md) — Styled Components
- [React Docs — Composition vs Inheritance](https://react.dev/learn/passing-props-to-a-component#passing-jsx-as-children)
- [Compound Components Pattern](https://kentcdodds.com/blog/compound-components-with-react-hooks)
