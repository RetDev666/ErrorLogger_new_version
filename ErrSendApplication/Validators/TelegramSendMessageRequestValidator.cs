using FluentValidation;

namespace ErrSendApplication.Validators
{
    public class TelegramSendMessageRequestValidator : AbstractValidator<TelegramSendMessageRequestValidator.TelegramSendMessageRequest>
    {
        public TelegramSendMessageRequestValidator()
        {
            try
            {
                RuleFor(x => x.ChatId)
                    .NotEmpty().WithMessage("Chat ID не може бути порожнім")
                    .Matches(@"^-?\d+$").WithMessage("Chat ID має бути числом");

                RuleFor(x => x.Text)
                    .NotEmpty().WithMessage("Текст повідомлення не може бути порожнім")
                    .MaximumLength(4096).WithMessage("Текст повідомлення занадто довгий (максимум 4096 символів)");

                RuleFor(x => x.ParseMode)
                    .IsInEnum().WithMessage("Parse Mode має бути валідним значенням");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка створення правил валідації TelegramSendMessageRequest: {ex.Message}", ex);
            }
        }

        // Клас для валідації (перенесений з TelegramService)
        public class TelegramSendMessageRequest
        {
            [System.Text.Json.Serialization.JsonPropertyName("chat_id")]
            public string ChatId { get; set; } = string.Empty;
            
            [System.Text.Json.Serialization.JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;
            
            [System.Text.Json.Serialization.JsonPropertyName("parse_mode")]
            public string ParseMode { get; set; } = string.Empty;
        }
    }
}
