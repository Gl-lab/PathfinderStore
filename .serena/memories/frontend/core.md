# Frontend map and invariants

- Root: `pathfinder.frontend`; Vue 3 + TypeScript SPA built by Vite.
- Dev server port `8080`; backend API expected on port `5001`.
- State/router/UI: Pinia, Vue Router, Vuetify; HTTP via Axios; localization via vue-i18n.
- Authoritative scripts/dependency versions: `pathfinder.frontend/package.json`.
- Character creation MVP is implemented; current flow/status and known gaps: `MemoryBank/30_task_notes/mvp_character_creation_frontend.md`.
- Backend contract/status reference: `MemoryBank/30_task_notes/mvp_character_creation_backend.md`.
- Validate frontend changes with lint + Vitest + production build; Prettier command writes the whole frontend tree, so use deliberately.
