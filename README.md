# ErrorSender

Проект для відправки помилок через Telegram бота в групу.

## Налаштування

### 1. Створити Telegram бота
1. Напишіть `@BotFather` в Telegram
2. Створіть нового бота командою `/newbot`
3. Скопіюйте токен бота

### 2. Отримати Chat ID групи
1. Додайте бота в групу
2. Напишіть щось в групу
3. Відкрийте `https://api.telegram.org/bot<BOT_TOKEN>/getUpdates`
4. Знайдіть `chat.id` в відповіді

### 3. Налаштувати конфігурацію
Замініть значення в `appsettings.json`:
```json
{
  "Telegram": {
    "BotToken": "YOUR_BOT_TOKEN_HERE",
    "ChatId": "YOUR_CHAT_ID_HERE"
  }
}
```

## Запуск
```bash
dotnet run --project ErrSendWebApi
```

## Тестування
- Відкрийте http://localhost:5001
- Використайте Swagger UI для тестування:
  - `GET /api/Test/health` - перевірка працездатності API
  - `POST /api/Test/send-telegram-message` - відправка тестового повідомлення в Telegram
  - `POST /api/Test/throw-error` - тестування відправки помилок через middleware

## Структура проекту
- **Domain** - доменні моделі
- **ErrSendApplication** - бізнес логіка
- **ErrSendPersistensTelegram** - сервіси для роботи з Telegram
- **ErrSendWebApi** - веб API 