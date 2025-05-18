
# C4 Context Diagram (text)

System: Quantra Payment Ledger

Containers:
- Transaction API (ASP.NET) -- writes to Postgres, publishes RabbitMQ events
- Onboarding API (ASP.NET) -- user/org management
- Audit Service (Worker) -- consumes events, writes audit_logs
- Console (Blazor) -- reads Postgres for reporting
- Postgres RDS
- RabbitMQ
- Observability Stack (Prometheus, Tempo, Loki, Grafana)

Relationships:
User -> Console -> Postgres (read)
Services emit metrics/traces -> Observability
