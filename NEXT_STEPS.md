# Próximos Passos e Melhorias Sugeridas

Este documento delineia melhorias identificadas que agregariam valor ao projeto, demonstrando uma visão Sênior de
arquitetura, qualidade e escalabilidade, respeitando as restrições e padrões originais do teste.

## 1. Segurança e Controle de Acesso

- **Autenticação e Autorização**: Atualmente, os endpoints de `Sales` estão públicos.
    - *Sugestão*: Adicionar o atributo `[Authorize]` no `SalesController` ou nos métodos individuais.
    - *Valor*: Garante que apenas usuários autenticados possam criar ou visualizar vendas.
- **Validação de Permissões (RBAC)**:
    - *Sugestão*: Implementar verificação de Roles (ex: `[Authorize(Roles = "Manager,Admin")]`) para operações sensíveis
      como Cancelamento de Venda.

## 2. Qualidade e Testes Automatizados

- **Testes de Integração (E2E)**:
    - *Sugestão*: Criar testes usando `Microsoft.AspNetCore.Mvc.Testing` e `Testcontainers`.
    - *Valor*: Valida o fluxo completo (API -> App -> Infra -> Banco) em um ambiente controlado, garantindo que a
      injeção de dependência e o mapeamento do banco estão perfeitos.
- **Testes de Carga**:
    - *Sugestão*: Adicionar scripts básicos (ex: k6) para validar a performance da criação de vendas concorrentes.

## 3. Observabilidade e Monitoramento

- **Logging Estruturado**:
    - *Sugestão*: Enriquecer os logs no `CreateSaleHandler` e `UpdateSaleHandler` com propriedades estruturadas (ex:
      `SaleId`, `CustomerId`) usando Serilog, facilitando buscas futuras.
- **Métricas**:
    - *Sugestão*: Expor métricas de negócio (ex: "Vendas Criadas por Hora", "Total de Descontos Concedidos") usando
      OpenTelemetry + Prometheus.

## 4. Resiliência e Performance

- **Idempotência**:
    - *Sugestão*: Implementar um mecanismo de Idempotência no endpoint `POST /sales` usando uma chave única enviada pelo
      cliente (Header `Idempotency-Key`).
    - *Valor*: Evita duplicação de vendas em caso de falhas de rede.
- **Cache**:
    - *Sugestão*: Implementar Cache (Redis) no endpoint `GET /sales/{id}` para vendas finalizadas/antigas que raramente
      mudam.

## 5. Documentação

- **Swagger Examples**:
    - *Sugestão*: Adicionar exemplos ricos de Request/Response no Swagger (usando `Swashbuckle.AspNetCore.Filters`).
    - *Valor*: Facilita muito o consumo da API por outros desenvolvedores/front-ends.

## 6. Arquitetura

- **Notification Pattern**:
    - *Sugestão*: Evoluir o retorno de erros de validação acumulando notificações de domínio ao invés de lançar exceções
      para validações de regra de negócio (se o padrão do projeto permitir).

## 7. CI/CD

- **Github Actions**:
    - *Sugestão*: Adicionar arquivo `.github/workflows/ci.yml` para buildar e rodar testes automaticamente a cada
      Push/PR.

---
> **Nota**: Estas melhorias foram documentadas mas não implementadas para manter o escopo alinhado estritamente ao
> solicitado no teste, demonstrando respeito aos requisitos iniciais (YAGNI - You Aren't Gonna Need It Yet).
