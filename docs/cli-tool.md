
# Quantra CLI as .NET Tool

Empacotar e instalar localmente:

```bash
dotnet pack -c Release src/Quantra.CLI
dotnet tool install --global --add-source nupkg midaz
```

Exemplo:

```bash
midaz migrate
midaz seed --orgs 2
midaz post --debit ORG1CASH --credit ORG1REV --amount 100
midaz balance ORG1CASH
```
