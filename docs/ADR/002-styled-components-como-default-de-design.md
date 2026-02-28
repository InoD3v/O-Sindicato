# ADR 002 — Styled Components como Estratégia de Estilização

## Status

`Aceita`

## Data

2026-02-23

## Contexto

O frontend usa React 19 + TypeScript. Precisamos escolher uma estratégia de estilização que seja rápida de desenvolver, fácil de revisar em PRs e que não gere conflitos de CSS global.

## Decisão

Adotar **Styled Components (v6+)** como estratégia de estilização do frontend. Todos os estilos devem ser escritos como componentes estilizados usando tagged template literals.

## Alternativas Consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| **Styled Components (escolhida)** | CSS-in-JS com escopo automático, dinâmico por props, co-localizado com o componente, boa DX com TypeScript | Bundle ligeiramente maior, runtime overhead (aceitável para o MVP) |
| Tailwind CSS | Zero CSS custom, responsividade fácil (`sm:`, `md:`), sem conflitos globais | Classes longas no JSX, curva de memorização de utilitários, menos familiar para o time |
| CSS Modules | Escopo isolado, CSS puro | Mais arquivos, difícil manter design system consistente |
| Sass/SCSS global | Familiar para maioria | Conflitos de seletores, difícil escalar em time |

## Consequências

### Positivas
- Escopo automático — zero conflitos de CSS entre componentes.
- Estilos dinâmicos via props com tipagem TypeScript (ex: `$healthy: boolean`).
- Código co-localizado: estilo e componente vivem no mesmo arquivo ou pasta.
- Fácil de fazer code review — o diff mostra claramente o que mudou.
- Já instalado e em uso no projeto (`styled-components ^6.1.0`).

### Negativas / Trade-offs
- Bundle ligeiramente maior que CSS puro/Tailwind (aceitável para MVP).
- Runtime overhead na geração de estilos (imperceptível para a escala do projeto).
- Props de estilo devem usar prefixo `$` (transient props) para não vazar para o DOM.

## Convenções

1. **Transient Props:** Use prefixo `$` para props que só existem para estilização (ex: `$healthy`, `$isActive`).
2. **Co-localização:** Componentes estilizados pequenos podem ficar no mesmo arquivo `.tsx`. Se crescerem, extraia para um arquivo `styles.ts` na mesma pasta.
3. **Naming:** Componentes estilizados seguem `PascalCase` (ex: `const StatusBadge = styled.span<...>`).
4. **Tema:** Se necessário, usar `ThemeProvider` do styled-components para tokens de design (cores, espaçamentos).
5. **Mobile First:** Todos os estilos devem ser escritos primeiro para mobile. Use media queries com `min-width` para adicionar estilos para telas maiores (ver [FRONTEND.md](../FRONTEND.md) para detalhes).

## Referências

- [Styled Components Docs](https://styled-components.com/docs)
- [Styled Components — Transient Props](https://styled-components.com/docs/api#transient-props)
- [FRONTEND.md](../FRONTEND.md)
- [ADR 007 — Arquitetura de Componentes e Padrões de Composição](007-arquitetura-componentes-composicao.md)
