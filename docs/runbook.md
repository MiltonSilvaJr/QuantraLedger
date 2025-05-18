
# Quantra Runbook

## Services
| Service | Port | Purpose |
|---------|------|---------|
| Transaction API | 5000 | Core ledger transactions |
| Onboarding API | 5000 | CRUD Org/Account + login |
| Audit Service | 5000 | Event consumer |
| Console | 5000 | Web dashboard |

## Routine Operations
* **Database migrations**: `midaz migrate`
* **Seed sample data**: `midaz seed --orgs 1`
* **Create admin user**: `midaz create-user ...`
* **Deploy Observability**: `terraform apply` in `infra/terraform`

## Backup
PostgreSQL: use `pg_dump` daily; store in S3 Glacier.

## Incident Response
1. Check Grafana alerts.
2. Inspect traces in Tempo for high latency endpoints.
3. Inspect logs in Loki.
4. Roll back via `kubectl rollout undo`.

