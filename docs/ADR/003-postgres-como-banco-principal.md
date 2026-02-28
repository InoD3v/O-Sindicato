# ADR 003 — PostgreSQL como Banco de Dados Principal

## Status

`Aceita`

## Data

2026-02-20

## Contexto

O sistema "O Sindicato" trabalha com um modelo de Ledger (livro-razão) onde a integridade transacional é crítica — Pikas são bloqueadas em escrow e o saldo é calculado pela soma de transações. Precisávamos de um banco robusto que suportasse transações ACID de forma confiável.

## Decisão

Adotar **PostgreSQL 16+** como banco de dados relacional principal, acessado via **Entity Framework Core**. O banco será executado via **Docker** para padronizar o ambiente de desenvolvimento (ver [ADR 006](006-docker-como-ambiente-dev.md)).

## Alternativas Consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| **PostgreSQL (escolhido)** | ACID completo, open source, excelente com EF Core, tipos ricos (JSON, arrays), extensível | Precisa instalar e configurar localmente |
| SQL Server | Integração nativa com .NET, LocalDB para dev | Licenciamento em produção, mais pesado para MVP |
| SQLite | Zero config, um arquivo, ótimo para protótipo | Sem transações concorrentes robustas, limitado para Ledger |
| MongoDB | Flexível, schema-less | Não é ACID por padrão entre documentos, péssimo para Ledger |

## Consequências

### Positivas
- Transações ACID garantem integridade no fluxo de Escrow.
- Tabelas em `snake_case` com convenção do Npgsql/EF Core.
- Migrations versionáveis e reversíveis (`Up()`/`Down()`).
- Gratuito em produção (open source).

### Negativas / Trade-offs
- Configuração inicial um pouco mais complexa que SQLite (mitigado pelo uso de Docker — ver ADR 006).

## Referências

- [PostgreSQL Docs](https://www.postgresql.org/docs/)
- [Npgsql EF Core Provider](https://www.npgsql.org/efcore/)
- [BACKEND.md — Seção 3: Database & EF Core](../BACKEND.md)
