# ADR 004 — Estratégia de Saldo: Ledger vs Coluna Balance

## Status

`Em Discussão`

## Data

2026-02-20

## Contexto

O sistema de Pikas precisa rastrear saldo de usuários dentro de grupos com operações de bloqueio (Escrow). Precisamos decidir como representar e calcular o saldo: uma coluna `balance` atualizada diretamente ou um sistema de transações imutáveis (Ledger). **Esta decisão ainda não foi tomada** — estamos avaliando os trade-offs antes de iniciar a implementação.

## Decisão

**Pendente.** A equipe ainda está avaliando qual abordagem adotar para o controle de saldo. As opções estão listadas abaixo.

## Alternativas em Avaliação

| Alternativa | Prós | Contras |
| --- | --- | --- |
| **Ledger / Transações** | Auditável, rastreável, imutável, seguro para concorrência | Query de saldo é um `SUM()`, complexidade inicial maior |
| **Coluna Balance direta** | Query simples (`SELECT balance`), fácil de entender | Race conditions em concorrência, sem histórico, difícil auditar, vulnerável a inconsistências |
| **Event Sourcing completo** | Máxima rastreabilidade | Overkill para o MVP, complexidade absurda |

## Critérios de Decisão

A escolha será baseada em:
- Complexidade aceitável para o MVP vs robustez necessária.
- Necessidade real de auditoria completa no escopo atual.
- Impacto na performance para o volume esperado de transações.
- Facilidade de implementação para o time.

## Consequências (a definir após decisão)

Serão documentadas quando a decisão for tomada. Este ADR será atualizado com status `Aceita` e as consequências positivas/negativas da opção escolhida.

## Referências

- [Martin Fowler — Accounting Patterns](https://martinfowler.com/eaaDev/AccountingNarrative.html)
- [MVP_REGRAS_NEGOCIOS_v1.0.md — BR03](../MVP_REGRAS_NEGOCIOS_v1.0.md)
- [BACKEND.md](../BACKEND.md)
