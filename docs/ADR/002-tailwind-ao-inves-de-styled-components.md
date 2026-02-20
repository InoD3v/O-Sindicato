# ADR 002 — Estratégia de Estilização do Frontend

## Status

`Em Discussão`

## Data

2026-02-20

## Contexto

O frontend usa React 19 + TypeScript. Precisamos escolher uma estratégia de estilização que seja rápida de desenvolver, fácil de revisar em PRs e que não gere conflitos de CSS global. **Esta decisão ainda não foi tomada** — estamos avaliando as opções abaixo antes de iniciar o desenvolvimento do frontend.

## Decisão

**Pendente.** A equipe ainda está avaliando qual abordagem de estilização adotar. As opções em consideração estão listadas abaixo.

## Alternativas em Avaliação

| Alternativa | Prós | Contras |
| --- | --- | --- |
| **Tailwind CSS** | Zero CSS custom, responsividade fácil (`sm:`, `md:`), sem conflitos globais, code review visual | Classes longas no JSX, curva inicial para memorizar utilitários |
| **Styled Components** | CSS-in-JS com escopo, dinâmico por props | Bundle maior, mais verboso, runtime overhead |
| **CSS Modules** | Escopo isolado, CSS puro | Mais arquivos, difícil manter design system consistente |
| **Sass/SCSS global** | Familiar para maioria | Conflitos de seletores, difícil escalar em time |

## Critérios de Decisão

A escolha será baseada em:
- Facilidade de onboarding para o time.
- Qualidade do code review (o diff mostra claramente o estilo?).
- Performance em build e runtime.
- Compatibilidade com React 19 e ecossistema atual.

## Consequências (a definir após decisão)

Serão documentadas quando a decisão for tomada. Este ADR será atualizado com status `Aceita` e as consequências positivas/negativas da opção escolhida.

## Referências

- [Tailwind CSS Docs](https://tailwindcss.com/docs)
- [Styled Components Docs](https://styled-components.com/docs)
- [CSS Modules Docs](https://github.com/css-modules/css-modules)
- [FRONTEND.md](../FRONTEND.md)
