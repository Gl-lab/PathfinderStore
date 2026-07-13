# MVP Character Creation Frontend

## Цель

Сделать Vue.js wizard создания персонажа: имя, выбор `Ancestry`, распределение free boosts, подтверждение, список персонажей и базовая карточка.

## Что готово

- Новый SPA реализован на Vue 3, TypeScript, Vite, Vuetify 3 и Pinia.
- Реализованы вход, регистрация и хранение сессии.
- Главная страница показывает список персонажей.
- Wizard поддерживает имя, концепцию, возраст, Ancestry, heritage, ancestry feat, ancestry free boosts, Background boosts, Class и key ability boost.
- Реализованы карточка и удаление персонажа.
- Есть единая обработка API errors, русская и английская локализация.
- Добавлены frontend tests для доменной локализации, Background и Class choices.

## Что не готово

- Фактическое применение skills, Lore и skill feat от Background ожидает отдельную Skill subsystem.
- Четыре финальных boosts ещё не входят в wizard.
- Class proficiencies, features, spells и mandatory choices отображаются декларативно, но не выбираются.
- Нет полноценного e2e test suite с реальным backend и PostgreSQL.

## Связанные файлы

- [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md)
- [`mvp_character_creation_backend.md`](mvp_character_creation_backend.md)
- `../../pathfinder.frontend/`

## Next steps

1. Отдельным шагом добавить четыре финальных boosts.
2. Отдельными задачами реализовать зависимые Class flows и Skill subsystem.
3. Добавить e2e-покрытие основного пользовательского сценария.
