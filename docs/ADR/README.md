# 📐 Architecture Decision Records (ADR)

Este diretório contém os **registros de decisões arquiteturais** do projeto "O Sindicato". Toda decisão técnica significativa deve ser documentada aqui para evitar re-discussões e dar contexto para novos membros.

---

## O que é um ADR?

Um ADR é um documento curto que registra **por que** uma decisão técnica foi tomada. Não precisa ser longo — o importante é o contexto e a justificativa.

---

## Quando Criar um ADR?

Crie um ADR sempre que:

- Escolher uma **tecnologia ou biblioteca** sobre outra (Ex: "Por que Tailwind e não Styled Components?")
- Definir um **padrão arquitetural** (Ex: "Por que Layered Architecture e não Clean Architecture?")
- Mudar uma **decisão anterior** (Ex: "Migrar de Context API para Zustand")
- Tomar uma decisão que **afeta todo o time** ou muda a forma de trabalhar

---

## Template

Use o arquivo [000-template.md](000-template.md) como base para cada novo ADR.

### Nomenclatura

`NNN-titulo-kebab-case.md`

Exemplos:
- `001-usar-mediator-no-backend.md`
- `002-tailwind-ao-inves-de-styled-components.md`
- `003-postgres-como-banco-principal.md`

---

## Índice de ADRs

| # | Título | Status | Data |
| --- | --- | --- | --- |
| 001 | [MediatR como padrão de Commands/Queries](001-mediator-como-padrao-cqrs.md) | ✅ Aceita | 2026-02-20 |
| 002 | [Estratégia de Estilização do Frontend](002-tailwind-ao-inves-de-styled-components.md) | 🟡 Em Discussão | 2026-02-20 |
| 003 | [PostgreSQL como banco principal](003-postgres-como-banco-principal.md) | ✅ Aceita | 2026-02-20 |
| 004 | [Estratégia de Saldo: Ledger vs Coluna Balance](004-ledger-ao-inves-de-coluna-balance.md) | 🟡 Em Discussão | 2026-02-20 |
| 005 | [Context API ao invés de Redux](005-context-api-ao-inves-de-redux.md) | ✅ Aceita | 2026-02-20 |
| 006 | [Docker como ambiente de desenvolvimento](006-docker-como-ambiente-dev.md) | ✅ Aceita | 2026-02-20 |

---

> Novos ADRs devem ser adicionados ao índice acima e seguir o template.
