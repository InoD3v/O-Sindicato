# ADR 006 — Docker como Ambiente de Desenvolvimento

## Status

`Aceita`

## Data

2026-02-20

## Contexto

O projeto "O Sindicato" envolve múltiplos serviços (backend .NET 10, frontend React 19, PostgreSQL). Precisávamos garantir que todos os desenvolvedores trabalhassem com o mesmo ambiente, evitando problemas de "funciona na minha máquina" e simplificando o onboarding de novos membros.

## Decisão

Adotar **Docker** e **Docker Compose** para containerizar os serviços de infraestrutura (banco de dados, etc.) e facilitar o setup do ambiente de desenvolvimento. No futuro, poderemos containerizar também o backend e o frontend conforme a necessidade.

## Alternativas Consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| **Docker + Docker Compose (escolhido)** | Ambiente reproduzível, setup rápido, isolamento, padrão da indústria | Curva de aprendizado inicial, consumo de recursos |
| Instalação local manual | Controle total, sem overhead de container | "Works on my machine", setup demorado, versões diferentes entre devs |
| Vagrant | VMs completas, isolamento total | Pesado, lento para iniciar, overkill para o escopo |
| Dev Containers (VS Code) | Ambiente 100% containerizado para dev | Pode ser lento dependendo do hardware, mais complexo de configurar |

## Consequências

### Positivas
- PostgreSQL sobe com um comando (`docker compose up`), sem instalar nada localmente.
- Todos usam a mesma versão do banco, mesma configuração.
- Onboarding de novos devs fica muito mais rápido.
- Prepara o projeto para deploy com containers no futuro.

### Negativas / Trade-offs
- Docker Desktop precisa estar instalado e rodando.
- Consumo adicional de memória/CPU no ambiente de desenvolvimento.
- Desenvolvedores precisam entender conceitos básicos de containers.

## Referências

- [Docker Docs](https://docs.docker.com/)
- [Docker Compose Docs](https://docs.docker.com/compose/)
- [SETUP.md — Seção 4: PostgreSQL via Docker](../SETUP.md)
