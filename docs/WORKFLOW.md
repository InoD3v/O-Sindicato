# 🌿 Guia de Git e Workflow: Projeto "O Sindicato"

Este guia define como o código deve fluir das máquinas dos desenvolvedores até o ambiente de produção. O objetivo é garantir **rastreabilidade**, **qualidade via revisão por pares** e **estabilidade**.

---

## 1. Estrutura de Branches (Hierarquia)

Adotamos um fluxo baseado em `main` e `dev`.

* **`main` (Produção):** Contém o código estável que já foi testado.
* *Regra:* **Bloqueada.** Proibido subir commit direto. Só recebe código vindo da `dev`.

* **`dev` (Desenvolvimento):** Branch principal de trabalho da sprint. É aqui que integramos as funcionalidades novas.
* *Regra:* Base para todas as novas `feat/` e `fix/`.

* **`feat/` (Funcionalidades):** Para desenvolver novas tarefas do MVP.
* **`fix/` (Correções):** Para corrigir bugs encontrados durante o desenvolvimento na `dev`.
* **`hotfix/` (Emergência):** Correções críticas que precisam ir direto para a `main` (produção).

---

## 2. Nomenclatura de Branches

Para manter a organização, toda branch deve seguir este padrão:
`tipo/[sprint]-[modulo]-[descricao-curta]`

* **Exemplos:**
* `feat/S1-DIVIDAS-logica-escrow`
* `fix/S1-UI-alinhamento-botao`
* `feat/S1-VOTACAO-calculo-peso`

> **Nota:** As branches devem ser **curtas**. pós o merge bem-sucedido, delete a sua branch para manter o projeto limpo.

---

## 3. Padrão de Commits (Conventional Commits)

Os prefixos ajudam a entender o que foi feito sem precisar ler o código. Use sempre em inglês o tipo, mas pode descrever em português se preferir (ou manter tudo em inglês para treinar):

* `feat:` Nova funcionalidade.
* `fix:` Correção de bug.
* `docs:` Mudanças apenas na documentação.
* `style:` Formatação, pontos e vírgulas (não afeta o código).
* `refactor:` Mudança no código que não altera comportamento (limpeza).

**Exemplo:** `feat: S1-VOTACAO: implementa cálculo de voto por pikas`

---

## 4. O Ciclo de Vida da Tarefa

1. **Sincronizar:** Garanta que sua `dev` local está atualizada (`git pull origin dev`).
2. **Criar Branch:** Saia sempre da `dev` (`git checkout -b feat/S1-LOGIN-google`).
3. **Codar:** Siga os padrões de C# e React definidos nos outros documentos.
4. **Subir:** Envie sua branch para o servidor (`git push origin feat/...`).
5. **Pull Request (PR):** Abra um pedido de integração da sua branch para a `dev`.

---

## 5. Regras de Code Review (Revisão de Código)

* **Revisão Obrigatória:** Todo PR **precisa** de pelo menos 1 aprovação de um colega que não participou daquela tarefa.
* **Proibido Auto-Merge:** Você nunca aprova o seu próprio trabalho.
* **Foco da Revisão:**
* O código segue os padrões do projeto?
* Existem nomes de variáveis em português ou genéricos (ex: `coisa`, `info`)?
* A lógica de "Pikas" e "Escrow" está consistente?

---

## 6. Template de Pull Request

Copie e cole este padrão na descrição de todo PR que abrir:

```markdown
## 📝 Descrição
O que foi feito nesta tarefa? (Ex: Implementada a trava de saldo no aceite da dívida).

## 🛠️ O que foi alterado?
- [ ] Adicionada tabela de Ledger no banco.
- [ ] Criado Hook `useDebt` no front.

## 🧪 Como testar?
1. Rode as migrations.
2. Tente aceitar uma dívida sem ter saldo de Pikas.
3. Verifique se o erro "Saldo Insuficiente" aparece.

## ✅ Checklist
- [ ] Meu código segue os padrões do projeto.
- [ ] Não deixei `console.log` ou comentários de teste.
- [ ] A branch foi criada a partir da `dev`.

```

---