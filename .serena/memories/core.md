# PathfinderStore
- ASP.NET Core backend + Vue SPA for Pathfinder 2e character creation.
- Active bounded context: CharacterManagement; do not touch Store without explicit task.
- Project instructions and authoritative changing context live in AGENTS.md and MemoryBank/.
- Read `mem:tech_stack` for runtime/toolchain, `mem:conventions` for coding rules, `mem:suggested_commands` for workflows, and `mem:task_completion` before handoff.
- Backend modules: CharacterManagement.{Domain,Application,Infrastructure}, Pathfinder.Web, Secure.*, shared utilities.
- Frontend: pathfinder.frontend.
- Character creation uses vertical slices across domain/application/persistence/API/frontend/tests; derived values are server-computed, not client-authoritative.