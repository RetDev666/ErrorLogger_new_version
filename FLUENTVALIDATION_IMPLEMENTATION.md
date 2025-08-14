# Заміна ручних перевірок на FluentValidation

## Огляд
Проект ErrorSender тепер повністю використовує FluentValidation замість ручних перевірок у всіх сервісах, middleware та конфігураціях.

## Створені валідатори

### 1. **TelegramServiceValidator**
- **Файл**: `ErrSendApplication/Validators/TelegramServiceValidator.cs`
- **Замінює**: Ручні перевірки в `TelegramService.SendErrorMessageAsync()`
- **Правила**:
  - `errorMessage` - не може бути порожнім, максимум 4096 символів
  - `config.BotToken` - не може бути порожнім, правильний формат
  - `config.ChatId` - не може бути порожнім, має бути числом
  - `additionalInfo` - максимум 1000 символів (якщо не порожній)

### 2. **TelegramSendMessageRequestValidator**
- **Файл**: `ErrSendApplication/Validators/TelegramSendMessageRequestValidator.cs`
- **Замінює**: Ручні перевірки payload перед відправкою в Telegram API
- **Правила**:
  - `ChatId` - не може бути порожнім, має бути числом
  - `Text` - не може бути порожнім, максимум 4096 символів
  - `ParseMode` - має бути валідним значенням

### 3. **TelegramConfigValidator**
- **Файл**: `ErrSendApplication/Validators/TelegramConfigValidator.cs`
- **Замінює**: Ручні перевірки конфігурації Telegram
- **Правила**:
  - `BotToken` - не може бути порожнім, правильний формат
  - `ChatId` - не може бути порожнім, має бути числом

### 4. **JwtConfigValidator**
- **Файл**: `ErrSendApplication/Validators/JwtConfigValidator.cs`
- **Замінює**: Ручні перевірки в `JwtTokenService.GenerateToken()`
- **Правила**:
  - `Secret` - не може бути порожнім, 32-512 символів
  - `ExpiryMinutes` - 1-1440 хвилин

### 5. **EnvironmentConfigValidator**
- **Файл**: `ErrSendApplication/Validators/EnvironmentConfigValidator.cs`
- **Замінює**: Ручні перевірки змінних середовища в `DependencyInjection.cs`
- **Правила**:
  - `configSecret` - не може бути порожнім, мінімум 32 символи
  - `envSecret` - мінімум 32 символи (якщо не порожній)

### 6. **HttpClientValidator**
- **Файл**: `ErrSendApplication/Validators/HttpClientValidator.cs`
- **Замінює**: Ручні перевірки в `StandartHttpClient.PostAsync()`
- **Правила**:
  - `url` - не може бути порожнім, має бути валідним URL
  - `content` - не може бути null
  - `token` - 10-1000 символів (якщо не порожній)

### 7. **OperationFilterValidator**
- **Файл**: `ErrSendWebApi/Validators/OperationFilterValidator.cs`
- **Замінює**: Ручні перевірки в `AddTimeAndTimeZoneOperationFilter.Apply()`
- **Правила**:
  - `operation` - не може бути null
  - `responseKey` - не може бути порожнім
  - `contentType` - не може бути порожнім, валідний MIME тип

### 8. **ExceptionHandlerValidator**
- **Файл**: `ErrSendWebApi/Validators/ExceptionHandlerValidator.cs`
- **Замінює**: Ручні перевірки в `CustomExceptionHandlerMiddleware.HandleExceptionAsync()`
- **Правила**:
  - `context` - не може бути null
  - `exception` - не може бути null
  - `statusCode` - має бути валідним HTTP статусом
  - `context.Request.Path` - не може бути порожнім
  - `context.Request.Method` - не може бути порожнім

## Замінені ручні перевірки

### ❌ Раніше (ручні перевірки):
```csharp
// TelegramService
if (string.IsNullOrWhiteSpace(errorMessage)) return;
if (string.IsNullOrEmpty(telegramConfig.BotToken)) return;
if (string.IsNullOrEmpty(telegramConfig.ChatId)) return;

// JwtTokenService
if (string.IsNullOrWhiteSpace(secret) || secret.Length < 16) 
    throw new InvalidOperationException("...");

// DependencyInjection
if (!string.IsNullOrEmpty(envSecret)) { ... }
if (string.IsNullOrWhiteSpace(jwtConfig.Secret) || jwtConfig.Secret.Length < 32) { ... }

// StandartHttpClient
// Без валідації параметрів

// AddTimeAndTimeZoneOperationFilter
// Без валідації параметрів

// CustomExceptionHandlerMiddleware
// Без валідації параметрів
```

### ✅ Тепер (FluentValidation):
```csharp
// TelegramService
var validationResult = await validator.ValidateAsync((errorMessage, additionalInfo, telegramConfig));
if (!validationResult.IsValid) return;

// JwtTokenService
var validationResult = configValidator.Validate(config);
if (!validationResult.IsValid) throw new InvalidOperationException($"...");

// DependencyInjection
var validationResult = envValidator.Validate((envSecret, jwtConfig.Secret, jwtConfig.Secret?.Length ?? 0));
if (!validationResult.IsValid) { ... }

// StandartHttpClient
var validationResult = await validator.ValidateAsync((url, content, token));
if (!validationResult.IsValid) throw new ArgumentException($"...");

// AddTimeAndTimeZoneOperationFilter
var validationResult = validator.Validate((operation, "200", "application/json"));
if (!validationResult.IsValid) return;

// CustomExceptionHandlerMiddleware
var validationResult = validator.Validate((context, exception, code));
if (!validationResult.IsValid) { ... }
```

## Налаштування FluentValidation

### ErrSendApplication
```csharp
services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });
```

### ErrSendPersistensTelegram
```csharp
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```

### ErrSendWebApi
```csharp
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```

## Переваги заміни

1. **Централізована валідація** - всі правила в одному місці
2. **Перевикористання** - валідатори можна використовувати в різних місцях
3. **Легке тестування** - можна легко створювати unit тести
4. **Локалізація** - підтримка різних мов
5. **Кастомізація** - можна створювати складні правила валідації
6. **Консистентність** - однаковий підхід до валідації по всьому проекту
7. **Обробка помилок** - структуровані повідомлення про помилки валідації

## Структура проекту після заміни

```
ErrSendApplication/Validators/
├── TelegramMessageRequestValidator.cs
├── ErrorRequestValidator.cs
├── TelegramServiceValidator.cs
├── TelegramSendMessageRequestValidator.cs
├── TelegramConfigValidator.cs
├── JwtConfigValidator.cs
├── EnvironmentConfigValidator.cs
└── HttpClientValidator.cs

ErrSendWebApi/Validators/
├── OperationFilterValidator.cs
└── ExceptionHandlerValidator.cs
```

## Висновок

**Всі ручні перевірки в проекті замінено на FluentValidation валідатори!** ✅

Проект тепер має:
- 8 спеціалізованих валідаторів
- Централізовану систему валідації
- Консистентний підхід до перевірки даних
- Легко тестовану та підтримувану архітектуру
