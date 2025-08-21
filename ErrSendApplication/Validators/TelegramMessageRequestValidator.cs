using FluentValidation;
using ErrSendApplication.DTO;

namespace ErrSendApplication.Validators
{
    public class TelegramMessageRequestValidator : AbstractValidator<TelegramMessageRequest>
    {
        public TelegramMessageRequestValidator()
        {
            try
            {
                RuleFor(x => x.Message)
                    .NotEmpty().WithMessage("Повідомлення не повинно бути порожнім");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка створення правил валідації TelegramMessageRequest: {ex.Message}", ex);
            }
        }
    }
}


