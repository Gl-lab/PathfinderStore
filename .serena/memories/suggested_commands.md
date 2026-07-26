# Suggested commands (PowerShell, project root)
- Backend targeted build: `dotnet build CharacterManagement.Infrastructure\CharacterManagement.Infrastructure.csproj --no-restore`.
- Backend tests: `dotnet test CharacterManagement.Domain.Tests\CharacterManagement.Domain.Tests.csproj`; `dotnet test CharacterManagement.Infrastructure.Tests\CharacterManagement.Infrastructure.Tests.csproj`.
- EF migration: build infrastructure first, then `dotnet ef migrations add <Name> --project CharacterManagement.Infrastructure\CharacterManagement.Infrastructure.csproj --context CharacterManagementDbContext --no-build`.
- Frontend (pathfinder.frontend): `npm run build`, `npm run lint`, `npm test`, `npm run dev`.
- PowerShell discovery: `Get-ChildItem`, `Select-String`, `Get-Content`; project workflow prefers these before rg.
- Git root: `git rev-parse --show-toplevel`; inspect with `git status --short`.