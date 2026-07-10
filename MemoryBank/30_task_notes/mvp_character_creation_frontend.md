# MVP Character Creation Frontend

## Цель

Сделать Vue.js wizard создания персонажа: имя, выбор `Ancestry`, распределение free boosts, подтверждение, список персонажей и базовая карточка.

## Что готово

- Backend endpoints и доменные правила MVP уже подготовлены.
- Нормативные правила frontend-flow описаны в [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md).
- API-ожидания и пользовательский сценарий описаны в backend status note.

## Что не готово

- UI wizard во Vue.js ещё не реализован.
- Нет полноценного frontend state flow для draft creation.
- Нет экранов списка/карточки персонажей, связанных с новым backend API.
- Нет frontend validation для free boosts и обязательных шагов.

## Связанные файлы

- [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md)
- [`mvp_character_creation_backend.md`](mvp_character_creation_backend.md)
- `../../pathfinder.vue/`

## Next steps

1. Проверить текущую структуру `pathfinder.vue`.
2. Спроектировать минимальный wizard без расширения scope за пределы MVP.
3. Подключить endpoints `GET /api/ancestries`, `POST /api/character`, `GET /api/character`.
4. Добавить базовую клиентскую валидацию, не дублируя доменную истину вместо backend.

