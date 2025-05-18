
# QuantraLedger

> Um **core-ledger** open-source que combina a **precisão quântica (quantum)** de valores financeiros com a lógica **contra/entry** da contabilidade de dupla-entrada. “Quantra” é um nome curto, memorável e ainda disponível em domínios `.io` e `.dev`, refletindo a ambição de fornecer registros imutáveis, multi-moeda e de alta performance para fintechs modernas.

## Structure
- quantra-ledger/
  ├── .github/
  │   └── workflows/
  │       ├── dotnet.yml
  │       └── load-test.yml
  ├── docs/
  │   ├── architecture-c4.md
  │   ├── backlog.md
  │   ├── cli-tool.md
  │   └── runbook.md
  ├── infra/
  │   └── terraform/
  │       ├── main.tf
  │       ├── observability.tf
  │       ├── rabbitmq.tf
  │       └── rds.tf
  ├── src/
  │   ├── Quantra.Audit/
  │   │   ├── Program.cs
  │   │   ├── TransactionCreatedConsumer.cs
  │   │   └── Quantra.Audit.csproj
  │   ├── Quantra.CLI/
  │   │   ├── Program.cs
  │   │   └── Quantra.CLI.csproj
  │   ├── Quantra.Console/
  │   │   ├── Program.cs
  │   │   ├── _Imports.razor
  │   │   ├── App.razor
  │   │   ├── Pages/
  │   │   │   └── Index.razor
  │   │   ├── Shared/
  │   │   │   └── MainLayout.razor
  │   │   └── Quantra.Console.csproj
  │   ├── Quantra.Domain/
  │   │   ├── LedgerService.cs
  │   │   ├── Models/
  │   │   │   ├── Account.cs
  │   │   │   ├── AssetRate.cs
  │   │   │   ├── AuditLog.cs
  │   │   │   ├── LedgerEntry.cs
  │   │   │   ├── LedgerInstruction.cs
  │   │   │   ├── Organization.cs
  │   │   │   ├── Transaction.cs
  │   │   │   └── User.cs
  │   │   └── Quantra.Domain.csproj
  │   ├── Quantra.Messaging/
  │   │   ├── BusConfigurator.cs
  │   │   ├── TransactionCreatedEvent.cs
  │   │   └── Quantra.Messaging.csproj
  │   ├── Quantra.Onboarding/
  │   │   ├── Program.cs
  │   │   ├── Commands/
  │   │   │   ├── CreateAccount.cs
  │   │   │   ├── CreateAccountHandler.cs
  │   │   │   ├── CreateOrganization.cs
  │   │   │   └── CreateOrganizationHandler.cs
  │   │   └── Quantra.Onboarding.csproj
  │   ├── Quantra.Persistence/
  │   │   ├── LedgerDbContext.cs
  │   │   ├── AccountConfiguration.cs
  │   │   ├── AssetRateConfiguration.cs
  │   │   ├── AuditLogConfiguration.cs
  │   │   ├── OrganizationConfiguration.cs
  │   │   ├── TransactionConfiguration.cs
  │   │   └── UserConfiguration.cs
  │   │   └── Quantra.Persistence.csproj
  │   ├── Quantra.Security/
  │   │   ├── JwtService.cs
  │   │   └── Quantra.Security.csproj
  │   └── Quantra.Transaction/
  │       ├── EfCoreLedgerService.cs
  │       ├── Program.cs
  │       ├── TxDslParser.cs
  │       └── Quantra.Transaction.csproj
  ├── tests/
  │   └── load_tx.js
  ├── Quantra.sln
  └── README.md
