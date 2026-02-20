# ADR 001 — MediatR como Padrão de Commands/Queries

## Status

`Aceita`

## Data

2026-02-20

## Contexto

O backend do "O Sindicato" utiliza .NET 10 (última versão estável) com Layered Architecture. Precisávamos definir como as Controllers se comunicam com a camada de negócio. O risco era criar Controllers "gordas" com lógica de negócio espalhada, dificultando testes e code review.

## Decisão

Adotar **MediatR** para implementar o padrão Command/Query (CQRS simplificado). Controllers apenas recebem o request e delegam via `_mediator.Send(command)`.

## Alternativas Consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| **MediatR (escolhida)** | Thin Controllers, handlers testáveis isoladamente, pipeline behaviors (validação, logging) | Dependência extra, curva de aprendizado para quem não conhece |
| Services injetados diretamente | Mais simples de entender, menos abstração | Controllers tendem a acumular lógica, difícil de padronizar |
| Minimal APIs sem pattern | Zero overhead de abstração | Não escala para a complexidade do Escrow e Ledger |

## Consequências

### Positivas
- Controllers ficam com 3-5 linhas por action (fácil de revisar).
- Cada handler é testável unitariamente sem HTTP.
- FluentValidation se integra via Pipeline Behavior.
- Feature Folders ficam organizados: Command + Handler + Validator por feature.

### Negativas / Trade-offs
- Desenvolvedores novos precisam entender o conceito de Mediator.
- Mais arquivos por feature (Command, Handler, Validator).

## Referências

- [MediatR GitHub](https://github.com/jbogard/MediatR)
- [BACKEND.md — Seção 4: MediatR](../BACKEND.md)
