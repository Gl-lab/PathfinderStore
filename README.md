# Pathfinder 2e Web App

Учебное/хобби веб-приложение для создания персонажей Pathfinder 2e и игры в рамках кампании. Backend разделён на bounded contexts, frontend реализован как Vue SPA.

## Что уже реализовано

- регистрация, вход, JWT-аутентификация, роли и permissions;
- полный flow создания персонажа 1 уровня для восьми классов Player Core: ancestry, background, class choices, boosts, feats, languages, starting equipment и spell loadout;
- финализация персонажа, список и карточка с HP, AC, Strikes, saves, Perception, class/spell DC и Skills/Lore;
- кампании, приглашения, роли участника, партии и назначение персонажей;
- версионируемый `ItemCatalog` и campaign-scoped `Inventory`;
- дарение, обмен, партийное хранилище и принудительное перемещение предметов ведущим;
- `Commerce`: поселения, магазины, предложения, кошельки, резервы, покупка и продажа.

Пока не реализованы полноценный encounter/gameplay loop, действия и состояния боя, runtime-исполнение spell/feat effects, progression и frontend для inventory/trade/commerce.

## Стек

- Backend: .NET 8, ASP.NET Core 8, C# 12, PostgreSQL, EF Core 8, MediatR, MassTransit, JWT и Serilog.
- Frontend: Vue 3, TypeScript, Vite, Vuetify 3, Pinia, Vue Router, Axios, vue-i18n и Vitest.
- Архитектура: Clean Architecture и DDD внутри bounded contexts `Secure`, `CharacterManagement`, `CampaignManagement`, `ItemCatalog`, `Inventory` и `Commerce`.

## Требования

- .NET 8 SDK;
- Node.js и npm, совместимые с зависимостями из [`pathfinder.frontend/package.json`](pathfinder.frontend/package.json);
- PostgreSQL.

`global.json` использует старый базовый SDK с `rollForward: latestMajor`, поэтому фактическая сборка выполняется установленным .NET 8 SDK.

## Быстрый запуск

1. Настройте PostgreSQL.
2. Передайте секреты через .NET user secrets или переменные окружения:

   ```powershell
   dotnet user-secrets set "DB:Secure" "<connection string>" --project Pathfinder.Web
   dotnet user-secrets set "DB:CharacterManagement" "<connection string>" --project Pathfinder.Web
   dotnet user-secrets set "Authentication:SecurityKey" "<long random key>" --project Pathfinder.Web
   ```

   `CampaignManagement`, `ItemCatalog`, `Inventory` и `Commerce` по умолчанию используют `DB:CharacterManagement`. Для отдельных баз задайте соответственно `DB:CampaignManagement`, `DB:ItemCatalog`, `DB:Inventory` и `DB:Commerce`.

3. Примените EF Core migrations для нужных контекстов по workflow из [`MemoryBank/10_workflow/ef.md`](MemoryBank/10_workflow/ef.md).
4. Запустите backend:

   ```powershell
   $env:ASPNETCORE_ENVIRONMENT = "Development"
   dotnet run --project Pathfinder.Web --no-launch-profile --urls "https://localhost:5001"
   ```

   API и Swagger будут доступны на `https://localhost:5001` и `https://localhost:5001/swagger`.

5. В отдельном терминале запустите frontend:

   ```powershell
   Set-Location pathfinder.frontend
   npm install
   npm run dev
   ```

   Vite использует `http://localhost:8080` и проксирует `/api` на backend.

Опциональный локальный Seq для структурных логов описан в [`MemoryBank/10_workflow/observability.md`](MemoryBank/10_workflow/observability.md).

## Проверки

```powershell
dotnet test Pathfinder.sln
Set-Location pathfinder.frontend
npm run lint
npm run test
npm run build
```

## Карта репозитория

- `Pathfinder.Web` — REST API, composition root, Swagger и integration adapters.
- `pathfinder.frontend` — Vue SPA.
- `Secure.*` — пользователи, роли, permissions и authentication.
- `CharacterManagement.*` — создание, финализация и карточка персонажа.
- `CampaignManagement.*` — кампании, членство, партии и назначения.
- `ItemCatalog.*` — определения, ревизии и конфигурации предметов.
- `Inventory.*` — физические экземпляры, контейнеры и передачи.
- `Commerce.*` — магазины, кошельки и сделки.
- `MemoryBank` — проектный контекст, workflow, нормативные доменные документы и исторические task notes.

Подробная и актуальная карта solution находится в [`MemoryBank/00_project/project_overview.md`](MemoryBank/00_project/project_overview.md). Правила создания персонажа — в [`MemoryBank/20_domain/character_creation/`](MemoryBank/20_domain/character_creation/).

## Лицензия

См. [`LICENSE`](LICENSE).
