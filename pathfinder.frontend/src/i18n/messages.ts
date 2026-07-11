export const messages = {
  ru: {
    app: {
      productName: 'Pathfinder', productSubtitle: 'Лист персонажа', language: 'Язык', languageNames: { ru: 'Русский', en: 'Английский' },
      navigation: { characters: 'Персонажи', createCharacter: 'Создать персонажа' },
      auth: { signIn: 'Войти', signOut: 'Выйти' },
    },
    routes: { characters: 'Мои персонажи', login: 'Вход', register: 'Регистрация', characterDetails: 'Персонаж', characterCreate: 'Создание персонажа', notFound: 'Страница не найдена' },
    common: { cancel: 'Отмена', back: 'Назад', next: 'Далее', retry: 'Повторить', open: 'Открыть', delete: 'Удалить', details: 'Детали', character: 'Персонаж', name: 'Имя' },
    errors: { connection: 'Не удалось подключиться к серверу. Повторите попытку позже.', request: 'Не удалось выполнить запрос. Повторите попытку позже.', invalidCredentials: 'Неверный логин или пароль.', passwordsMismatch: 'Пароли не совпадают.' },
    auth: { ledger: 'Лист персонажа', welcome: 'С возвращением', signInPrompt: 'Войдите, чтобы продолжить приключение.', userNameOrEmail: 'Никнейм или эл. почта', password: 'Пароль', noAccount: 'Нет аккаунта?', register: 'Зарегистрироваться', createAccount: 'Создайте аккаунт', registrationPrompt: 'Он понадобится для сохранения персонажей.', userName: 'Никнейм', email: 'Электронная почта', firstName: 'Имя', surname: 'Фамилия', passwordRepeat: 'Повтор пароля', hasAccount: 'Уже есть аккаунт?' },
    dashboard: { eyebrow: 'Ваше приключение', lead: 'Соберите историю героя, сохраните его выборы и возвращайтесь к игре без лишних таблиц.', emptyTitle: 'Первый герой ждёт вас', emptyText: 'Создайте персонажа, чтобы начать вести его историю.' },
    characters: { title: 'Мои персонажи', ancestry: 'Родословная', abilities: 'Характеристики', noConcept: 'История ещё не написана.', age: 'Возраст: {age}', ageUnknown: 'Возраст не указан', deleteTitle: 'Удалить персонажа?', deleteText: 'Это действие нельзя отменить.' },
    wizard: { eyebrow: 'Новый герой', title: 'Создание персонажа', lead: 'Сервер применит правила родословной и рассчитает итоговые характеристики.', loading: 'Загружаем родословные…', basic: 'Основное', ancestry: 'Родословная', boosts: 'Бусты', review: 'Проверка', name: 'Имя персонажа', nameRequired: 'Укажите имя', concept: 'Краткая концепция', conceptHint: 'Кем является ваш персонаж?', age: 'Возраст', ancestryHint: 'Выберите происхождение героя. Его правила определит сервер.', speed: 'скорость {speed}', fixedBoosts: '{count} фикс. буста', freeBoostsHint: 'Выберите {count}: {kind}. Фиксированные бусты: {boosts}.', oneAbility: 'характеристику', severalAbilities: 'разные характеристики', none: 'нет', selectedAncestry: 'Родословная', selectedBoosts: 'Свободные бусты', selectedConcept: 'Концепция', resultHint: 'Итоговые характеристики рассчитывает сервер после создания.', create: 'Создать персонажа' },
    notFound: { title: 'Эта страница не найдена', subtitle: 'Вернитесь к списку персонажей.' },
    domain: { ancestry: { Gnome: 'Гном', Goblin: 'Гоблин', Dwarf: 'Дварф', Halfling: 'Полурослик', Human: 'Человек', Elf: 'Эльф' }, ability: { Strength: 'Сила', Dexterity: 'Ловкость', Constitution: 'Выносливость', Intelligence: 'Интеллект', Wisdom: 'Мудрость', Charisma: 'Харизма' } },
  },
  en: {
    app: {
      productName: 'Pathfinder', productSubtitle: 'Character Ledger', language: 'Language', languageNames: { ru: 'Russian', en: 'English' },
      navigation: { characters: 'Characters', createCharacter: 'Create character' },
      auth: { signIn: 'Sign in', signOut: 'Sign out' },
    },
    routes: { characters: 'My characters', login: 'Sign in', register: 'Register', characterDetails: 'Character', characterCreate: 'Create character', notFound: 'Page not found' },
    common: { cancel: 'Cancel', back: 'Back', next: 'Next', retry: 'Retry', open: 'Open', delete: 'Delete', details: 'Details', character: 'Character', name: 'Name' },
    errors: { connection: 'Unable to connect to the server. Please try again later.', request: 'Unable to complete the request. Please try again later.', invalidCredentials: 'Invalid username or password.', passwordsMismatch: 'Passwords do not match.' },
    auth: { ledger: 'Character Ledger', welcome: 'Welcome back', signInPrompt: 'Sign in to continue your adventure.', userNameOrEmail: 'Username or email', password: 'Password', noAccount: 'No account?', register: 'Register', createAccount: 'Create an account', registrationPrompt: 'You need one to save your characters.', userName: 'Username', email: 'Email address', firstName: 'First name', surname: 'Last name', passwordRepeat: 'Repeat password', hasAccount: 'Already have an account?' },
    dashboard: { eyebrow: 'Your adventure', lead: 'Build your hero’s story, save their choices, and return to the game without extra tables.', emptyTitle: 'Your first hero awaits', emptyText: 'Create a character to begin their story.' },
    characters: { title: 'My characters', ancestry: 'Ancestry', abilities: 'Abilities', noConcept: 'No character story yet.', age: 'Age: {age}', ageUnknown: 'Age is not specified', deleteTitle: 'Delete character?', deleteText: 'This action cannot be undone.' },
    wizard: { eyebrow: 'New hero', title: 'Create character', lead: 'The server will apply ancestry rules and calculate final ability scores.', loading: 'Loading ancestries…', basic: 'Basics', ancestry: 'Ancestry', boosts: 'Boosts', review: 'Review', name: 'Character name', nameRequired: 'Enter a name', concept: 'Brief concept', conceptHint: 'Who is your character?', age: 'Age', ancestryHint: 'Choose the character’s ancestry. The server determines its rules.', speed: 'speed {speed}', fixedBoosts: '{count} fixed boosts', freeBoostsHint: 'Choose {count}: {kind}. Fixed boosts: {boosts}.', oneAbility: 'an ability', severalAbilities: 'different abilities', none: 'none', selectedAncestry: 'Ancestry', selectedBoosts: 'Free boosts', selectedConcept: 'Concept', resultHint: 'The server calculates final ability scores after creation.', create: 'Create character' },
    notFound: { title: 'This page was not found', subtitle: 'Return to the character list.' },
    domain: { ancestry: { Gnome: 'Gnome', Goblin: 'Goblin', Dwarf: 'Dwarf', Halfling: 'Halfling', Human: 'Human', Elf: 'Elf' }, ability: { Strength: 'Strength', Dexterity: 'Dexterity', Constitution: 'Constitution', Intelligence: 'Intelligence', Wisdom: 'Wisdom', Charisma: 'Charisma' } },
  },
} as const
