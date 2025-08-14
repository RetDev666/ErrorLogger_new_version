using FluentValidation;
using ErrSendApplication.Common.Configs;

namespace ErrSendApplication.Validators
{
    public class TelegramConfigValidator : AbstractValidator<TelegramConfig>
    {
        public TelegramConfigValidator()
        {
            try
            {
                RuleFor(x => x.BotToken)
                    .NotEmpty().WithMessage("Bot Token не може бути порожнім")
                    .Matches(@"^\d+:[A-Za-z0-9_-]+$").WithMessage("Bot Token має неправильний формат (має бути у форматі: 123456789:ABCdefGHIjklMNOpqrsTUVwxyz)");

                RuleFor(x => x.ChatId)
                    .NotEmpty().WithMessage("Chat ID не може бути порожнім")
                    .Matches(@"^-?\d+$").WithMessage("Chat ID має бути числом (може бути від'ємним для груп)");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка створення правил валідації TelegramConfig: {ex.Message}", ex);
            }
        }
    }
}
