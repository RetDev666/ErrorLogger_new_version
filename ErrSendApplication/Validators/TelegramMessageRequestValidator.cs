using FluentValidation;
using ErrSendApplication.DTO;

namespace ErrSendApplication.Validators
{
    public class TelegramMessageRequestValidator : AbstractValidator<TelegramMessageRequest>
    {
        public TelegramMessageRequestValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Повідомлення не повинно бути порожнім");
        }
    }
} 