using FluentValidation;
using ErrSendApplication.Common.Configs;

namespace ErrSendApplication.Validators
{
    public class TelegramServiceValidator : AbstractValidator<(string errorMessage, string? additionalInfo, TelegramConfig config)>
    {
        public TelegramServiceValidator()
        {
            try
            {
                RuleFor(x => x.errorMessage)
                    .NotEmpty().WithMessage("Повідомлення про помилку не може бути порожнім")
                    .MaximumLength(4096).WithMessage("Повідомлення про помилку занадто довге (максимум 4096 символів)");

                RuleFor(x => x.config.BotToken)
                    .NotEmpty().WithMessage("Bot Token не може бути порожнім")
                    .Matches(@"^\d+:[A-Za-z0-9_-]+$").WithMessage("Bot Token має неправильний формат");

                RuleFor(x => x.config.ChatId)
                    .NotEmpty().WithMessage("Chat ID не може бути порожнім")
                    .Matches(@"^-?\d+$").WithMessage("Chat ID має бути числом");

                When(x => !string.IsNullOrWhiteSpace(x.additionalInfo), () =>
                {
                    RuleFor(x => x.additionalInfo)
                        .MaximumLength(1000).WithMessage("Додаткова інформація занадто довга (максимум 1000 символів)");
                });
            }
            catch (Exception ex)
            {
                // Логуємо помилку створення правил валідації
                throw new InvalidOperationException($"Помилка створення правил валідації TelegramService: {ex.Message}", ex);
            }
        }
    }
}
