# Relatórios de Implementação - Módulo de Vendas

Este documento resume as implementações realizadas no módulo de Vendas (`Sales`), mapeando as funcionalidades entregues
em relação aos requisitos e padrões arquiteturais estabelecidos.

## 1. Arquitetura e Padrões

O desenvolvimento seguiu estritamente os princípios de **Clean Architecture** e **DDD (Domain-Driven Design)** já
existentes no projeto.

* **Camadas**:
    * `Domain`: Entidades (`Sale`, `SaleItem`), Enums (`SaleStatus`), Eventos (`SaleCreated`, `SaleModified`),
      Interfaces (`ISaleRepository`).
    * `Application`: Casos de uso (Handlers), DTOs, Mapeamentos (AutoMapper), Validações (FluentValidation - com
      validação externa de usuário).
    * `ORM`: Implementação do Repositório com Entity Framework Core.
    * `WebApi`: Controllers, ViewModels, Tratamento de `PaginatedList`.
* **Padrões**:
    * **CQS/CQRS**: Uso de `MediatR` para separação de Comandos e Queries.
    * **Repository Pattern**: Isolamento do acesso a dados.
    * **Soft Delete**: Exclusão lógica implementada via método `Cancel()` e atualização de status.
    * **Notification Pattern**: Publicação de eventos de domínio (`SaleCreatedEvent`, `SaleModifiedEvent`) para
      desacoplamento.

## 2. Requisitos Funcionais Implementados

### 2.1. Criação de Venda (`POST /api/sales`)

* **Validação de Usuário**: Verificação se o `CustomerId` existe no serviço de usuários (Padrão External Identity).
* **Regras de Desconto**:
    *   [x] Compras acima de 4 itens idênticos: **10% de desconto**.
    *   [x] Compras entre 10 e 20 itens idênticos: **20% de desconto**.
    *   [x] **Restrição**: Não é permitido vender mais de 20 itens idênticos.
    *   [x] **Restrição**: Não é permitido vender menos de 4 itens (sem desconto, lógica padrão aplicada).
* **Cálculo**: Total da venda calculado automaticamente somando os subtotais dos itens.
* **Eventos**: Disparo de `SaleCreatedEvent` após sucesso.

### 2.2. Atualização de Venda (`PUT /api/sales/{id}`)

* **Re-cálculo**: Ao alterar itens, todos os totais e descontos são recalculados.
* **Eventos**: Disparo de `SaleModifiedEvent` com os dados atualizados.
* **Auditabilidade**: Atualização do campo `UpdatedAt`.

### 2.3. Consulta de Venda (`GET /api/sales/{id}`)

* Retorna os detalhes completos da venda, incluindo itens, descontos aplicados e status atual.

### 2.4. Listagem de Vendas (`GET /api/sales`)

* **Paginação**: Implementada suportando parâmetros `_page`, `_size` e `_order`.
* **Arquitetura**: Utiliza `GetSalesResult` no Application e mapeia para `PaginatedList` na WebApi, seguindo o padrão de
  separação de responsabilidades.

### 2.5. Cancelamento/Exclusão (`DELETE /api/sales/{id}`)

* **Soft Delete**: Implementado através do método `Cancel()`.
* **Integridade**: O método físico `DeleteAsync` foi removido do repositório para garantir que vendas nunca sejam
  apagadas fisicamente, apenas canceladas.

### 2.6. Validation Refactoring

I identified that ValidationBehavior was already registered in IoC, but it wasn't being used. I removed the
manual/explicit validation from the Controllers and Handlers to ensure the DRY principle and correctly utilize the
MediatR Pipeline, centralizing error handling in the existing Middleware.

## 3. Melhorias e Refatorações (Nível Sênior)

* **SaleStatus Enum**: Substituição de flags booleanas por um Enum (`Pending`, `Completed`, `Cancelled`) para melhor
  gestão de estado.
* **Refatoração PaginatedList**: Remoção de duplicidades e restauração do padrão original na camada `WebApi`.
* **Limpeza de Código**: Remoção de variáveis não utilizadas e imports desnecessários.

## 4. Testes e Qualidade

* **Testes Unitários**: Cobertura de testes implementada no projeto `Ambev.DeveloperEvaluation.Unit`.
    * `SaleHandlerTests`: Validação dos fluxos de criação, atualização e deleção.
    * `SaleRepositoryTests`: Validação da persistência.
    * **Total**: 58 Testes passando (0 falhas).

## 5. Documentação da API

* Arquivo gerado: `.doc/sales-api.md` contendo exemplos de JSON para Request/Response de todos os endpoints.
