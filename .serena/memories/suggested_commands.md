# ??????? ??????? (PowerShell, ?? ????? ???????????)

## Backend
- Restore: `dotnet restore Pathfinder.sln`
- Build solution: `dotnet build Pathfinder.sln --no-restore`
- Run API: `dotnet run --project Pathfinder.Web/Pathfinder.Web.csproj --launch-profile Pathfinder.Web`
- Domain tests: `dotnet test CharacterManagement.Domain.Tests/CharacterManagement.Domain.Tests.csproj`
- Integration tests: `dotnet test CharacterManagement.Infrastructure.Tests/CharacterManagement.Infrastructure.Tests.csproj`

## Frontend
- Install: `npm install --prefix pathfinder.frontend`
- Dev: `npm run dev --prefix pathfinder.frontend`
- Test: `npm run test --prefix pathfinder.frontend`
- Lint: `npm run lint --prefix pathfinder.frontend`
- Build: `npm run build --prefix pathfinder.frontend`
- Format (writes broadly): `npm run format --prefix pathfinder.frontend`

## Windows navigation/search
- Files: `Get-ChildItem`; content: `Get-Content`; text search: `Select-String`.
- Per project workflow prefer PowerShell cmdlets; use `rg` only when available and error-free.
- Git root: `git rev-parse --show-toplevel`; status: `git status --short`.

## EF
- Exact CharacterManagement migration commands and failure handling: `MemoryBank/10_workflow/ef.md`.
