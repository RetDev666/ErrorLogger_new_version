using ErrSendApplication.Common.Configs;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using ErrSendApplication.Interfaces.Client;
using ErrSendApplication.Interfaces.Telegram;

namespace ErrSendPersistensTelegram.Services
{
    public class TelegramService : ITelegramService
    {
        private const string TelegramApiUrl = "https://api.telegram.org/bot";
        private const string SendMessageEndpoint = "/sendMessage";
        private const string HtmlParseMode = "HTML";

        private readonly IHttpClientWr httpClient;
        private readonly TelegramConfig telegramConfig;

        public TelegramService(
            IHttpClientWr httpClient, 
            IOptions<TelegramConfig> telegramConfig)
        {
            this.httpClient = httpClient;
            this.telegramConfig = telegramConfig.Value;
        }

        public async Task SendErrorMessageAsync(string errorMessage, string? additionalInfo = null)
        {
            try
            {
                // Валідація параметрів
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    return;
                }

                // Перевірка конфігурації
                if (string.IsNullOrEmpty(telegramConfig.BotToken) || string.IsNullOrEmpty(telegramConfig.ChatId))
                {
                    return;
                }

                var message = FormatErrorMessage(errorMessage, additionalInfo);
                var url = $"{TelegramApiUrl}{telegramConfig.BotToken}{SendMessageEndpoint}";

                var payload = new TelegramSendMessageRequest
                {
                    ChatId = telegramConfig.ChatId,
                    Text = message,
                    ParseMode = HtmlParseMode
                };

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

            if (!string.IsNullOrWhiteSpace(additionalInfo))
            {
                // Декодуємо, якщо раптом прийшов URL-encoded текст
                var decodedInfo = System.Net.WebUtility.UrlDecode(additionalInfo);
                sb.AppendLine($"ℹ️ <b>Додаткова інформація:</b> {System.Net.WebUtility.HtmlEncode(decodedInfo)}");
            }

            return sb.ToString();
        }
    }

    // Типізований клас для Telegram API запиту (треба буде перенести в інший файл)
    internal class TelegramSendMessageRequest
    {
        [System.Text.Json.Serialization.JsonPropertyName("chat_id")]
        public string ChatId { get; set; } 
        [System.Text.Json.Serialization.JsonPropertyName("text")]
        public string Text { get; set; } 
        [System.Text.Json.Serialization.JsonPropertyName("parse_mode")]
        public string ParseMode { get; set; } 
    }
} 

