# MVP Character Creation Frontend

## Цель

Сделать Vue.js wizard создания персонажа: имя, выбор `Ancestry`, распределение free boosts, подтверждение, список персонажей и базовая карточка.

## Что готово

- Новый SPA реализован на Vue 3, TypeScript, Vite, Vuetify 3 и Pinia.
- Реализованы вход, регистрация и хранение сессии.
- Главная страница показывает список персонажей.
- Wizard поддерживает имя, концепцию, возраст, Ancestry, heritage, ancestry feat, ancestry free boosts и Background boosts.
- Реализованы карточка и удаление персонажа.
- Есть единая обработка API errors, русская и английская локализация.
- Добавлены frontend tests для доменной локализации и Background choice.

## Что не готово

- Фактическое применение skills, Lore и skill feat от Background ожидает отдельную Skill subsystem.
- `Class` и четыре финальных boosts ещё не входят в wizard.
- Нет полноценного e2e test suite с реальным backend и PostgreSQL.

## Связанные файлы

- [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md)
- [`mvp_character_creation_backend.md`](mvp_character_creation_backend.md)
- `../../pathfinder.frontend/`

## Next steps

1. После отдельного доменного решения добавить `Class` и key ability boost.
2. Отдельной задачей реализовать Skill subsystem и разрешение deferred Background grants.
3. Добавить e2e-покрытие основного пользовательского сценария.
