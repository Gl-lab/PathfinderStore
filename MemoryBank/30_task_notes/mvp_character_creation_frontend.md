# MVP Character Creation Frontend

## Цель

Сделать Vue.js wizard создания персонажа: имя, выбор `Ancestry`, распределение free boosts, подтверждение, список персонажей и базовая карточка.

## Что готово

- Новый SPA реализован на Vue 3, TypeScript, Vite, Vuetify 3 и Pinia.
- Реализованы вход, регистрация и хранение сессии.
- Главная страница показывает список персонажей.
- Wizard поддерживает имя, концепцию, возраст, Ancestry, heritage, ancestry feat, ancestry free boosts, Background boosts и skill/Lore choices, Class, key ability boost и четыре финальных свободных boosts.
- Реализованы карточка и удаление персонажа.
- Есть единая обработка API errors, русская и английская локализация.
- Добавлены frontend tests для доменной локализации, Background, Class и final free boost choices.
- Dashboard показывает maximum HP, карточка персонажа — maximum HP и breakdown ancestry/class/Constitution.
- Background step разрешает fixed, finite-choice и open Lore grants; review/details показывают фактический training.
- Class step и review показывают категории typed starting proficiencies; карточка группирует targets и локализованные ranks.

## Что не готово

- Skill feat от Background пока не применяется.
- Class skills, higher proficiency ranks и replacement choices при cross-source duplicate training пока не реализованы.
- Class skills, spell proficiencies, features, spells и mandatory choices отображаются декларативно, но не выбираются.
- Нет полноценного e2e test suite с реальным backend и PostgreSQL.

## Связанные файлы

- [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md)
- [`mvp_character_creation_backend.md`](mvp_character_creation_backend.md)
- `../../pathfinder.frontend/`

## Next steps

1. Добавлять обязательные class choices отдельными flows, начиная с Rogue's Racket.
2. Подключить class skills отдельной задачей с replacement flow для повторного training.
3. Добавить e2e-покрытие основного пользовательского сценария.
