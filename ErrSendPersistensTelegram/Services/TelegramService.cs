using ErrSendApplication.Common.Configs;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using ErrSendApplication.Interfaces.Client;
using ErrSendApplication.Interfaces.Telegram;
using FluentValidation;
using ErrSendApplication.Validators;

namespace ErrSendPersistensTelegram.Services
{
    public class TelegramService : ITelegramService
    {
        private const string TelegramApiUrl = "https://api.telegram.org/bot";
        private const string SendMessageEndpoint = "/sendMessage";
        private const string HtmlParseMode = "HTML";

        private readonly IHttpClientWr httpClient;
        private readonly TelegramConfig telegramConfig;
        private readonly IValidator<(string errorMessage, string? additionalInfo, TelegramConfig config)> validator;
        private readonly IValidator<TelegramSendMessageRequestValidator.TelegramSendMessageRequest> messageValidator;

        public TelegramService(
            IHttpClientWr httpClient, 
            IOptions<TelegramConfig> telegramConfig,
            IValidator<(string errorMessage, string? additionalInfo, TelegramConfig config)> validator,
            IValidator<TelegramSendMessageRequestValidator.TelegramSendMessageRequest> messageValidator)
        {
            this.httpClient = httpClient;
            this.telegramConfig = telegramConfig.Value;
            this.validator = validator;
            this.messageValidator = messageValidator;
        }

        public async Task SendErrorMessageAsync(string errorMessage, string? additionalInfo = null)
        {
            try
            {
                // Валідація через FluentValidation замість ручних перевірок
                var validationResult = await validator.ValidateAsync((errorMessage, additionalInfo, telegramConfig));
                
                if (!validationResult.IsValid)
                {
                    // Логуємо помилки валідації (можна додати логер)
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return; // Виходимо, якщо валідація не пройшла
                }

                var message = FormatErrorMessage(errorMessage, additionalInfo);
                var url = $"{TelegramApiUrl}{telegramConfig.BotToken}{SendMessageEndpoint}";

                var payload = new TelegramSendMessageRequestValidator.TelegramSendMessageRequest
                {
                    ChatId = telegramConfig.ChatId,
                    Text = message,
                    ParseMode = HtmlParseMode
                };

                // Валідація payload перед відправкою
                var messageValidationResult = await messageValidator.ValidateAsync(payload);
                if (!messageValidationResult.IsValid)
                {
                    var messageErrors = string.Join(", ", messageValidationResult.Errors.Select(e => e.ErrorMessage));
                    return; // Виходимо, якщо валідація payload не пройшла
                }

                var jsonPayload = JsonSerializer.Serialize(payload);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                await httpClient.PostAsync(url, content);
            }
            catch
            {
                // Ігноруємо помилки
            }
        }

        private string FormatErrorMessage(string errorMessage, string? additionalInfo)
        {
            var sb = new StringBuilder();
            sb.AppendLine("🚨 <b>Помилка в додатку</b>");
            sb.AppendLine($"⏰ <b>Час:</b> {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            sb.AppendLine($"❌ <b>Помилка:</b> {System.Net.WebUtility.HtmlEncode(errorMessage)}");

            // Валідація additionalInfo через FluentValidation
            if (additionalInfo != null)
            {
                var additionalInfoValidator = new FluentValidation.Validators.LengthValidator(0, 1000);
                var validationResult = additionalInfoValidator.Validate(additionalInfo);
                
                if (validationResult.IsValid && !string.IsNullOrWhiteSpace(additionalInfo))
                {
                    // Декодуємо, якщо раптом прийшов URL-encoded текст
                    var decodedInfo = System.Net.WebUtility.UrlDecode(additionalInfo);
                    sb.AppendLine($"ℹ️ <b>Додаткова інформація:</b> {System.Net.WebUtility.HtmlEncode(decodedInfo)}");
                }
            }

            return sb.ToString();
        }
    }
} 

