# Backend map and invariants

- Entry/composition/API: `Pathfinder.Web`; dev launch profile `Pathfinder.Web`, port `5001`.
- CharacterManagement layers: `CharacterManagement.Domain`, `.Application`, `.Infrastructure`; unit tests in `.Domain.Tests`, integration/API tests in `.Infrastructure.Tests`.
- Secure layers: `Secure.Domain`, `.Application`, `.Infrastructure`; owns users, roles, permissions, Identity persistence and JWT auth.
- Store has Domain/Application/Infrastructure projects but is inactive and disconnected from Web; do not modify without explicit scope.
- Shared: `Pathfinder.Utils`, `Infrasturture.Shared` (spelling is repository name), `Domain.Contracts` for cross-context events.
- CharacterManagement aggregate direction: `Account -> DraftCharacter -> AbilityScores -> Characteristic`.
- Detailed CharacterManagement implementation map: `MemoryBank/00_project/project_pathfinder_character_domain.md`.
- Domain rules for character creation are normative under `MemoryBank/20_domain/character_creation/`; do not infer PF2e rules from memory when these files cover them.
- Database contexts use separate connection keys for Secure and CharacterManagement.
