# Pathfinder 2e Web App

Учебное веб-приложение для создания и управления персонажами Pathfinder 2e. Основной готовый сценарий — создание draft-персонажа с ancestry, background, class и связанными стартовыми выборами.

## Стек

- Backend: ASP.NET Core 8, C# 12, PostgreSQL, EF Core 8, MediatR, JWT и MassTransit.
- Frontend: Vue 3, TypeScript, Vite, Vuetify 3, Pinia, Vue Router, Axios и Vitest.

## Быстрый запуск

1. Настройте PostgreSQL и передайте `DB__Secure`, `DB__CharacterManagement` и `Authentication__SecurityKey` через user secrets или переменные окружения. Не добавляйте реальные значения в отслеживаемые файлы.
2. Запустите backend из `Pathfinder.Web` с профилем `Pathfinder.Web`; development URL — `https://localhost:5001`.
3. В `pathfinder.frontend` выполните `npm install`, затем `npm run dev`. Vite использует порт `8080`.

Подробные требования к окружению, архитектура и текущий статус находятся в [MemoryBank/00_project/project_overview.md](MemoryBank/00_project/project_overview.md). Доменные правила создания персонажа — в [MemoryBank/20_domain/character_creation/README.md](MemoryBank/20_domain/character_creation/README.md).
