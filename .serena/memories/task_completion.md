# Completion checks

- Run the narrowest checks that prove the changed behavior; expand according to impact.
- Backend/C# minimum: build affected `.csproj` with `--no-restore`, then run affected test project(s). Use solution build only when cross-project impact warrants it.
- CharacterManagement domain changes: run `CharacterManagement.Domain.Tests`.
- Application/infrastructure/API changes: run relevant `CharacterManagement.Infrastructure.Tests`; build `Pathfinder.Web` when composition/API wiring changes.
- Frontend changes: `npm run lint --prefix pathfinder.frontend`, `npm run test --prefix pathfinder.frontend`, and `npm run build --prefix pathfinder.frontend` for type/build verification.
- Migration work: follow `MemoryBank/10_workflow/ef.md`; build target infrastructure project before `dotnet ef`; do not use mere migration-file presence as proof.
- Treat access/lock/empty-build symptoms according to `MemoryBank/10_workflow/sandbox.md`; do not diagnose code breakage from one sandbox-like failure.
- Inspect `git diff`/`git status --short`; ensure no secrets, generated outputs, or unrelated edits were introduced.
- Update relevant `MemoryBank` task/status notes only when implementation state materially changed.
