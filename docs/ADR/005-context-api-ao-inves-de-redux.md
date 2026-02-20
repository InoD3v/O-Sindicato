# ADR 005 — Context API ao Invés de Redux

## Status

`Aceita`

## Data

2026-02-20

## Contexto

O frontend precisa gerenciar estado global (usuário autenticado, tema) e estado local por feature (lista de dívidas, enquetes). Precisávamos decidir entre uma solução robusta como Redux ou algo mais simples para o escopo do MVP.

## Decisão

Adotar **React Context API** para estado global (`AuthContext`, `ThemeContext`) e **useState/hooks locais** para estado de features. Não usar Redux, Zustand ou MobX no MVP.

## Alternativas Consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| **Context API + Hooks (escolhida)** | Nativa do React, zero dependências, simples para MVP | Re-renders em cascata se mal organizado, não escala para estado muito complexo |
| Redux Toolkit | Padrão da indústria, DevTools, middleware | Boilerplate, complexidade desnecessária para o MVP, curva de aprendizado |
| Zustand | Leve, API simples, boa performance | Dependência extra, time precisa aprender mais uma lib |
| MobX | Reatividade automática | Padrão diferente do React idiomático, menor comunidade |

## Consequências

### Positivas
- Zero dependências extras de state management.
- Código simples e direto — fácil para devs iniciantes entenderem.
- Estado local fica dentro da feature (feature hooks) — alinhado com a arquitetura Feature-Based.
- Se precisar escalar no futuro, pode migrar para Zustand sem reescrever tudo.

### Negativas / Trade-offs
- Se o estado global crescer muito, Context pode causar re-renders desnecessários.
- Sem DevTools nativos para debug de estado (mitigado com React DevTools).
- Precisa de disciplina para não criar "God Contexts" com tudo dentro.

## Referências

- [React Docs — Context](https://react.dev/reference/react/useContext)
- [FRONTEND.md — Seção 6: State Management](../FRONTEND.md)
