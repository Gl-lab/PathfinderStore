# Task completion
- Review git diff and exclude unrelated/untracked user changes (notably local .serena state unless explicitly intended).
- Run proportional targeted backend tests for CharacterManagement.Domain.Tests and CharacterManagement.Infrastructure.Tests.
- For frontend changes run `npm test`, `npm run lint`, and `npm run build` in pathfinder.frontend.
- Build affected backend projects; do not infer code failure from a single known sandbox access/empty-output symptom.
- Review vertical slice consistency: domain/application/API/frontend/tests/docs, no client-authoritative derived state.
- For requested task workflow, review before commit; inspect committed diff/status after commit.