# FluentValidation Валідатори в проекті ErrorSender

## Огляд
Проект тепер використовує FluentValidation замість ручних перевірок у всіх сервісах та конфігураціях.

## Створені валідатори

### 1. **TelegramMessageRequestValidator**
- **Файл**: `ErrSendApplication/Validators/TelegramMessageRequestValidator.cs`
- **Призначення**: Валідація вхідних DTO для Telegram повідомлень
- **Правила**:
  - `Message` - не може бути порожнім

### 2. **ErrorRequestValidator**
- **Файл**: `ErrSendApplication/Validators/ErrorRequestValidator.cs`
- **Призначення**: Валідація вхідних DTO для помилок
- **Правила**:
  - `ErrorMessage` - не може бути порожнім

### 3. **TelegramServiceValidator**
- **Файл**: `ErrSendApplication/Validators/TelegramServiceValidator.cs`
- **Призначення**: Валідація параметрів для Telegram сервісу
- **Правила**:
  - `errorMessage` - не може бути порожнім, максимум 4096 символів
  - `config.BotToken` - не може бути порожнім, має правильний формат
  - `config.ChatId` - не може бути порожнім, має бути числом
  - `additionalInfo` - максимум 1000 символів (якщо не порожній)

### 4. **TelegramSendMessageRequestValidator**
- **Файл**: `ErrSendApplication/Validators/TelegramSendMessageRequestValidator.cs`
- **Призначення**: Валідація payload для відправки в Telegram API
- **Правила**:
  - `ChatId` - не може бути порожнім, має бути числом
  - `Text` - не може бути порожнім, максимум 4096 символів
  - `ParseMode` - має бути валідним значенням

### 5. **TelegramConfigValidator**
- **Файл**: `ErrSendApplication/Validators/TelegramConfigValidator.cs`
- **Призначення**: Валідація конфігурації Telegram
- **Правила**:
  - `BotToken` - не може бути порожнім, правильний формат
  - `ChatId` - не може бути порожнім, має бути числом

### 6. **JwtConfigValidator**
- **Файл**: `ErrSendApplication/Validators/JwtConfigValidator.cs`
- **Призначення**: Валідація конфігурації JWT
- **Правила**:
  - `Secret` - не може бути порожнім, 32-512 символів
  - `ExpiryMinutes` - 1-1440 хвилин

## Замінені ручні перевірки

### TelegramService
- ❌ Ручні перевірки `string.IsNullOrWhiteSpace(errorMessage)`
- ❌ Ручні перевірки `string.IsNullOrEmpty(telegramConfig.BotToken)`
- ✅ Замінено на `TelegramServiceValidator`

### JwtTokenService
- ❌ Ручні перевірки `string.IsNullOrWhiteSpace(secret) || secret.Length < 16`
- ✅ Замінено на `JwtConfigValidator`

## Налаштування

### ErrSendApplication
```csharp
services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });
```

### ErrSendPersistensTelegram
```csharp
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```

## Переваги FluentValidation

1. **Централізована валідація** - всі правила в одному місці
2. **Перевикористання** - валідатори можна використовувати в різних місцях
3. **Легке тестування** - можна легко створювати unit тести
4. **Локалізація** - підтримка різних мов
5. **Кастомізація** - можна створювати складні правила валідації

## Використання

```csharp
// В сервісі
var validationResult = await validator.ValidateAsync((errorMessage, additionalInfo, telegramConfig));
if (!validationResult.IsValid)
{
    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
    return; // Обробка помилок валідації
}
```
