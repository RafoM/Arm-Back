# Arbito

## TransactionCore configuration

TransactionCore includes a `UseDatabase` flag (default `true`).
Set `UseDatabase=false` in `appsettings.json` or via environment variable to
run the service without a database. For example:

```bash
UseDatabase=false dotnet run --project TransactionCore
```

When disabled, the service uses an in-memory database and skips migrations and
database-dependent background services.

