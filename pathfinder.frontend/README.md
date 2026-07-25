# Pathfinder frontend

Vue 3 + TypeScript SPA для Pathfinder 2e Web App.

## Пользовательские сценарии

- регистрация и вход;
- список, создание, финализация, просмотр и удаление персонажей;
- полный wizard персонажа 1 уровня для восьми классов Player Core;
- карточка персонажа с боевыми показателями, навыками, снаряжением и spell loadout;
- список и создание кампаний.

UI для управления участниками/партиями, runtime inventory, transfer/trade и commerce пока не реализован, хотя соответствующие backend-сценарии существуют.

## Запуск

```powershell
npm install
npm run dev
```

Development server доступен на `http://localhost:8080`. Запросы `/api` проксируются на `https://localhost:5001`; backend нужно запустить отдельно по инструкции из [корневого README](../README.md).

## Команды

- `npm run dev` — development server;
- `npm run build` — TypeScript type-check и production build;
- `npm run lint` — ESLint без допустимых warnings;
- `npm run test` — Vitest;
- `npm run format` — форматирование всего frontend-дерева через Prettier;
- `npm run preview` — локальный просмотр production build.

## Структура

- `src/app` — router и Vuetify;
- `src/api` — общий HTTP client и обработка ошибок;
- `src/features/auth` — authentication и Pinia store;
- `src/features/character-creation` — API и доменная логика wizard;
- `src/features/characters` — карточка и presentation helpers;
- `src/features/campaigns` — campaign API и validation;
- `src/i18n` — локализация;
- `src/views` — страницы приложения.

Общий контекст проекта находится в [`../MemoryBank/00_project/project_overview.md`](../MemoryBank/00_project/project_overview.md), актуальный инженерный статус character creation — в [`../MemoryBank/30_task_notes/mvp_character_creation_frontend.md`](../MemoryBank/30_task_notes/mvp_character_creation_frontend.md).
